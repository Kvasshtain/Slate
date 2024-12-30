using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAndAuthorization
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "mysupersecret_secretsecretsecretkey!123";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
