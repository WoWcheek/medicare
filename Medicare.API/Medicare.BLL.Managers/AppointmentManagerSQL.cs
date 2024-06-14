using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Medicare.BLL.DTO;
using Medicare.BLL.Interfaces;
using Medicare.DAL.Models;
using Medicare.DAL.Storage;
using Microsoft.AspNetCore.Mvc;
using Medicare.BLL.Managers.Configuration;

namespace Medicare.BLL.Managers;

public class AppointmentManagerSQL : IAppointmentManager
{
    private readonly MedicareContext _context;
    private readonly ZoomConfig _zoomConfig;

    public AppointmentManagerSQL(MedicareContext context, ZoomConfig zoomConfig)
    {
        _context = context;
        _zoomConfig = zoomConfig;
    }

    public List<AppointmentDTO> GetAppointmentsForUser(Guid id)
    {
        var appointments = _context.Appointments
            .AsSplitQuery()
            .Include(x => x.Doctor)
            .Include(x => x.User)
            .Where(x => x.DoctorId == id || x.UserId == id)
            .ToList()
            .Where(x =>
            {
                var time = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day)
                    .AddHours(Convert.ToInt64(x.Time.Substring(0, 2)));

                if (x.Time[3] == '3')
                    time.AddMinutes(30);

                return time > DateTime.Now;
            })
            .ToList();

        var apps = appointments
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Time)
            .Select(app => new AppointmentDTO
            {
                Appointment = app,
                Specializations = _context.Users
                    .AsNoTracking()
                    .Include(x => x.UserSpecializations)
                    .ThenInclude(x => x.Specialization)
                    .First(t => t.Id == app.DoctorId)
                    .UserSpecializations
                    .Select(t => t.Specialization.Name)
                    .ToList()
            })
            .ToList();

        return apps;
    }

    public List<AvailableDateTimeDTO> GetAvailableDateTimesForDoctor(Guid id)
    {
        var appsByDate = _context.Appointments
            .Where(x => x.Date > DateTime.Today && x.DoctorId == id)
            .GroupBy(x => x.Date);

        var timesIndices = new Dictionary<string, int>() {
            {"10:00", 0},
            {"10:30", 1},
            {"11:00", 2},
            {"11:30", 3},
            {"12:00", 4},
            {"12:30", 5},
            {"13:00", 6},
            {"13:30", 7},
            {"14:00", 8},
            {"14:30", 9},
            {"15:00", 10},
            {"15:30", 11},
            {"16:00", 12},
            {"16:30", 13},
            {"17:00", 14},
            {"17:30", 15}};

        List<AvailableDateTimeDTO> res = new();
        foreach (var date in appsByDate)
        {
            var availableDto = new AvailableDateTimeDTO() { Date = date.Key };
            foreach (var item in date.OrderBy(x => x.Time))
            {
                availableDto.Available[timesIndices[item.Time]] = true;
                if (!item.IsShort)
                    availableDto.Available[timesIndices[item.Time] + 1] = true;
            }
            res.Add(availableDto);
        }

        return res;
    }

    public async Task<bool> MakeAppointment(MakeAppointmentDTO makeAppointmentDTO)
    {
        try
        {
            MeetingDTO meeting = new MeetingDTO
            {
                IsShort = makeAppointmentDTO.IsShort,
                Date = makeAppointmentDTO.Date,
                Time = makeAppointmentDTO.Time,
            };

            string? link = await CreateMeeting(meeting);

            if (link == null)
            {
                return false;
            }

            Appointment appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                UserId = makeAppointmentDTO.UserId,
                DoctorId = makeAppointmentDTO.DoctorId,
                Date = makeAppointmentDTO.Date.ToLocalTime(),
                Time = makeAppointmentDTO.Time,
                IsShort = makeAppointmentDTO.IsShort,
                Url = link
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string?> CreateMeeting(MeetingDTO meeting)
    {
        try
        {
            var jwtToken = await GenerateJWTToken();

            if (string.IsNullOrEmpty(jwtToken))
            {
                return null;
            }

            DateTime startDateTime = new DateTime(meeting.Date.Year, meeting.Date.Month, meeting.Date.Day)
                .AddHours(Convert.ToInt64(meeting.Time.Substring(0, 2)));

            if (meeting.Time[3] == '3')
                startDateTime.AddMinutes(30);

            string topic = "Medicare";
            string start = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            int meetingDuration = meeting.IsShort ? 30 : 60;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string zoomApiUrl = "https://api.zoom.us/v2/users/me/meetings";

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestBody = new
            {
                topic,
                type = 2,
                start_time = start,
                duration = meetingDuration,
                timezone = "UTC",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = true
                }
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(zoomApiUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ZoomDTO>(responseContent)?.join_url ?? null;
        }
        catch
        {
            return null;
        }
    }

    [NonAction]
    private async Task<string?> GenerateJWTToken()
    {
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var zoomConfigString = $"{_zoomConfig.ClientId}:{_zoomConfig.ClientSecret}";

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(zoomConfigString)));

        var requestContent = new StringContent(
            $"account_id={_zoomConfig.AccountId}&grant_type=account_credentials",
            Encoding.UTF8,
            "application/x-www-form-urlencoded");

        string zoomApiTokenUrl = "https://zoom.us/oauth/token";
        var response = await httpClient.PostAsync(zoomApiTokenUrl, requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (responseContent == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ZoomDTO>(responseContent)?.access_token ?? null;
    }
}
