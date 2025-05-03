using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly HotelsContext _context;
        public UserRepository(HotelsContext context)
        {
            _context = context;
        }
        public async Task<List<ApplicationUser>> GetTopClientsAsync(int count)
        {
            return await _context.Users
                .Include(u => u.Bookings)
                .Where(u => u.Bookings.Count() != 0)
                .OrderByDescending(u => u.Bookings.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalClientsCountAsync()
        {
            return await _context.Users
                           .Include(u => u.Bookings)
                           .Where(u => u.Bookings.Count() != 0)
                           .CountAsync();
        }
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}
