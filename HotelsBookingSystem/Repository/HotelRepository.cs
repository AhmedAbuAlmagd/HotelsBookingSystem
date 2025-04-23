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
