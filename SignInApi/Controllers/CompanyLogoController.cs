using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyLogoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _baseImageUrl = "https://apidev.myinteriormart.com";

        public CompanyLogoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        // GET api/CompanyLogo?listingId=123
        [HttpGet]
        public async Task<IActionResult> GetLogoImage([FromQuery] int listingId)
        {
            if (listingId <= 0)
            {
                return BadRequest("Invalid ListingID.");
            }

            var logoImage = await GetLogoImageByListingId(listingId);

            // Define the default image path
            var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "myinteriorlog.png");

            // Determine the image path
            var imagePath = logoImage != null
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", logoImage.ImagePath.TrimStart('/')) // Remove leading slash if present
                : defaultImagePath;

            // Check if the file exists
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found.");
            }

            // Get the file extension to determine the content type
            var fileExtension = Path.GetExtension(imagePath).ToLower();
            string contentType;

            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".svg":
                    contentType = "image/svg+xml";
                    break;
                default:
                    contentType = "application/octet-stream"; // fallback
                    break;
            }

            // Return the image file directly
            var image = System.IO.File.OpenRead(imagePath);
            return File(image, contentType);
        }

        private async Task<LogoImage> GetLogoImageByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT ImagePath FROM [dbo].[LogoImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new LogoImage { ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")) };
                    }
                }
            }
            return null;
        }
    }
}
