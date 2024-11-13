using Microsoft.AspNetCore.Components.Web;
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
        private readonly BinddetailsManagelistingRepository _binddetailsListing;
        private readonly string _connectionString;
        private readonly string _connectionCountryString;

        public ImageUploadController(IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, UserService userService , IHttpContextAccessor httpContextAccessor, ImageuploadRepository imageuploadRepository, BinddetailsManagelistingRepository binddetailsListing)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionCountryString = configuration.GetConnectionString("MimShared");
            _companydetailsRepository = companydetailsRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _imageuploadRepository = imageuploadRepository;
            _binddetailsListing = binddetailsListing;
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
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);


                            var logoimage = await _imageuploadRepository.GetlogoImageByListingIdAsync(listingId);
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
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "LogoImage Upload successfully", ImageUrl = imageUrl, Listing = listingId, OwnerGuidId = currentUserGuid });
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
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "LogoImage Updated successfully", ImageUrl = imageUrl, Listing = listingId, OwnerGuidId = currentUserGuid });
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

        //                    // Validate the model
        //                    if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Designation) || model.CountryID == 0 || model.StateID == 0)
        //                    {
        //                        return BadRequest("All fields are compulsory!");
        //                    }

        //                    // Count existing images
        //                    var existingImages = ownerimage?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    int existingImageCount = existingImages.Count;

        //                    // Validate if the new upload exceeds the maximum allowed images
        //                    int maxImages = 3;
        //                    int newImagesCount = model.File.Count;

        //                    if (existingImageCount + newImagesCount > maxImages)
        //                    {
        //                        int allowedCount = maxImages - existingImageCount;
        //                        return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
        //                    }

        //                    // Proceed with uploading the images
        //                    var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                    if (!Directory.Exists(userDirectory))
        //                    {
        //                        Directory.CreateDirectory(userDirectory);
        //                    }

        //                    foreach (var file in model.File)
        //                    {
        //                        var imagePath = Path.Combine(userDirectory, file.FileName);

        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await file.CopyToAsync(stream);
        //                        }

        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
        //                        existingImages.Add(imageUrl);
        //                    }

        //                    var imageUrlsCommaSeparated = string.Join(",", existingImages);

        //                    // Now proceed with database operations as you have already implemented.
        //                    using (var connection = new SqlConnection(_connectionString))
        //                    {
        //                        SqlCommand command;
        //                        if (recordNotFound)
        //                        {
        //                            command = new SqlCommand("Usp_AddOwnerImage", connection);
        //                        }
        //                        else
        //                        {
        //                            command = new SqlCommand("Usp_UpdateOwnerImage", connection);
        //                        }

        //                        command.CommandType = CommandType.StoredProcedure;
        //                        command.Parameters.AddWithValue("@ListingId", listing.Listingid);
        //                        command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
        //                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
        //                        command.Parameters.AddWithValue("@LastName", model.LastName);
        //                        command.Parameters.AddWithValue("@Designation", model.Designation);
        //                        command.Parameters.AddWithValue("@ImageUrl", imageUrlsCommaSeparated);
        //                        command.Parameters.AddWithValue("@CountryID", model.CountryID);
        //                        command.Parameters.AddWithValue("@StateID", model.StateID);
        //                        command.Parameters.AddWithValue("@MrndMs", model.MrndMs);

        //                        connection.Open();
        //                        int result = await command.ExecuteNonQueryAsync();

        //                        if (result > 0)
        //                        {
        //                            var ownerImageDetails = new
        //                            {
        //                                ListingId = listing.Listingid,
        //                                OwnerId = currentUserGuid,
        //                                FirstName = model.FirstName,
        //                                LastName = model.LastName,
        //                                Designation = model.Designation,
        //                                ImageUrls = existingImages, // Return all images
        //                                CountryID = model.CountryID,
        //                                StateID = model.StateID,
        //                                NamePrefix = model.MrndMs

        //                            };

        //                            var response = new
        //                            {
        //                                Message = recordNotFound ? "Owner Image uploaded successfully!" : "Owner Image updated successfully!",
        //                                OwnerImageDetails = ownerImageDetails,
        //                                Countries = countries
        //                            };

        //                            return Ok(response);
        //                        }
        //                        else
        //                        {
        //                            return StatusCode(500, "Something went wrong, please contact Administrator!");
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

        //                    // Count existing images
        //                    var existingImages = ownerimage?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    var existingFirstname = ownerimage?.OwnerName?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    var existingLastname = ownerimage?.LastName?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    var existingDesignation = ownerimage?.Designation?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();

        //                    int existingImageCount = existingImages.Count;

        //                    // Validate if the new upload exceeds the maximum allowed images
        //                    int maxImages = 3;
        //                    int newImagesCount = model.File.Count;

        //                    if (existingImageCount + newImagesCount > maxImages)
        //                    {
        //                        int allowedCount = maxImages - existingImageCount;
        //                        return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
        //                    }

        //                    // Proceed with uploading the images
        //                    var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                    if (!Directory.Exists(userDirectory))
        //                    {
        //                        Directory.CreateDirectory(userDirectory);
        //                    }


        //                    for (int i = 0; i < model.File.Count; i++)
        //                    {
        //                        var file = model.File[i];
        //                        var imagePath = Path.Combine(userDirectory, file.FileName);

        //                        using (var stream = new FileStream(imagePath, FileMode.Create))
        //                        {
        //                            await file.CopyToAsync(stream);
        //                        }

        //                        var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
        //                        existingImages.Add(imageUrl);

        //                        if (i < model.FirstName.Count)
        //                        {
        //                            existingFirstname.Add(model.FirstName[i]);
        //                        }

        //                        if (i < model.LastName.Count)
        //                        {
        //                            existingLastname.Add(model.LastName[i]);
        //                        }

        //                        // Ensure designations and files have the same count
        //                        if (i < model.Designation.Count)
        //                        {
        //                            existingDesignation.Add(model.Designation[i]);
        //                        }
        //                    }

        //                    var imageUrlsCommaSeparated = string.Join(",", existingImages);
        //                    var newFirstname = string.Join(",", model.FirstName); // Update titles
        //                    var existingFirstName = string.Join(",", existingFirstname);
        //                    var newLastname = string.Join(",", model.LastName); // Update titles
        //                    var existingLastName = string.Join(",", existingLastname);
        //                    var newDesignation = string.Join(",", model.Designation);
        //                    var existingDesignatioN = string.Join(",", existingDesignation);


        //                    var updatedFirstane = model == null ? newFirstname : $"{existingFirstName}";
        //                    var updatedLastane = model == null ? newLastname : $"{existingLastName}";
        //                    var updatedDesignation = model == null ? newDesignation : $"{existingDesignatioN}";


        //                    // Now proceed with database operations as you have already implemented.
        //                    using (var connection = new SqlConnection(_connectionString))
        //                    {
        //                        SqlCommand command;
        //                        if (recordNotFound)
        //                        {
        //                            command = new SqlCommand("Usp_AddOwnerImage", connection);
        //                        }
        //                        else
        //                        {
        //                            command = new SqlCommand("Usp_UpdateOwnerImage", connection);
        //                        }

        //                        command.CommandType = CommandType.StoredProcedure;
        //                        command.Parameters.AddWithValue("@ListingId", listing.Listingid);
        //                        command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
        //                        command.Parameters.AddWithValue("@FirstName", updatedFirstane);
        //                        command.Parameters.AddWithValue("@LastName", updatedLastane);
        //                        command.Parameters.AddWithValue("@Designation", updatedDesignation);
        //                        command.Parameters.AddWithValue("@ImageUrl", imageUrlsCommaSeparated);
        //                        command.Parameters.AddWithValue("@CountryID", model.CountryID);
        //                        command.Parameters.AddWithValue("@StateID", model.StateID);
        //                        command.Parameters.AddWithValue("@MrndMs", model.MrndMs);

        //                        connection.Open();
        //                        int result = await command.ExecuteNonQueryAsync();

        //                        var allFirstname = updatedFirstane.Split(',').ToList();
        //                        var allLastname = updatedLastane.Split(',').ToList();
        //                        var allDesignation = updatedDesignation.Split(',').ToList();

        //                        if (result > 0)
        //                        {
        //                            var ownerImageDetails = new
        //                            {
        //                                ListingId = listing.Listingid,
        //                                OwnerId = currentUserGuid,
        //                                FirstName = allFirstname,
        //                                LastName = allLastname,
        //                                Designation = allDesignation,
        //                                ImageUrls = existingImages, // Return all images
        //                                CountryID = model.CountryID,
        //                                StateID = model.StateID,
        //                                NamePrefix = model.MrndMs

        //                            };

        //                            var response = new
        //                            {
        //                                Message = recordNotFound ? "Owner Image uploaded successfully!" : "Owner Image updated successfully!",
        //                                OwnerImageDetails = ownerImageDetails,
        //                                Countries = countries
        //                            };

        //                            return Ok(response);
        //                        }
        //                        else
        //                        {
        //                            return StatusCode(500, "Something went wrong, please contact Administrator!");
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

                            var ownerImage = await _imageuploadRepository.GetOwnerImageByListingIdAsync(listing.Listingid);
                            bool recordNotFound = ownerImage == null;

                            // Initialize lists for existing data or with empty lists if no prior data
                            var existingImages = ownerImage?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingFirstNames = ownerImage?.OwnerName?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingLastNames = ownerImage?.LastName?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingDesignations = ownerImage?.Designation?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingPrefix = ownerImage?.Prefix?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();

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

                            for (int i = 0; i < model.File.Count; i++)
                            {
                                var file = model.File[i];
                                var imagePath = Path.Combine(userDirectory, file.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                existingImages.Add(imageUrl);

                                // Ensure correct values go into their respective lists
                                if (i < model.FirstName.Count)
                                {
                                    existingFirstNames.Add(model.FirstName[i]);
                                }

                                if (i < model.LastName.Count)
                                {
                                    existingLastNames.Add(model.LastName[i]);
                                }

                                if (i < model.Designation.Count)
                                {
                                    existingDesignations.Add(model.Designation[i]);
                                }

                                if (i < model.MrndMs.Count)
                                {
                                    existingPrefix.Add(model.MrndMs[i]);
                                }
                            }

                            var imageUrlsCommaSeparated = string.Join(",", existingImages);
                            var allFirstNames = string.Join(",", existingFirstNames);
                            var allLastNames = string.Join(",", existingLastNames);
                            var allDesignations = string.Join(",", existingDesignations);
                            var allPrefix = string.Join(",", existingPrefix);

                            // Now proceed with database operations
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
                                command.Parameters.AddWithValue("@FirstName", allFirstNames);
                                command.Parameters.AddWithValue("@LastName", allLastNames);
                                command.Parameters.AddWithValue("@Designation", allDesignations);
                                command.Parameters.AddWithValue("@ImageUrl", imageUrlsCommaSeparated);
                                command.Parameters.AddWithValue("@CountryID", model.CountryID);
                                command.Parameters.AddWithValue("@StateID", model.StateID);
                                command.Parameters.AddWithValue("@MrndMs", allPrefix);

                                connection.Open();
                                int result = await command.ExecuteNonQueryAsync();

                                if (result > 0)
                                {
                                    var ownerImageDetails = new
                                    {
                                        ListingId = listing.Listingid,
                                        OwnerId = currentUserGuid,
                                        FirstName = existingFirstNames,
                                        LastName = existingLastNames,
                                        Designation = existingDesignations,
                                        ImageUrls = existingImages,
                                        CountryID = model.CountryID,
                                        StateID = model.StateID,
                                        NamePrefix = existingPrefix
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
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);

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

                            var ownerImage = await _imageuploadRepository.GetOwnerImageByListingIdAsync(listingId);
                            bool recordNotFound = ownerImage == null;

                            var existingImages = (ownerImage?.Imagepath as IEnumerable<string>)?.SelectMany(path => path.Split(',')).ToList() ?? new List<string>();
                            var existingFirstNames = (ownerImage?.OwnerName as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            var existingLastNames = (ownerImage?.LastName as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            var existingDesignations = (ownerImage?.Designation as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            var existingPrefix = (ownerImage?.Prefix as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            
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

                            for (int i = 0; i < model.File.Count; i++)
                            {
                                var file = model.File[i];
                                var imagePath = Path.Combine(userDirectory, file.FileName);

                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                existingImages.Add(imageUrl);

                                // Ensure correct values go into their respective lists
                                if (i < model.FirstName.Count)
                                {
                                    existingFirstNames.Add(model.FirstName[i]);
                                }

                                if (i < model.LastName.Count)
                                {
                                    existingLastNames.Add(model.LastName[i]);
                                }

                                if (i < model.Designation.Count)
                                {
                                    existingDesignations.Add(model.Designation[i]);
                                }

                                if (i < model.MrndMs.Count)
                                {
                                    existingPrefix.Add(model.MrndMs[i]);
                                }
                            }

                            var imageUrlsCommaSeparated = string.Join(",", existingImages);
                            var allFirstNames = string.Join(",", existingFirstNames);
                            var allLastNames = string.Join(",", existingLastNames);
                            var allDesignations = string.Join(",", existingDesignations);
                            var allPrefix = string.Join(",", existingPrefix);

                            // Now proceed with database operations
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
                                command.Parameters.AddWithValue("@ListingId", listingId);
                                command.Parameters.AddWithValue("@OwnerId", currentUserGuid);
                                command.Parameters.AddWithValue("@FirstName", allFirstNames);
                                command.Parameters.AddWithValue("@LastName", allLastNames);
                                command.Parameters.AddWithValue("@Designation", allDesignations);
                                command.Parameters.AddWithValue("@ImageUrl", imageUrlsCommaSeparated);
                                command.Parameters.AddWithValue("@CountryID", model.CountryID);
                                command.Parameters.AddWithValue("@StateID", model.StateID);
                                command.Parameters.AddWithValue("@MrndMs", allPrefix);

                                connection.Open();
                                int result = await command.ExecuteNonQueryAsync();

                                if (result > 0)
                                {
                                    var ownerImageDetails = new
                                    {
                                        ListingId = listingId,
                                        OwnerId = currentUserGuid,
                                        FirstName = existingFirstNames,
                                        LastName = existingLastNames,
                                        Designation = existingDesignations,
                                        ImageUrls = existingImages,
                                        CountryID = model.CountryID,
                                        StateID = model.StateID,
                                        NamePrefix = existingPrefix
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

        //[HttpPost]
        //[Route("UploadGalleryImage")]
        //public async Task<IActionResult> UploadGalleryImage([FromForm] List<IFormFile> File, [FromForm] string imageTitle)
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

        //                    // Split the existing ImagePath into a list and count the existing images
        //                    var existingImages = gallery?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    int existingImageCount = existingImages.Count;

        //                    // Validate if the new upload exceeds the maximum allowed images
        //                    int maxImages = 20;
        //                    int newImagesCount = File.Count;

        //                    if (existingImageCount + newImagesCount > maxImages)
        //                    {
        //                        int allowedCount = maxImages - existingImageCount;
        //                        return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
        //                    }

        //                    var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                    if (!Directory.Exists(userDirectory))
        //                    {
        //                        Directory.CreateDirectory(userDirectory);
        //                    }

        //                    var imageUrls = new List<string>();
        //                    foreach (var file in File)
        //                    {
        //                        if (file.Length > 0)
        //                        {
        //                            var imagePath = Path.Combine(userDirectory, file.FileName);
        //                            using (var stream = new FileStream(imagePath, FileMode.Create))
        //                            {
        //                                await file.CopyToAsync(stream);
        //                            }
        //                            var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
        //                            imageUrls.Add(imageUrl);
        //                        }
        //                    }

        //                    // Combine new image URLs with existing ones
        //                    var newImagePaths = string.Join(",", imageUrls);
        //                    var existingImagePaths = string.Join(",", existingImages);
        //                    var updatedImagePaths = gallery == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";

        //                    using (var connection = new SqlConnection(_connectionString))
        //                    {
        //                        SqlCommand command;
        //                        if (gallery == null)
        //                        {
        //                            command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
        //                            command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
        //                            command.Parameters.AddWithValue("@ListingID", listing.Listingid);
        //                            command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
        //                            command.Parameters.AddWithValue("@ImageTitle", imageTitle);
        //                        }
        //                        else
        //                        {
        //                            command = new SqlCommand("UPDATE [dbo].[GalleryImage] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
        //                            command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
        //                            command.Parameters.AddWithValue("@ListingID", listing.Listingid);
        //                            command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
        //                            command.Parameters.AddWithValue("@ImageTitle", imageTitle);
        //                        }
        //                        connection.Open();
        //                        await command.ExecuteNonQueryAsync();
        //                    }

        //                    // Return the response with all image paths
        //                    var allImagePaths = updatedImagePaths.Split(',').ToList();

        //                    return Ok(new
        //                    {
        //                        Message = "Gallery images uploaded successfully",
        //                        Listing = listing.Listingid,
        //                        OwnerGuidId = currentUserGuid,
        //                        ImageUrls = allImagePaths,
        //                        ImageTitle = imageTitle
        //                    });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Handle exception
        //                return StatusCode(500, "Internal server error");
        //            }
        //        }
        //        return NotFound("User not found");
        //    }
        //    return Unauthorized();
        //}

        [HttpPost]
        [Route("UploadGalleryImage")]
        public async Task<IActionResult> UploadGalleryImage([FromForm] List<IFormFile> File, [FromForm] List<string> ImageTitle)
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

                            // Split existing ImagePath and ImageTitle into lists
                            var existingImages = gallery?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingTitles = gallery?.Imagetitle?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();

                            int existingImageCount = existingImages.Count;
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    // Add corresponding title for the uploaded image
                                    existingTitles.Add(ImageTitle[i]);
                                }
                            }

                            // Combine new image URLs and titles with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", ImageTitle); // Update titles
                            var existingImageTitle = string.Join(",", existingTitles);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = gallery == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
                            var updatedImageTitle = gallery == null ? newTitles : $"{existingImageTitle}";


                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (gallery == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle); // Store updated titles
                                }
                                else
                                {
                                    command = new SqlCommand("UPDATE [dbo].[GalleryImage] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle); // Store updated titles
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            // Return the response with all image paths and titles
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = updatedImageTitle.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Gallery images uploaded successfully",
                                Listing = listing.Listingid,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitles = allImageTitles
                            });
                        }
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);
                            var gallery = await _imageuploadRepository.GetGallerysImageByListingIdAsync(listingId);

                            var existingImages = (gallery?.Imagepath as IEnumerable<string>)?.SelectMany(path => path.Split(',')).ToList() ?? new List<string>();
                            var existingTitles = (gallery?.Imagetitle as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            int existingImageCount = existingImages.Count;
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    // Add corresponding title for the uploaded image
                                    existingTitles.Add(ImageTitle[i]);
                                }
                            }

                            // Combine new image URLs and titles with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", ImageTitle); // Update titles
                            var existingImageTitle = string.Join(",", existingTitles);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = gallery == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
                            var updatedImageTitle = gallery == null ? newTitles : $"{existingImageTitle}";


                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (gallery == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[GalleryImage] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle); // Store updated titles
                                }
                                else
                                {
                                    command = new SqlCommand("UPDATE [dbo].[GalleryImage] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle); // Store updated titles
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            // Return the response with all image paths and titles
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = updatedImageTitle.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Gallery images uploaded successfully",
                                Listing = listingId,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitles = allImageTitles
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
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);
                            var banner = await _imageuploadRepository.GetBannerImageByListingIdAsync(listingId);
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
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", imageUrl);
                                    command.Parameters.AddWithValue("@ImageTitle", galleryImage.ImageTitle);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "BannerImage Upload successfully", Listing = listingId, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
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
                                    var command = new SqlCommand("Update [dbo].[BannerDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + imageUrl + "',ImageTitle='" + galleryImage.ImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listingId + "'", connection);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }

                                return Ok(new { Message = "BannerImage Updated successfully", Listing = listingId, OwnerGuidId = currentUserGuid, ImageUrl = imageUrl, ImageTitle = galleryImage.ImageTitle });
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


        //[HttpPost]
        //[Route("UploadCertificateImage")]
        //public async Task<IActionResult> UploadCertificateImage([FromForm] List<IFormFile> File, [FromForm] string imageTitle)
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
        //                    var certificate = await _imageuploadRepository.GetCertificateImageByListingIdAsync(listing.Listingid);
        //                    var existingImages = certificate?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
        //                    int existingImageCount = existingImages.Count;

        //                    // Validate if the new upload exceeds the maximum allowed images
        //                    int maxImages = 20;
        //                    int newImagesCount = File.Count;

        //                    if (existingImageCount + newImagesCount > maxImages)
        //                    {
        //                        int allowedCount = maxImages - existingImageCount;
        //                        return BadRequest($"You have already uploaded {existingImageCount} image(s). You can upload {allowedCount} more image(s).");
        //                    }

        //                    var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
        //                    if (!Directory.Exists(userDirectory))
        //                    {
        //                        Directory.CreateDirectory(userDirectory);
        //                    }

        //                    var imageUrls = new List<string>();
        //                    foreach (var file in File)
        //                    {
        //                        if (file.Length > 0)
        //                        {
        //                            var imagePath = Path.Combine(userDirectory, file.FileName);
        //                            using (var stream = new FileStream(imagePath, FileMode.Create))
        //                            {
        //                                await file.CopyToAsync(stream);
        //                            }
        //                            var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
        //                            imageUrls.Add(imageUrl);
        //                        }
        //                    }

        //                    // Combine new image URLs with existing ones
        //                    var newImagePaths = string.Join(",", imageUrls);
        //                    var existingImagePaths = string.Join(",", existingImages);
        //                    var updatedImagePaths = certificate == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
        //                    using (var connection = new SqlConnection(_connectionString))
        //                    {
        //                        SqlCommand command;
        //                        if (certificate == null)
        //                        {
        //                            command = new SqlCommand("INSERT INTO [dbo].[CertificationDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
        //                            command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
        //                            command.Parameters.AddWithValue("@ListingID", listing.Listingid);
        //                            command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
        //                            command.Parameters.AddWithValue("@ImageTitle", imageTitle);
        //                        }
        //                        else
        //                        {
        //                            command = new SqlCommand("Update [dbo].[CertificationDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + updatedImagePaths + "',ImageTitle='" + imageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listing.Listingid + "'", connection);
        //                        }
        //                        connection.Open();
        //                        await command.ExecuteNonQueryAsync();
        //                    }

        //                    // Return the response with all image paths
        //                    var allImagePaths = updatedImagePaths.Split(',').ToList();

        //                    return Ok(new
        //                    {
        //                        Message = "Certificate images uploaded successfully",
        //                        Listing = listing.Listingid,
        //                        OwnerGuidId = currentUserGuid,
        //                        ImageUrls = allImagePaths,
        //                        ImageTitle = imageTitle
        //                    });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, new
        //                {
        //                    Message = "An error occurred while uploading images.",
        //                    Error = ex.Message
        //                });
        //            }
        //        }
        //        return NotFound(new { Message = "User not found" });
        //    }
        //    return Unauthorized(new { Message = "User is not authenticated" });
        //}

        [HttpPost]
        [Route("UploadCertificateImage")]
        public async Task<IActionResult> UploadCertificateImage([FromForm] List<IFormFile> File, [FromForm] List<string> ImageTitle)
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
                            var existingImages = certificate?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingTitles = certificate?.Imagetitle?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    if (i < ImageTitle.Count) 
                                    {
                                        existingTitles.Add(ImageTitle[i]);
                                    }
                                }
                            }

                            // Combine new image URLs and titles with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", ImageTitle); // Update titles
                            var existingImageTitle = string.Join(",", existingTitles);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = certificate == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
                            var updatedImageTitle = certificate == null ? newTitles : $"{existingImageTitle}";


                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (certificate == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[CertificationDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle);
                                }
                                else
                                {
                                    command = new SqlCommand("UPDATE [dbo].[CertificationDetail] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle);
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            // Return the response with all image paths and titles
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = updatedImageTitle.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Certificate images uploaded successfully",
                                Listing = listing.Listingid,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitles = allImageTitles
                            });
                        }
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);
                            var certificate = await _imageuploadRepository.GetCertificateImageByListingIdAsync(listingId);

                            // Explicitly cast dynamic properties to IEnumerable<string> to use LINQ
                            var existingImages = (certificate?.Imagepath as IEnumerable<string>)?.SelectMany(path => path.Split(',')).ToList() ?? new List<string>();
                            var existingTitles = (certificate?.Imagetitle as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    // Only add new title if it's not already in existingTitles
                                    if (i < ImageTitle.Count && !existingTitles.Contains(ImageTitle[i]))
                                    {
                                        existingTitles.Add(ImageTitle[i]);
                                    }
                                }
                            }

                            // Combine new image URLs with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", existingTitles); // Update titles without duplication
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = certificate == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (certificate == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[CertificationDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", newTitles);
                                }
                                else
                                {
                                    command = new SqlCommand("UPDATE [dbo].[CertificationDetail] SET OwnerGuid=@OwnerGuid,ImagePath=@ImagePath,ImageTitle=@ImageTitle,UpdateDate=GETDATE() WHERE ListingID=@ListingID", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", newTitles);
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }

                            // Return the response with all image paths and titles
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = newTitles.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Certificate images uploaded successfully",
                                Listing = listingId,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitles = allImageTitles
                            });
                        }


                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            Message = "An error occurred while uploading images.",
                            Error = ex.Message
                        });
                    }
                }
                return NotFound(new { Message = "User not found" });
            }
            return Unauthorized(new { Message = "User is not authenticated" });
        }

        [HttpPost]
        [Route("UploadClientImage")]
        public async Task<IActionResult> UploadClientImage([FromForm] List<IFormFile> File, [FromForm] List<string> ImageTitle)
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
                            var existingImages = client?.Imagepath?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            var existingTitles = client?.Imagetitle?.FirstOrDefault()?.Split(',').ToList() ?? new List<string>();
                            int existingImageCount = existingImages.Count;

                            // Validate if the new upload exceeds the maximum allowed images
                            int maxImages = 50;
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    if (i < ImageTitle.Count)
                                    {
                                        existingTitles.Add(ImageTitle[i]);
                                    }
                                }
                            }

                            // Combine new image URLs with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", ImageTitle); // Update titles
                            var existingImageTitle = string.Join(",", existingTitles);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = client == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
                            var updatedImageTitle = client == null ? newTitles : $"{existingImageTitle}";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (client == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[ClientDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listing.Listingid);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle);
                                }
                                else
                                {
                                    command = new SqlCommand("Update [dbo].[ClientDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + updatedImagePaths + "',ImageTitle='" + updatedImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listing.Listingid + "'", connection);
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }
                            // Return the response with all image paths
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = updatedImageTitle.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Client images uploaded successfully",
                                Listing = listing.Listingid,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitle = allImageTitles
                            });
                        }
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);
                            var client = await _imageuploadRepository.GetClientImageByListingIdAsync(listingId);

                            var existingImages = (client?.Imagepath as IEnumerable<string>)?.SelectMany(path => path.Split(',')).ToList() ?? new List<string>();
                            var existingTitles = (client?.Imagetitle as IEnumerable<string>)?.SelectMany(title => title.Split(',')).ToList() ?? new List<string>();
                            int existingImageCount = existingImages.Count;

                            // Validate if the new upload exceeds the maximum allowed images
                            int maxImages = 50;
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
                            for (int i = 0; i < File.Count; i++)
                            {
                                var file = File[i];
                                if (file.Length > 0)
                                {
                                    var imagePath = Path.Combine(userDirectory, file.FileName);
                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var imageUrl = $"/images/logos/{currentUserGuid}/{file.FileName}";
                                    imageUrls.Add(imageUrl);

                                    if (i < ImageTitle.Count)
                                    {
                                        existingTitles.Add(ImageTitle[i]);
                                    }
                                }
                            }

                            // Combine new image URLs with existing ones
                            var newImagePaths = string.Join(",", imageUrls);
                            var newTitles = string.Join(",", ImageTitle); // Update titles
                            var existingImageTitle = string.Join(",", existingTitles);
                            var existingImagePaths = string.Join(",", existingImages);
                            var updatedImagePaths = client == null ? newImagePaths : $"{existingImagePaths},{newImagePaths}";
                            var updatedImageTitle = client == null ? newTitles : $"{existingImageTitle}";

                            using (var connection = new SqlConnection(_connectionString))
                            {
                                SqlCommand command;
                                if (client == null)
                                {
                                    command = new SqlCommand("INSERT INTO [dbo].[ClientDetail] (OwnerGuid,ListingID,ImagePath,ImageTitle,CreatedDate,UpdateDate) VALUES (@OwnerGuid,@ListingID,@ImagePath,@ImageTitle,GETDATE(),GETDATE())", connection);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@ListingID", listingId);
                                    command.Parameters.AddWithValue("@ImagePath", updatedImagePaths);
                                    command.Parameters.AddWithValue("@ImageTitle", updatedImageTitle);
                                }
                                else
                                {
                                    command = new SqlCommand("Update [dbo].[ClientDetail] Set OwnerGuid='" + currentUserGuid + "',ImagePath='" + updatedImagePaths + "',ImageTitle='" + updatedImageTitle + "',CreatedDate=GETDATE(),UpdateDate=GETDATE() Where ListingID='" + listingId + "'", connection);
                                }
                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                            }
                            // Return the response with all image paths
                            var allImagePaths = updatedImagePaths.Split(',').ToList();
                            var allImageTitles = updatedImageTitle.Split(',').ToList();

                            return Ok(new
                            {
                                Message = "Client images uploaded successfully",
                                Listing = listingId,
                                OwnerGuidId = currentUserGuid,
                                ImageUrls = allImagePaths,
                                ImageTitle = allImageTitles
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            Message = "An error occurred while uploading images.",
                            Error = ex.Message
                        }); 
                    }
                }
                return NotFound(new { Message = "User not found" });
            }
            return Unauthorized(new { Message = "User is not authenticated" });
        }
    }
}

