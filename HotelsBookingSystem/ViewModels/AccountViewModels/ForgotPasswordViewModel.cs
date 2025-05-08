using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AccountViewModels
{
        public class ForgotPasswordViewModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }
        }
}
