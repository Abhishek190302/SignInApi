namespace SignInApi.Models
{
    public class ListingActivityVM
    {
        public int Listingid { get; set; }
        public string OwnerGuid { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string ProfileImage { get; set; }
        public string VisitDate { get; set; }
        public string TimeAgo { get; set; }
        public int ActivityType { get; set; }
        public string ActivityText { get; set; }
        public bool isNotification { get; set; }
        public string ReviewComment { get; set; }
        public string EnquiryComment { get; set; }
    }
}
