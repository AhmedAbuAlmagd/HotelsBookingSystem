using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class ReviewViewModel
    {

        public string? Comment { get; set; }
        public string? HotelName { get; set; }
        public string? UserName { get; set; }
        
        
        // for dropdown
        public List<Hotel>? hotels { get; set; }
        #region review
        public int? HotelId { get; set; }
        public int? Rating { get; set; }
        #endregion
    }
}
