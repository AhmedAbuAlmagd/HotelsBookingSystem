using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.Services;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly HotelsContext _context;

        public ServiceRepository(HotelsContext context)
        {
            _context = context;
        }

        public int GetCountByHotelId(int hotelId)
        {
            return _context.Hotel_Service.Where(hs => hs.HotelId == hotelId).Count();
        }
    
        public List<Service> GetPagedByHotelId(int hotelId, int pageNumber, int pageSize)
        {
            return _context.Hotel_Service
                .Where(hs => hs.HotelId == hotelId)
                .Include(hs => hs.Service)
                .OrderBy(hs => hs.serviceId)  
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(hs => hs.Service)
                .ToList();
        }

        public Service GetById(int id)
        {
            return  _context.Services.FirstOrDefault(x=> x.Id == id);
        }

        public void Add(Service service)
        {
            _context.Services.Add(service);
             SaveChanges();
        }

        public void  Update(Service service)
        {
            _context.Services.Update(service);
             SaveChanges();
        }

        public void Delete(int id)
        {
            var service = GetById(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                 SaveChanges();
            }
        }

        public IPagedList<Service> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public void AddHotelService(Hotel_Service hotelService)
        {
            _context.Hotel_Service.Add(hotelService);
            SaveChanges();
        }

        public int SaveChanges()
        {
          return _context.SaveChanges();
        }


        #region for Payment
        public async Task<List<Service>> GetServicesByIdsAsync(IEnumerable<int> serviceIds)
        {
            return await _context.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();
        }
        #endregion
    }
}
