namespace SignInApi.Models
{
    public interface ILocalityService
    {
        Task<Location> GetLocalityByLocalityNameAsync(string localityName);
        Task CreateLocalityAsync(LocalityCreateRequest request);
        Task<City> GetCityByIdAsync(int cityId);
    }
}