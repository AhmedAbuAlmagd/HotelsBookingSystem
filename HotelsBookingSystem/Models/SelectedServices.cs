using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class SelectedServices
    {
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }
        public Service Service { get; set; }
    }
}
