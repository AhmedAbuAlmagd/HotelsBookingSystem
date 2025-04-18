namespace HotelsBookingSystem.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int PricePerNight { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
