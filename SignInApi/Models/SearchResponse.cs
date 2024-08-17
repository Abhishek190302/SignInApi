namespace SignInApi.Models
{
    public class SearchResponse
    {
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<SearchHomeListingViewModel> Listings { get; set; }
    }
}
