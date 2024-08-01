namespace SignInApi.Models
{
    public class UserNewProfileVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public IFormFile File { get; set; }
        public bool IsVendor { get; set; }
    }
}
