using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

using X.PagedList;
using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Repository
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        List<Hotel> GetAllhotels();
        List<Hotel> GetHotelsWithRoomsAndImages();
        Hotel GetHotelWithRoomsAndImages(int id);
      
        Task<List<HotelViewModel>> GetTopRatedHotelsAsync(int count = 4);
        Task<List<ReviewViewModel>> GetRecentReviewsAsync(int count = 5);

        Hotel GetHotelWithRoomsAndImages(int id);
        Hotel GetById(int id);


        Task<int> GetTotalHotelsCountAsync();
        Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
