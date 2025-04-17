using HotelsBookingSystem.Models;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HotelsBookingSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<(bool Succeeded, string ErrorMessage, ApplicationUser User)> LoginAsync(LoginViewModel loginVM)
        {
            var user = await userManager.FindByNameAsync(loginVM.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                await signInManager.SignInAsync(user, loginVM.RememberMe);
                return (true, string.Empty, user);
            }
            return (false, "UserName or Password is incorrect.", null);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel registerVM)
        {
            var user = new ApplicationUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FullName = registerVM.FullName,
                Country = registerVM.Country,
                City = registerVM.City,
                NationalId = registerVM.NationalId
            };

            var result = await userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                //await userManager.AddToRoleAsync(user, "User");
                return (true, Enumerable.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description));
        }

    }
}
