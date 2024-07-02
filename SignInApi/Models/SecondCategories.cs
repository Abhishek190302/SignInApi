namespace SignInApi.Models
{
    public class SecondCategories
    {
        public int FirstCategoryId { get; set; }
        public int SecondCategoryId { get; set; }
        public string SecondCategoryName { get; set; }
        public List<ThirdCategories> ThirdCategories { get; set; } = new List<ThirdCategories>();
        public List<FourthCategories> FourthCategories { get; set; } = new List<FourthCategories>();
        public List<FifthCategories> FifthCategories { get; set; } = new List<FifthCategories>();
        public List<SixthCategories> SixthCategories { get; set; } = new List<SixthCategories>();
    }
}
