using HotelsBookingSystem.Models;
using HotelsBookingSystem.Services;

namespace HotelsBookingSystem.Repository
{
    public interface IServiceRepository : IRepository<Service>
    {
        int GetCountByHotelId(int hotelId);
        List<Service> GetPagedByHotelId(int hotelId, int pageNumber, int pageSize);
        void AddHotelService(Hotel_Service hotelService);
    }
}
