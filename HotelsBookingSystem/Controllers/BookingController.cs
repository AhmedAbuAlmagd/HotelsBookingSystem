using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHotelRepository _hotelRepository;

        public BookingController(IBookingRepository bookingRepository, IHotelRepository hotelRepository)
        {
            _bookingRepository = bookingRepository;
            _hotelRepository = hotelRepository;
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

    }


}
