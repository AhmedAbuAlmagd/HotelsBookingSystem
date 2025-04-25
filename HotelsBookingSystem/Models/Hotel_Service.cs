using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Hotel_Service
    {
        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; }
        [ForeignKey("Service")]
        public int serviceId { get; set; }
        public Service Service { get; set; }
    }
}
