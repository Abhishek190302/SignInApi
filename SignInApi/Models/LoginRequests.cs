namespace SignInApi.Models
{
    public class LoginRequests
    {
        public string EmailOrMobile { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
