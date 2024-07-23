using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingHoursController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly WorkingHoursRepository _workingsRepository;
        public WorkingHoursController(UserService userService, WorkingHoursRepository workingsRepository, CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _workingsRepository = workingsRepository;
            _companydetailsRepository = companydetailsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("WorkingHours")]
        public async Task<IActionResult> WorkingHours([FromBody] WorkingHoursViewModel workinghoursVM)
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
                            var workinghours = await _workingsRepository.GetWorkingHoursByListingId(listing.Listingid);
                            bool recordNotFound = workinghours == null;
                            if (recordNotFound)
                            {
                                workinghours = new WorkingHours
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                                };
                            }

                            // Map properties from WorkingHoursViewModel to WorkingHours
                            workinghours.MondayFrom = Convert.ToDateTime(workinghoursVM.MondayFrom);
                            workinghours.MondayTo = Convert.ToDateTime(workinghoursVM.MondayTo);
                            workinghours.TuesdayFrom = Convert.ToDateTime(workinghoursVM.TuesdayFrom);
                            workinghours.TuesdayTo = Convert.ToDateTime(workinghoursVM.TuesdayTo);
                            workinghours.WednesdayFrom = Convert.ToDateTime(workinghoursVM.WednesdayFrom);
                            workinghours.WednesdayTo = Convert.ToDateTime(workinghoursVM.WednesdayTo);
                            workinghours.ThursdayFrom = Convert.ToDateTime(workinghoursVM.ThursdayFrom);
                            workinghours.ThursdayTo = Convert.ToDateTime(workinghoursVM.ThursdayTo);
                            workinghours.FridayFrom = Convert.ToDateTime(workinghoursVM.FridayFrom);
                            workinghours.FridayTo = Convert.ToDateTime(workinghoursVM.FridayTo);
                            workinghours.SaturdayFrom = Convert.ToDateTime(workinghoursVM.SaturdayFrom);
                            workinghours.SaturdayTo = Convert.ToDateTime(workinghoursVM.SaturdayTo);
                            workinghours.SundayFrom = Convert.ToDateTime(workinghoursVM.SundayFrom);
                            workinghours.SundayTo = Convert.ToDateTime(workinghoursVM.SundayTo);
                            workinghours.SaturdayHoliday = workinghoursVM.SaturdayHoliday;
                            workinghours.SundayHoliday = workinghoursVM.SundayHoliday;

                            if (recordNotFound)
                            {
                                await _workingsRepository.WorkingHoursAddAsync(workinghours);
                                return Ok(new { Message = "WorkingHours Details created successfully", WorkingHours = workinghours });
                            }
                            else
                            {
                                await _workingsRepository.WorkingHoursUpdateAsync(workinghours);
                                return Ok(new { Message = "WorkingHours Details Updated successfully", WorkingHours = workinghours });
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

        private DateTime CombineDateAndTime(TimeSpan time)
        {
            DateTime today = DateTime.Today;
            return today.Add(time);
        }
    }
}
