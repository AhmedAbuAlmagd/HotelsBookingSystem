namespace HotelsBookingSystem.ViewModels
{
    public class PaymentSuccessViewModel
    {
        public int BookingId { get; set; }
        public string TransactionId { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
