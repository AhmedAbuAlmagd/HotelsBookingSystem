using HotelsBookingSystem.Models;
using X.PagedList;

namespace HotelsBookingSystem.Repository
{
    public interface IRepository<T>
    {
        IPagedList<T> GetAll(int page, int pageSize);
        T GetById(int id);
        void Add(T t);
        void Update(T t);
        void Delete(int id);
        int SaveChanges();
    }
}
