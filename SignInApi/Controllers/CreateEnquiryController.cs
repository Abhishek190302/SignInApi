using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateEnquiryController : ControllerBase
    {
        private readonly ListingEnquiryService _listingEnquiryService;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateEnquiryController(ListingEnquiryService listingEnquiryService, CompanyDetailsRepository companyDetailsRepository, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _listingEnquiryService = listingEnquiryService;
            _companydetailsRepository = companyDetailsRepository;
        }

        [HttpPost]
        [Route("CreateEnquiry")]
        public async Task<IActionResult> CreateEnquiry([FromBody] ListingEnquiryModel listingEnquiry)
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
                        var userprofile = await _listingEnquiryService.GetUserByUserNameAsync(userName);
                        var userProfile = await _listingEnquiryService.GetProfileByOwnerGuidAsync(currentUserGuid);
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            var enquiry = await _listingEnquiryService.GetEnquiryListingIdAsync(listing.Listingid);
                            bool recordNotFound = enquiry == null;
                            if (recordNotFound)
                            {
                                enquiry = new ListingEnquiry
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                                };
                            }


                            //listingEnquiry.FullName = userName?.Name;
                            //listingEnquiry.Email = user.Email;
                            //listingEnquiry.MobileNumber = user.PhoneNumber;


                            // Map the property for enquiry 
                            enquiry.FullName = userProfile.Name;
                            enquiry.Email = userprofile.Email;
                            enquiry.MobileNumber = userprofile.PhoneNumber;
                            enquiry.EnquiryTitle = listingEnquiry.EnquiryTitle;
                            enquiry.Message = listingEnquiry.Message;


                            await _listingEnquiryService.AddAsync(enquiry);
                            return Ok(new { Message = "Enquiry Sent Successfully", Response = enquiry });

                                //await _listingEnquiryService.UpdateAsync(enquiry);
                                //return Ok(new { Response = enquiry });
                            
                        }
                        else
                        {
                            
                            var enquiry = new ListingEnquiry
                            {
                                OwnerGuid = currentUserGuid,
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                            };
                            

                            enquiry.FullName = userProfile.Name;
                            enquiry.Email = userprofile.Email;
                            enquiry.MobileNumber = userprofile.PhoneNumber;
                            enquiry.EnquiryTitle = listingEnquiry.EnquiryTitle;
                            enquiry.Message = listingEnquiry.Message;


                            await _listingEnquiryService.AddAsync(enquiry);
                            return Ok(new { Message = "Enquiry Sent Successfully", Response = enquiry });
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }
    }
}
