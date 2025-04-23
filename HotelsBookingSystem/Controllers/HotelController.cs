using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class HotelController : Controller
    {
        private readonly IHotelRepository hotelRepository;
        private readonly IHotelService _hotelService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HotelController(IHotelRepository hotelRepository ,IHotelService hotelService , IWebHostEnvironment webHostEnvironment)
        {
            this.hotelRepository = hotelRepository;
            _hotelService = hotelService;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult HotelsManagement(int page = 1)
        {
            const int pageSize = 9; 
            var hotels = _hotelService.GetHotelsPaged(page, pageSize);
            var totalHotels = _hotelService.GetTotalHotelsCount();
            var totalPages = (int)Math.Ceiling((double)totalHotels / pageSize);

            var viewModel = new HotelsManagementViewModel
            {
                Hotels = hotels,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Update(int id ,HotelFormViewModel hotel)
        {
            if (ModelState.IsValid)
            {
                _hotelService.UpdateHotel(id, hotel);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction(nameof(HotelsManagement));
            }

            else
            {
                var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                   );
                return Json(new { success = false, errors });
            }
           
        }


        public IActionResult GetHotel(int id)
        {
            var hotel = _hotelService.GetHotelDetails(id);
            if (hotel == null)
                return NotFound();

            return Json(hotel);
        }

        [HttpPost]
        public IActionResult Create(HotelFormViewModel hotel , IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string uploadsPhoto = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Hotels");
                string filePath = Path.Combine(uploadsPhoto, image.FileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(fileStream);
                hotel.ImageUrl = "/images/Hotels/" + image.FileName;
                _hotelService.AddHotel(hotel);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });
                return RedirectToAction(nameof(HotelsManagement));
            }

            else
            {
                var errors = ModelState.ToDictionary(
                 kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
               );
                return Json(new { success = false, errors });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _hotelService.DeleteHotel(id);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction(nameof(HotelsManagement));
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }
                // Handle non-AJAX request error
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(HotelsManagement));
            }
        }

    }
}
