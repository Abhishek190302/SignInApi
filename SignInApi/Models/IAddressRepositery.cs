using Microsoft.AspNetCore.Mvc;

namespace SignInApi.Models
{
    public interface IAddressRepositery
    {
        Task<List<Country>> GetAddressDetails();
        Task<Address> GetAddressByListingIdAsync(int listingId);
        Task CreateAddress([FromBody] Address address);
        Task UpdateAddress([FromBody] Address address);
    }
}