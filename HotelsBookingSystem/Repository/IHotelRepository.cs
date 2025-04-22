using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

using X.PagedList;
using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;

namespace HotelsBookingSystem.Repository
{
    public interface IHotelRepository : IRepository<Hotel>
    {

        List<Hotel> GetHotelsWithRoomsAndImages();
        Hotel GetHotelWithRoomsAndImages(int id);
      
        Task<int> GetTotalHotelsCountAsync();
        Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
