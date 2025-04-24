using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class ServiceViewModelAd
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must  be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

    }
}
