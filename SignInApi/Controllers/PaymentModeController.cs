using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentModeController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PaymentModeRepository _paymentModeRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public PaymentModeController(PaymentModeRepository paymentModeRepository, UserService userService, CompanyDetailsRepository companyDetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _paymentModeRepository = paymentModeRepository;
            _companydetailsRepository = companyDetailsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("CreatePaymentMode")]
        public async Task<IActionResult> CreatePaymentMode([FromBody] PaymentModeViewModel paymentmodeVM)
        {
            //var user = _httpContextAccessor.HttpContext.User;
            //if (user.Identity.IsAuthenticated)
            //{
            //    var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName("web@jeb.com");
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            var paymentmode = await _paymentModeRepository.GetPaymentModeByListingId(listing.Listingid);
                            bool recordNotFound = paymentmode == null;
                            if (recordNotFound)
                            {
                                paymentmode = new PaymentMode
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                                };
                            }

                            // Map properties from SpecializationViewModel to Specialization

                            paymentmode.OwnerGuid = currentUserGuid;
                            paymentmode.ListingID = listing.Listingid;
                            paymentmode.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();


                            if (paymentmodeVM.SelectAll)
                            {
                                paymentmode.Cash = true;
                                paymentmode.Cheque = true;
                                paymentmode.RtgsNeft = true;
                                paymentmode.DebitCard = true;
                                paymentmode.CreditCard = true;
                                paymentmode.NetBanking = true;
                            }
                            else
                            {
                                paymentmode.Cash = paymentmodeVM.Cash;
                                paymentmode.Cheque = paymentmodeVM.Cheque;
                                paymentmode.RtgsNeft = paymentmodeVM.RtgsNeft;
                                paymentmode.DebitCard = paymentmodeVM.DebitCard;
                                paymentmode.CreditCard = paymentmodeVM.CreditCard;
                                paymentmode.NetBanking = paymentmodeVM.NetBanking;
                            }

                            if (recordNotFound)
                            {
                                await _paymentModeRepository.AddAsync(paymentmode);
                                return Ok(new { Message = "PaymentMode Details created successfully", PaymantMode = paymentmode });
                            }
                            else
                            {
                                await _paymentModeRepository.UpdateAsync(paymentmode);
                                return Ok(new { Message = "PaymentMode Details updated successfully", PaymantMode = paymentmode });
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        return StatusCode(500, exc.Message);
                    }
                }
                return NotFound("User not found");


            //}
            //return Unauthorized();
        }
    }
}
