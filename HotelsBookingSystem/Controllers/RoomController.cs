using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace HotelsBookingSystem.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository roomRepository;
        public RoomController(IRoomRepository roomRepo) {
            roomRepository = roomRepo;
        }
        public IActionResult Index(int page =1 )
        {
            int pageSize = 10;
            IPagedList<Room> rooms = roomRepository.GetAll(page, pageSize);
            return View(rooms);
        }


        public IActionResult Detail(int id) { 
            
            Room room = roomRepository.GetById(id);
            if (room == null) {
                return NotFound();
            }
           return View(room);
        
        }
    }
}
