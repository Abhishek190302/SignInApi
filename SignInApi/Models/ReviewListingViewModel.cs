namespace SignInApi.Models
{
    public class ReviewListingViewModel
    {
        public int RatingId { get; set; }
        public int ListingId { get; set; }
        public string OwnerGuid { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string NameFirstLetter { get; set; }
        public string ListingUrl { get; set; }
        public string BusinessCategory { get; set; }
        public string SecondCat { get; set; }
        public string Date { get; set; }
        public int Ratings { get; set; }
        public decimal Rating
        {
            get
            {
                decimal rate = 0;
                decimal.TryParse(Ratings.ToString(), out rate);
                return rate;
            }
            set
            {
                int.TryParse(value.ToString(), out int rating);
                Ratings = rating;
            }
        }
        public string Comment { get; set; }
        public string VisitTime { get; set; } // Ensure this is a string
        public int RatingLimit { get { return 5; } }

        public RatingReply RatingReply { get; set; }
    }
}
