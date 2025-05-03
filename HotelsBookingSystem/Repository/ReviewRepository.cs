using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly HotelsContext _context;
        public ReviewRepository(HotelsContext context)
        {
            _context = context;
        }
        #region add review

        public void Add(Review t)
        {
            _context.Reviews.Add(t);

        }
        public int SaveChanges()
        {

            return _context.SaveChanges();
        }
        #endregion

        #region justfor interface
        public void Delete(int id)
        {

            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            else
            {
                throw new Exception("Review not found");
            }
        }

        public IPagedList<Review> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Review GetById(int id)
        {
            throw new NotImplementedException();
        }

       
        public void Update(Review t)
        {
            throw new NotImplementedException();
        }
        #endregion
        
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

