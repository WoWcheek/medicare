namespace Medicare.BLL.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsPatient { get; set; }
        public string Avatar { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int AllAppointmentsCount { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
        public int WeekAppointmentsCount { get; set; }
        public int DayAppointmentsCount { get; set; }
        public int UsersCount { get; set; }
        public List<Guid> Specializations { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
