using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels.AdminViewModels;

namespace HotelsBookingSystem.Repository
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<int> GetTotalHotelsCountAsync();
        Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
