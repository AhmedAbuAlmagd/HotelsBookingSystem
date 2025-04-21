using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<int> GetTotalBookingsCountAsync();
        Task<List<Booking>> GetRecentBookingsAsync(int count);

    }
}
