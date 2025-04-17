namespace HotelsBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public string? Status { get; set; }
        public int PricePerNight {  get; set; }
        public virtual List<RoomImage>? RoomImages { get; set; } = new List<RoomImage>();
        public virtual List<Booking>? Bookings { get; set; } = new List<Booking>();
        public virtual List<BookingRoom>? BookingRooms { get; set; }= new List<BookingRoom>();

    }
}
