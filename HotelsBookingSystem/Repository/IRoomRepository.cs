using HotelsBookingSystem.Models;

using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public interface IRoomRepository
    {

        IPagedList<Room> GetAll(int page, int pageSize);
        List<Room> GetAllroom( );
        Room GetById(int id);
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
        void SaveChanges();
        IPagedList<Room> FilterRooms(string type, int? minPrice, int? maxPrice,
                                   int? hotelId, string city, int pageNumber, int pageSize);
        List<Hotel> GetAllhotels();


    }
}
