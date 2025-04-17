using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Phone { get; set; }
        public string? Status { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        
        public virtual List<HotelImage>? HotelImages { get; set; } = new List<HotelImage>();
        public virtual List<Room> Rooms { get; set; } = new List<Room>();
        public virtual List<Review>? Reviews { get; set; } = new List<Review>();
        public virtual List<Booking>? Bookings { get; set; } = new List<Booking>();

    }
}
