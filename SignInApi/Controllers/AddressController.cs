using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IAddressRepositery _addressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public AddressController(IAddressRepositery addressRepository, UserService userService, CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _addressRepository = addressRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;

        }

        [HttpPost]
        [Route("GetAddressDropdownMaster")]
        public async Task<IActionResult> GetAddressDropdownMaster(AddressVM addressVM)
        {
            var countries = await _addressRepository.GetAddressDetails();
            object response = new { Countries = countries };

            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            var address = await _addressRepository.GetAddressByListingIdAsync(listing.Listingid);
                            bool recordNotFound = address == null;
                            if (recordNotFound)
                            {
                                // Assuming you have some logic to select specific IDs from the list of countries and nested entities
                                var selectedCountry = countries.FirstOrDefault();
                                var selectedState = selectedCountry?.States.FirstOrDefault();
                                var selectedCity = selectedState?.Cities.FirstOrDefault();
                                var selectedAssembly = selectedCity?.Assemblies.FirstOrDefault();
                                var selectedPincode = selectedAssembly?.Pincodes.FirstOrDefault();
                                var selectedLocality = selectedPincode?.Localities.FirstOrDefault();

                                address = new Address
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                    CountryID = selectedCountry?.CountryID ?? 0,
                                    StateID = selectedState?.StateID ?? 0,
                                    CityID = selectedCity?.CityID ?? 0,
                                    AssemblyID = selectedAssembly?.AssemblyID ?? 0,
                                    PincodeID = selectedPincode?.PincodeID ?? 0,
                                    LocalityID = selectedLocality?.LocalityID ?? 0,
                                    LocalAddress = addressVM.LocalAddress // Set as needed
                                };
                            }
                            else
                            {
                                // Assuming you have some logic to update specific IDs from the list of countries and nested entities
                                var selectedCountry = countries.FirstOrDefault();
                                var selectedState = selectedCountry?.States.FirstOrDefault();
                                var selectedCity = selectedState?.Cities.FirstOrDefault();
                                var selectedAssembly = selectedCity?.Assemblies.FirstOrDefault();
                                var selectedPincode = selectedAssembly?.Pincodes.FirstOrDefault();
                                var selectedLocality = selectedPincode?.Localities.FirstOrDefault();

                                address.CountryID = selectedCountry?.CountryID ?? 0;
                                address.StateID = selectedState?.StateID ?? 0;
                                address.CityID = selectedCity?.CityID ?? 0;
                                address.AssemblyID = selectedAssembly?.AssemblyID ?? 0;
                                address.PincodeID = selectedPincode?.PincodeID ?? 0;
                                address.LocalityID = selectedLocality?.LocalityID ?? 0;
                                address.LocalAddress = addressVM.LocalAddress; // Set as needed
                            }

                            if (recordNotFound)
                            {
                                await _addressRepository.CreateAddress(address);
                                response = new { Message = "Address Details created successfully", Address = address, Country = countries };
                            }
                            else
                            {
                                await _addressRepository.UpdateAddress(address);
                                response = new { Message = "Address Details Updated successfully", Address = address, Country = countries };
                            }
                        }

                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, "Internal server error");
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }
    }
}
