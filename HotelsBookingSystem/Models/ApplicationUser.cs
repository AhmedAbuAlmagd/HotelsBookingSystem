using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FullName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        [StringLength(20, MinimumLength = 6)]
        [RegularExpression(@"^[A-Za-z0-9\-]{6,20}$", ErrorMessage = "Invalid National ID format.")]
        public string? NationalId { get; set; }

        public virtual List<Review>? Reviews { get; set; } = new List<Review>();
        public virtual List<Booking>? Bookings { get; set; } = new List<Booking>();
    }
}
