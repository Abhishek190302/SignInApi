namespace SignInApi.Models
{
    public class CompanyFreeSearchResult
    {
        public int ListingId { get; set; }
        public string CompanyName { get; set; }
        public string ListingURL { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
