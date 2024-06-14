namespace Medicare.BLL.Interfaces;

public interface IStatisticsManager
{
    int AppointmentsCount(Guid id);
    int DayAppointmentsCount(Guid id);
    int WeekAppointmentsCount(Guid id);
    int UpcomingAppointmentsCount(Guid id);
    int AppointmentsWithDistinctUsersCount(Guid id);
    int AppointmentsWithDistinctDoctorsCount(Guid id);
}
