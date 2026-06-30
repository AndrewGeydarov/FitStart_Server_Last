using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitStart_Server.UniversalMethods
{
    public class JwtGenerator
    {
        private readonly string _secretKey;
        public JwtGenerator(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:Key"] ?? throw new Exception("JWT не найден");
        }

        public string GenerateToken(int userID, int roleID)
        {
            var claims = new[]
            {
                new Claim("userID", userID.ToString()),
                new Claim("roleID", roleID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
