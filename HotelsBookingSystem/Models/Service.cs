namespace HotelsBookingSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public List<BookingService>? BookingServices { get; set; } = new List<BookingService>();
    }
}
