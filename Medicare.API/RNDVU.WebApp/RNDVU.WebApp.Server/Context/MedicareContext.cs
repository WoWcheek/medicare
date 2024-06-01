using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Medicare.Domain.Common.Models;
using System.Reflection.Emit;

namespace Medicare.WebApp.Server.Context;

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
             .HasKey(us => new { us.UserId, us.SpecializationId });

        builder.Entity<UserSpecialization>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSpecializations)
            .HasForeignKey(us => us.UserId);

        builder.Entity<UserSpecialization>()
            .HasOne(us => us.Specialization)
            .WithMany(s => s.UserSpecializations)
            .HasForeignKey(us => us.SpecializationId);
        builder.Entity<Appointment>()
            .HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Appointment>()
                 .HasOne(us => us.Doctor)
                 .WithMany()
                 .HasForeignKey(us => us.DoctorId).OnDelete(DeleteBehavior.Restrict);
    }
}
