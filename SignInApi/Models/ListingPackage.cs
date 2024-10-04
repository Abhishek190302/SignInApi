namespace SignInApi.Models
{
    public class ListingPackage
    {
        public int id { get; set; }
        public string PackageTitle { get; set; }
        public string Price { get; set; }
        public string PackageImagepath { get; set; }
        public string PackageStatus { get; set; }
        public string PackageDescription { get; set; }
        public string PackageCreatedDate { get; set; }
        public string PackageUpdatedDate { get; set; }
    }
}
