using HotelsBookingSystem.Models;
using PagedList;

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
    }
}
