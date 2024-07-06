namespace SignInApi.Models
{
    public class ApplicationUserRequest
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsVendor { get; set; }
    }
}
