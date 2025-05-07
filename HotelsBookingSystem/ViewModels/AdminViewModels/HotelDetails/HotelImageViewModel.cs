using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AdminViewModels.HotelDetails
{
    public class HotelImageViewModel
    {
        public int Id { get; set; }
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [Display(Name = "Image File")]
        public IFormFile Image { get; set; }

        [Display(Name = "Set as Primary Image")]
        public bool IsPrimary { get; set; } = false;

        [StringLength(200, ErrorMessage = "Caption cannot exceed 200 characters")]
        [Display(Name = "Image Caption")]
        public string? Caption { get; set; }
    }
}
