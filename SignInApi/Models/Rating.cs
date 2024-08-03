namespace SignInApi.Models
{
    public class Rating
    {
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public int Ratings { get; set; }
        public string Comment { get; set; }
    }
}
