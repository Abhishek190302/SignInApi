using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Net;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialisationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly SpecialisationRepository _specialisationRepository;
        public SpecialisationController(UserService userService, SpecialisationRepository specialisationRepository, CompanyDetailsRepository companydetailsRepository)
        {
            _userService = userService;
            _specialisationRepository = specialisationRepository;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpPost]
        [Route("CreateSpecialisation")]
        public async Task<IActionResult> CreateSpecialisation([FromBody] SpecialisationVM specialisationVM)
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
                        var specialisation = await _specialisationRepository.GetSpecialisationByListingId(listing.Listingid);
                        bool recordNotFound = specialisation == null;
                        if (recordNotFound)
                        {
                            specialisation = new Specialisation
                            {
                                OwnerGuid=currentUserGuid,
                                ListingID=listing.Listingid,
                                IPAddress= HttpContext.Connection.RemoteIpAddress.ToString()
                            };                            
                        }

                        // Map properties from SpecializationViewModel to Specialization

                        specialisation.OwnerGuid = currentUserGuid;
                        specialisation.ListingID = listing.Listingid;
                        specialisation.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                        specialisation.AcceptTenderWork = specialisationVM.AcceptTenderWork;
                        specialisation.Bank = specialisationVM.Bank;
                        specialisation.BeautyParlors = specialisationVM.BeautyParlors;
                        specialisation.Bungalow = specialisationVM.Bungalow;
                        specialisation.CallCenter = specialisationVM.CallCenter;
                        specialisation.Church = specialisationVM.Church;
                        specialisation.Company = specialisationVM.Company;
                        specialisation.ComputerInstitute = specialisationVM.ComputerInstitute;
                        specialisation.Dispensary = specialisationVM.Dispensary;
                        specialisation.ExhibitionStall = specialisationVM.ExhibitionStall;
                        specialisation.Factory = specialisationVM.Factory;
                        specialisation.Farmhouse = specialisationVM.Farmhouse;
                        specialisation.Gurudwara = specialisationVM.Gurudwara;
                        specialisation.Gym = specialisationVM.Gym;
                        specialisation.HealthClub = specialisationVM.HealthClub;
                        specialisation.Home = specialisationVM.Home;
                        specialisation.Hospital = specialisationVM.Hospital;
                        specialisation.Hotel = specialisationVM.Hotel;
                        specialisation.Laboratory = specialisationVM.Laboratory;
                        specialisation.Mandir = specialisationVM.Mandir;
                        specialisation.Mosque = specialisationVM.Mosque;
                        specialisation.Office = specialisationVM.Office;
                        specialisation.Plazas = specialisationVM.Plazas;
                        specialisation.ResidentialSociety = specialisationVM.ResidentialSociety;
                        specialisation.Resorts = specialisationVM.Resorts;
                        specialisation.Restaurants = specialisationVM.Restaurants;
                        specialisation.Salons = specialisationVM.Salons;
                        specialisation.Shop = specialisationVM.Shop;
                        specialisation.ShoppingMall = specialisationVM.ShoppingMall;
                        specialisation.Showroom = specialisationVM.Showroom;
                        specialisation.Warehouse = specialisationVM.Warehouse;

                        if (recordNotFound)
                        {
                            await _specialisationRepository.AddAsync(specialisation);
                            return Ok(new { Message = "Specialization Details created successfully", Specialization = specialisation });
                        }
                        else
                        {
                            await _specialisationRepository.UpdateAsync(specialisation);
                            return Ok(new { Message = "Specialization Details updated successfully", Specialization = specialisation });
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
    }
}
