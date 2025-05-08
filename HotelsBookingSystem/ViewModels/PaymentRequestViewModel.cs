public class PaymentRequestViewModel
{
    public int TotalPrice { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; } 
    public List<int> RoomIds { get; set; }
    
}
