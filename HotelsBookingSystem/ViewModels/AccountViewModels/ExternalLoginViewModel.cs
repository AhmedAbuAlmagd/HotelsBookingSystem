using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AccountViewModels
{
    public class ExternalLoginViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
    }
}
