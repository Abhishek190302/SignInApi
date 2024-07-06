namespace SignInApi.Models
{
    public class OwnerImageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public IFormFile File { get; set; }
    }
}
