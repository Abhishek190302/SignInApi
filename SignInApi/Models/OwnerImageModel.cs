namespace SignInApi.Models
{
    public class OwnerImageModel
    {
        public List<string> FirstName { get; set; }
        public List<string> LastName { get; set; }
        public List<string> Designation { get; set; }
        public List<IFormFile> File { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public List<string> MrndMs { get; set; }
    }
}
