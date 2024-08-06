namespace SignInApi.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsVendor { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BussinessCategory { get; set; }

    }
}
