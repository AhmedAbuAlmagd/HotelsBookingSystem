using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly HotelsContext _hotelsContext;

        public ReviewRepository(HotelsContext hotelsContext)
        {
            _hotelsContext = hotelsContext;
        }
       public List<Review> GetAll(int hotelId = 0, int rating = 0)
        {
            var query = _hotelsContext.Reviews
                .Include(r => r.Hotel)
                .Include(r => r.User)
                .AsQueryable();

            if (hotelId > 0)
            {
                query = query.Where(r => r.HotelId == hotelId);
            }

            if (rating > 0)
            {
                query = query.Where(r => r.Rating == rating);
            }

            return query.ToList();
        }
    }
}   
