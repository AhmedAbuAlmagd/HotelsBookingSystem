using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class HotelController : Controller
    {
        public readonly IHotelRepository hotelRepository;
        public HotelController(IHotelRepository hotelRepository)
        {
            this.hotelRepository = hotelRepository;
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

    }
}
