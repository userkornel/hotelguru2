using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using HotelGuru.Models;
using HotelGuru.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // alapból védett
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ValidateUser _validator;
        private readonly AppDbContext _context;

        public AuthController(
            IConfiguration configuration,
            ValidateUser validator,
            AppDbContext context)
        {
            _configuration = configuration;
            _validator = validator;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisztracioDto dto)
        {
            // csak Recepcios-okat engedünk regisztrálni
            if (await _context.Felhasznalok.AnyAsync(u => u.Felhasznalonev == dto.FelhasznaloNev))
                return BadRequest("Ez a felhasználónév már foglalt.");

            var user = new Recepcios
            {
                Felhasznalonev = dto.FelhasznaloNev,
                TeljesNev = dto.TeljesNev,
                JelszoHash = dto.Jelszo
            };

            _context.Recepciosok.Add(user);
            await _context.SaveChangesAsync();

            var resultDto = new FelhasznaloGetDto
            {
                Id = user.Id,
                Felhasznalonev = user.Felhasznalonev,
                TeljesNev = user.TeljesNev,
                Role = "Recepcios"
            };
            return Ok(resultDto);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] BejelentkezesDto dto)
        {
            var user = await _validator.ValidateAsync(dto.FelhasznaloNev, dto.Jelszo);
            if (user == null)
                return Unauthorized("Érvénytelen felhasználónév vagy jelszó.");

            var jwtSection = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                                 Encoding.UTF8.GetBytes(jwtSection["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // A Discriminator mezőt olvassuk ki szerepként
            var role = _context.Entry(user).Property("Discriminator").CurrentValue?.ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Felhasznalonev),
                new Claim(ClaimTypes.Role,            role ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenObj = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                                        int.Parse(jwtSection["ExpiresInMinutes"])),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(tokenObj),
                ervenyes = tokenObj.ValidTo
            });
        }
    }
}
