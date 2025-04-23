using System.ComponentModel.DataAnnotations;

namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class HotelFormViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string City { get; set; }
        //public string Address { get; set; }


        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        //[Range(1, 5)]
        //public int Rating { get; set; }
      
        public string Status { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
