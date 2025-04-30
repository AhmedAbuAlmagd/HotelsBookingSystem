using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HotelsBookingSystem.Controllers
{
    [Authorize]

    public class ReviewsController : Controller
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IHotelRepository hotelRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReviewsController(IReviewRepository reviewRepo, IHotelRepository hotelRepo, UserManager<ApplicationUser> userManager)
        {
            reviewRepository = reviewRepo;
            hotelRepository = hotelRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {


            if (!User.Identity.IsAuthenticated)
            {
                ViewData["Message"] = "Please log in to leave a review.";
            }

            var hotels = hotelRepository.GetHotelsWithRoomsAndImages();
            var model = new ReviewViewModel
            {
                Comment = "",
                HotelName = "",
                UserName = "",
                HotelId = 0,
                Rating = 0,

                hotels = hotels
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create( ReviewViewModel reviewVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); 
            }
            int hotelid = reviewVM.HotelId ?? 0;    
            var hotel = hotelRepository.GetById(hotelid);
            if (hotel == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name;
            

            var review = new Review
            {

                HotelId = hotelid,
               
                Comment = reviewVM.Comment,
                Rating = reviewVM.Rating,
                User = user,


                Hotel = hotel,
                UserId = userId,
                
            };
            if (ModelState.IsValid)
            {

                reviewRepository.Add(review);
                reviewRepository.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(review);
             
        }

        
        //[HttpPost]
        //public IActionResult Create(Review review)
        //{
        //    if (ModelState.IsValid)
        //    {
               
        //        reviewRepository.Add(review);
        //        reviewRepository.SaveChanges();
        //        return RedirectToAction("Index"); 
        //    }

           
        //    return View(review);
        //}
    }
}
