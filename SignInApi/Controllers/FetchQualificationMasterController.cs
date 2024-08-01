using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchQualificationMasterController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        public FetchQualificationMasterController(ISharedService sharedService)
        {
            _sharedService = sharedService;
        }

        [HttpGet]
        [Route("GetQualificationMaster")]
        public async Task<IActionResult> GetQualificationMaster()
        {
            var Qualification = await _sharedService.GetQualifications();
            object response = new { Qualification = Qualification };
            return Ok(response);
        }
    }
}
