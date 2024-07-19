namespace SignInApi.Models
{
    public interface ISharedService
    {
        Task<List<Qualification>> GetQualifications();
        Task<List<Country>> GetCountries();
        Task<List<State>> GetStatesByCountryId(int countryId);
        Task<List<City>> GetCitiesByStateId(int stateId);
        Task<List<Assembly>> GetLocalitiesByCityId(int cityId);
        Task<List<Pincode>> GetPincodesByLocalityId(int assemblyId);
        Task<List<Locality>> GetAreasByPincodeId(int pincodeId);
    }
}