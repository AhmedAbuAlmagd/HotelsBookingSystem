using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IBookingRepository bookingRepository, IHotelRepository hotelRepository, IPaymentRepository paymentRepository,
        UserManager<ApplicationUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _hotelRepository = hotelRepository;
            _paymentRepository = paymentRepository;
            _userManager = userManager;
        }

        [Authorize(Roles ="Admin")]
        public  async Task<IActionResult> Index(string status = "", int? hotelId = null,
                         DateTime? bookingDateFrom = null, DateTime? bookingDateTo = null,
                         string clientName = "", decimal? minPrice = null, decimal? maxPrice = null , int page = 1)
        {
            var filter = new BookingFilterViewModel
            {
                Status = status,
                HotelId = hotelId,
                BookingDateFrom = bookingDateFrom,
                BookingDateTo = bookingDateTo,
                ClientName = clientName,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var bookings = await _bookingRepository.GetByFilterAsync(filter);

            int totalItems = bookings.Count();
            int pageSize = 10;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            bookings = bookings
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToList();

            ViewBag.Hotels = _hotelRepository.GetHotelsWithRoomsAndImages()
                .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.Name }).Distinct()
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(bookings);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(DateTime checkIn, DateTime checkOut, decimal totalAmount)
        {
            var userId = _userManager.GetUserId(User);

            var booking = new Booking
            {
                Status = "Confirmed",
                Booking_date = DateTime.Now,
                CheckIn = checkIn,
                CheckOut = checkOut,
                GuestsCount = 2,
                paymentStatus = 1,
                TotalPrice = (int)totalAmount,
                HotelId = 1, 
                UserId = userId
            };

            _bookingRepository.AddBooking(booking);
            await _bookingRepository.SaveAsync(); 

            var payment = new Payment
            {
                BookingID = booking.Id,
                Amount = (int)totalAmount,
                status = "Paid",
                Method = "Online",
                Date = DateTime.Now,
                TransactionId = Guid.NewGuid().ToString()
            };

            _paymentRepository.AddPayment(payment);
            await _paymentRepository.SaveAsync();

            return RedirectToAction("Success");
        }

    }


}
