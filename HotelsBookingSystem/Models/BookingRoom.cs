using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    [PrimaryKey(nameof(RoomId) , nameof(BookingId))]
    public class BookingRoom
    {
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        [ForeignKey ("Booking")]
        public int BookingId { get; set; }
        public virtual Booking booking { get; set; }
    }
}
