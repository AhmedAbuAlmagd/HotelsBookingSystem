namespace HotelsBookingSystem.ViewModels.AdminViewModels
{
    public class HotelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int RoomCount { get; set; }
        public string Status { get; set; }
        public double Rating { get; internal set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

}
