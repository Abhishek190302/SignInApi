namespace SignInApi.Models
{
    public class Complaint
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime Date { get; set; }
        public string OwnerGuid { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
