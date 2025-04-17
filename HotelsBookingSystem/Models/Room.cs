namespace HotelsBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public string? Status { get; set; }
        public int PricePerNight {  get; set; }
        public virtual Booking? Booking { get; set; }
        public virtual List<RoomImage>? RoomImages { get; set; } = new List<RoomImage>();
    }
}
