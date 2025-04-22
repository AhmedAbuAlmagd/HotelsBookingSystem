using HotelsBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace HotelsBookingSystem.Repository
{
    public class HotelRepostory : IHotelRepository
    {
        private readonly HotelsContext con;
        public HotelRepostory(HotelsContext con)
        {
            this.con = con;
        }
        //public List<Hotel> GetHotelsWithRoomsAndImages()
        //{
        //    return con.Hotels
        //       .Include(h => h.HotelImages)
        //        .Include(h => h.Rooms)
        //            .ThenInclude(r => r.RoomImages)
        //        .ToList();
        //}
        public List<Hotel> GetHotelsWithRoomsAndImages()
        {
            return con.Hotels
                .Include(h => h.HotelImages)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomImages)
                .ToList();
        }
        public Hotel GetHotelWithRoomsAndImages(int id)
        {
            return con.Hotels
                      .Include(h => h.HotelImages)
                      .Include(h => h.Rooms)
                          .ThenInclude(r => r.RoomImages)
                      .FirstOrDefault(h => h.Id == id);
        }
        public void Add(Hotel hotel)
        {
         con.Add(hotel);
        }

       public void Delete(int id)
        {
         Hotel hotel=GetById(id);
            con.Remove(hotel);
        }

        public List<Hotel> GetAllhotels()
        {

            List<Hotel> hotels = con.Hotels.Include(h=>h.Rooms).Include(h=>h.HotelImages).ToList();
            return hotels;

        }
      public  Hotel GetById(int id)
        {
        
           Hotel hotel = con.Hotels.FirstOrDefault(h => h.Id == id);
            return hotel;
        
        }

       public  void SaveChanges()
        {
           con.SaveChanges();
        }

        public void Update(Hotel hotel)
        {
          con.Update(hotel);
        }
    }
}
