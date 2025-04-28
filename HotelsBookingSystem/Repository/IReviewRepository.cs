using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.Repository
{
    public interface IReviewRepository
    {
        public List<Review> GetAll(int hotelId, int rating);
    }
}
