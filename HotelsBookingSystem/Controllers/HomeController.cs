using System.Diagnostics;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IHotelRepository hotelRepository, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if(User.IsInRole("Admin"))
                return RedirectToAction("DAshboard","Admin");
            var totalHotels = await _hotelRepository.GetTotalHotelsCountAsync();
            var totalRooms = await _roomRepository.GetTotalRoomsCountAsync();
            var totalUsers = await _userRepository.GetTotalUsersCountAsync();
            var topRooms = await _roomRepository.GetTopRoomsAsync(3);
            var topRatedHotels = await _hotelRepository.GetTopRatedHotelsAsync(4);
            var reviews = await _hotelRepository.GetRecentReviewsAsync(5);
            ViewBag.TopRatedHotels = topRatedHotels;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.TotalHotels = totalHotels;  
            ViewBag.TopRooms = topRooms;        
            ViewBag.Reviews = reviews;
            foreach (var room in ViewBag.TopRooms)
            {
                if (room.RoomImages == null)
                {
                    room.RoomImages = new List<RoomImage>();
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
