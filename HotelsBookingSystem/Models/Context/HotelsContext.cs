using HotelsBookingSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using HotelsBookingSystem;

namespace HotelsBookingSystem.Models.Context
{
    public class HotelsContext : IdentityDbContext<ApplicationUser>
    {
        public HotelsContext(DbContextOptions<HotelsContext> Options) : base(Options)
        {

        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingService> BookingServices { get; set; }
        public virtual DbSet<Hotel_Service> Hotel_Service { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelImage> HotelImages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomImage> RoomImages { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<BookingRoom> BookingRooms { get; set; }
        public virtual DbSet<SelectedServices> SelectedServices { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string adminRoleId = "ADMIN-ROLE-001";
            string adminUserId = "ADMIN-USER-001";
            string userRoleId = "USER-ROLE-001";
            string normalUserId = "USER-USER-001";


            // Admin Role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER"
            });
            builder.Entity<Hotel_Service>()
    .HasKey(hs => new { hs.HotelId, hs.serviceId });
              builder.Entity<SelectedServices>()
             .HasKey(ss => new { ss.CartId, ss.ServiceID });

            // Admin User
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@site.com",
                NormalizedUserName = "ADMIN@SITE.COM",
                Email = "admin@site.com",
                NormalizedEmail = "ADMIN@SITE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEIjJh6/LXD2Bg+3MJGc+CmiaE471FJWBEmlTQ/1OhqkFw0NIgG/beU7wkTfmnuQ/sQ==",
                SecurityStamp = "STATIC-SECURITY-STAMP-001",
                ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-001",
                FullName = "Admin User",
                City = "Cairo",
                Country = "Egypt",
                NationalId = "12345678901234"
            });

            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = normalUserId,
                UserName = "user@site.com",
                NormalizedUserName = "USER@SITE.COM",
                Email = "user@site.com",
                NormalizedEmail = "USER@SITE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAELyWnA3L8xp5Hrs6WnKF/jGfxKmtgxJyDrE/5cKoAAu34yOBFoySgbzAWe3pqdH3BA==",
                SecurityStamp = "STATIC-SECURITY-STAMP-002",
                ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-002",
                FullName = "Normal User",
                City = "Alexandria",
                Country = "Egypt",
                NationalId = "98765432109876"
            });

  

            // Assign Admin Role
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            });

            // Assign User Role to Normal User
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = normalUserId,
                RoleId = userRoleId
            });

        }


    }
}
