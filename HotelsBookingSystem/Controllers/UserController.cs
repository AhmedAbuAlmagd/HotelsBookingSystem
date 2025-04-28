using HotelsBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string name = "" , string country = "" , string city = "")
        {
            var usersQuery = _userManager.Users.Include(u => u.Bookings).OrderByDescending(u => u.Bookings.Count()).AsQueryable();

            if (name != "")
                usersQuery = usersQuery.Where(u => u.FullName.ToLower() == name.ToLower());

            if(country != "")
                usersQuery = usersQuery.Where(u => u.Country.ToLower() == country.ToLower());
            if (city != "")
                usersQuery = usersQuery.Where(u => u.City.ToLower() == city.ToLower());

            return View(usersQuery.ToList());
        }
    }
}
