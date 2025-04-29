using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class ReviewViewModel
    {

        public string? Comment { get; set; }
        public string? HotelName { get; set; }
        public string? UserName { get; set; }
        public List<Hotel>? hotels { get; set; }
        public int? HotelId { get; set; }
    }
}
