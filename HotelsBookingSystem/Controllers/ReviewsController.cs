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
        #region index
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
        #endregion


        #region create
        [HttpPost]
      
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create( ReviewViewModel reviewVM)
        {
            reviewVM.hotels = hotelRepository.GetHotelsWithRoomsAndImages();

            #region user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); 
            }
            #endregion

            if (string.IsNullOrWhiteSpace(reviewVM.Comment))
            {
                ModelState.AddModelError(nameof(reviewVM.Comment), "Please enter a comment.");
            }

            
            if(reviewVM.Rating == 0|| (reviewVM.Rating == null || reviewVM.Rating < 1 || reviewVM.Rating > 5))
            {
                ModelState.AddModelError("Rating", "Please select a rating .");
                ViewBag.Message = "Please select a rating ";
                return View("index",reviewVM);
            }
            #region hotel dropdown
            int hotelid = reviewVM.HotelId ;    
            var hotel = hotelRepository.GetById(hotelid);
            if (hotel == null)
            {
                return NotFound();
            }
            #endregion
            var userId = _userManager.GetUserId(User);

            #region review
            var review = new Review
            {

                HotelId = hotelid,
               
                Comment = reviewVM.Comment,
                Rating = reviewVM.Rating,
                User = user,


                Hotel = hotel,
                UserId = userId,
                
            };
            #endregion
            if (ModelState.IsValid)
            {

                reviewRepository.Add(review);
                reviewRepository.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(reviewVM);
             
        }

        #endregion
    }
}
