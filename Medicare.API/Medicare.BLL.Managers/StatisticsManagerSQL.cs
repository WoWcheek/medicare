using System.Globalization;
using Medicare.BLL.Interfaces;
using Medicare.DAL.Storage;

namespace Medicare.BLL.Managers;

public class StatisticsManagerSQL : IStatisticsManager
{
    private readonly MedicareContext _context;

    public StatisticsManagerSQL(MedicareContext context)
    {
        _context = context;
    }

    public int AppointmentsCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.DoctorId == id || x.UserId == id)
            .Count();

        return count;
    }

    public int DayAppointmentsCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.DoctorId == id || x.UserId == id)
            .ToList()
            .Where(x => x.Date.Date == DateTime.Now.Date)
            .Count();

        return count;
    }

    public int WeekAppointmentsCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.DoctorId == id || x.UserId == id)
            .ToList()
            .Where(x =>
            {
                CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                DayOfWeek firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

                DateTime startOfWeek = DateTime.Now.Date;
                while (startOfWeek.DayOfWeek != firstDayOfWeek)
                    startOfWeek = startOfWeek.AddDays(-1);

                DateTime endOfWeek = startOfWeek.AddDays(6);

                return x.Date.Date >= startOfWeek && x.Date.Date <= endOfWeek;
            })
            .Count();

        return count;
    }

    public int UpcomingAppointmentsCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.DoctorId == id || x.UserId == id)
            .ToList()
            .Where(x =>
            {
                var time = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day)
                    .AddHours(Convert.ToInt64(x.Time.Substring(0, 2)));

                if (x.Time[3] == '3') time.AddMinutes(30);

                return time > DateTime.Now;
            })
            .Count();

        return count;
    }

    public int AppointmentsWithDistinctUsersCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.UserId == id)
            .Select(x => x.UserId)
            .Distinct()
            .Count();

        return count;
    }

    public int AppointmentsWithDistinctDoctorsCount(Guid id)
    {
        int count = _context
            .Appointments
            .Where(x => x.DoctorId == id)
            .Select(x => x.DoctorId)
            .Distinct()
            .Count();

        return count;
    }
}
