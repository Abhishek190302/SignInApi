namespace SignInApi.Models
{
    public class VerifyforgotpasswordRequest
    {
        public string Otp { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
