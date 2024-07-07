namespace SignInApi.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string RedirectToUrl { get; set; }
        public ApplicationUsers User { get; set; }
    }
}
