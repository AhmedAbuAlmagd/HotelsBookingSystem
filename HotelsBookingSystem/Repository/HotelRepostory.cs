using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
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
        public async Task<List<Review>> GetReviewsByHotelIdAsync(int hotelId)
        {
            return await con.Reviews
                                 .Where(r => r.HotelId == hotelId)
                                 .Include(r => r.User)
                                 .Include(r => r.Hotel)
                                 .ToListAsync();
        }

        public  void SaveChanges()
        {
           con.SaveChanges();
        }

        public void Update(Hotel hotel)
        {
          con.Update(hotel);
        }

        public Task<int> GetTotalHotelsCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6)
        {
            throw new NotImplementedException();
        }

        public IPagedList<Hotel> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        int IRepository<Hotel>.SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<List<HotelViewModel>> GetTopRatedHotelsAsync(int count = 4)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReviewViewModel>> GetRecentReviewsAsync(int count = 5)
        {
            throw new NotImplementedException();
        }
    }
}
