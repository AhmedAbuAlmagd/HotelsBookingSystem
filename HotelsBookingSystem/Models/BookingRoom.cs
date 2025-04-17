using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    [PrimaryKey(nameof(BookingId) , nameof(RoomId))]
    public class BookingRoom
    {
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
