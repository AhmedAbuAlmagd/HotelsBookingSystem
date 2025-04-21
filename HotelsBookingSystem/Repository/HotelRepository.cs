using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public class HotelRepository :  IHotelRepository
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

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

       
    }
}
