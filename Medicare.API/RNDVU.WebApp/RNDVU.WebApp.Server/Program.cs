using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Medicare.WebApp.Server.Configuration;
using Medicare.WebApp.Server.Extensions;
using Medicare.WebApp.Server.Context;
using Medicare.Domain.Common.Models;
using Microsoft.AspNetCore.Identity;
using Medicare.WebApp.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

AuthOptions.KEY = builder.Configuration["Auth:KEY"]!;
AuthOptions.ISSUER = builder.Configuration["Auth:ISSUER"]!;
AuthOptions.AUDIENCE = builder.Configuration["Auth:AUDIENCE"]!;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MedicareContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<User, IdentityRole<Guid>>(x =>
{
    x.Password.RequiredLength = 0; 
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireDigit = false;
})
        .AddEntityFrameworkStores<MedicareContext>(); 
builder.Services.AddControllers(o =>
{
    o.UseRoutePrefix("api");
});

builder.Services
    .AddAuthentication((options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }))
    .AddJwtBearer(x =>
    {
        //x.RequireHttpsMetadata = true;
        //x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MedicareContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Specializations.Any())
    {
        dbContext.Specializations.AddRange(
            new[]
            {
                new Specialization(){ Name = "Cardiology"},
                new Specialization(){ Name = "Dermatology"},
                new Specialization(){ Name = "Endocrinology"},
                new Specialization(){ Name = "Gastroenterology"},
                new Specialization(){ Name = "Hematology"},
                new Specialization(){ Name = "Infectious Disease"},
                new Specialization(){ Name = "Nephrology"},
                new Specialization(){ Name = "Neurology"},
                new Specialization(){ Name = "Oncology"},
                new Specialization(){ Name = "Ophthalmology"},
                new Specialization(){ Name = "Orthopedics"},
                new Specialization(){ Name = "Otolaryngology"},
                new Specialization(){ Name = "Pathology"},
                new Specialization(){ Name = "Pediatrics"},
                new Specialization(){ Name = "Psychiatry"},
                new Specialization(){ Name = "Pulmonology"},
                new Specialization(){ Name = "Radiology"},
                new Specialization(){ Name = "Rheumatology"},
                new Specialization(){ Name = "Surgery"},
                new Specialization(){ Name = "Urology"},
                new Specialization(){ Name = "Allergy and Immunology"},
                new Specialization(){ Name = "Anesthesiology"},
                new Specialization(){ Name = "Geriatrics"},
                new Specialization(){ Name = "Obstetrics and Gynecology"},
                new Specialization(){ Name = "Physical Medicine and Rehabilitation"}
            });
        dbContext.SaveChanges();
    }
}


app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
