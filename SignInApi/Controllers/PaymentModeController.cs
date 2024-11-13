using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Net.Mail;
using System.Net;

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
        private readonly CommunicationRepository _communicationRepository;
        public PaymentModeController(PaymentModeRepository paymentModeRepository, UserService userService, CompanyDetailsRepository companyDetailsRepository, IHttpContextAccessor httpContextAccessor, CommunicationRepository communicationRepository)
        {
            _userService = userService;
            _paymentModeRepository = paymentModeRepository;
            _companydetailsRepository = companyDetailsRepository;
            _httpContextAccessor = httpContextAccessor;
            _communicationRepository = communicationRepository;

        }

        [HttpPost]
        [Route("CreatePaymentMode")]
        public async Task<IActionResult> CreatePaymentMode([FromBody] PaymentModeViewModel paymentmodeVM)
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

                            var communication = await _communicationRepository.GetCommunicationByListingIdAsync(listing.Listingid);
                            if (communication == null || string.IsNullOrEmpty(communication.Mobile))
                            {
                                return BadRequest("Mobile number not found for this listing.");
                            }

                            if (recordNotFound)
                            {
                                string companyName = listing.ListingURL;  // Get the company name
                                string message = $"Hello {companyName} Congratulations! Your Business Listing has been Under review. After review your listing has been Live within 48 hour's My Interior Mart Team";
                                using (HttpClient httpClient = new HttpClient())
                                {
                                    string smsApiUrl = $"http://text.bluemedia.in/http-tokenkeyapi.php?authentic-key=32316d79696e746572696f726d6172743737391729246892&senderid=MYINTR&route=2&number={communication.Mobile}&message={message}&templateid=1207172949318660895";
                                    //string smsApiUrl = $"http://vas.hexaroute.com/api.php?username=myinteriormart&password=pass1234&route=1&sender=MyIntM&mobile[]={communication.Mobile}&message[]={message}&te_id=1207172526119813069";
                                    HttpResponseMessage response = await httpClient.GetAsync(smsApiUrl);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string responseContent = await response.Content.ReadAsStringAsync();
                                    }
                                    else
                                    {
                                        return StatusCode((int)response.StatusCode, "Failed to send Message via SMS.");
                                    }
                                }

                                await SendPaymentSubmittedEmail(paymentmode.ListingID);

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
            }
            return Unauthorized();
        }

        private async Task SendPaymentSubmittedEmail(int listingId)
        {
            var listing = await _companydetailsRepository.GetListingByIdAsync(listingId);

            if (listing == null)
            {
                Console.WriteLine("Listing not found.");
                return;
            }

            // Retrieve communication details using the listingId
            var communication = await _communicationRepository.GetCommunicationByListingIdAsync(listing.Listingid);

            if (communication == null || string.IsNullOrEmpty(communication.Email))
            {
                Console.WriteLine("Vendor email not found.");
                return;
            }

            var fromAddress = new MailAddress("contact@myinteriormart.com", "Myinteriormart");
            var toAddress = new MailAddress(communication.Email);
            var fromPassword = "Hamza@313#";
            var subject = "" + listing.ListingURL + " Your Listing Created";

            string body = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Your Listing Created Successfully</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #ffffff; margin: 0; padding: 0; text-align: center; }}
                        .container {{ padding: 20px; max-width: 600px; margin: 0 auto; }}
                        h1 {{ color: #f57c00; font-size: 24px; }}
                        p {{ font-size: 16px; color: #333; margin: 20px 0; }}
                        a {{ color: #f57c00; text-decoration: none; font-weight: bold; }}
                        .footer {{ margin-top: 40px; }}
                        .footer img {{ width: 150px; }}
                    </style>
                </head>
                    <body>
                    <div class='container'>
                        <h1>Hello {listing.ListingURL}, Listing Created Successfully</h1>
                        <p>Congratulation! Your Business Listing has Under review. After</p>
                        <p>review your listing has been live within 48 hour's - My Interior Mart Team.</p>
                        <p>For any help, connect over WhatsApp: <strong>+91 9876543210</strong></p>
                        <p>Thank you!<br>Have a wonderful day!!!</p>
                    </div>
                </body>
                </html>";

            using (var message = new MailMessage())
            {
                message.From = fromAddress;
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("mail.myinteriormart.com", 587))
                {
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);

                    smtp.SendCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            Console.WriteLine($"Send failed: {e.Error.Message}");
                        }
                        else
                        {
                            Console.WriteLine("Payment submission email sent successfully.");
                        }
                    };

                    await smtp.SendMailAsync(message);
                }
            }
        }
    }
}
