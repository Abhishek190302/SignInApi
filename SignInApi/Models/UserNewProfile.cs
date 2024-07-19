namespace SignInApi.Models
{
    public class UserNewProfile
    {
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TimeZoneOfCountry { get; set; }
        public string ImageUrl { get; set; }
    }
}
