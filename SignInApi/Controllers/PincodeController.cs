using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PincodeController : ControllerBase
    {
        private readonly IPincodeService _pincodeService;

        public PincodeController(IPincodeService pincodeService)
        {
            _pincodeService = pincodeService;
        }

        [HttpPost]
        [Route("CreatePincode")]
        public async Task<IActionResult> CreatePincodeAsync([FromBody] PincodeCreateRequest request)
        {
            if (request.PinNumber <= 0 || request.LocalityId <= 0)
            {
                return BadRequest("Country, State, City, and Area must be selected, and pincode number must not be blank.");
            }

            try
            {
                var pincodeExist = await _pincodeService.GetPincodeByPinNumberAsync(request.PinNumber);
                if (pincodeExist != null)
                {
                    return Conflict($"Pincode {request.PinNumber} already exists.");
                }

                await _pincodeService.CreatePincodeAsync(request);

                var locality = await _pincodeService.GetLocalityByIdAsync(request.LocalityId);

                return Ok($"Pincode {request.PinNumber} created inside {locality.Name}.");
            }
            catch (Exception exc)
            {
                return StatusCode(500, exc.Message);
            }
        }
    }
}
