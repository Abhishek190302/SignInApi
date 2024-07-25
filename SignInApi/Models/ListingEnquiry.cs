namespace SignInApi.Models
{
    public class ListingEnquiry
    {
        public int EnquiryID { get; set; }
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EnquiryTitle { get; set; }
        public string Message { get; set; }
        public string CreatedDate { get; set; }
    }
}
