using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelGuru.Services
{
    /// <summary>
    /// Felhasználónevet és jelszót validáló szolgáltatás.
    /// </summary>
    public class ValidateUser
    {
        private readonly AppDbContext _context;

        public ValidateUser(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ellenőrzi a bejelentkezési adatokat.
        /// Siker esetén visszaadja a FelhasznaloGetDto-t, egyébként null-t.
        /// </summary>
        public async Task<FelhasznaloGetDto> ValidateAsync(string felhasznalonev, string jelszo)
        {
            var entity = await _context.Recepciosok
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Felhasznalonev == felhasznalonev &&
                    u.JelszoHash == jelszo);

            if (entity == null)
                return null;

            return new FelhasznaloGetDto
            {
                Id = entity.Id,
                Felhasznalonev = entity.Felhasznalonev,
                TeljesNev = entity.TeljesNev
                // Ha később szerepkört is szeretnél, bővítsd ide
            };
        }
    }
}
