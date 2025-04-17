using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public DateTime?  Booking_date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? GuestsCount { get; set; }
        public int? paymentStatus { get; set; }
        public int TotalPrice { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }
        public virtual Hotel? Hotel { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User {get; set;}
        public virtual Payment? Payment { get; set; }
        public virtual List<BookingService>? BookingServices { get; set; } = new List<BookingService>();
        public List<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    }
}
