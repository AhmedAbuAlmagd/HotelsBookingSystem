using System.Linq;
using HotelsBookingSystem.Models;
using HotelsBookingSystem.Models.Context;
using HotelsBookingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

using X.PagedList;
using X.PagedList.Extensions;

namespace HotelsBookingSystem.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelsContext _context;

        public RoomRepository(HotelsContext context)
        {
            _context = context;
        }
        public IPagedList<Room> GetAll(int page, int pageSize)
        {
            var rooms = _context.Rooms
                .Where(r => r.Status == "available")   
                .Include(r => r.Hotel)                 
                .Include(r => r.RoomImages)       
                .OrderBy(r => r.Id)                  
                .ToPagedList(page, pageSize);        

            return rooms;
        }

    
        Room IRoomRepository.GetById(int id)
        {

            //var room = con.Rooms.Find(id);
            var room = _context.Rooms.Include(r => r.Hotel)
                .Include(r => r.RoomImages).FirstOrDefault(r => r.Id == id);
            if (room != null)
            {
                return room;
            }
            else
            {
                throw new Exception("Room not found");
            }
        }



        #region forCrud 
        void IRoomRepository.Add(Room room)
        {

            _context.Rooms.Add(room); 
        }

        void IRoomRepository.Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }
            else
            {
                throw new Exception("Room not found");
            }
        }

        void IRoomRepository.SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(Room room)
        {
            _context.Rooms.Update(room);
        }
        #endregion

        #region Room page filter
       
        public IPagedList<Room> FilterRooms(RoomViewModel roomViewModel, int pageNumber, int pageSize)
        {
            var query = _context.Rooms.Where(r => r.Status == "available")
               .Include(r => r.RoomImages)
               .Include(r => r.Hotel)
               .AsQueryable();
            if (!string.IsNullOrEmpty(roomViewModel.TypeFilter))
                query = query.Where(r => r.Type.Contains(roomViewModel.TypeFilter));
            if (roomViewModel.MinPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= roomViewModel.MinPrice.Value);
            if (roomViewModel.MaxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= roomViewModel.MaxPrice.Value);
            if (roomViewModel.HotelId.HasValue)
                query = query.Where(r => r.HotelId == roomViewModel.HotelId.Value);
            if (!string.IsNullOrEmpty(roomViewModel.City))
                query = query.Where(r => r.Hotel.City.Contains(roomViewModel.City));
            // return query.ToList();
            return query.OrderBy(r => r.Id).ToPagedList(pageNumber, pageSize);

        }
        public List<Hotel> GetAllhotels()
        {
           var hotels= _context.Hotels.Where(h=>h.Status.ToLower()== "available")
                .Include(h => h.Rooms)
                .Include(h => h.HotelImages)
                .Include(h => h.Reviews)
                .Include(h => h.Bookings)
                .ToList();
            return hotels;
        }
        public List<Room> GetAllroom()
        {
            var rooms = _context.Rooms
               .Where(r => r.Status == "available")
               .Include(r => r.Hotel)
               .Include(r => r.RoomImages)
               .Include(r => r.BookingRooms)
               .ThenInclude(r => r.booking)
              .ToList();
            return rooms;
        }

        #endregion
        //public List<Room> GetAllroom()
        //{
        //    var rooms = _context.Rooms
        //       .Where(r => r.Status == "available")
        //       .Include(r => r.Hotel)
        //       .Include(r => r.RoomImages)
        //       .Include(r => r.BookingRooms)
        //       .ThenInclude(r => r.booking)
        //      .ToList();
        //    return rooms;
        //}

        #region Admin
        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .ToListAsync();
        }
        public async Task<int> GetTotalRoomsCountAsync()
        {
            return await _context.Rooms.CountAsync();
        }

        public async Task<List<Room>> GetTopRoomsAsync(int count)
        {
            return await _context.Rooms
                .Include(r=>r.RoomImages)
                .Include(r => r.Hotel)
                .OrderBy(r => r.Hotel.Name)
                .Take(count)
                .ToListAsync();
        }
       
        public int GetCountByHotelId(int hotelId)
        {
            return _context.Rooms.Count(r => r.HotelId == hotelId);
        }

        public List<Room> GetPagedByHotelId(int hotelId, int pageNumber, int pageSize)
        {
            return _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.RoomImages)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public bool RoomNumberExists(int hotelId, int roomNumber, int? roomId = null)
        {
            if (roomId.HasValue)
            {
                return GetAllroom().Any(r => r.HotelId == hotelId &&
                                           r.RoomNumber == roomNumber &&
                                           r.Id != roomId.Value);
            }

            return GetAllroom().Any(r => r.HotelId == hotelId &&
                                       r.RoomNumber == roomNumber);
        }
        #endregion

    }
}
