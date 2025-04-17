using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsBookingSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string? status { get; set; }
        public string? Method { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string? TransactionId { get; set; }

        [ForeignKey("Booking")]
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; }
        
    }
}
