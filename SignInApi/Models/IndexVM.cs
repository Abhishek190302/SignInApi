namespace SignInApi.Models
{
    public class IndexVM
    {
        public List<CategoryRequest> PremiumCategories { get; set; }
        public List<CategoryRequest> Repairs { get; set; }
        public List<CategoryRequest> Services { get; set; }
        public List<CategoryRequest> Contractors { get; set; }
        public List<CategoryRequest> Dealers { get; set; }
        public List<CategoryRequest> Manufacturers { get; set; }
        public List<CategoryRequest> Labours { get; set; }
        public List<CategoryRequest> RentalServices { get; set; }
        public List<CategoryRequest> LaborContractors { get; set; }
        public List<CategoryRequest> Wholesalers { get; set; }
        public List<CategoryRequest> Distributors { get; set; }
    }
}
