using System.ComponentModel.DataAnnotations;
using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class ReviewViewModel
    {

       [Required(ErrorMessage = "Please enter comment.")]

        public string Comment { get; set; }
        public string? HotelName { get; set; }
        public string? UserName { get; set; }
        
        
        // for dropdown
        public List<Hotel>? hotels { get; set; }
        #region review
        [Required(ErrorMessage = "Please select Hotel.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Please select a rating.")]
        
        public int Rating { get; set; }

        #endregion
    }
}
