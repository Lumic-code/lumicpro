using LumicPro.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LumicPro.Infrastructure.Security
{
    public  class Utils
    {
        private  readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJWT(AppUser appUser)
        {
            var listOfClaims = new List<Claim>();

            listOfClaims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id));
            listOfClaims.Add(new Claim(ClaimTypes.Name, $"{appUser.FirstName} {appUser.LastName}"));

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(listOfClaims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(createdToken);
            return token;
        }
    }
}
