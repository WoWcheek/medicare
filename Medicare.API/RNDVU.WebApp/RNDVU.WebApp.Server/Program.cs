using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Medicare.WebApp.Server.Configuration;
using Medicare.WebApp.Server.Extensions;
using Microsoft.AspNetCore.Identity;
using Medicare.BLL.Managers;
using Medicare.BLL.Interfaces;
using Medicare.DAL.Models;
using Medicare.DAL.Storage;
using Medicare.BLL.Managers.Configuration;

var builder = WebApplication.CreateBuilder(args);

AuthOptions.KEY = builder.Configuration["Auth:KEY"]!;
AuthOptions.ISSUER = builder.Configuration["Auth:ISSUER"]!;
AuthOptions.AUDIENCE = builder.Configuration["Auth:AUDIENCE"]!;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MedicareContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(x =>
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

builder.Services.AddSingleton(
    _ => new AuthConfig(
        AuthOptions.KEY,
        AuthOptions.ISSUER,
        AuthOptions.AUDIENCE,
        AuthOptions.LIFETIME,
        AuthOptions.GetSymmetricSecurityKey()));

builder.Services.AddSingleton(
    _ => new ZoomConfig(
        builder.Configuration.GetValue<string>("Zoom:ClientId")!,
        builder.Configuration.GetValue<string>("Zoom:ClientSecret")!,
        builder.Configuration.GetValue<string>("Zoom:AccountId")!));

builder.Services.AddScoped<IAuthManager, AuthManagerSQL>();
builder.Services.AddScoped<IUserManager, UserManagerSQL>();
builder.Services.AddScoped<ICatalogManager, CatalogManagerSQL>();
builder.Services.AddScoped<IStatisticsManager, StatisticsManagerSQL>();
builder.Services.AddScoped<IAppointmentManager, AppointmentManagerSQL>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
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

    var dbInitializer = new DbInitializer(dbContext);

    if (!dbContext.Specializations.Any())
        await dbInitializer.AddSpecializations();

    if (!dbContext.Users.Any(x => !x.IsPatient))
        await dbInitializer.AddDoctors();
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
