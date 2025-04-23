using System.Collections.Generic;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Humanizer;
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
                typslist = roomRepository.GetAvailablerooms().Select(r => r.Type).Distinct().ToList(),
            }).ToPagedList(page, PageSize);
           // ViewData["forMap"] = roomRepository.GetAllhotels();

            #endregion

            return View("Index", roomViewModels);
        }

        #endregion



    }
}
