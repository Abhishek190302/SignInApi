namespace SignInApi.Models
{
    public class ApplicationUsers
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsVendor { get; set; }
    }
}
