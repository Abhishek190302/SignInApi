using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Net;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchAddressMasterController : ControllerBase
    {
        private readonly IAddressRepositery _addressRepository;
       
        public FetchAddressMasterController(IAddressRepositery addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpPost]
        [Route("FetchAddressDropdownMaster")]
        public async Task<IActionResult> FetchAddressDropdownMaster(AddressVM addressVM)
        {
            var countries = await _addressRepository.GetAddressDetails();

            var address = new Address
            {
                CountryID = addressVM.CountryID,
                StateID = addressVM.StateID,
                CityID = addressVM.CityID,
                AssemblyID = addressVM.AssemblyID,
                PincodeID = addressVM.PincodeID,
                LocalityID = addressVM.LocalityID,
                LocalAddress = addressVM.LocalAddress // Set as needed
            };

            // If you need to include the address in the response, you can add it here
            object response = new
            {
                Countries = countries,
                Address = address
            };

            return Ok(response); // Return the response object wrapped in an OkResult
        }
    }
}
