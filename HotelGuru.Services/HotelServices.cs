using HotelGuru.DataContext.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IHotelServices
    {

    }

    public class HotelServices : IHotelServices
    {
        private readonly AppDbContext _context;
        

        public HotelServices(AppDbContext context)
        {
            _context = context;
        }

       
    }
}
