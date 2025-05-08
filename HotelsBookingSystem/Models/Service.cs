using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public List<BookingService>? BookingServices { get; set; } = new List<BookingService>();
        public virtual List<Hotel_Service>? HotelServices { get; set; } = new List<Hotel_Service>();
        public virtual List<SelectedServices>? SelectedServices { get; set; } = new List<SelectedServices>();
    }
}
