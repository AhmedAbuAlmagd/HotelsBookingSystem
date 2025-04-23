namespace HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard
{
    public class BookingViewModel
    {
        public string ClientName { get; set; }
        public string HotelName { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BookingRoomViewModel> Rooms { get; set; }

        public string MainRoomDisplay
        {
            get
            {
                if (Rooms == null || !Rooms.Any())
                    return "No rooms";

                if (Rooms.Count == 1)
                    return $"{Rooms[0].RoomType}";

                return $"{Rooms[0].RoomType} +{Rooms.Count - 1}";
            }
        }
    }

    public class BookingRoomViewModel
    {
        public string? RoomType { get; set; }
        public string HotelName { get; set; }
    }
}
