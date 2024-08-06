namespace SignInApi.Models
{
    public class Categories
    {
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public int FirstCategoryID { get; set; }
        public int SecondCategoryID { get; set; }
        public string ThirdCategoryID { get; set; }
        public string FourthCategoryID { get; set; }
        public string FifthCategoryID { get; set; }
        public string SixthCategoryID { get; set; }
    }

    public class FirstBussinessCategories
    {
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public string firstCategoryID { get; set; }
    }
}
