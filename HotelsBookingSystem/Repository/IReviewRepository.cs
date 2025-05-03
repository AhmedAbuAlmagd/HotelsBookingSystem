using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IReviewRepository:IRepository<Review>
    {
      public List<Review> GetAll(int hotelId, int rating);
    }  

}
