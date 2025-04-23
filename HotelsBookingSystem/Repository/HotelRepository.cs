using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelsContext _context;

        public HotelRepository(HotelsContext context)
        {
            _context = context;
        }
        public void Add(Hotel hotel)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IPagedList<Hotel> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Hotel GetById(int id)
        {
            throw new NotImplementedException();
        }
        public void Update(Hotel t)
        {
            throw new NotImplementedException();
        }
        public async Task<int> GetTotalHotelsCountAsync()
        {
            return await _context.Hotels.CountAsync();
        }

        public async Task<List<HotelViewModel>> GetTopHotelsAsync(int count = 6)
        {
            return await _context.Hotels
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
        public async Task<List<HotelViewModel>> GetTopRatedHotelsAsync(int count = 4)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .Include(h => h.Reviews) 
                .OrderByDescending(h => h.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .Select(h => new HotelViewModel
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
            return await _context.Reviews
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
        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public List<Hotel> GetAllhotels()
        {
            throw new NotImplementedException();
        }

        public List<Hotel> GetHotelsWithRoomsAndImages()
        {
            throw new NotImplementedException();
        }

        public Hotel GetHotelWithRoomsAndImages(int id)
        {
            throw new NotImplementedException();
        }
    }
}
