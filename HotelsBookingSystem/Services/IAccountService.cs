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
        #region External Login
        Task<ApplicationUser> FindByExternalLoginAsync(string provider, string providerKey);
        Task<IdentityResult> CreateExternalUserAsync(ApplicationUser user, ExternalLoginInfo loginInfo);
        Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalLoginSignInAsync(string provider, string providerKey, bool isPersistent);
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        #endregion

    }
}
