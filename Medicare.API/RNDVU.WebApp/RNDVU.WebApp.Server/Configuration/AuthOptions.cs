using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Medicare.WebApp.Server.Configuration
{
    public class AuthOptions
    {
        public static string ISSUER = string.Empty;
        public static string AUDIENCE = string.Empty;
        public static string KEY = string.Empty;

        public static int LIFETIME { get => 360; }

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new (Encoding.UTF8.GetBytes(KEY));
    }
}
