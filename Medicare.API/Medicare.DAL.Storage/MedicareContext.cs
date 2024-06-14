using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Medicare.DAL.Models;

namespace Medicare.DAL.Storage;

public partial class MedicareContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<UserSpecialization> UserSpecializations { get; set; }

    public MedicareContext()
    {
        Database.EnsureCreated();
    }

    public MedicareContext(DbContextOptions<MedicareContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserSpecialization>()
             .HasKey(x => new { x.UserId, x.SpecializationId });

        builder.Entity<UserSpecialization>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserSpecializations)
            .HasForeignKey(x => x.UserId);

        builder.Entity<UserSpecialization>()
            .HasOne(x => x.Specialization)
            .WithMany(x => x.UserSpecializations)
            .HasForeignKey(x => x.SpecializationId);

        builder.Entity<Appointment>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
