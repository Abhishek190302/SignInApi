using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinddetailsListingController : ControllerBase
    {
        private readonly BinddetailsManagelistingRepository _binddetailsListing;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BinddetailsListingController(BinddetailsManagelistingRepository binddetailsListing, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _binddetailsListing = binddetailsListing;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetCompanyDetailslisting")]
        public async Task<IActionResult> GetCompanyDetailslisting()
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
                        var Companydetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        return Ok(Companydetails);
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

        [HttpGet]
        [Route("GetCommunicationDetailslisting")]
        public async Task<IActionResult> GetCommunicationDetailslisting()
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
                        var communicationdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (communicationdetails != null)
                        {
                            var Communication = await _binddetailsListing.GetCommunicationByListingIdAsync(communicationdetails.Listingid);
                            return Ok(Communication);
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

        [HttpGet]
        [Route("GetAddressDetailslisting")]
        public async Task<IActionResult> GetAddressDetailslisting()
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
                        var addressdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (addressdetails != null)
                        {
                            var Address = await _binddetailsListing.GetAddressByListingIdAsync(addressdetails.Listingid);
                            return Ok(Address);
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

        [HttpGet]
        [Route("GetCategoriesDetailslisting")]
        public async Task<IActionResult> GetCategoriesDetailslisting()
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
                        var categoriesdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (categoriesdetails != null)
                        {
                            var Categories = await _binddetailsListing.GetCategoryByListingIdAsync(categoriesdetails.Listingid);
                            return Ok(Categories);
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

        [HttpGet]
        [Route("GetSpecializationDetailslisting")]
        public async Task<IActionResult> GetSpecializationDetailslisting()
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
                        var specializationdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (specializationdetails != null)
                        {
                            var Specialization = await _binddetailsListing.GetSpecialisationByListingId(specializationdetails.Listingid);
                            return Ok(Specialization);
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

        [HttpGet]
        [Route("GetWorkingDetailslisting")]
        public async Task<IActionResult> GetWorkingDetailslisting()
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
                        var workinghoursdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (workinghoursdetails != null)
                        {
                            var WorkingHourse = await _binddetailsListing.GetWorkingHoursByListingId(workinghoursdetails.Listingid);
                            return Ok(WorkingHourse);
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

        [HttpGet]
        [Route("GetPaymentmodeDetailslisting")]
        public async Task<IActionResult> GetPaymentmodeDetailslisting()
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
                        var paymentmodedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (paymentmodedetails != null)
                        {
                            var PaymentMode = await _binddetailsListing.GetPaymentModeByListingId(paymentmodedetails.Listingid);
                            return Ok(PaymentMode);
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

        [HttpGet]
        [Route("GetAddSocialLinkDetailslisting")]
        public async Task<IActionResult> GetAddSocialLinkDetailslisting()
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
                        var addsociallinkdetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (addsociallinkdetails != null)
                        {
                            var AddSocialLink = await _binddetailsListing.GetSocialNetworkByListingId(addsociallinkdetails.Listingid);
                            return Ok(AddSocialLink);
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


        [HttpGet]
        [Route("GetLogoimageDetailslisting")]
        public async Task<IActionResult> GetLogoimageDetailslisting()
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
                        var logoimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (logoimagedetails != null)
                        {
                            var LogoImage = await _binddetailsListing.GetlogoImageByListingIdAsync(logoimagedetails.Listingid);
                            return Ok(LogoImage);
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

        [HttpGet]
        [Route("GetOwnerImageDetailslisting")]
        public async Task<IActionResult> GetOwnerImageDetailslisting()
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
                        var ownerimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (ownerimagedetails != null)
                        {
                            var OwnerImage = await _binddetailsListing.GetOwnerImageByListingIdAsync(ownerimagedetails.Listingid);
                            return Ok(OwnerImage);
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

        [HttpGet]
        [Route("GetGalleryImageDetailslisting")]
        public async Task<IActionResult> GetGalleryImageDetailslisting()
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
                        var galleryimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (galleryimagedetails != null)
                        {
                            var GalleryImage = await _binddetailsListing.GetGallerysImageByListingIdAsync(galleryimagedetails.Listingid);
                            return Ok(GalleryImage);
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

        [HttpGet]
        [Route("GetBannerImageDetailslisting")]
        public async Task<IActionResult> GetBannerImageDetailslisting()
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
                        var bannerimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (bannerimagedetails != null)
                        {
                            var BannerImage = await _binddetailsListing.GetBannerImageByListingIdAsync(bannerimagedetails.Listingid);
                            return Ok(BannerImage);
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

        [HttpGet]
        [Route("GetCertificationImageDetailslisting")]
        public async Task<IActionResult> GetCertificationImageDetailslisting()
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
                        var certificationimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (certificationimagedetails != null)
                        {
                            var CertificationImage = await _binddetailsListing.GetCertificateImageByListingIdAsync(certificationimagedetails.Listingid);
                            return Ok(CertificationImage);
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

        [HttpGet]
        [Route("GetClientImageDetailslisting")]
        public async Task<IActionResult> GetClientImageDetailslisting()
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
                        var clientimagedetails = await _binddetailsListing.GetListingByOwnerIdAsync(currentUserGuid);
                        if (clientimagedetails != null)
                        {
                            var ClientImage = await _binddetailsListing.GetClientImageByListingIdAsync(clientimagedetails.Listingid);
                            return Ok(ClientImage);
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
