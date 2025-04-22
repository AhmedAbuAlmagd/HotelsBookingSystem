using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;
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
        public async Task<int> GetTotalHotelsCountAsync()
        {
            return await con.Hotels.CountAsync();
        }

        public async Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6)
        {
            return await con.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .OrderBy(h => h.Name)
                .Take(count)
                .Select(h => new HotelViewModel
                {
                    Name = h.Name,
                    Location = h.Address,
                    RoomCount = h.Rooms.Count,
                    ImageUrl = h.HotelImages.FirstOrDefault(x => x.IsPrimary == true).ImageUrl,
                    Status = h.Status
                })
                .ToListAsync();
        }

        public IPagedList<Hotel> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();

        }

        int IRepository<Hotel>.SaveChanges()
        {
          return  con.SaveChanges();    
        }
    }
}
