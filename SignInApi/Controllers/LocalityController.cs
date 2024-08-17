using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalityController : ControllerBase
    {
        private readonly ILocalityService _localityService;

        public LocalityController(ILocalityService localityService)
        {
            _localityService = localityService;
        }

        [HttpPost]
        [Route("CreateLocality")]
        public async Task<IActionResult> CreateLocalityAsync([FromBody] LocalityCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.LocalityName) || request.CityId <= 0)
            {
                return BadRequest("Country, State, and City must be selected, and locality name must not be blank.");
            }

            try
            {
                var localityExist = await _localityService.GetLocalityByLocalityNameAsync(request.LocalityName);
                if (localityExist != null)
                {
                    return Conflict($"Locality {request.LocalityName} already exists.");
                }

                await _localityService.CreateLocalityAsync(request);

                var city = await _localityService.GetCityByIdAsync(request.CityId);

                return Ok($"Locality {request.LocalityName} created inside city {city.Name}.");
            }
            catch (Exception exc)
            {
                return StatusCode(500, exc.Message);
            }
        }

    }
}
