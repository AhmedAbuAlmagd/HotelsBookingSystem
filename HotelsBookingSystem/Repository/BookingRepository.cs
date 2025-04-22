using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelsContext _context;

        public BookingRepository(HotelsContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Hotel)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Hotel)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<List<Booking>> GetRecentBookingsAsync(int count)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Hotel)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Hotel)
                .OrderByDescending(b => b.Booking_date)
                .Take(count)
                .ToListAsync();
        }
    }
}
