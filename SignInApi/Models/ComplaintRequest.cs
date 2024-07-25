namespace SignInApi.Models
{
    public class ComplaintRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
