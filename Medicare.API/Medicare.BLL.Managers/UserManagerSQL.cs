using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Medicare.BLL.DTO;
using Medicare.BLL.Interfaces;
using Medicare.DAL.Storage;
using Medicare.DAL.Models;

namespace Medicare.BLL.Managers;

public class UserManagerSQL : IUserManager
{
    private readonly MedicareContext _context;
    private readonly UserManager<User> _userManagerIdentity;
    private readonly IStatisticsManager _statisticsManager;

    public UserManagerSQL(
        MedicareContext context,
        UserManager<User> userManagerIdentity,
        IStatisticsManager statisticsManager)
    {
        _context = context;
        _userManagerIdentity = userManagerIdentity;
        _statisticsManager = statisticsManager;
    }

    public UserDTO? GetUserByUsername(string username)
    {
        var user = _context.Users
                .Include(x => x.UserSpecializations)
                .FirstOrDefault(x => x.Email == username);

        if (user == null)
        {
            return null;
        }

        var allAppointmentsCount = _statisticsManager
            .AppointmentsCount(user.Id);

        var upcomingCount = _statisticsManager
            .UpcomingAppointmentsCount(user.Id);

        var differentDoctorsCount = _statisticsManager
            .AppointmentsWithDistinctDoctorsCount(user.Id);

        var differentUsersCount = _statisticsManager
            .AppointmentsWithDistinctUsersCount(user.Id);

        var weekAppointmentsCount = _statisticsManager
            .WeekAppointmentsCount(user.Id);

        var dayAppointmentsCount = _statisticsManager
            .DayAppointmentsCount(user.Id);

        var res = new UserDTO
        {
            Id = user.Id,
            IsPatient = user.IsPatient,
            FullName = user.FullName,
            Description = user.Description,
            Avatar = user.Avatar,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AllAppointmentsCount = allAppointmentsCount,
            UpcomingAppointmentsCount = upcomingCount,
            UsersCount = user.IsPatient ? differentDoctorsCount : differentUsersCount,
            WeekAppointmentsCount = weekAppointmentsCount,
            DayAppointmentsCount = dayAppointmentsCount,
            Specializations = user.UserSpecializations?.Select(x => x.SpecializationId).ToList()
        };

        return res;
    }

    public DoctorDTO? GetDoctorById(Guid id)
    {
        var user = _context.Users
            .Include(x => x.UserSpecializations)
            .ThenInclude(x => x.Specialization)
            .FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            return null;
        }

        return new DoctorDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Description = user.Description,
            Avatar = user.Avatar,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Specializations = user.UserSpecializations?
                .Select(x => x.Specialization.Name)
                .ToList()
        };
    }

    public DoctorsDTO? GetDoctors(FilterDTO filter)
    {
        var doctors = _context.Users
            .AsNoTracking()
            .Include(x => x.UserSpecializations)
            .ThenInclude(x => x.Specialization)
            .Where(x => !x.IsPatient)
            .ToList()
            .Where(x => filter.Specializations?
                .Any(y => x.UserSpecializations
                    .Any(u => u.SpecializationId == y)) ?? true);

        var totalCount = doctors.Count();

        var doctorsForPage = doctors
            .Skip((filter.Page ?? 0) * 9)
            .Take(9);

        var dtos = doctorsForPage
            .Select(x => new DoctorDTO
            {
                Id = x.Id,
                FullName = x.FullName,
                Description = x.Description,
                Avatar = x.Avatar,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Specializations = x.UserSpecializations
                    .Select(x => x.Specialization.Name)
                    .ToList()
            })
            .ToList();

        return new DoctorsDTO
        {
            Count = totalCount,
            Doctors = dtos
        };
    }

    public async Task<bool> ChangeAvatar(AvatarDTO avatarDTO)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == avatarDTO.Username);

        if (user == null)
        {
            return false;
        }

        user.Avatar = avatarDTO.Avatar;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangePassword(PasswordDTO passwordDTO)
    {
        var userId = _context.Users
            .AsNoTracking()?
            .FirstOrDefault(x => x.Email == passwordDTO.Username)?.Id ?? null;

        if (userId == null)
        {
            return false;
        }

        var user = await _userManagerIdentity
            .FindByIdAsync(userId.ToString()!);

        var sha1data = await _userManagerIdentity
            .ChangePasswordAsync(user, passwordDTO.OldPassword, passwordDTO.NewPassword);

        if (!sha1data.Succeeded)
        {
            return false;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EditUserInfo(UserInfoDTO userInfoDTO)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == userInfoDTO.Username);

        if (user == null)
        {
            return false;
        }

        user.FullName = userInfoDTO.FullName;
        user.Description = userInfoDTO.Description;
        user.PhoneNumber = userInfoDTO.PhoneNumber;

        _context.RemoveRange(_context.UserSpecializations
            .Where(x => x.UserId == user.Id)
            .ToList());

        _context.UserSpecializations
            .AddRange(userInfoDTO.Specializations
            .Select(x => new UserSpecialization() 
            {
                UserId = user.Id,
                SpecializationId = x
            }));

        await _context.SaveChangesAsync();

        return true;
    }
}
