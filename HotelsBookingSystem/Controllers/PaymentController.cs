using System.Security.Claims;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;


namespace HotelsBookingSystem.Controllers
{

    public class PaymentController : Controller
    {

        private readonly IBookingRepository _bookingRepository;


        public PaymentController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }


        BookingViewModel bookingViewModel;
        public IActionResult Index()
        {

            return View();
        }

        // [HttpPost]
        // public IActionResult CreatePayment()
        [HttpPost]
        public IActionResult CreatePayment(DateTime checkIn, DateTime checkOut, decimal totalAmount)

        {
            bookingViewModel = new BookingViewModel();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalAmount * 100),
                Currency = "USD",
                PaymentMethodTypes = new List<string> { "card" },
                Description = $"Booking Room from {checkIn} To {checkOut}",
            };

            //var options = new PaymentIntentCreateOptions
            //{
            //    Amount = (long)(bookingViewModel.TotalPrice * 100),
            //    Currency = "USD",
            //    PaymentMethodTypes = new List<string>
            //    {
            //      "card",
            //    },
            //    Description = $"Booking Room from {bookingViewModel.CheckIn} To {bookingViewModel.CheckOut}",
            //};
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = new Booking
            {
                UserId = userId,
                Status = "Confirmed",
                Booking_date = DateTime.Now,
                HotelId = 1,
                TotalPrice = bookingViewModel.TotalPrice,
                CheckIn = bookingViewModel.CheckIn,
                CheckOut = bookingViewModel.CheckOut

            };

            _bookingRepository.AddBooking(booking);


            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Json(new
            {
                clientSecret = paymentIntent.ClientSecret
            });
        }

        public IActionResult Success()
        {
            return View();
        }

    }
}
