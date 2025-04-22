using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using HotelsBookingSystem.ViewModels.AdminViewModels;

using X.PagedList;

namespace HotelsBookingSystem.Repository 
{
    public interface IHotelRepository : IRepository<Hotel>
    {

        //List<Hotel> GetAllhotels();
        List<Hotel> GetHotelsWithRoomsAndImages();
         Hotel GetHotelWithRoomsAndImages(int id);
      
        Task<int> GetTotalHotelsCountAsync();
        Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6);
    }
}
