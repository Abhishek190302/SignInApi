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
                                    BannerLink = reader["BannerLink"].ToString(),
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
                },
                CategoryBanners = new
                {
                    HomecategoryBanners = new[]
                    {
                        new
                        {
                            priority = 1,
                            location = "Mumbai",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Medical Clinic_1.png",
                            bannerLink = "https://www.facebook.com/myinteriormart",
                            bannerType = "Category Home Image",
                            galleryBannerType = ""
                        },
                        new
                        {
                            priority = 2,
                            location = "Delhi",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Medical Clinic.png",
                            bannerLink = "https://www.twitter.com/myinteriormart",
                            bannerType = "Category Home Image",
                            galleryBannerType = ""
                        }
                    },

                    AdvertiseCategoryBanner = new[]
                    {
                        new
                        {
                            priority = 3,
                            location = "Mumbai",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Services_1.png",
                            bannerLink = "https://www.facebook.com/myinteriormart",
                            bannerType = "Category Advertise Image",
                            galleryBannerType = ""
                        },
                        new
                        {
                            priority = 4,
                            location = "Delhi",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Contractor.png",
                            bannerLink = "https://www.twitter.com/myinteriormart",
                            bannerType = "Category Advertise Image",
                            galleryBannerType = ""
                        },
                        new
                        {
                            priority = 5,
                            location = "Kolhapur",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Contractor_1.png",
                            bannerLink = "https://www.twitter.com/myinteriormart",
                            bannerType = "Category Advertise Image",
                            galleryBannerType = ""
                        },
                        new
                        {
                            priority = 6,
                            location = "Kolhapur",
                            imagePath = "/Banners/GalleryBanners/6894b8b3-1507-4056-9360-33f8d564fe83/Dealear_1.png",
                            bannerLink = "https://www.twitter.com/myinteriormart",
                            bannerType = "Category Advertise Image",
                            galleryBannerType = ""
                        }
                    }
                }
            };

            return Ok(response);
        }
    }
}
