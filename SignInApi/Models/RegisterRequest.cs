namespace SignInApi.Models
{
    public class RegisterRequest
    {
        public string Vendortype { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Confirmpassword { get; set; }
    }
}
