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
        public IActionResult Index(string name = "", string country = "", string city = "", int page = 1)
        {
            var usersQuery = _userManager.Users.Include(u => u.Bookings)
                .OrderByDescending(u => u.Bookings.Count())
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                usersQuery = usersQuery.Where(u => u.FullName.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(country))
                usersQuery = usersQuery.Where(u => u.Country.ToLower().Contains(country.ToLower()));

            if (!string.IsNullOrEmpty(city))
                usersQuery = usersQuery.Where(u => u.City.ToLower().Contains(city.ToLower()));

            int pageSize = 10;
            int totalItems = usersQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var users = usersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Name = name;
            ViewBag.Country = country;
            ViewBag.City = city;

            return View(users);
        }
    }
}
