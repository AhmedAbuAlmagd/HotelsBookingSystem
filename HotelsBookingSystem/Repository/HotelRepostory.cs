using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
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
        public Hotel GetHotelWithServices(int hotelId)
        {
            return con.Hotels
                .Include(h => h.HotelServices)
                    .ThenInclude(hs => hs.Service)
                .FirstOrDefault(h => h.Id == hotelId);
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
                 .Include(h => h.HotelServices)
                .Include(h => h.Rooms)
                .ThenInclude(r => r.RoomImages)
                .ToList();
        }
        public Hotel GetHotelWithRoomsAndImages(int id)
        {
            return con.Hotels
                      .Include(h => h.HotelImages)
                      .Include(h => h.HotelServices)
                      .Include(h => h.Rooms)
                          .ThenInclude(r => r.RoomImages)
                      .FirstOrDefault(h => h.Id == id)
                      ;
        }
        public void Add(Hotel hotel)
        {
            con.Add(hotel);
        }

        public void Delete(int id)
        {
            Hotel hotel = GetById(id);
            con.Remove(hotel);
        }


        public Hotel GetById(int id)
        {

            Hotel hotel = con.Hotels.Include(h => h.HotelImages).FirstOrDefault(h => h.Id == id);
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


        public void Update(Hotel hotel)
        {
            con.Update(hotel);

        }

        public async Task<int> GetTotalHotelsCountAsync()
        {
            return await con.Hotels.CountAsync();
        }


        public async Task<List<ViewModels.AdminViewModels.Dashboard.HotelViewModel>> GetTopHotelsAsync(int count = 6)
        {
            return await con.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .OrderBy(h => h.Name)
                .Take(count)
                .Select(h => new ViewModels.AdminViewModels.Dashboard.HotelViewModel
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
            return con.SaveChanges();
        }




        public async Task<List<ViewModels.AdminViewModels.HotelViewModel>> GetTopRatedHotelsAsync(int count = 4)
        {
            return await con.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .Include(h => h.Reviews)
                .OrderByDescending(h => h.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .Select(h => new ViewModels.AdminViewModels.HotelViewModel
                {
                    Name = h.Name,
                    Location = h.Address,
                    RoomCount = h.Rooms.Count,
                    ImageUrl = h.HotelImages.FirstOrDefault(x => x.IsPrimary == true).ImageUrl,
                    Status = h.Status,
                    Rating = h.Reviews.Any() ? h.Reviews.Average(r => r.Rating ?? 0) : 0
                })
                .ToListAsync();
        }


        public async Task<List<ReviewViewModel>> GetRecentReviewsAsync(int count = 5)
        {
            return await con.Reviews
                .Include(r => r.Hotel)
                .Include(r => r.User)
                .OrderByDescending(r => r.Id)
                .Take(count)
                .Select(r => new ReviewViewModel
                {
                    Comment = r.Comment,
                    HotelName = r.Hotel != null ? r.Hotel.Name : "Unknown",
                    UserName = r.User != null ? r.User.UserName : "Anonymous"
                })
                .ToListAsync();
        }

        public List<Hotel> GetAllhotels()
        {
            throw new NotImplementedException();
        }
    }
}
