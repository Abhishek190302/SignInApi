using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost]
        [Route("CreateArea")]
        public async Task<IActionResult> CreateAreaAsync([FromBody] AreaCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.AreaName) || request.PincodeId <= 0)
            {
                return BadRequest(new { StatusCode = 400, Message = "Country, State, City, Area, and Pincode must be selected, and Area Name must not be blank." });
                //return BadRequest("Country, State, City, Area, and Pincode must be selected, and Area Name must not be blank.");
            }

            try
            {
                var areaExist = await _areaService.GetAreaByAreaNameAsync(request.AreaName);
                if (areaExist != null)
                {
                    return Conflict(new { StatusCode = 409, Message = $"Area {request.AreaName} already exists." });
                    //return Conflict($"Area {request.AreaName} already exists.");
                }

                await _areaService.CreateAreaAsync(request);

                var pincode = await _areaService.GetPincodeByIdAsync(request.PincodeId);

                return Ok(new { StatusCode = 200, Message = $"Area {request.AreaName} created inside pincode {pincode.Number}." });
                //return Ok($"Area {request.AreaName} created inside pincode {pincode.Number}.");
            }
            catch (Exception exc)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An unexpected error occurred while creating the Area.", Details = exc.Message });
                //return StatusCode(500, new { StatusCode = 500, Message = "An unexpected error occurred while creating the Area.", Details = exc.Message });
            }
        }
    }
}
