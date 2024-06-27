namespace SignInApi.Models
{
    public interface IListingService
    {
        Task<List<ListingResult>> GetListings();
    }
}