namespace SignInApi.Models
{
    public class UserProfile
    {
        public string OwnerGuid { get; set; }
        public bool IsProfileCompleted { get; set; }
        public string Name { get; set; }
    }
}
