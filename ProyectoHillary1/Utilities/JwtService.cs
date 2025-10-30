using Microsoft.IdentityModel.Tokens;
using ProyectoHillary1.Models.En;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoHillary1.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // ✅ CAMBIO IMPORTANTE: Usar ClaimTypes.NameIdentifier en lugar de Sub
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? string.Empty),
                new Claim("EmpresaId", usuario.EmpresaId.ToString()),
                new Claim("RolId", usuario.RolId.ToString()),
                new Claim("RolNombre", usuario.Rol?.Nombre ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));
        }
    }
}