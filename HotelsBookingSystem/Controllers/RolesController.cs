using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> rolemanager;
        private readonly UserManager<ApplicationUser> userManager;

        public RolesController(RoleManager<IdentityRole> rolemanager , UserManager<ApplicationUser> userManager)
        {
            this.rolemanager = rolemanager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> New(RoleViewModel roleVm)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole();
                role.Name = roleVm.Role;
                IdentityResult result = await rolemanager.CreateAsync(role);
                if (result.Succeeded)
                    return View("New" ,new RoleViewModel());
                else
                {
                    foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                    return View(roleVm);
                }
            }
            else
            return View(roleVm);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);
            await userManager.AddToRoleAsync(user, role);

            return RedirectToAction("index","User");
        }
    }
}
 







 

 




 







 


