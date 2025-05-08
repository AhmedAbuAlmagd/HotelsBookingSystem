using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HotelsBookingSystem.Services
{
    public interface IAccountService
    {
        Task<LoginResult> LoginAsync(LoginViewModel loginVM);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel registerVM);
        Task LogoutAsync();
        Task<ApplicationUser> FindEmail(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string password);


    }
}
