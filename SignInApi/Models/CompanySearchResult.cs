namespace SignInApi.Models
{
    public class CompanySearchResult
    {
        public int ListingID { get; set; }
        public string CompanyName { get; set; }
        public string ListingURL { get; set; }
        public string City { get; set; }
        public string Locality { get; set; }
        public string Keyword { get; set; }
        public int KeywordID { get; set; }
        public string Category { get; set; }   // Category Name
        public int CategoryId { get; set; }    // Category ID
        public string Specialization { get; set; }
    }
}
