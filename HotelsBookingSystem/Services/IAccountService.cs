using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Results;
using HotelsBookingSystem.ViewModels;
using HotelsBookingSystem.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
        Task<ExternalLoginResult> ProcessExternalLoginAsync(ClaimsPrincipal principal);
        Task<IdentityResult> CreateExternalUserAsync(ExternalLoginViewModel model);

    }
}
