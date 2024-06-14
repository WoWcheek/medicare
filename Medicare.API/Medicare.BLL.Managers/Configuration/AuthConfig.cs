using Microsoft.IdentityModel.Tokens;

namespace Medicare.BLL.Managers.Configuration;

public record AuthConfig(string Key,
                         string Issuer,
                         string Audience,
                         int Lifetime,
                         SymmetricSecurityKey SymmetricSecurityKey);
