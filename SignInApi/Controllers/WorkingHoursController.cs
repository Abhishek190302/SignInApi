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
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly WorkingHoursRepository _workingsRepository;
        public WorkingHoursController(UserService userService, WorkingHoursRepository workingsRepository, CompanyDetailsRepository companydetailsRepository)
        {
            _userService= userService;
            _workingsRepository= workingsRepository;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpPost]
        [Route("WorkingHours")]
        public async Task<IActionResult> WorkingHours([FromBody] WorkingHoursViewModel workinghoursVM)
        {
            var applicationUser = await _userService.GetUserByUserName("web@jeb.com");
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
                        workinghours.MondayFrom = CombineDateAndTime(workinghoursVM.MondayFrom);
                        workinghours.MondayTo = CombineDateAndTime(workinghoursVM.MondayTo);
                        workinghours.TuesdayFrom = CombineDateAndTime(workinghoursVM.TuesdayFrom);
                        workinghours.TuesdayTo = CombineDateAndTime(workinghoursVM.TuesdayTo);
                        workinghours.WednesdayFrom = CombineDateAndTime(workinghoursVM.WednesdayFrom);
                        workinghours.WednesdayTo = CombineDateAndTime(workinghoursVM.WednesdayTo);
                        workinghours.ThursdayFrom = CombineDateAndTime(workinghoursVM.ThursdayFrom);
                        workinghours.ThursdayTo = CombineDateAndTime(workinghoursVM.ThursdayTo);
                        workinghours.FridayFrom = CombineDateAndTime(workinghoursVM.FridayFrom);
                        workinghours.FridayTo = CombineDateAndTime(workinghoursVM.FridayTo);
                        workinghours.SaturdayFrom = CombineDateAndTime(workinghoursVM.SaturdayFrom);
                        workinghours.SaturdayTo = CombineDateAndTime(workinghoursVM.SaturdayTo);
                        workinghours.SundayFrom = CombineDateAndTime(workinghoursVM.SundayFrom);
                        workinghours.SundayTo = CombineDateAndTime(workinghoursVM.SundayTo);
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

        private DateTime CombineDateAndTime(TimeSpan time)
        {
            DateTime today = DateTime.Today;
            return today.Add(time);
        }
    }
}
