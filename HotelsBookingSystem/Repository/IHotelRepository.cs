using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using HotelsBookingSystem.ViewModels.AdminViewModels;

using X.PagedList;
using HotelsBookingSystem.ViewModels;

namespace HotelsBookingSystem.Repository 
{
    public interface IHotelRepository : IRepository<Hotel>
    {

        List<Hotel> GetAllhotels();
        List<Hotel> GetHotelsWithRoomsAndImages();
        Task<List<HotelViewModel>> GetTopRatedHotelsAsync(int count = 4);
        Task<List<ReviewViewModel>> GetRecentReviewsAsync(int count = 5);

        Hotel GetHotelWithRoomsAndImages(int id);
        Hotel GetById(int id);
//         void Add(Hotel hotel);
//         void Update(Hotel hotel);
//         void Delete(int id);
//         void SaveChanges();

        Task<int> GetTotalHotelsCountAsync();
        Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
