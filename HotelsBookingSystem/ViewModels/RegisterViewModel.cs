using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels
{
    public class RegisterViewModel
    {
        [StringLength(100)]
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\-]{6,20}$", ErrorMessage = "Invalid National ID format.")]
        public string NationalId { get; set; }
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]   
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
