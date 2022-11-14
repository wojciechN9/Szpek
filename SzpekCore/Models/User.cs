using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Szpek.Core.Models
{
    public class User : IdentityUser
    {
        public SensorOwner SensorOwner { get; private set; }
        public long? SensorOwnerId { get; private set; }

        public User(string userName, string email) : base(userName)
        {
            Email = email;
        }

        public User()
        {
        }

        public void AddSensorOwner(SensorOwner sensorOwner)
        {
            if (sensorOwner == null)
            {
                SensorOwner = sensorOwner;
                SensorOwnerId = SensorOwner.Id;
            }
        }

        public string GenerateToken(string secret, IList<string> userRoles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GenerateClaims(userRoles)),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private List<Claim> GenerateClaims(IList<string> userRoles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Id.ToString())
            };
            claims.AddRange(GenerateRoleClaims(userRoles));

            return claims;
        }

        private List<Claim> GenerateRoleClaims(IList<string> userRoles)
        {
            var roleClaims = new List<Claim>();

            foreach (var userRole in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return roleClaims;
        }
    }
}
