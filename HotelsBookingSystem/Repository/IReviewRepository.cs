using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IReviewRepository:IRepository<Review>
    {
      public List<Review> GetAllReviews(int hotelId, int rating);
    }  

}
