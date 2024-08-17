using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ImageuploadRepository _imageuploadRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly string _connectionString;
        private readonly string _connectionCountryString;

        public ImageUploadController(IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, UserService userService , IHttpContextAccessor httpContextAccessor, ImageuploadRepository imageuploadRepository)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionCountryString = configuration.GetConnectionString("MimShared");
            _companydetailsRepository = companydetailsRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _imageuploadRepository = imageuploadRepository;
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
                            var logoimage = await _imageuploadRepository.GetlogoImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = logoimage == null;
                            if (recordNotFound)
                            {
                                if (file == null || file.Length == 0)
                                    return BadRequest("No file uploaded.");

                                // Create the directory if it doesn't exist
                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, file.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";

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
                            else
                            {
                                if (file == null || file.Length == 0)
                                    return BadRequest("No file uploaded.");

                                // Create the directory if it doesn't exist
                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, file.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";

                                using (var connection = new SqlConnection(_connectionString))
                                {
                                    var command = new SqlCommand("UPDATE [dbo].[LogoImage] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,CreatedDate=GETDATE(),UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "LogoImage Updated successfully", ImageUrl = imageUrl, Listing = listing.Listingid, OwnerGuidId = currentUserGuid });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Consider logging the exception or returning a more specific error message
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


                            var ownerimage = await _imageuploadRepository.GetOwnerImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = ownerimage == null;

                            // Validate the model
                            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Designation) || model.CountryID == 0 || model.StateID == 0)
                            {
                                return BadRequest("All fields are compulsory!");
                            }

                            // Count existing images
                            var existingImages = ownerimage?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            int existingImageCount = existingImages.Count;

                            // Validate if the new upload exceeds the maximum allowed images
                            int maxImages = 3;
                            int newImagesCount = model.File.Count;

                            if (existingImageCount + newImagesCount > maxImages)
                            {
                                int allowedCount = maxImages - existingImageCount;
                                return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
                            }

                            // Proceed with uploading the images
                            var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                            if (!Directory.Exists(userDirectory))
                            {
                                Directory.CreateDirectory(userDirectory);
                            }

                            foreach (var file in model.File)
                            {
                                var imagePath = Path.Combine(userDirectory, file.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                existingImages.Add(imageUrl);
                            }

                            var imageUrlsCommaSeparated = string.Join(",", existingImages);

                            // Now proceed with database operations as you have already implemented.
                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (recordNotFound)
                                {
                                    command = new SqlCommand("Usp_AddOwnerImage", connection);
                                }
                                else
                                {
                                    command = new SqlCommand("Usp_UpdateOwnerImage", connection);
                                }

                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ListingId", listing.Listingid);
                                command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
                                command.Parameters.AddWithValue("@FirstName", model.FirstName);
                                command.Parameters.AddWithValue("@LastName", model.LastName);
                                command.Parameters.AddWithValue("@Designation", model.Designation);
                                command.Parameters.AddWithValue("@ImageUrl", imageUrlsCommaSeparated);
                                command.Parameters.AddWithValue("@CountryID", model.CountryID);
                                command.Parameters.AddWithValue("@StateID", model.StateID);

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
                                        ImageUrls = existingImages, // Return all images
                                        CountryID = model.CountryID,
                                        StateID = model.StateID
                                    };

                                    var response = new
                                    {
                                        Message = recordNotFound ? "Owner Image uploaded successfully!" : "Owner Image updated successfully!",
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

        //[HttpPost]
        //[Route("UploadOwnerImage")]
        //public async Task<IActionResult> UploadOwnerImage([FromForm] OwnerImageModel model)
        //{
        //    var user = _httpContextAccessor.HttpContext.User;
        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userName = user.Identity.Name;

        //        var applicationUser = await _userService.GetUserByUserName(userName);
        //        if (applicationUser != null)
        //        {
        //            try
        //            {
        //                string currentUserGuid = applicationUser.Id.ToString();
        //                var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
        //                if (listing != null)
        //                {
        //                    // Retrieve countries and states
        //                    var countries = new List<Country>();
        //                    using (var con = new SqlConnection(_connectionCountryString))
        //                    {
        //                        await con.OpenAsync();

        //                        // Retrieve countries
        //                        var countryCmd = new SqlCommand("Usp_Countrysall", con)
        //                        {
        //                            CommandType = CommandType.StoredProcedure
        //                        };
        //                        var countryDa = new SqlDataAdapter(countryCmd);
        //                        var countryDt = new DataTable();
        //                        countryDa.Fill(countryDt);
        //                        foreach (DataRow row in countryDt.Rows)
        //                        {
        //                            var country = new Country
        //                            {
        //                                CountryID = (int)row["CountryID"],
        //                                Name = (string)row["Name"],
        //                                States = new List<State>()
        //                            };
        //                            countries.Add(country);

        //                            // Retrieve states for the current country
        //                            var stateCmd = new SqlCommand("Usp_Statesall", con)
        //                            {
        //                                CommandType = CommandType.StoredProcedure
        //                            };
        //                            stateCmd.Parameters.AddWithValue("@CountryID", country.CountryID);
        //                            var stateDa = new SqlDataAdapter(stateCmd);
        //                            var stateDt = new DataTable();
        //                            stateDa.Fill(stateDt);
        //                            foreach (DataRow stateRow in stateDt.Rows)
        //                            {
        //                                var state = new State
        //                                {
        //                                    StateID = (int)stateRow["StateID"],
        //                                    Name = (string)stateRow["Name"],
        //                                    CountryID = (int)stateRow["CountryID"]
        //                                };
        //                                country.States.Add(state);
        //                            }
        //                        }
        //                    }

        //                    var ownerimage = await _imageuploadRepository.GetOwnerImageByListingIdAsync(listing.Listingid);
        //                    bool recordNotFound = ownerimage == null;

        //                    if (recordNotFound)
        //                    {
        //                        // Validate the model
        //                        if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Designation) || model.CountryID == 0 || model.StateID == 0)
        //                        {
        //                            return BadRequest("All fields are compulsory!");
        //                        }


        //                        var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                        if (!Directory.Exists(userDirectory))
        //                        {
        //                            Directory.CreateDirectory(userDirectory);
        //                        }

        //                        var imagePath = Path.Combine(userDirectory, model.File.FileName);

        //                        // Save the image file
        //                        //var imagePath = Path.Combine("wwwroot/images/logos/", model.File.FileName);
        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await model.File.CopyToAsync(stream);
        //                        }
        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{model.File.FileName}";


        //                        // Insert owner image details into the database
        //                        using (var connection = new SqlConnection(_connectionString))
        //                        {
        //                            var command = new SqlCommand("Usp_AddOwnerImage", connection);
        //                            command.CommandType = CommandType.StoredProcedure;
        //                            command.Parameters.AddWithValue("@ListingId", listing.Listingid);
        //                            command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
        //                            command.Parameters.AddWithValue("@FirstName", model.FirstName);
        //                            command.Parameters.AddWithValue("@LastName", model.LastName);
        //                            command.Parameters.AddWithValue("@Designation", model.Designation);
        //                            command.Parameters.AddWithValue("@ImageUrl", imageUrl);
        //                            command.Parameters.AddWithValue("@CountryID", model.CountryID);
        //                            command.Parameters.AddWithValue("@StateID", model.StateID);
        //                            connection.Open();
        //                            int result = await command.ExecuteNonQueryAsync();

        //                            if (result > 0)
        //                            {
        //                                var ownerImageDetails = new
        //                                {
        //                                    ListingId = listing.Listingid,
        //                                    OwnerId = currentUserGuid,
        //                                    FirstName = model.FirstName,
        //                                    LastName = model.LastName,
        //                                    Designation = model.Designation,
        //                                    ImageUrl = imageUrl,
        //                                    CountryID = model.CountryID,
        //                                    StateID = model.StateID
        //                                };

        //                                var response = new
        //                                {
        //                                    Message = "Owner Image uploaded successfully!",
        //                                    OwnerImageDetails = ownerImageDetails,
        //                                    Countries = countries
        //                                };

        //                                return Ok(response);
        //                            }
        //                            else
        //                            {
        //                                return StatusCode(500, "Something went wrong, please contact Administrator!");
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // Validate the model
        //                        if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Designation) || model.CountryID == 0 || model.StateID == 0)
        //                        {
        //                            return BadRequest("All fields are compulsory!");
        //                        }

        //                        var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                        if (!Directory.Exists(userDirectory))
        //                        {
        //                            Directory.CreateDirectory(userDirectory);
        //                        }

        //                        var imagePath = Path.Combine(userDirectory, model.File.FileName);

        //                        // Save the image file
        //                        //var imagePath = Path.Combine("wwwroot/images/logos/", model.File.FileName);
        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await model.File.CopyToAsync(stream);
        //                        }
        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{model.File.FileName}";


        //                        // Update owner image details into the database
        //                        using (var connection = new SqlConnection(_connectionString))
        //                        {
        //                            var command = new SqlCommand("Usp_UpdateOwnerImage", connection);
        //                            command.CommandType = CommandType.StoredProcedure;
        //                            command.Parameters.AddWithValue("@ListingId", listing.Listingid);
        //                            command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
        //                            command.Parameters.AddWithValue("@FirstName", model.FirstName);
        //                            command.Parameters.AddWithValue("@LastName", model.LastName);
        //                            command.Parameters.AddWithValue("@Designation", model.Designation);
        //                            command.Parameters.AddWithValue("@ImageUrl", imageUrl);
        //                            command.Parameters.AddWithValue("@CountryID", model.CountryID);
        //                            command.Parameters.AddWithValue("@StateID", model.StateID);
        //                            connection.Open();
        //                            int result = await command.ExecuteNonQueryAsync();

        //                            if (result > 0)
        //                            {
        //                                var ownerImageDetails = new
        //                                {
        //                                    ListingId = listing.Listingid,
        //                                    OwnerId = currentUserGuid,
        //                                    FirstName = model.FirstName,
        //                                    LastName = model.LastName,
        //                                    Designation = model.Designation,
        //                                    ImageUrl = imageUrl,
        //                                    CountryID = model.CountryID,
        //                                    StateID = model.StateID
        //                                };

        //                                var response = new
        //                                {
        //                                    Message = "Owner Image Updated successfully!",
        //                                    OwnerImageDetails = ownerImageDetails,
        //                                    Countries = countries
        //                                };

        //                                return Ok(response);
        //                            }
        //                            else
        //                            {
        //                                return StatusCode(500, "Something went wrong, please contact Administrator!");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, ex.Message);
        //            }
        //        }
        //        return NotFound("User not found");
        //    }
        //    return Unauthorized();
        //}

        [HttpPost]
        [Route("UploadGalleryImage")]
        public async Task<IActionResult> UploadGalleryImage([FromForm] List<IFormFile> File, [FromForm] string imageTitle)
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
                            var gallery = await _imageuploadRepository.GetGallerysImageByListingIdAsync(listing.Listingid);

                            // Split the existing ImagePath into a list and count the existing images
                            var existingImages = gallery?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            int existingImageCount = existingImages.Count;

                            // Validate if the new upload exceeds the maximum allowed images
                            int maxImages = 20;
                            int newImagesCount = File.Count;

                            if (existingImageCount + newImagesCount > maxImages)
                            {
                                int allowedCount = maxImages - existingImageCount;
                                return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
                            }

                            var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                            if (!Directory.Exists(userDirectory))
                            {
                                Directory.CreateDirectory(userDirectory);
                            }

                            var imageUrls = new List<string>();
                            foreach (var file in File)
                            {
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);
                                }
                            }

                            // Combine new image URLs with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = gallery == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (gallery == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", imageTitle);
                                }
                                else
                                {
                                    command = new SqlCommand("UPDATE [dbo].[GalleryImage] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", imageTitle);
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            // Return the response with all image paths
                            var allImagePaths = updatedImagePaths.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Gallery images uploaded successfully",
                                Listing = listing.Listingid,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitle = imageTitle
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        return StatusCode(500, "Internal server error");
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }

        //[HttpPost]
        //[Route("UploadGalleryImage")]
        //public async Task<IActionResult> UploadGalleryImage([FromForm] GalleryImage galleryImage )
        //{
        //    var user = _httpContextAccessor.HttpContext.User;
        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userName = user.Identity.Name;

        //        var applicationUser = await _userService.GetUserByUserName(userName);
        //        if (applicationUser != null)
        //        {
        //            try
        //            {
        //                string currentUserGuid = applicationUser.Id.ToString();
        //                var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
        //                if (listing != null)
        //                {
        //                    var gallery = await _imageuploadRepository.GetGallerysImageByListingIdAsync(listing.Listingid);
        //                    bool recordNotFound = gallery == null;

        //                    if (recordNotFound)
        //                    {
        //                        if (galleryImage.File == null || galleryImage.File.Length == 0)
        //                            return BadRequest("No file uploaded.");

        //                        var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                        if (!Directory.Exists(userDirectory))
        //                        {
        //                            Directory.CreateDirectory(userDirectory);
        //                        }
        //                        var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

        //                        //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await galleryImage.File.CopyToAsync(stream);
        //                        }

        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

        //                        using (var connection = new SqlConnection(_connectionString))
        //                        {
        //                            var command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
        //                            command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
        //                            command.Parameters.AddWithValue("@ListingID", listing.Listingid);
        //                            command.Parameters.AddWithValue("@ImagePath", imageUrl);
        //                            command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
        //                            connection.Open();
        //                            await command.ExecuteNonQueryAsync();
        //                        }

        //                        return Ok(new { Message = "GalleryImage Upload successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
        //                    }
        //                    else
        //                    {
        //                        if (galleryImage.File == null || galleryImage.File.Length == 0)
        //                            return BadRequest("No file uploaded.");


        //                        var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                        if (!Directory.Exists(userDirectory))
        //                        {
        //                            Directory.CreateDirectory(userDirectory);
        //                        }
        //                        var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

        //                        //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await galleryImage.File.CopyToAsync(stream);
        //                        }

        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";


        //                        using (var connection = new SqlConnection(_connectionString))
        //                        {
        //                            var command = new SqlCommand("Update [dbo].[GalleryImage] Set OwnerGuid='"+ currentUserGuid + "',ImagePath='"+ imageUrl + "',ImageTitle='"+ galleryImage.ImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='"+ listing.Listingid + "'", connection);
        //                            connection.Open();
        //                            await command.ExecuteNonQueryAsync();
        //                        }

        //                        return Ok(new { Message = "GalleryImage Updated successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw;
        //            }
        //        }
        //        return NotFound("User not found");
        //    }
        //    return Unauthorized();
        //}

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
                            var banner = await _imageuploadRepository.GetBannerImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = banner == null;

                            if (recordNotFound)
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");

                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }
                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

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
                            else
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");


                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }
                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }
                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

                                using (var connection = new SqlConnection(_connectionString))
                                {
                                    var command = new SqlCommand("Update [dbo].[BannerDetail] Set OwnerGuid='"+ currentUserGuid + "',ImagePath='"+ imageUrl + "',ImageTitle='"+ galleryImage.ImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='"+ listing.Listingid +"'", connection);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "BannerImage Updated successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                            } 
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
                            var certificate = await _imageuploadRepository.GetCertificateImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = certificate == null;

                            if (recordNotFound)
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");

                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }
                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

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
                            else
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");

                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

                                using (var connection = new SqlConnection(_connectionString))
                                {
                                    var command = new SqlCommand("Update [dbo].[CertificationDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + imageUrl + "',ImageTitle='" + galleryImage.ImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listing.Listingid + "'", connection);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "CertificateImage Update successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                            } 
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

                            var client = await _imageuploadRepository.GetClientImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = client == null;

                            if (recordNotFound)
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");

                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

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
                            else
                            {
                                if (galleryImage.File == null || galleryImage.File.Length == 0)
                                    return BadRequest("No file uploaded.");

                                var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                                if (!Directory.Exists(userDirectory))
                                {
                                    Directory.CreateDirectory(userDirectory);
                                }

                                var imagePath = Path.Combine(userDirectory, galleryImage.File.FileName);

                                //var imagePath = Path.Combine("wwwroot/images/logos", galleryImage.File.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await galleryImage.File.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{galleryImage.File.FileName}";

                                using (var connection = new SqlConnection(_connectionString))
                                {
                                    var command = new SqlCommand("Update [dbo].[ClientDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + imageUrl + "',ImageTitle='" + galleryImage.ImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listing.Listingid + "'", connection);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "ClientDetailImage Update successfully", Listing = listing.Listingid, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
                            }
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

