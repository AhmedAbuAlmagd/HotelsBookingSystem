using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.Repository;
using HotelsBookingSystem.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelsBookingSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(
            IHotelRepository hotelRepository,
            IRoomRepository roomRepository,
            IBookingRepository bookingRepository , 
            IUserRepository userRepository)
        {
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var totalHotels = await _hotelRepository.GetTotalHotelsCountAsync();
            var totalRooms = await _roomRepository.GetTotalRoomsCountAsync();
            var totalBookings = await _bookingRepository.GetTotalBookingsCountAsync();
            var recentBookings = await _bookingRepository.GetRecentBookingsAsync(5);
            var topHotels = await _hotelRepository.GetTopHotelsAsync(6);
            var topRooms = await _roomRepository.GetTopRoomsAsync(6);
            var topClients = await _userRepository.GetTopClientsAsync(6);




            var viewModel = new DashboardViewModel
            {
                TotalHotels = totalHotels,
                TotalRooms = totalRooms,
                TotalBookings = totalBookings,

                RecentBookings = (recentBookings).Select(b => new BookingViewModel
                {
                    ClientName = b.User.FullName,
                    HotelName = b.Hotel.Name,
                    CheckInDate = b.CheckIn,
                    CheckOutDate = b.CheckOut,
                    Status = b.Status,
                    TotalAmount = b.TotalPrice,
                    Rooms = b.BookingRooms.Select(br => new BookingRoomViewModel
                    {
                        RoomType = br.Room.Type,
                        HotelName = br.Room.Hotel.Name
                    }).ToList()
                }).ToList(),

                Hotels = (topHotels).Select(h => new HotelViewModel
                {
                    Name = h.Name,
                    Location = h.Location,
                    RoomCount = h.RoomCount,
                    ImageUrl = h.ImageUrl,
                    Status = h.Status
                }).ToList(),

                Rooms = (topRooms).Select(r => new RoomViewModel
                {
                    Type = r.Type,
                    HotelName = r.Hotel.Name,
                    PricePerNight = r.PricePerNight,
                    ImageUrl = r.RoomImages.FirstOrDefault(r => r.IsPrimary == true).ImageUrl,
                    Status = r.Status
                }).ToList(),

                Clients = (topClients).Select(c => new ClientViewModel
                {
                    Name = c.FullName,
                    Country = c.Country,
                    City = c.City,
                    BookingCount = c.Bookings.Count()
                }).ToList()
            };

            return viewModel;
        }
    }
    }
