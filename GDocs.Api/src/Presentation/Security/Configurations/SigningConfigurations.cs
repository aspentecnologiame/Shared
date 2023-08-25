using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;

namespace ICE.GDocs.Api.Security
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(IConfiguration configuration)
        {
            var secret = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(configuration.GetValue<string>("TokenConfigurations:SecretKey")));
            var symmetricKey = Convert.FromBase64String(secret);

            Key = new SymmetricSecurityKey(symmetricKey);

            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
