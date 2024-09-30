namespace SignInApi.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public string Location { get; set; }
        public string ImagePath { get; set; }
        public string BannerLink { get; set; }
        public string BannerType { get; set; }
        public string GalleryBannerType { get; set; }
    }
}
