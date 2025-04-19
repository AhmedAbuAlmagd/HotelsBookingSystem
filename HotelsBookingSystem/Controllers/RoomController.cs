using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;

namespace HotelsBookingSystem.Controllers
{

    public class RoomController : Controller
    { 
        private readonly IRoomRepository roomRepository;
        public RoomController(IRoomRepository roomRepo)
        {
            roomRepository = roomRepo;
        }

        public IActionResult Index(int page = 1)
        {
            int PageSize = 3;
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
                hotels = roomRepository.GetAllhotels()
            }).ToPagedList(page, PageSize);

            return View(roomViewModels);
        }

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

        public IActionResult FilterRooms(
     int page = 1,
     string type = null,
     int? minPrice = null,
     int? maxPrice = null,
     int? hotelId = null,
     string city = null)
        {
            int PageSize = 3;
            List<Hotel> hotellist = roomRepository.GetAllhotels();

            // Check if all filter parameters are null
            if (string.IsNullOrEmpty(type) &&
                !minPrice.HasValue &&
                !maxPrice.HasValue &&
                !hotelId.HasValue &&
                string.IsNullOrEmpty(city))
            {
               // ViewBag.FilterError = "Please provide at least one filter criteria";
                var emptyRoomViewModel = new List<RoomViewModel>
        {
            new RoomViewModel { hotels = hotellist }
        };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }

            IPagedList<Room> filteredRooms = roomRepository.FilterRooms(
                type, minPrice, maxPrice, hotelId, city, page, PageSize);

            // Store filter values in ViewData to repopulate form
            ViewData["CurrentType"] = type;
            ViewData["CurrentMinPrice"] = minPrice;
            ViewData["CurrentMaxPrice"] = maxPrice;
            ViewData["CurrentHotelId"] = hotelId;
            ViewData["CurrentCity"] = city;

            if (filteredRooms == null || !filteredRooms.Any())
            {
                ViewBag.Message = "No rooms found matching your criteria.";
                var emptyRoomViewModel = new List<RoomViewModel>
        {
            new RoomViewModel { hotels = hotellist }
        };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }

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
            }).ToPagedList(page, PageSize);

            return View("Index", roomViewModels);
        }

        #endregion



    }
}
