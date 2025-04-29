using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IHotelRepository hotelRepository;

        public ReviewsController(IReviewRepository reviewRepo, IHotelRepository hotelRepo)
        {
            reviewRepository = reviewRepo;
            hotelRepository = hotelRepo;

        }
        public IActionResult Index()
        {
            var hotels = hotelRepository.GetHotelsWithRoomsAndImages();  
            var model = new ReviewViewModel
            {
                hotels = hotels
            };
            return View(model);
           
        }

        //public IActionResult Create(int hotelId)
        //{
        //    var hotel = hotelRepository.GetById(hotelId);
        //    if (hotel == null)
        //    {
        //        return NotFound();
        //    }
        //    var review = new Review
        //    {
        //        HotelId = hotelId,
        //        Hotel = hotel
        //    };
        //    return View(review);
        //}
        
    }
}
