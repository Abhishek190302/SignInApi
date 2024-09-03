using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlldetailsparticularListingbindController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly CategoryRepository _categoryRepository;
        public AlldetailsparticularListingbindController(IConfiguration configuration, CategoryRepository categoryRepository)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Route("GetOwnerImage")]
        public async Task<IActionResult> GetOwnerImage(OwnerimageVM ownerimageVM)
        {
            var OwnerImage = await GetOwnerImageByListingIdAsync(ownerimageVM.companyID);
            return Ok(OwnerImage);        
        }

        [HttpPost]
        [Route("GetBannerImage")]
        public async Task<IActionResult> GetBannerImage(BannerimageVM bannerimageVM)
        {
            var BannerImage = await GetBannerImageByListingIdAsync(bannerimageVM.companyID);
            return Ok(BannerImage);
        }

        [HttpPost]
        [Route("GetGalleryImage")]
        public async Task<IActionResult> GetGalleryImage(GalleryimageVM galleryimageVM)
        {
            var GalleryImage = await GetGallerysImageByListingIdAsync(galleryimageVM.companyID);
            return Ok(GalleryImage);
        }

        [HttpPost]
        [Route("GetServicescategory")]
        public async Task<IActionResult> GetServicescategory(ServicescategoryVM servicescategoryVM)
        {
            var categories = await _categoryRepository.GetFirstCategoriesAsync();
            object response = new { AllCategories = categories };

            var Servicescategory = await GetCategoryByListingIdAsync(servicescategoryVM.companyID);
            response = new { Category = Servicescategory, AllCategories = categories };
            return Ok(response);
        }

        [HttpPost]
        [Route("GetSpecialization")]
        public async Task<IActionResult> GetSpecialization(SpecializationVM specializationVM)
        {
            var Specialisation = await GetSpecialisationByListingId(specializationVM.companyID);
            return Ok(Specialisation);
        }

        [HttpPost]
        [Route("GetPaymentDetails")]
        public async Task<IActionResult> GetPaymentDetails(PaymentVM paymentVM)
        {
            var Paymentmode = await GetPaymentModeByListingId(paymentVM.companyID);
            return Ok(Paymentmode);
        }

        [HttpPost]
        [Route("GetKeywordDetails")]
        public async Task<IActionResult> GetKeywordDetails(KeywordVM keywordVM)
        {
            var Keyword = await GetKeywordsByListingIdAsync(keywordVM.companyID);
            return Ok(Keyword);
        }


        private async Task<OwnerImage> GetOwnerImageByListingIdAsync(int companyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ListingID, ImagePath, OwnerName, LastName FROM [dbo].[OwnerImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", companyID);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow firstRow = dt.Rows[0];

                    // Assuming the ImagePath column contains a single concatenated string
                    string concatenatedImagePaths = firstRow.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    return new OwnerImage
                    {
                        Listingid = firstRow.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        OwnerName = firstRow.Field<string>("OwnerName") ?? string.Empty,
                        LastName = firstRow.Field<string>("LastName") ?? string.Empty,
                    };
                }
                return null;
            }
        }

        private async Task<BannerImage> GetBannerImageByListingIdAsync(int companyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ListingID,ImagePath FROM [dbo].[BannerDetail] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", companyID);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new BannerImage
                    {
                        
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                    };
                }
                return null;
            }
        }

        private async Task<GallerysImage> GetGallerysImageByListingIdAsync(int companyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ListingID,ImagePath,ImageTitle FROM [dbo].[GalleryImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", companyID);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    string concatenatedImagePaths = row.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();


                    return new GallerysImage
                    {
                        
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
                        
                    };
                }
                return null;
            }
        }

        private async Task<Categories> GetCategoryByListingIdAsync(int companyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Categories] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", companyID);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Categories
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        FirstCategoryID = row.Field<int?>("FirstCategoryID") ?? 0,
                        SecondCategoryID = row.Field<int?>("SecondCategoryID") ?? 0,
                        ThirdCategoryID = row.Field<string>("ThirdCategories") ?? string.Empty,
                        FourthCategoryID = row.Field<string>("FourthCategories") ?? string.Empty,
                        FifthCategoryID = row.Field<string>("FifthCategories") ?? string.Empty,
                        SixthCategoryID = row.Field<string>("SixthCategories") ?? string.Empty,
                    };
                }
                return null;
            }
        }

        private async Task<Specialisation> GetSpecialisationByListingId(int companyID)
        {
            Specialisation specialisation = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Specialisation] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", companyID);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            specialisation = new Specialisation
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                AcceptTenderWork = Convert.ToBoolean(row["AcceptTenderWork"]),
                                Bank = Convert.ToBoolean(row["Banks"]),
                                BeautyParlors = Convert.ToBoolean(row["BeautyParlors"]),
                                Bungalow = Convert.ToBoolean(row["Bungalow"]),
                                CallCenter = Convert.ToBoolean(row["CallCenter"]),
                                Church = Convert.ToBoolean(row["Church"]),
                                Company = Convert.ToBoolean(row["Company"]),
                                ComputerInstitute = Convert.ToBoolean(row["ComputerInstitute"]),
                                Dispensary = Convert.ToBoolean(row["Dispensary"]),
                                ExhibitionStall = Convert.ToBoolean(row["ExhibitionStall"]),
                                Factory = Convert.ToBoolean(row["Factory"]),
                                Farmhouse = Convert.ToBoolean(row["Farmhouse"]),
                                Gurudwara = Convert.ToBoolean(row["Gurudwara"]),
                                Gym = Convert.ToBoolean(row["Gym"]),
                                HealthClub = Convert.ToBoolean(row["HealthClub"]),
                                Home = Convert.ToBoolean(row["Home"]),
                                Hospital = Convert.ToBoolean(row["Hospital"]),
                                Hotel = Convert.ToBoolean(row["Hotel"]),
                                Laboratory = Convert.ToBoolean(row["Laboratory"]),
                                Mandir = Convert.ToBoolean(row["Mandir"]),
                                Mosque = Convert.ToBoolean(row["Mosque"]),
                                Office = Convert.ToBoolean(row["Office"]),
                                Plazas = Convert.ToBoolean(row["Plazas"]),
                                ResidentialSociety = Convert.ToBoolean(row["ResidentialSociety"]),
                                Resorts = Convert.ToBoolean(row["Resorts"]),
                                Restaurants = Convert.ToBoolean(row["Restaurants"]),
                                Salons = Convert.ToBoolean(row["Salons"]),
                                Shop = Convert.ToBoolean(row["Shop"]),
                                ShoppingMall = Convert.ToBoolean(row["ShoppingMall"]),
                                Showroom = Convert.ToBoolean(row["Showroom"]),
                                Warehouse = Convert.ToBoolean(row["Warehouse"])
                            };
                        }
                    }
                }
            }
            return specialisation;
        }

        private async Task<PaymentMode> GetPaymentModeByListingId(int companyID)
        {
            PaymentMode paymentmode = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[PaymentMode] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", companyID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            paymentmode = new PaymentMode
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                Cash = Convert.ToBoolean(row["Cash"]),
                                Cheque = Convert.ToBoolean(row["Cheque"]),
                                RtgsNeft = Convert.ToBoolean(row["RtgsNeft"]),
                                DebitCard = Convert.ToBoolean(row["DebitCard"]),
                                CreditCard = Convert.ToBoolean(row["CreditCard"]),
                                NetBanking = Convert.ToBoolean(row["NetBanking"]),
                            };
                        }
                    }
                }
            }
            return paymentmode;
        }

        private async Task<List<Keyword>> GetKeywordsByListingIdAsync(int listingId)
        {
            var keywords = new List<Keyword>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT ListingID, OwnerGuid, SeoKeyword FROM [dbo].[Keyword] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        keywords.Add(new Keyword
                        {
                            ListingID = (int)row["ListingID"],
                            OwnerGuid = (string)row["OwnerGuid"],
                            SeoKeyword = (string)row["SeoKeyword"]
                        });
                    }
                }
            }

            return keywords;
        }
    }
}
