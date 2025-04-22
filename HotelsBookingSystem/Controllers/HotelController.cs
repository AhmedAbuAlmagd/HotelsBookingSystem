using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class HotelController : Controller
    {
        private readonly IHotelRepository hotelRepository;
        private readonly IHotelService _hotelService;
        public HotelController(IHotelRepository hotelRepository ,IHotelService hotelService)
        {
            this.hotelRepository = hotelRepository;
            _hotelService = hotelService;
        }


        public IActionResult AddHolelForm()
        {
             Hotel hotel = new Hotel();
            return View("AddHolelForm",hotel);
        }
        //public IActionResult Index()
        //{
        //    var hotels =hotelRepository.GetHotelsWithRoomsAndImages();

        //    var hotelViewModels = hotels.Select(h => new HotelModelView
        //    {
        //        Id = h.Id,
        //        Name = h.Name,
        //        Address = h.Address,
        //        Phone = h.Phone,
        //        City = h.City,
        //        Latitude = h.Latitude,
        //        Longitude = h.Longitude,
        //        Description = h.Description,
        //        Status = h.Status,
        //        HotelImages = h.HotelImages.Select(img => img.ImageUrl).ToList(),
        //        hotelRooms = h.Rooms.ToList(),
        //        roomimages = h.Rooms
        //                        .SelectMany(r => r.RoomImages)
        //                        .Select(img => img.ImageUrl)
        //                        .ToList()
        //    }).ToList();

        //    return View("AllHotels", hotelViewModels);
        //}
        public IActionResult Index()
        {
            var hotels = hotelRepository.GetHotelsWithRoomsAndImages(); 
            var hotelViews = hotels.Select(h => new HotelModelView
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                City = h.City,
                Phone = h.Phone,
                Description = h.Description,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                HotelImages = h.HotelImages.Select(img => img.ImageUrl).ToList()
               
            }).ToList();

            return View("AllHotels", hotelViews);
        }
        public IActionResult ViewRooms(int hotelId)
        {
            var hotel = hotelRepository.GetHotelWithRoomsAndImages(hotelId);
            if (hotel == null) return NotFound();

            var hotelView = new HotelModelView
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Description = hotel.Description,
                Phone = hotel.Phone,
                HotelImages = hotel.HotelImages.Select(img => img.ImageUrl).ToList(),
                hotelRooms = hotel.Rooms,
            };

            return View("ViewRooms", hotelView);
        }

        public IActionResult HotelsManagement()
        {
            var hotels = _hotelService.GetAllHotels();
            var viewModel = new HotelsManagementViewModel { Hotels = hotels };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Update(int id ,HotelFormViewModel hotel)
        {

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            _hotelService.UpdateHotel(id, hotel);
            var hotels = _hotelService.GetAllHotels();
            var viewModel = new HotelsManagementViewModel { Hotels = hotels };
            return RedirectToAction(nameof(HotelsManagement),viewModel);
        }


        public IActionResult GetHotel(int id)
        {
            var hotel = _hotelService.GetHotelDetails(id);
            if (hotel == null)
                return NotFound();

            return Json(hotel);
        }
    }
}
