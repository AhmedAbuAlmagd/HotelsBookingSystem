using System.Collections.Generic;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace HotelsBookingSystem.Controllers
{

    public class RoomController : Controller
    {
        private readonly IRoomRepository roomRepository;
        private readonly ILogger<RoomController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        public RoomController(IRoomRepository roomRepo , ILogger<RoomController> logger , IWebHostEnvironment webHostEnvironment)
        {
            roomRepository = roomRepo;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }
        #region index
        public IActionResult Index(int page = 1)
        {
            int PageSize = 10;
            var rooms = roomRepository.GetAllroom();

            var roomViewModels = rooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                Description = r.Description,
                Type = r.Type,
                Status = r.Status,
                PricePerNight = r.PricePerNight,
                RoomImages = r.RoomImages.Select(img => img.ImageUrl).ToList(),
                hotel = r.Hotel,
                hotels = roomRepository.GetAllhotels(),
                RoomNumber = r.RoomNumber,
                NumberOfBeds = r.NumberOfBeds,                
                typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),

            }).ToPagedList(page, PageSize);
          //  ViewData["forMap"] = roomRepository.GetAllhotels();
            return View(roomViewModels);
        }
        #endregion
        #region detail
        public IActionResult Detail(int id)
        {

            Room room = roomRepository.GetById(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);

        }
        #endregion


       

        #region filter
        public IActionResult AvailableRooms(DateTime checkIn, DateTime checkOut, int adults, int children, int page = 1)
        {
            int pageSize = 3;


            var allRooms = roomRepository.GetAllroom();

          
            var availableRooms = allRooms
               .Where(room => !room.BookingRooms
              .Any(b =>
             b.booking.CheckIn.HasValue && b.booking.CheckOut.HasValue &&
             b.booking.CheckIn.Value <= checkOut &&
             b.booking.CheckOut.Value >= checkIn)); 


            var roomViewModels = availableRooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                Description = r.Description,
                Type = r.Type,
                Status = r.Status,
                PricePerNight = r.PricePerNight,
                RoomImages = r.RoomImages?.Select(img => img.ImageUrl).ToList(),
                hotel = r.Hotel,
                hotels = roomRepository.GetAllhotels()
            }).ToPagedList(page, pageSize);

            return View("Index", roomViewModels); 
        }

        public IActionResult FilterRooms(
            int page = 1,  string type = null,  int? minPrice = null, int? maxPrice = null,
            int? hotelId = null,  string city = null)
        {
            int PageSize = 3;
            List<Hotel> hotellist = roomRepository.GetAllhotels();


            #region iftheyAllEqAll
            if (type == "All" && !minPrice.HasValue && !maxPrice.HasValue && (hotelId == null || hotelId == 0) && city == "All")
            {
                ViewBag.Message = "Please provide min Price or max Price criteria.";

                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel { hotels = hotellist ,
                        typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
                        citys = hotellist.Select(h => h.City).Distinct().ToList()

                    }
                };
                if (hotelId == 0)
                    hotelId = null;

                ViewData["CurrentType"] = type;
                ViewData["CurrentMinPrice"] = minPrice;
                ViewData["CurrentMaxPrice"] = maxPrice;
                ViewData["CurrentHotelId"] = hotelId?.ToString() ?? "All";
                ViewData["CurrentCity"] = city;

                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }

            #endregion

            #region conversion to null
            if (string.IsNullOrWhiteSpace(type) || type == "All")
                type = null;
            if (string.IsNullOrWhiteSpace(city) || city == "All")
                city = null;
            if (hotelId == 0)
                hotelId = null;
            #endregion


            ViewData["CurrentType"] = type ?? "All";
            ViewData["CurrentMinPrice"] = minPrice;
            ViewData["CurrentMaxPrice"] = maxPrice;
            ViewData["CurrentHotelId"] = hotelId?.ToString() ?? "All"; 
            ViewData["CurrentCity"] = city ?? "All";

            #region ifAllisNull
            if (type == null && !minPrice.HasValue && !maxPrice.HasValue && hotelId == null && city == null)
            {
                ViewBag.Message = "Please provide at least one filter criteria.";
                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel { hotels = hotellist,
                    typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
                        citys = hotellist.Select(h => h.City).Distinct().ToList()

                    }
                };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }
            #endregion

           

            IPagedList<Room> filteredRooms = roomRepository.FilterRooms(
                type, minPrice, maxPrice, hotelId, city, page, PageSize);

            #region emptyview

            if (filteredRooms == null || !filteredRooms.Any())
            {
                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel { hotels = hotellist }
                };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }
            #endregion

            #region modelmaping
            var roomViewModels = filteredRooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                Description = r.Description,
                Type = r.Type,
                Status = r.Status,
                PricePerNight = r.PricePerNight,
                RoomImages = r.RoomImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                hotel = r.Hotel,
                hotels = hotellist,
                RoomNumber = r.RoomNumber,
                NumberOfBeds = r.NumberOfBeds,
                typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
            }).ToPagedList(page, PageSize);
           // ViewData["forMap"] = roomRepository.GetAllhotels();

            #endregion

            return View("Index", roomViewModels);
        }

        #endregion

        #region Admin
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRoom(int id)
        {
            try
            {
                var room =  roomRepository.GetById(id);
                if (room == null)
                {
                    return NotFound(new { success = false, message = "Room not found" });
                }

                return Json(room);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting room");
                return StatusCode(500, new { success = false, message = "An error occurred retrieving room details" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ViewModels.AdminViewModels.RoomViewModelAd model , IFormFile image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }

                string uploadsPhoto = Path.Combine(webHostEnvironment.WebRootPath, "images", "Rooms");
                string filePath = Path.Combine(uploadsPhoto, image.FileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(fileStream);
                model.image = "/images/Rooms/" + image.FileName;

                var room = new Room
                {
                    HotelId = model.HotelId,
                    Type = model.Type,
                    PricePerNight = model.PricePerNight,
                    NumberOfBeds = model.NumberOfBeds,
                    Description = model.Description,
                    Status = model.Status,
                    
                };

                room.RoomImages.Add(new RoomImage { ImageUrl = model.image, IsPrimary = true });

                 roomRepository.Add(room);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating room");
                return StatusCode(500, new { success = false, message = "An error occurred creating the room" });
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, ViewModels.AdminViewModels.RoomViewModelAd model , IFormFile image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });
                }

                var room =  roomRepository.GetById(id);
                if (room == null)
                {
                    return NotFound(new { success = false, message = "Room not found" });
                }

                room.Type = model.Type;
                room.PricePerNight = model.PricePerNight;
                room.NumberOfBeds = model.NumberOfBeds;
                room.Description = model.Description;
                room.Status = model.Status;

                string uploadsPhoto = Path.Combine(webHostEnvironment.WebRootPath, "images", "Rooms");
                string filePath = Path.Combine(uploadsPhoto, image.FileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(fileStream);
                model.image = "/images/Rooms/" + image.FileName;

                if (model.image != null)
                {

                    room.RoomImages.FirstOrDefault(r => r.IsPrimary == true).ImageUrl = model.image;
                }

                 roomRepository.Update(room);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating room");
                return StatusCode(500, new { success = false, message = "An error occurred updating the room" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var room =  roomRepository.GetById(id);
                if (room == null)
                {
                    return NotFound(new { success = false, message = "Room not found" });
                }

                 roomRepository.Delete(id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting room");
                return StatusCode(500, new { success = false, message = "An error occurred deleting the room" });
            }
        }

        #endregion

    }
}
