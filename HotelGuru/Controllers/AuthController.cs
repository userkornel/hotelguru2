using HotelGuru.DataContext.Dtos;       
using HotelGuru.Models;
using HotelGuru.Services;               
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ValidateUser _validator;

        public AuthController(IConfiguration configuration, ValidateUser validator)
        {
            _configuration = configuration;
            _validator = validator;
        }

        [HttpPost("bejelentkezes")]
        public async Task<IActionResult> Bejelentkezes([FromBody] BejelentkezesDto dto)
        {
            
            var user = await _validator.ValidateAsync(dto.FelhasznaloNev, dto.Jelszo);
            if (user == null)
                return Unauthorized(new { uzenet = "Hibás felhasználónév vagy jelszó" });

            
            var jwtSection = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresInMinutes = int.Parse(jwtSection["ExpiresInMinutes"]);

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,  user.Felhasznalonev),
                new Claim(ClaimTypes.Name,              user.TeljesNev),
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString())
            };

            
            var tokenObj = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenObj);

            
            return Ok(new
            {
                token = token,
                ervenyes = tokenObj.ValidTo
            });
        }
    }
}
