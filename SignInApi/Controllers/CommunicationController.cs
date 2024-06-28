using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationController : ControllerBase
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserService _userService;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly CommunicationRepository _communicationRepository;

        public CommunicationController(AuthenticationStateProvider authenticationStateProvider,UserService userService, CompanyDetailsRepository companydetailsRepository, CommunicationRepository communicationRepository)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userService = userService;
            _companydetailsRepository = companydetailsRepository;
            _communicationRepository = communicationRepository;
        }

        [HttpPost]
        [Route("AddOrUpdateCommunication")]
        public async Task<IActionResult> AddOrUpdateCommunication(CommunicationViewModel communicationVM)
        {
            //var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            //var user = authState.User;
            //if (user.Identity.IsAuthenticated)
            //{
            var applicationUser = await _userService.GetUserByUserName("web@jeb.com");
            if (applicationUser != null)
            {
                try
                {
                    string currentUserGuid = applicationUser.Id.ToString();
                    var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                    if (listing != null)
                    {
                        var communication = await _communicationRepository.GetCommunicationByListingIdAsync(listing.Listingid);
                        bool recordNotFound = communication == null;
                        if (recordNotFound)
                        {
                            communication = new Communication
                            {
                                OwnerGuid = currentUserGuid,
                                ListingID = listing.Listingid,
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                            };
                        }

                        // Map properties from CommunicationViewModel to Communication

                        communication.Email = communicationVM.Email;
                        communication.Mobile = communicationVM.Mobile;
                        communication.Telephone = communicationVM.Telephone;
                        communication.Website = communicationVM.Website;
                        communication.Language = communicationVM.Language;
                        communication.TelephoneSecond = communicationVM.RegisterMobile;
                        communication.TollFree = communicationVM.Tollfree;


                        if (recordNotFound)
                        {
                            await _communicationRepository.AddCommunicationAsync(communication);
                            return Ok(new { Message = "Communication Details created successfully", Communication = communication });
                        }
                        else
                        {
                            await _communicationRepository.UpdateCommunicationAsync(communication);
                            return Ok(new { Message = "Communication Details Updated successfully", Communication = communication });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }
            }
            return NotFound("User not found");

            //}
            //return Unauthorized();
        }
    }
}
