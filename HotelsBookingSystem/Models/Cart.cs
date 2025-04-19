using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual List<CartItem>? CartItems { get; set; }   

    }
}
