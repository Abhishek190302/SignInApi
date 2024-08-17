namespace SignInApi.Models
{
    public interface IAreaService
    {
        Task<Area> GetAreaByAreaNameAsync(string areaName);
        Task CreateAreaAsync(AreaCreateRequest request);
        Task<Pincode> GetPincodeByIdAsync(int pincodeId);
    }
}