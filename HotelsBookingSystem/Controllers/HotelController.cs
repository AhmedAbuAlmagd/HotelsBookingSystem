using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.Services;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard;
using HotelsBookingSystem.ViewModels.AdminViewModels.HotelDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class HotelController : Controller
    {
        private readonly IHotelRepository hotelRepository;
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;

        public HotelController(IHotelRepository hotelRepository ,IHotelService hotelService , ILogger<HotelController> logger
         , IWebHostEnvironment webHostEnvironment , IRoomRepository roomRepository , IServiceRepository serviceRepository)
        {
            this.hotelRepository = hotelRepository;
            _hotelService = hotelService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
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
                Services = h.HotelServices.Select(hs => hs.Service).ToList(),
                Description = h.Description,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                HotelImages = h.HotelImages.Select(img => img.ImageUrl).ToList()
               
            }).ToList();

            return View("AllHotels", hotelViews);
        }

        public IActionResult Services(int hotelId)
        {
            var hotel = hotelRepository.GetHotelWithServices(hotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            var viewModel = new HotelModelView
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Services = hotel.HotelServices.Select(hs => hs.Service).ToList()
            };

            return View("services", viewModel);
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

        #region Admin

        [Authorize(Roles = "Admin")]
        public IActionResult HotelsManagement(int page = 1, string searchTerm = "", string status = "", string city = "")
        {
            const int pageSize = 8;

            status = string.IsNullOrWhiteSpace(status) ? null : status;
            city = string.IsNullOrWhiteSpace(city) ? null : city;
            searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm;

            var hotels = _hotelService.GetHotelsPaged(page, pageSize, searchTerm, status, city);
            var totalHotels = _hotelService.GetTotalHotelsCount(searchTerm, status, city);
            var totalPages = (int)Math.Ceiling((double)totalHotels / pageSize);

            ViewBag.CurrentSearchTerm = searchTerm;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentCity = city;

            var viewModel = new HotelsManagementViewModel
            {
                Hotels = hotels,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(HotelFormViewModel hotel, IFormFile image)
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
        [HttpGet]
        public IActionResult GetHotel(int id)
        {
            var hotel = hotelRepository.GetById(id);
            if (hotel == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = hotel.Id,
                name = hotel.Name,
                city = hotel.City,
                location = hotel.Address,
                phone = hotel.Phone,
                description = hotel.Description,
                rating = hotel.rating,
                status = hotel.Status
                
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id, int roomPage = 1, int servicePage = 1, int pageSize = 6, string activeTab = "hotel-info")
        {
            try
            {
                var hotel = hotelRepository.GetById(id);
                if (hotel == null)
                {
                    return NotFound();
                }

                // Get total count and paginated data
                var totalRooms = _roomRepository.GetCountByHotelId(id);
                var totalServices = _serviceRepository.GetCountByHotelId(id);

                var rooms = _roomRepository.GetPagedByHotelId(id, roomPage, pageSize);
                var services = _serviceRepository.GetPagedByHotelId(id, servicePage, pageSize);

                var viewModel = new HotelDetailsPageViewModel
                {
                    Hotel = hotel,
                    Rooms = rooms,
                    Services = services,
                    RoomPagination = new PaginationInfo
                    {
                        CurrentPage = roomPage,
                        ItemsPerPage = pageSize,
                        TotalItems = totalRooms,
                        TotalPages = (int)Math.Ceiling(totalRooms / (double)pageSize)
                    },
                    ServicePagination = new PaginationInfo
                    {
                        CurrentPage = servicePage,
                        ItemsPerPage = pageSize,
                        TotalItems = totalServices,
                        TotalPages = (int)Math.Ceiling(totalServices / (double)pageSize)
                    }
                };

                ViewBag.ActiveTab = activeTab;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel details");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id ,HotelFormViewModel hotel , IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string uploadsPhoto = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Hotels");
                string filePath = Path.Combine(uploadsPhoto, image.FileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(fileStream);
                hotel.ImageUrl = "/images/Hotels/" + image.FileName;
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


      

      

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        #endregion

    }
}
