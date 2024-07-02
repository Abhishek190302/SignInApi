namespace SignInApi.Models
{
    public class FirstCategories
    {
        public int FirstCategoryID { get; set; }
        public string FirstCategoryName { get; set; }
        public List<SecondCategories> SecondCategories { get; set; } = new List<SecondCategories>();
    }
}
