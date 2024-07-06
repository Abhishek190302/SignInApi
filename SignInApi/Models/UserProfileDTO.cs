namespace SignInApi.Models
{
    public class UserProfileDTO
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsVendor { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string ImgUrl { get; set; }
    }
}
