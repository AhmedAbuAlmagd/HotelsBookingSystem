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
            var rooms = roomRepository.GetAvailablerooms();

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
                typslist = roomRepository.GetAvailablerooms().Select(r => r.Type).Distinct().ToList(),
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
            string hotelIdStr = null,  
            string city = null)
        {
            int PageSize = 3;
            List<Hotel> hotellist = roomRepository.GetAllhotels();

           
            if (type == "All") type = null;
            if (city == "All") city = null;

            int? hotelId = null;
            if (!string.IsNullOrEmpty(hotelIdStr) && hotelIdStr != "All")
            {
                if (int.TryParse(hotelIdStr, out int parsedId))
                    hotelId = parsedId;
            }

            
            ViewData["CurrentType"] = type ?? "All";
            ViewData["CurrentMinPrice"] = minPrice;
            ViewData["CurrentMaxPrice"] = maxPrice;
            ViewData["CurrentHotelId"] = hotelIdStr ?? "All";
            ViewData["CurrentCity"] = city ?? "All";

           
            if (type == null && !minPrice.HasValue && !maxPrice.HasValue && hotelId == null && city == null)
            {
                ViewBag.Message = "Please provide at least one filter criteria.";
                var emptyRoomViewModel = new List<RoomViewModel>
            {
                new RoomViewModel { hotels = hotellist }
            };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }

             
            var filteredRooms = roomRepository.FilterRooms(type, minPrice, maxPrice, hotelId, city, page, PageSize);

            if (filteredRooms == null || !filteredRooms.Any())
            {
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
                RoomNumber = r.RoomNumber,
                NumberOfBeds = r.NumberOfBeds,
                typslist = roomRepository.GetAvailablerooms().Select(r => r.Type).Distinct().ToList(),
            }).ToPagedList(page, PageSize);

            return View("Index", roomViewModels);
        }

        #endregion
    }
}
