using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ApiUci.Interfaces;
using ApiUci.Models;

namespace ApiUci.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("UsuarioId", user.Id), //todo;arreglar esto
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id) // ID del usuario
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}