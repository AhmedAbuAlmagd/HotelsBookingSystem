using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HotelsBookingSystem.Models
{
    public class HotelsContext : IdentityDbContext<ApplicationUser>
    {
        public HotelsContext(DbContextOptions<HotelsContext> Options) : base(Options)
        {

        }
       
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingService> BookingServices { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelImage> HotelImages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomImage> RoomImages { get; set; }
        public virtual DbSet<Service> Services { get; set; }

    }
}
