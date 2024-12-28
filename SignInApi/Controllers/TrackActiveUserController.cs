using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackActiveUserController : ControllerBase
    {
        private readonly ITrackUserService _trackUserService;
        public TrackActiveUserController(ITrackUserService trackUserService)
        {
            _trackUserService = trackUserService;
        }

        [HttpPost]
        [Route("TrackActiveUser")]
        public IActionResult TrackActiveUser([FromBody] ActiveUserModel model)
        {
            _trackUserService.TrackActiveUser(model);
            return Ok(new { message = "User activity tracked successfully." });
        }
    }
}
