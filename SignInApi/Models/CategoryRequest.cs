namespace SignInApi.Models
{
    public class CategoryRequest
    {
        public string FirstCategoryID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string SearchKeywordName { get; set; }
        public string SortOrder { get; set; } // Add this property if you need to store the image path
        public List<CategoryRequest> ThirdCategories { get; set; } = new List<CategoryRequest>();
        public List<CategoryRequest> FourthCategories { get; set; } = new List<CategoryRequest>();
        public List<CategoryRequest> FifthCategories { get; set; } = new List<CategoryRequest>();
        public List<CategoryRequest> SixthCategories { get; set; } = new List<CategoryRequest>();

        // Add properties to store parent category IDs for mapping
        public string SecondCategoryID { get; set; }
        public string ThirdCategoryID { get; set; }
        public string FourthCategoryID { get; set; }
        public string FifthCategoryID { get; set; }

    }
}
