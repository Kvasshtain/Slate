using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace slate.AuthenticationAndAuthorization
{
    public abstract class AuthOptions
    {
        public const string Issuer = "MyAuthServer";
        public const string Audience = "MyAuthClient";
        private const string key = "mysupersecret_secretsecretsecretkey!123";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}
