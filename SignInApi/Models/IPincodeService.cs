namespace SignInApi.Models
{
    public interface IPincodeService
    {
        Task<Pincode> GetPincodeByPinNumberAsync(int pinNumber);
        Task CreatePincodeAsync(PincodeCreateRequest request);
        Task<Location> GetLocalityByIdAsync(int localityId);
    }
}