namespace SignInApi.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsVendor { get; set; }
    }
}
