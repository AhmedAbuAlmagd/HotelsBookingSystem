using HotelsBookingSystem.Models;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;

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

        public int GetTotalHotelsCount(string searchTerm, string status, string city)
        {
            var query = _hotelRepository.GetHotelsWithRoomsAndImages().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h => h.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                query = query.Where(h => h.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(city) && city.ToLower() != "all")
            {
                query = query.Where(h => h.City.ToLower() == city.ToLower());
            }

            return query.Count();
        }
        public List<HotelViewModel> GetHotelsPaged(int pageNumber, int pageSize, string searchTerm, string status, string city)
        {
            if (pageNumber < 1) 
                pageNumber = 1;
            var query = _hotelRepository.GetHotelsWithRoomsAndImages().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h => h.Name.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                query = query.Where(h => h.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(city) && city.ToLower() != "all")
            {
                query = query.Where(h => h.City.ToLower() == city.ToLower());
            }


            var hotels = query.OrderBy(h => h.Id)
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize).ToList();

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
                AllImages = hotel.HotelImages.Select(img => img.ImageUrl).ToList(), 
                Rating = (Double)hotel.rating,
                City = hotel.City,  
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
                Phone = hotel.Phone,
                City = hotel.City,
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
                rating = model.Rating,
                Status = model.Status,
                City = model.City ,
                Phone = model.Phone,
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
            hotel.City = model.City;
            hotel.Phone = model.Phone;
            hotel.rating = model.Rating;
            hotel.Status = model.Status;
            hotel.Longitude = model.Longitude.ToString();
            hotel.Latitude = model.Latitude.ToString();

            _hotelRepository.GetById(id).HotelImages.FirstOrDefault(x => x.IsPrimary == true).ImageUrl = model.ImageUrl;           

            _hotelRepository.Update(hotel);
            _hotelRepository.SaveChanges();
        }
        
        public void DeleteHotel(int id)
        {
            _hotelRepository.Delete(id);
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
