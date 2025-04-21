namespace HotelsBookingSystem.Models.Results
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public ApplicationUser User { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsAdmin { get; set; }
    }
}
