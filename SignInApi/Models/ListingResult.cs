namespace SignInApi.Models
{
    public class ListingResult
    {
        public int ListingId { get; set; }
        public string OwnerId { get; set; }
        public string Id { get; set; }
        public string CompanyName { get; set; }
        //public string SubCategory { get; set; }
        public List<SubCategory> SubCategory { get; set; }
        public List<SeoKeyword> Keyword { get; set; }
        public string ListingKeyword { get; set; }
        public string ListingUrl { get; set; }
        public string Turnover { get; set; }
        public string YearOfEstablishment { get; set; }
        public string NumberOfEmployees { get; set; }
        public string GSTNumber { get; set; }
        public int PackageID { get; set; }
        public string Languges { get; set; }
        public string TollFree { get; set; }
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string Locality { get; set; }
        public string Area { get; set; }
        public string Mobile { get; set; }
        public string RegisterMobile { get; set; }
        public string Whatsapp { get; set; }
        public string Telephone { get; set; }
        public double RatingAverage { get; set; }
        public int RatingCount { get; set; }
        public int BusinessYear { get; set; }
        public BusinessWorkingHour BusinessWorking { get; set; } = new BusinessWorkingHour();
        public LogoImage LogoImage { get; set; } = new LogoImage();
        public string CompanyFirstLetter => CompanyName.Substring(0, 1);
        public string LogoImageUrl
        {
            get
            {
                return LogoImage == null || string.IsNullOrWhiteSpace(LogoImage.ImagePath) ? "" : LogoImage.ImagePath;
            }
        }
        public string Url { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public bool SelfCreated { get; set; }
        public bool ClaimedListing { get; set; }
        public WorkingTime Workingtime { get; set; }
        public string Description { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
