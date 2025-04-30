using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

using X.PagedList;
using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Repository
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Hotel GetHotelWithServices(int hotelId);
        List<Hotel> GetAllhotels();
        List<Hotel> GetHotelsWithRoomsAndImages();
        Hotel GetHotelWithRoomsAndImages(int id);

        Task<List<ViewModels.AdminViewModels.HotelViewModel>> GetTopRatedHotelsAsync(int count = 4);
        Task<List<ReviewViewModel>> GetRecentReviewsAsync(int count = 5);

        Hotel GetById(int id);

        Task<int> GetTotalHotelsCountAsync();
        Task<List<ViewModels.AdminViewModels.Dashboard.HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
