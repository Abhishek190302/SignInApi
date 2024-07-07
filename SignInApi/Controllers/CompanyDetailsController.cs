using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CompanyDetailsController(UserService userService,CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _companydetailsRepository = companydetailsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("AddOrUpdateCompanyDetails")]
        public async Task<IActionResult> AddOrUpdateCompanyDetails(CompanyViewModel companyVM)
        {
            if (!companyVM.IsValid())
            {
                return BadRequest("All fields are compulsory.");
            }

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
                        bool recordNotFound = listing == null;

                        if (recordNotFound)
                        {
                            listing = new Listing
                            {
                                OwnerGuid = currentUserGuid,
                                CreatedDate = DateTime.UtcNow,
                                CreatedTime = DateTime.UtcNow,
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                Status = 0
                            };
                        }

                        // Map properties from CompanyViewModel to Listing
                        listing.CompanyName = companyVM.CompanyName;
                        listing.BusinessCategory = companyVM.BusinessCategory;
                        listing.NatureOfBusiness = companyVM.NatureOfBusiness;
                        listing.YearOfEstablishment = companyVM.YearOfEstablishment;
                        listing.NumberOfEmployees = companyVM.NumberOfEmployees;
                        listing.Turnover = companyVM.Turnover;
                        listing.GSTNumber = companyVM.GSTNumber;
                        listing.Description = companyVM.Description;

                        if (recordNotFound)
                        {

                            await _companydetailsRepository.AddListingAsync(listing);
                            return Ok(new { Message = "Company Details created successfully", Listing = listing });
                        }
                        else
                        {
                            listing.CreatedDate = DateTime.UtcNow;
                            listing.CreatedTime = DateTime.UtcNow;
                            listing.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                            listing.Status = 1;

                            await _companydetailsRepository.UpdateListingAsync(listing);
                            return Ok(new { Message = "Company Details updated successfully", Listing = listing });
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
