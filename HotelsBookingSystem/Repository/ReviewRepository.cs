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
        public List<Review> GetAll(int hotelId , int rating)
        {
            var query = _hotelsContext.Reviews.Include(x => x.Hotel).Include(x => x.User).AsQueryable();

            if(hotelId != 0)
                return query.Where(x => x.HotelId == hotelId).ToList();
            
            if (rating != 0)
                return query.Where(x => x.Rating == rating).ToList();

            return query.ToList();

        }
    }
}   
