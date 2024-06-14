using Medicare.BLL.DTO;

namespace Medicare.BLL.Interfaces
{
    public interface IUserManager
    {
        UserDTO? GetUserByUsername(string username);
        DoctorDTO? GetDoctorById(Guid id);
        DoctorsDTO? GetDoctors(FilterDTO filter);
        Task<bool> ChangeAvatar(AvatarDTO avatarDTO);
        Task<bool> ChangePassword(PasswordDTO passwordDTO);
        Task<bool> EditUserInfo(UserInfoDTO userInfoDTO);
    }
}
