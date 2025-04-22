using HotelsBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = await _adminService.GetDashboardDataAsync();
            return View(dashboardData);
        }
        
    }
}
