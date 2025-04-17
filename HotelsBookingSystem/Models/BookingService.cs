using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    [PrimaryKey(nameof(BookingID),nameof(ServiceID))]
    public class BookingService
    {
        [ForeignKey("Booking")]
        public int BookingID { get; set; }
        public Booking Booking { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }
        public Service Service { get; set; }
    }
}
