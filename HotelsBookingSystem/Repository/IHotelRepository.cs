using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public interface IHotelRepository
    {

        List<Hotel> GetAllhotels();
        List<Hotel> GetHotelsWithRoomsAndImages();
         Hotel GetHotelWithRoomsAndImages(int id);
        Hotel GetById(int id);
        void Add(Hotel hotel);
        void Update(Hotel hotel);
        void Delete(int id);
        void SaveChanges();
       
     
    }
}
