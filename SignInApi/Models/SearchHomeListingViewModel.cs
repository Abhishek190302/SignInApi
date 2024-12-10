namespace SignInApi.Models
{
    public class SearchHomeListingViewModel
    {
        public string listingId { get; set; }
        public string CompanyName { get; set; }
        public string ListingUrl { get; set; }
        public string CityName { get; set; }
        public string LocalityName { get; set; }
        public string AreaName { get; set; }
        public string category { get; set; }
        public int CategoryId { get; set; }
        public string keyword { get; set; }
        public string keywordId { get; set; }
        public string Mobilenumber { get; set; }
        public string Gstnumber { get; set; }
        public string Ownername { get; set; }
        public string Specialization { get; set; }
        public string OwnerPrefix { get; set; }
    }

    public class ListingSearch
    {
        public string ListingId { get; set; }
        public string CompanyName { get; set; }
        public string ListingURL { get; set; }
        public string City { get; set; }
        public string Locality { get; set; }
        public string Area { get; set; }
        public AddressSearch Address { get; set; }
        public string Keyword { get; set; }
        public int KeywordID { get; set; }
        public string Mobilenumber { get; set; }
        public string Gstnumber { get; set; }
        public string Ownername { get; set; }
        public string Specialiazation { get; set; }
        public string OwnerPrefix { get; set; }
        //public List<ListingSearch> SubListings { get; set; }
    }

    public class ListingFreeSearch
    {
        public string ListingId { get; set; }
        public string CompanyName { get; set; }
        public string ListingURL { get; set; }
        public AddressSearch Address { get; set; }
        public string Keyword { get; set; }
        public string OwnerName { get; set; }
        public AddressSearch Addreses { get; set; }
    }

    public class AddressSearch
    {
        public string AssemblyID { get; set; }
    }

    public class LocalitySearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CitySearch City { get; set; }
        public string AreaName { get; set; }
    }

    public class CitySearch
    {
        public string Name { get; set; }
    }

    public class category
    {
        public string categories { get; set; }
        public int CategoryId { get; set; }
    }
}
