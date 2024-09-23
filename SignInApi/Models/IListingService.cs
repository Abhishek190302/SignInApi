namespace SignInApi.Models
{
    public interface IListingService
    {

        Task<List<ListingResult>> GetListings(int pageNumber, int pageSize,int subCategoryid, string cityName);

        Task<List<ListingResult>> GetListingsid(int subCategoryid, string cityName, int listingIds);
    }
}