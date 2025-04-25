using System.ComponentModel.DataAnnotations;
using HotelsBookingSystem.Models;

namespace HotelsBookingSystem.ViewModels
{
    public class RoomViewModel
    {
        #region roomData
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int PricePerNight { get; set; }
        public Hotel hotel { get; set; }
        public int? RoomNumber { get; set; }
        public int? NumberOfBeds { get; set; }
        public List<string> RoomImages { get; set; }
        #endregion
        #region dropdown
        //for drobdown
        public List<string> citys { get; set; }
        public List<Hotel> hotels { get; set; }
         public List<string> typslist { get; set; }
        #endregion

        #region filter
        [Range(0, 100000000, ErrorMessage = "Minimum price must be at least 0.")]
        public int? MinPrice { get; set; }

        [Range(0, 100000000, ErrorMessage = "Maximum price must be at least 0.")]
        public int? MaxPrice { get; set; }
        public string? TypeFilter { get; set; }
        public string? City { get; set; }
        public int? HotelId { get; set; }

         
        #endregion



    }
}
