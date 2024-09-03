namespace SignInApi.Models
{
    public interface IListingService
    {

        Task<List<ListingResult>> GetListings(int pageNumber, int pageSize,int subCategoryid, string cityName);
    }
}