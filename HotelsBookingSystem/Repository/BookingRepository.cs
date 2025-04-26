using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
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

        public async Task<List<Booking>> GetByFilterAsync(BookingFilterViewModel filter)
        {
            var query = _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(b => b.Status == filter.Status);
            }

            if (filter.HotelId.HasValue)
            {
                query = query.Where(b => b.HotelId == filter.HotelId.Value);
            }

            if (filter.BookingDateFrom.HasValue)
            {
                query = query.Where(b => b.Booking_date >= filter.BookingDateFrom.Value.Date);
            }

            if (filter.BookingDateTo.HasValue)
            {
                query = query.Where(b => b.Booking_date <= filter.BookingDateTo.Value.Date);
            }

            if (!string.IsNullOrEmpty(filter.ClientName))
            {
                query = query.Where(b => b.User.FullName.Contains(filter.ClientName));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(b => b.TotalPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(b => b.TotalPrice <= filter.MaxPrice.Value);
            }

            return await query.ToListAsync();
        }
    }
}
