using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Repository
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<int> GetTotalBookingsCountAsync();
        Task<List<Booking>> GetRecentBookingsAsync(int count);
        Task<List<Booking>> GetByFilterAsync(BookingFilterViewModel filter);

    }
}
