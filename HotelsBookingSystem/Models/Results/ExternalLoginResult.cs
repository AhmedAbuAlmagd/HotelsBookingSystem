namespace HotelsBookingSystem.Models.Results
{
    public class ExternalLoginResult : LoginResult
    {
        public bool RequiresConfirmation { get; set; }
        public string Email { get; set; }
        public string Provider { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsAdmin { get; set; }
    }

}
