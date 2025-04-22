using HotelsBookingSystem.Models;

using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public interface IRoomRepository
    {

        IPagedList<Room> GetAll(int page, int pageSize);
        Room GetById(int id);
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
        void SaveChanges();
        IPagedList<Room> FilterRooms(string type, int? minPrice, int? maxPrice,
                                   int? hotelId, string city, int pageNumber, int pageSize);
        List<Hotel> GetAllhotels();
        List<Room> GetAllroom();
        // For Admin
        Task<List<Room>> GetAllRoomsAsync();
        Task<int> GetTotalRoomsCountAsync();
        Task<List<Room>> GetTopRoomsAsync(int count);

    }
}
