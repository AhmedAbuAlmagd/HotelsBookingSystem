using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey("Cart")]
        public int? CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        [NotMapped]
        public int Nights => (CheckOut - CheckIn).Days;
        public decimal TotalPrice { get; set; }


    }
}
