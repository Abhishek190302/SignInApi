namespace SignInApi.Models
{
    public class GalleryDeleteRequest
    {
        public int ListingID { get; set; }
        public List<string> ImagePaths { get; set; }
    }
}
