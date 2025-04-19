using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelsBookingSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> rolemanager;
        public RolesController(RoleManager<IdentityRole> rolemanager)
        {
            this.rolemanager = rolemanager;
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
    }
}
 







 

 




 







 


