using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        [Range(1,5)]
        public int? Rating { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }
        public virtual Hotel? Hotel { get; set; }

    }
}
