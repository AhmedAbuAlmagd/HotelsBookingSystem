using HotelsBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class RoomViewModelAd
    {
        public int Id { get; set; }
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Room type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int PricePerNight { get; set; }
        [Required]
        public string image { get; set; }
        public int RoomNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
        public int NumberOfBeds { get; set; }
    }
}
