namespace SignInApi.Models
{
    public class SearchHomeListingViewModel
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string ListingUrl { get; set; }
        public string CityName { get; set; }
        public string LocalityName { get; set; }
    }

    public class ListingSearch
    {
        public string ListingId { get; set; }
        public string CompanyName { get; set; }
        public string ListingURL { get; set; }
        public AddressSearch Address { get; set; }
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
    }

    public class CitySearch
    {
        public string Name { get; set; }
    }
}
