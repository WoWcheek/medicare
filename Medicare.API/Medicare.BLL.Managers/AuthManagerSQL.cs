using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Medicare.BLL.DTO;
using Medicare.BLL.Interfaces;
using Medicare.DAL.Models;
using Medicare.DAL.Storage;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Medicare.BLL.Managers.Configuration;

namespace Medicare.BLL.Managers;

public class AuthManagerSQL : IAuthManager
{
    private readonly MedicareContext _context;
    private readonly AuthConfig _authConfig;

    public AuthManagerSQL(MedicareContext context, AuthConfig authConfig)
    {
        _context = context;
        _authConfig = authConfig;
    }

    public async Task<bool> Register(RegisterDTO registerDTO)
    {
        if (HasEmptyField(registerDTO))
        {
            return false;
        }

        var sha1data = new PasswordHasher<User>()
            .HashPassword(null!, registerDTO.Password);

        await _context.Users
            .AddAsync(new User
            {
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                UserName = registerDTO.Email,
                IsPatient = registerDTO.IsPatient,
                PasswordHash = sha1data,
            });

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<LoggedInUserWithTokenDTO?> Login(LoginDTO loginDTO)
    {
        if (HasEmptyField(loginDTO))
        {
            return null;
        }

        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == loginDTO.Email);

            if (user == null || user.PasswordHash == null)
            {
                return null;
            }

            var sha1data = new PasswordHasher<User>()
                .VerifyHashedPassword(null!, user.PasswordHash, loginDTO.Password);

            if (sha1data == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var loggedInUser = new LoggedInUserDTO
            {
                Id = user.Id,
                Email = loginDTO.Email,
                IsPatient = user.IsPatient
            };

            var loggedInUserWithToken = AddToken(loggedInUser);

            return loggedInUserWithToken;
        }
        catch
        {
            return null;
        }
    }

    private LoggedInUserWithTokenDTO AddToken(LoggedInUserDTO loggedInUser)
    {
        ClaimsIdentity claim = new ClaimsIdentity(
            new List<Claim>()
            {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, loggedInUser.Email)
            },
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        var now = DateTime.Now;
        var securityToken = new JwtSecurityToken(
            issuer: _authConfig.Issuer,
            audience: _authConfig.Audience,
        notBefore: now,
            claims: claim.Claims,
            expires: now.Add(TimeSpan.FromMinutes(_authConfig.Lifetime)),
            signingCredentials:
                new SigningCredentials(_authConfig.SymmetricSecurityKey,
                                       SecurityAlgorithms.HmacSha256));

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        var userWithToken = new LoggedInUserWithTokenDTO
        {
            Id = loggedInUser.Id,
            IsPatient = loggedInUser.IsPatient,
            Token = token
        };

        return userWithToken;
    }

    private bool HasEmptyField(RegisterDTO registerDTO)
    {
        return string.IsNullOrEmpty(registerDTO.FullName) ||
               string.IsNullOrEmpty(registerDTO.Email) ||
               string.IsNullOrEmpty(registerDTO.Password);
    }

    private bool HasEmptyField(LoginDTO loginDTO)
    {
        return string.IsNullOrEmpty(loginDTO.Email) ||
               string.IsNullOrEmpty(loginDTO.Password);
    }
}
