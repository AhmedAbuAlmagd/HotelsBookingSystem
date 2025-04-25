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
            int PageSize = 6;
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
                City = null,
                TypeFilter = null,
                MinPrice = null,
                MaxPrice = null,
                HotelId = null,


            }).ToPagedList(page, PageSize);
            ViewBag.IsFiltered = false;
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
       
        public IActionResult filter(RoomViewModel roomVM, int page = 1)
        {
            int PageSize = 3;
            List<Hotel> hotellist = roomRepository.GetAllhotels();
            #region Validation
            if (roomVM.MinPrice.HasValue && roomVM.MinPrice < 1)
            {
                ModelState.AddModelError("MinPrice", "Minimum price must be at least 0.");
            }

            if (roomVM.MaxPrice.HasValue && roomVM.MaxPrice < 1)
            {
                ModelState.AddModelError("MaxPrice", "Maximum price must be at least 0.");
            }

            if (roomVM.MinPrice.HasValue && roomVM.MaxPrice.HasValue && roomVM.MinPrice > roomVM.MaxPrice)
            {
                ModelState.AddModelError("MaxPrice", "Maximum price must be greater than minimum price.");
            }


            #endregion
            #region iftheyAllEqAll
            if (roomVM.TypeFilter == "All" && !roomVM.MinPrice.HasValue && !roomVM.MaxPrice.HasValue && (roomVM.HotelId == null || roomVM.HotelId == 0) && roomVM.City == "All")
            {
                ViewBag.Message = "Please provide min Price or max Price criteria.";

                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel {
                        hotels = hotellist ,
                        typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
                        citys = hotellist.Select(h => h.City).Distinct().ToList(),
                        TypeFilter = roomVM.TypeFilter,
                        MinPrice = roomVM.MinPrice,
                        MaxPrice = roomVM.MaxPrice,
                        HotelId = roomVM.HotelId,
                        City = roomVM.City
                    }
                };
                if (roomVM.HotelId == 0)
                    roomVM.HotelId = null;



                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }

            #endregion

            #region conversion to null
            if (string.IsNullOrWhiteSpace(roomVM.TypeFilter) || roomVM.TypeFilter == "All")
                roomVM.TypeFilter = null;
            if (string.IsNullOrWhiteSpace(roomVM.City) || roomVM.City == "All")
                roomVM.City = null;
            if (roomVM.HotelId == 0)
                roomVM.HotelId = null;
            #endregion

            #region ifAllisNull
            if (roomVM.TypeFilter == null && !roomVM.MinPrice.HasValue && !roomVM.MaxPrice.HasValue && roomVM.HotelId == null && roomVM.City == null)
            {
                ViewBag.Message = "Please provide at least one filter criteria.";
                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel {
                        hotels = hotellist,
                        typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
                        citys = hotellist.Select(h => h.City).Distinct().ToList(),
                         TypeFilter = roomVM.TypeFilter,
                        MinPrice = roomVM.MinPrice,
                        MaxPrice = roomVM.MaxPrice,
                        HotelId = roomVM.HotelId,
                        City = roomVM.City
                    }
                };
                return View("Index", emptyRoomViewModel.ToPagedList(page, PageSize));
            }
            #endregion

            IPagedList<Room> filteredRooms = roomRepository.FilterRooms(roomVM, page, PageSize);
            #region emptyview

            if (filteredRooms == null || !filteredRooms.Any())
            {
                var emptyRoomViewModel = new List<RoomViewModel>
                {
                    new RoomViewModel { hotels = hotellist,
                    typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
                        citys = hotellist.Select(h => h.City).Distinct().ToList(),
                     TypeFilter = roomVM.TypeFilter,
                        MinPrice = roomVM.MinPrice,
                        MaxPrice = roomVM.MaxPrice,
                        HotelId = roomVM.HotelId,
                        City = roomVM.City

                    }
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
                ////
                City = roomVM.City,
                TypeFilter = roomVM.TypeFilter,
                MinPrice = roomVM.MinPrice,
                MaxPrice = roomVM.MaxPrice,
                HotelId = roomVM.HotelId,
                typslist = roomRepository.GetAllroom().Select(r => r.Type).Distinct().ToList(),
            });


            #endregion
            ViewBag.IsFiltered = true;
            return View("Index", roomViewModels);
        }

        #endregion
       
    
    }
}
