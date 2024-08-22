using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NZWalks.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        public TokenRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            // create claims: claims are the data/payload
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //get key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]));
            
            //create signature
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //create the token
            var token = new JwtSecurityToken(
                Configuration["Jwt:issuer"],
                Configuration["Jwt:audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );

            // send the token
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
