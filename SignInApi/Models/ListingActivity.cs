using Microsoft.VisualBasic;

namespace SignInApi.Models
{
    public class ListingActivity
    {
        public int Listingid { get; set; }
        public string CompanyName { get; set; }
        public string Reviewcomment { get; set; }
        public string Enquirycomment { get; set; }
        public string UserGuid { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
