using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly string _connectionString;

        public ImageUploadController(IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, UserService userService)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _companydetailsRepository = companydetailsRepository;
            _userService = userService;
        }

        [HttpPost]
        [Route("UploadLogoImage")]
        public async Task<IActionResult> UploadLogoImage(IFormFile file)
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

                        if (file == null || file.Length == 0)
                            return BadRequest("No file uploaded.");

                        var imagePath = Path.Combine("wwwroot/images/logos", file.FileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var imageUrl = $"/images/logos/{file.FileName}";

                        using (var connection = new SqlConnection(_connectionString))
                        {
                            var command = new SqlCommand("INSERT INTO [dbo].[LogoImage] (OwnerGuid,ListingID,ImagePath,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,GETDATE(),GETDATE())", connection);
                            command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                            command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                            command.Parameters.AddWithValue("@ImagePath", imageUrl);

                            connection.Open();
                            await command.ExecuteNonQueryAsync();
                        }

                        return Ok(new { ImageUrl = imageUrl });

                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            return NotFound("User not found");
        }
    }
}

