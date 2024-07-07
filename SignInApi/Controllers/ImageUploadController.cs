using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using static System.Net.Mime.MediaTypeNames;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly string _connectionString;
        private readonly string _connectionCountryString;

        public ImageUploadController(IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, UserService userService , IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionCountryString = configuration.GetConnectionString("MimShared");
            _companydetailsRepository = companydetailsRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("UploadLogoImage")]
        public async Task<IActionResult> UploadLogoImage(IFormFile file)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
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

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + file.FileName + "";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("INSERT INTO [dbo].[LogoImage] (OwnerGuid,ListingID,ImagePath,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,GETDATE(),GETDATE())", connection);
                                command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            return Ok(new { Message = "LogoImage Upload successfully", ImageUrl = imageUrl, Listing = listing.Listingid, OwnerGuidId = currentUserGuid });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("UploadOwnerImage")]
        public async Task<IActionResult> UploadOwnerImage([FromForm] OwnerImageModel model)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            // Retrieve countries and states
                            var countries = new List<Country>();
                            using (var con = new SqlConnection(_connectionCountryString))
                            {
                                await con.OpenAsync();

                                // Retrieve countries
                                var countryCmd = new SqlCommand("Usp_Countrysall", con)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                var countryDa = new SqlDataAdapter(countryCmd);
                                var countryDt = new DataTable();
                                countryDa.Fill(countryDt);
                                foreach (DataRow row in countryDt.Rows)
                                {
                                    var country = new Country
                                    {
                                        CountryID = (int)row["CountryID"],
                                        Name = (string)row["Name"],
                                        States = new List<State>()
                                    };
                                    countries.Add(country);

                                    // Retrieve states for the current country
                                    var stateCmd = new SqlCommand("Usp_Statesall", con)
                                    {
                                        CommandType = CommandType.StoredProcedure
                                    };
                                    stateCmd.Parameters.AddWithValue("@CountryID", country.CountryID);
                                    var stateDa = new SqlDataAdapter(stateCmd);
                                    var stateDt = new DataTable();
                                    stateDa.Fill(stateDt);
                                    foreach (DataRow stateRow in stateDt.Rows)
                                    {
                                        var state = new State
                                        {
                                            StateID = (int)stateRow["StateID"],
                                            Name = (string)stateRow["Name"],
                                            CountryID = (int)stateRow["CountryID"]
                                        };
                                        country.States.Add(state);
                                    }
                                }
                            }

                            // Select the first country and state as default (you can modify this logic as needed)
                            var selectedCountry = countries.FirstOrDefault();
                            int selectedCountryID = selectedCountry?.CountryID ?? 0;

                            var selectedState = selectedCountry?.States.FirstOrDefault();
                            int selectedStateID = selectedState?.StateID ?? 0;

                            // Validate the model
                            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Designation) || selectedCountryID == 0 || selectedStateID == 0)
                            {
                                return BadRequest("All fields are compulsory!");
                            }

                            // Save the image file
                            var imagePath = Path.Combine("wwwroot/images/logos/", model.File.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await model.File.CopyToAsync(stream);
                            }
                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + model.Designation + "/" + model.File.FileName + "";


                            // Insert owner image details into the database
                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("Usp_AddOwnerImage", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ListingId", listing.Listingid);
                                command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
                                command.Parameters.AddWithValue("@FirstName", model.FirstName);
                                command.Parameters.AddWithValue("@LastName", model.LastName);
                                command.Parameters.AddWithValue("@Designation", model.Designation);
                                command.Parameters.AddWithValue("@ImageUrl", imageUrl);
                                command.Parameters.AddWithValue("@CountryID", selectedCountryID);
                                command.Parameters.AddWithValue("@StateID", selectedStateID);
                                connection.Open();
                                int result = await command.ExecuteNonQueryAsync();

                                if (result > 0)
                                {
                                    var ownerImageDetails = new
                                    {
                                        ListingId = listing.Listingid,
                                        OwnerId = currentUserGuid,
                                        FirstName = model.FirstName,
                                        LastName = model.LastName,
                                        Designation = model.Designation,
                                        ImageUrl = imageUrl,
                                        CountryID = selectedCountryID,
                                        StateID = selectedStateID
                                    };

                                    var response = new
                                    {
                                        Message = "Owner Image uploaded successfully!",
                                        OwnerImageDetails = ownerImageDetails,
                                        Countries = countries
                                    };

                                    return Ok(response);
                                }
                                else
                                {
                                    return StatusCode(500, "Something went wrong, please contact Administrator!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("UploadGalleryImage")]
        public async Task<IActionResult> UploadGalleryImage([FromForm] GalleryImage galleryImage )
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {

                            if (galleryImage.File == null || galleryImage.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await galleryImage.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + galleryImage.File.FileName + "";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            return Ok(new { Message = "GalleryImage Upload successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("UploadBannerImage")]
        public async Task<IActionResult> UploadBannerImage([FromForm] GalleryImage galleryImage)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {

                            if (galleryImage.File == null || galleryImage.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await galleryImage.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + galleryImage.File.FileName + "";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("INSERT INTO [dbo].[BannerDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            return Ok(new { Message = "BannerImage Upload successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");

            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("UploadCertificateImage")]
        public async Task<IActionResult> UploadCertificateImage([FromForm] GalleryImage galleryImage)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {

                            if (galleryImage.File == null || galleryImage.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await galleryImage.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + galleryImage.File.FileName + "";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("INSERT INTO [dbo].[CertificationDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            return Ok(new { Message = "CertificateImage Upload successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");

            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("UploadClientImage")]
        public async Task<IActionResult> UploadClientImage([FromForm] GalleryImage galleryImage)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {

                            if (galleryImage.File == null || galleryImage.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await galleryImage.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + galleryImage.File.FileName + "";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                var command = new SqlCommand("INSERT INTO [dbo].[ClientDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            return Ok(new { Message = "ClientDetailImage Upload successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }
    }
}

