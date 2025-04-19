using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int? RoomNumber { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public int? NumberOfBeds { get; set; }
        public string? Status { get; set; }
        public int PricePerNight {  get; set; }
        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public virtual Hotel Hotel { get; set; }

        public virtual List<BookingRoom>? BookingRooms { get; set; }
        public virtual List<RoomImage>? RoomImages { get; set; } = new List<RoomImage>();
        public virtual List<CartItem>? CartItems { get; set; } = new List<CartItem>();

    }
}
