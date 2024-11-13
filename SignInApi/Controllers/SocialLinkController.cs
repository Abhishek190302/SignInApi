using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialLinkController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SocialNetworkRepository _socialNetworkRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly BinddetailsManagelistingRepository _binddetailsListing;
        public SocialLinkController(SocialNetworkRepository socialNetworkRepository, UserService userService, CompanyDetailsRepository companyDetailsRepository , IHttpContextAccessor httpContextAccessor, BinddetailsManagelistingRepository binddetailsListing)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _userService = userService;
            _companydetailsRepository = companyDetailsRepository;
            _httpContextAccessor = httpContextAccessor;
            _binddetailsListing = binddetailsListing;

        }

        [HttpPost]
        [Route("CreateSocialLink")]
        public async Task<IActionResult> CreateSocialLink(SocialNetworkViewModel socialnetworkVM)
        {
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
                            var socialnetwork = await _socialNetworkRepository.GetSocialNetworkByListingId(listing.Listingid);
                            bool recordNotFound = socialnetwork == null;
                            if (recordNotFound)
                            {
                                socialnetwork = new SocialNetwork
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                                };
                            }

                            // Map properties from CommunicationViewModel to Communication

                            socialnetwork.Facebook = socialnetworkVM.Facebook;
                            socialnetwork.WhatsappGroupLink = socialnetworkVM.WhatsappGroupLink;
                            socialnetwork.Linkedin = socialnetworkVM.Linkedin;
                            socialnetwork.Twitter = socialnetworkVM.Twitter;
                            socialnetwork.Youtube = socialnetworkVM.Youtube;
                            socialnetwork.Instagram = socialnetworkVM.Instagram;
                            socialnetwork.Pinterest = socialnetworkVM.Pinterest;

                            if (recordNotFound)
                            {
                                await _socialNetworkRepository.AddAsync(socialnetwork);
                                return Ok(new { Message = "SocialLink Details created successfully", SocialLink = socialnetwork });
                            }
                            else
                            {
                                await _socialNetworkRepository.UpdateAsync(socialnetwork);
                                return Ok(new { Message = "SocialLink Details Updated successfully", Communication = socialnetwork });
                            }
                        }
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);

                            var socialnetwork = await _socialNetworkRepository.GetSocialNetworkByListingId(listingId);
                            bool recordNotFound = socialnetwork == null;
                            if (recordNotFound)
                            {
                                socialnetwork = new SocialNetwork
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listingId,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                                };
                            }

                            // Map properties from CommunicationViewModel to Communication

                            socialnetwork.Facebook = socialnetworkVM.Facebook;
                            socialnetwork.WhatsappGroupLink = socialnetworkVM.WhatsappGroupLink;
                            socialnetwork.Linkedin = socialnetworkVM.Linkedin;
                            socialnetwork.Twitter = socialnetworkVM.Twitter;
                            socialnetwork.Youtube = socialnetworkVM.Youtube;
                            socialnetwork.Instagram = socialnetworkVM.Instagram;
                            socialnetwork.Pinterest = socialnetworkVM.Pinterest;

                            if (recordNotFound)
                            {
                                await _socialNetworkRepository.AddAsync(socialnetwork);
                                return Ok(new { Message = "SocialLink Details created successfully", SocialLink = socialnetwork });
                            }
                            else
                            {
                                await _socialNetworkRepository.UpdateAsync(socialnetwork);
                                return Ok(new { Message = "SocialLink Details Updated successfully", Communication = socialnetwork });
                            }
                        }
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
