namespace SignInApi.Models
{
    public class Bookmarks
    {
        public int ListingID { get; set; }
        public string UserGuid { get; set; }
        //public string IPAddress { get; set; }
        //public string UserAgent { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime VisitTime { get; set; }
        public bool Bookmark { get; set; }
    }
}
