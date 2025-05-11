using HotelsBookingSystem.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public AboutController(ILogger<HomeController> logger, IHotelRepository hotelRepository, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Index()
        {

            var totalHotels = await _hotelRepository.GetTotalHotelsCountAsync();
            var totalRooms = await _roomRepository.GetTotalRoomsCountAsync();
            var totalUsers = await _userRepository.GetTotalUsersCountAsync();
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.TotalHotels = totalHotels;


            return View();
        }


    }
}
