namespace HotelsBookingSystem.Models.Results
{
    public class ExternalLoginResult : LoginResult
    {
        public bool Succeeded { get; set; }
        public bool IsAdmin { get; set; }
        public string ErrorMessage { get; set; }
    }

}
