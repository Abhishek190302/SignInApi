using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;

        public SubCategoriesController(ICategoryServices categoryService)
        {
            _categoryServices = categoryService;
        }

        [HttpGet]
        [Route("GetSubCategoriesWebDevelopment")]
        public async Task<IActionResult> GetSubCategoriesWebDevelopment()
        {
            var categories = await _categoryServices.GetCategoriesForWebDevelopment();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesNetting")]
        public async Task<IActionResult> GetSubCategoriesNetting()
        {
            var categories = await _categoryServices.GetCategoriesForNetting();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPrinting")]
        public async Task<IActionResult> GetSubCategoriesPrinting()
        {
            var categories = await _categoryServices.GetCategoriesForPrinting();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPackcaging")]
        public async Task<IActionResult> GetSubCategoriesPackcaging()
        {
            var categories = await _categoryServices.GetCategoriesForPackcaging();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesArtModuling")]
        public async Task<IActionResult> GetSubCategoriesArtModuling()
        {
            var categories = await _categoryServices.GetCategoriesForArtModuling();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesComputeriseCutting")]
        public async Task<IActionResult> GetSubCategoriesComputeriseCutting()
        {
            var categories = await _categoryServices.GetCategoriesForComputeriseCutting();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesMetalBending")]
        public async Task<IActionResult> GetSubCategoriesMetalBending()
        {
            var categories = await _categoryServices.GetCategoriesForMetalBending();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPump")]
        public async Task<IActionResult> GetSubCategoriesPump()
        {
            var categories = await _categoryServices.GetCategoriesForPump();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesSolarPanal")]
        public async Task<IActionResult> GetSubCategoriesSolarPanal()
        {
            var categories = await _categoryServices.GetCategoriesForSolarPanel();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesTankCleaning")]
        public async Task<IActionResult> GetSubCategoriesTankCleaning()
        {
            var categories = await _categoryServices.GetCategoriesForTankCleaning();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPackersMovers")]
        public async Task<IActionResult> GetSubCategoriesPackersMovers()
        {
            var categories = await _categoryServices.GetCategoriesForPackersMovers();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesGardening")]
        public async Task<IActionResult> GetSubCategoriesGardening()
        {
            var categories = await _categoryServices.GetCategoriesForGardening();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesLegalAdvesory")]
        public async Task<IActionResult> GetSubCategoriesLegalAdvesory()
        {
            var categories = await _categoryServices.GetCategoriesForLegalAdvisory();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesSecurity")]
        public async Task<IActionResult> GetSubCategoriesSecurity()
        {
            var categories = await _categoryServices.GetCategoriesForSecurity();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesAutomation")]
        public async Task<IActionResult> GetSubCategoriesAutomation()
        {
            var categories = await _categoryServices.GetCategoriesForAutomation();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesRealEstate")]
        public async Task<IActionResult> GetSubCategoriesRealEstate()
        {
            var categories = await _categoryServices.GetCategoriesForRealEastate();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPestControl")]
        public async Task<IActionResult> GetSubCategoriesPestControl()
        {
            var categories = await _categoryServices.GetCategoriesForPestControl();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesHouseKeping")]
        public async Task<IActionResult> GetSubCategoriesHouseKeping()
        {
            var categories = await _categoryServices.GetCategoriesForHouseKeping();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesTransport")]
        public async Task<IActionResult> GetSubCategoriesTransport()
        {
            var categories = await _categoryServices.GetCategoriesForTransport();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesAdvertisingAgency")]
        public async Task<IActionResult> GetSubCategoriesAdvertisingAgency()
        {
            var categories = await _categoryServices.GetCategoriesForAdvertisingAgency();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesWaterSuppliers")]
        public async Task<IActionResult> GetSubCategoriesWaterSuppliers()
        {
            var categories = await _categoryServices.GetCategoriesForWaterSuppliers();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesPolishCoating")]
        public async Task<IActionResult> GetSubCategoriesPolishCoating()
        {
            var categories = await _categoryServices.GetCategoriesForPolishCoating();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesToursTravels")]
        public async Task<IActionResult> GetSubCategoriesToursTravels()
        {
            var categories = await _categoryServices.GetCategoriesForToursTravels();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesCourier")]
        public async Task<IActionResult> GetSubCategoriesCourier()
        {
            var categories = await _categoryServices.GetCategoriesForCourier();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesLeafing")]
        public async Task<IActionResult> GetSubCategoriesLeafing()
        {
            var categories = await _categoryServices.GetCategoriesForLeafing();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesConsultants")]
        public async Task<IActionResult> GetSubCategoriesConsultants()
        {
            var categories = await _categoryServices.GetCategoriesForConsultants();
            return Ok(categories);
        }


        [HttpGet]
        [Route("GetSubCategoriesSurveyours")]
        public async Task<IActionResult> GetSubCategoriesSurveyours()
        {
            var categories = await _categoryServices.GetCategoriesForSurveyors();
            return Ok(categories);
        }

        [HttpGet]
        [Route("GetSubCategoriesEngineers")]
        public async Task<IActionResult> GetSubCategoriesEngineers()
        {
            var categories = await _categoryServices.GetCategoriesForEngineers();
            return Ok(categories);
        }
    }
}
