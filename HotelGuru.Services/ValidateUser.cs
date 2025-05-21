using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelGuru.Services
{
    public class ValidateUser
    {
        private readonly AppDbContext _context;
        public ValidateUser(AppDbContext context)
            => _context = context;

        public async Task<Felhasznalo> ValidateAsync(string username, string password)
        {
            // Jelszó most Plain text; később érdemes hash-elni + VerifyHashedPassword
            return await _context.Felhasznalok
                .FirstOrDefaultAsync(f =>
                    f.Felhasznalonev == username &&
                    f.JelszoHash == password);
        }
    }
}
