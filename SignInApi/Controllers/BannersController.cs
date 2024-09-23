using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public BannersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        [Route("GetFilteredBanners")]
        public async Task<IActionResult> GetFilteredBanners()
        {
            string connectionString = _configuration.GetConnectionString("MimListing");

            List<Banner> homeMegaBannerImages = new List<Banner>();
            List<Banner> galleryBannerImages = new List<Banner>();
            List<Banner> servicesBanners = new List<Banner>();
            List<Banner> contractorBanners = new List<Banner>();
            List<Banner> dealearBanners = new List<Banner>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Galleryhome_Banner]", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Banner banner = new Banner
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Priority = Convert.ToInt32(reader["Priority"]),
                                    Location = reader["Location"].ToString(),
                                    ImagePath = reader["imagepath"].ToString(),
                                    BannerType = reader["BannerType"].ToString(),
                                    GalleryBannerType = reader["GalleryBannersType"].ToString()
                                };

                                // Filter based on BannerType
                                if (banner.BannerType == "Home Mega Banner Image")
                                {
                                    homeMegaBannerImages.Add(banner);
                                }
                                else if (banner.BannerType == "Gallery Banner Image")
                                {
                                    galleryBannerImages.Add(banner);

                                    // Check for "Services Banners"
                                    if (banner.GalleryBannerType == "Services Banners")
                                    {
                                        servicesBanners.Add(banner);
                                    }
                                    if (banner.GalleryBannerType == "Contractor Banners")
                                    {
                                        contractorBanners.Add(banner);
                                    }
                                    if (banner.GalleryBannerType == "Dealers Banners")
                                    {
                                        dealearBanners.Add(banner);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw;
                }
                
            }

            var response = new
            {
                HomeMegaBannerImages = homeMegaBannerImages,
                GalleryBannerImages = new
                {
                    ServicesBanners = servicesBanners,
                    ContractorBanners = contractorBanners,
                    DealerBanners = dealearBanners
                }
            };

            return Ok(response);
        }
    }
}
