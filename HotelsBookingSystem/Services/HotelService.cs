using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels.AdminViewModels;

namespace HotelsBookingSystem.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }


        public List<HotelViewModel> GetAllHotels()
        {
            var hotels = _hotelRepository.GetHotelsWithRoomsAndImages();
            return hotels.Select(h => MapToViewModel(h)).ToList();
        }
        public HotelViewModel MapToViewModel(Hotel hotel)
        {
            return new HotelViewModel
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Address,
                Description = hotel.Description,
                ImageUrl = hotel.HotelImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                //Rating = hotel.Rating,
                RoomCount = hotel.Rooms?.Count ?? 0,
                Status = hotel.Status,
                Longitude =Double.Parse(hotel.Longitude),
                Latitude = Double.Parse(hotel.Latitude)
            };
        }

        public HotelDetailsViewModel GetHotelDetails(int id)
        {
            var hotel = _hotelRepository.GetHotelWithRoomsAndImages(id);
            if (hotel == null) return null;

            return new HotelDetailsViewModel
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Address,
                Description = hotel.Description,
                ImageUrl = hotel.HotelImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                //Rating = hotel.Rating,
                Status = hotel.Status,
                RoomCount = hotel.Rooms.Count,
                AvailableRooms = hotel.Rooms.Count(r => r.Status == "Available"),
                Rooms = hotel.Rooms.Select(r => MapToRoomViewModel(r)).ToList() ,
                Longitude = Double.Parse(hotel.Longitude),
                Latitude = Double.Parse(hotel.Latitude)
            };
        }

        public void AddHotel(HotelFormViewModel model)
        {
            var hotel = new Hotel
            {
                Id = model.Id,
                Name = model.Name,
                Address = model.Location,
                Description = model.Description,
                //Rating = model.Rating,
                Status = model.Status,
                Longitude = model.Longitude.ToString(),
                Latitude = model.Latitude.ToString()
            };

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                hotel.HotelImages = new List<HotelImage>
            {
                new HotelImage { ImageUrl = model.ImageUrl, IsPrimary = true }
            };
            }

            _hotelRepository.Add(hotel);
            _hotelRepository.SaveChanges();
        }

        public void UpdateHotel(int id, HotelFormViewModel model)
        {
            var hotel = _hotelRepository.GetById(id);
            if (hotel == null) throw new Exception("Hotel not found");
            hotel.Name = model.Name;
            hotel.Address = model.Location;
            hotel.Description = model.Description;
            //hotel.Rating = model.Rating;
            hotel.Status = model.Status;
            hotel.Longitude = model.Longitude.ToString();
            hotel.Latitude = model.Latitude.ToString();

            _hotelRepository.Update(hotel);
            _hotelRepository.SaveChanges();
        }
      

        private HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard.RoomViewModel MapToRoomViewModel(Room room)
        {
            return new HotelsBookingSystem.ViewModels.AdminViewModels.Dashboard.RoomViewModel
            {
                Type = room.Type,
                PricePerNight = room.PricePerNight,
                Status = room.Status,
                ImageUrl = room.RoomImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
            };
        }

     
    }
}
