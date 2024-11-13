using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteImagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DeleteImagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("LogoDeleteImage")]
        public IActionResult LogoDeleteImage([FromBody] LogoDeleteRequest model)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Query to check if the image exists in the database for the given ListingID
                    string selectQuery = "SELECT ImagePath FROM [dbo].[LogoImage] WHERE ListingID = @ListingID";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
                        var imagePath = selectCommand.ExecuteScalar()?.ToString();  // Get ImagePath value

                        // Check if ImagePath is not already empty
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            // Update ImagePath to blank
                            string updateQuery = "UPDATE [dbo].[LogoImage] SET ImagePath = '',ImageTitle='' WHERE ListingID = @ListingID";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // If ImagePath is already empty, return a different response
                            transaction.Rollback();
                            return NotFound(new { message = "No image found to delete for this listing." });
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while updating the image: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }


            //string connectionString = _configuration.GetConnectionString("MimListing");
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    SqlTransaction transaction = conn.BeginTransaction();

            //    try
            //    {
            //        string updateQuery = "UPDATE [dbo].[LogoImage] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
            //        using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
            //        {
            //            string defaultImagePath = "/images/logo/default-logo.jpg";

            //            if (string.IsNullOrEmpty(model.ImagePaths))
            //            {
            //                updateCommand.Parameters.AddWithValue("@ImagePath", defaultImagePath);
            //            }
            //            else
            //            {
            //                // Remove the base URL and store the relative path in the database
            //                string relativeImagePath = model.ImagePaths.Replace("https://apidev.myinteriormart.com", "");
            //                updateCommand.Parameters.AddWithValue("@ImagePath", relativeImagePath);
            //            }

            //            updateCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
            //            updateCommand.ExecuteNonQuery();
            //        }

            //        transaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        // Rollback in case of an error
            //        transaction.Rollback();
            //        return StatusCode(500, "An error occurred while saving the images: " + ex.Message);
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //}

            return Ok(new { message = "Images updated successfully" });
        }

        //[HttpPost]
        //[Route("OwnerDeleteImages")]
        //public IActionResult OwnerDeleteImages([FromBody] GalleryDeleteRequest request)
        //{
        //    if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
        //    {
        //        return BadRequest("No images to save.");
        //    }

        //    // Process the image paths to remove the base URL
        //    var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
        //    string combinedImagePaths = string.Join(",", relativeImagePaths);

        //    // Define your connection string
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        // Begin a transaction
        //        SqlTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            // Update the ImagePath with the combined image paths for the specified ListingID
        //            string updateQuery = "UPDATE [dbo].[OwnerImage] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
        //            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
        //            {
        //                updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
        //                updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
        //                updateCommand.ExecuteNonQuery();
        //            }

        //            // Commit the transaction
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of an error
        //            transaction.Rollback();
        //            return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return Ok(new { message = "Images updated successfully" });
        //}

        [HttpPost]
        [Route("OwnerDeleteImages")]
        public IActionResult OwnerDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            
            // Process the image paths to remove the base URL
            var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
            string combinedImagePaths = string.Join(",", relativeImagePaths);

            // Define your connection string
            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Begin a transaction
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Retrieve the current ImagePath, Designation, and OwnerName from the database
                    string selectQuery = "SELECT ImagePath, Designation, OwnerName, LastName, MrndMs FROM [dbo].[OwnerImage] WHERE ListingID = @ListingID";
                    List<string> updatedOwnerNames = new List<string>();
                    List<string> updatedDesignations = new List<string>();
                    List<string> updatedImagePaths = new List<string>();
                    List<string> updatedLastname = new List<string>();
                    List<string> updatedPrefix = new List<string>();

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var currentImagePaths = reader["ImagePath"].ToString().Split(',').ToList();
                                var currentDesignations = reader["Designation"].ToString().Split(',').ToList();
                                var currentOwnerNames = reader["OwnerName"].ToString().Split(',').ToList();
                                var currentLastnames = reader["LastName"].ToString().Split(',').ToList();
                                var currentPrefixs = reader["MrndMs"].ToString().Split(',').ToList();

                                // Keep only the specified images, designations, and owner names
                                foreach (var imagePath in relativeImagePaths)
                                {
                                    int index = currentImagePaths.IndexOf(imagePath);
                                    if (index >= 0)
                                    {
                                        updatedImagePaths.Add(currentImagePaths[index]);
                                        updatedDesignations.Add(currentDesignations[index]);
                                        updatedOwnerNames.Add(currentOwnerNames[index]);
                                        updatedLastname.Add(currentLastnames[index]);
                                        updatedPrefix.Add(currentPrefixs[index]);
                                    }
                                }
                            }
                        }
                    }

                    // Combine updated values for ImagePath, Designation, and OwnerName
                    string combinedUpdatedImagePaths = string.Join(",", updatedImagePaths);
                    string combinedUpdatedDesignations = string.Join(",", updatedDesignations);
                    string combinedUpdatedOwnerNames = string.Join(",", updatedOwnerNames);
                    string combinedUpdatedLastNames = string.Join(",", updatedLastname);
                    string combinedUpdatedPrefixs = string.Join(",", updatedPrefix);

                    // Update the database with the filtered values
                    string updateQuery = "UPDATE [dbo].[OwnerImage] SET ImagePath = @ImagePath, Designation = @Designation, OwnerName = @OwnerName, LastName = @LastName, MrndMs = @MrndMs WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedUpdatedImagePaths);
                        updateCommand.Parameters.AddWithValue("@Designation", combinedUpdatedDesignations);
                        updateCommand.Parameters.AddWithValue("@OwnerName", combinedUpdatedOwnerNames);
                        updateCommand.Parameters.AddWithValue("@LastName", combinedUpdatedLastNames);
                        updateCommand.Parameters.AddWithValue("@MrndMs", combinedUpdatedPrefixs);
                        updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, new { message = "An error occurred while updating the images, designations, and owner names", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images, designations, and owner names updated successfully" });
        }

        //[HttpPost]
        //[Route("GalleryDeleteImages")]
        //public IActionResult GalleryDeleteImages([FromBody] GalleryDeleteRequest request)
        //{
        //    if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
        //    {
        //        return BadRequest("No images to save.");
        //    }

        //    // Process the image paths to remove the base URL
        //    var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
        //    string combinedImagePaths = string.Join(",", relativeImagePaths);

        //    // Define your connection string
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        // Begin a transaction
        //        SqlTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            // Update the ImagePath with the combined image paths for the specified ListingID
        //            string updateQuery = "UPDATE [dbo].[GalleryImage] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
        //            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
        //            {
        //                updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
        //                updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
        //                updateCommand.ExecuteNonQuery();
        //            }

        //            // Commit the transaction
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of an error
        //            transaction.Rollback();
        //            return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return Ok(new { message = "Images updated successfully" });
        //}


        [HttpPost]
        [Route("GalleryDeleteImages")]
        public IActionResult GalleryDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to update.");
            }

            // Process the image paths to remove the base URL
            var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
            string combinedImagePaths = string.Join(",", relativeImagePaths);

            // Define your connection string
            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Begin a transaction
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Retrieve the current ImagePath and ImageTitle from the database
                    string selectQuery = "SELECT ImagePath, ImageTitle FROM [dbo].[GalleryImage] WHERE ListingID = @ListingID";
                    List<string> updatedTitles = new List<string>();

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var currentImagePaths = reader["ImagePath"].ToString().Split(',').ToList();
                                var currentImageTitles = reader["ImageTitle"].ToString().Split(',').ToList();

                                // Keep only the titles of the specified images
                                foreach (var imagePath in relativeImagePaths)
                                {
                                    int index = currentImagePaths.IndexOf(imagePath);
                                    if (index >= 0 && index < currentImageTitles.Count)
                                    {
                                        updatedTitles.Add(currentImageTitles[index]);
                                    }
                                }
                            }
                        }
                    }

                    // Combine the titles to match the updated image paths
                    string combinedImageTitles = string.Join(",", updatedTitles);

                    // Update the ImagePath and ImageTitle with only the specified items
                    string updateQuery = "UPDATE [dbo].[GalleryImage] SET ImagePath = @ImagePath, ImageTitle = @ImageTitle WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
                        updateCommand.Parameters.AddWithValue("@ImageTitle", combinedImageTitles);
                        updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, new { message = "An error occurred while updating the images and titles", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images and titles updated successfully" });
        }

        [HttpPost]
        [Route("BannerDeleteImage")]
        public IActionResult BannerDeleteImage([FromBody] LogoDeleteRequest model)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Query to check if the image exists in the database for the given ListingID
                    string selectQuery = "SELECT ImagePath FROM [dbo].[BannerDetail] WHERE ListingID = @ListingID";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
                        var imagePath = selectCommand.ExecuteScalar()?.ToString();  // Get ImagePath value

                        // Check if ImagePath is not already empty
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            // Update ImagePath to blank
                            string updateQuery = "UPDATE [dbo].[BannerDetail] SET ImagePath = '',ImageTitle='' WHERE ListingID = @ListingID";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // If ImagePath is already empty, return a different response
                            transaction.Rollback();
                            return NotFound(new { message = "No image found to delete for this listing." });
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while updating the image: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Image path updated successfully" });
        }

        //[HttpPost]
        //[Route("CertificateDeleteImages")]
        //public IActionResult CertificateDeleteImages([FromBody] GalleryDeleteRequest request)
        //{
        //    if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
        //    {
        //        return BadRequest("No images to save.");
        //    }

        //    // Process the image paths to remove the base URL
        //    var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
        //    string combinedImagePaths = string.Join(",", relativeImagePaths);

        //    // Define your connection string
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        // Begin a transaction
        //        SqlTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            // Update the ImagePath with the combined image paths for the specified ListingID
        //            string updateQuery = "UPDATE [dbo].[CertificationDetail] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
        //            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
        //            {
        //                updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
        //                updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
        //                updateCommand.ExecuteNonQuery();
        //            }

        //            // Commit the transaction
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of an error
        //            transaction.Rollback();
        //            return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return Ok(new { message = "Images updated successfully" });
        //}

        [HttpPost]
        [Route("CertificateDeleteImages")]
        public IActionResult CertificateDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to update.");
            }

            // Process the image paths to remove the base URL
            var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
            string combinedImagePaths = string.Join(",", relativeImagePaths);

            // Define your connection string
            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Begin a transaction
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Retrieve the current ImagePath and ImageTitle from the database
                    string selectQuery = "SELECT ImagePath, ImageTitle FROM [dbo].[CertificationDetail] WHERE ListingID = @ListingID";
                    List<string> updatedTitles = new List<string>();

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var currentImagePaths = reader["ImagePath"].ToString().Split(',').ToList();
                                var currentImageTitles = reader["ImageTitle"].ToString().Split(',').ToList();

                                // Keep only the titles of the specified images
                                foreach (var imagePath in relativeImagePaths)
                                {
                                    int index = currentImagePaths.IndexOf(imagePath);
                                    if (index >= 0 && index < currentImageTitles.Count)
                                    {
                                        updatedTitles.Add(currentImageTitles[index]);
                                    }
                                }
                            }
                        }
                    }

                    // Combine the titles to match the updated image paths
                    string combinedImageTitles = string.Join(",", updatedTitles);

                    // Update the ImagePath and ImageTitle with only the specified items
                    string updateQuery = "UPDATE [dbo].[CertificationDetail] SET ImagePath = @ImagePath, ImageTitle = @ImageTitle WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
                        updateCommand.Parameters.AddWithValue("@ImageTitle", combinedImageTitles);
                        updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, new { message = "An error occurred while updating the images and titles", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images and titles updated successfully" });
        }

        //[HttpPost]
        //[Route("ClientDeleteImages")]
        //public IActionResult ClientDeleteImages([FromBody] GalleryDeleteRequest request)
        //{
        //    if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
        //    {
        //        return BadRequest("No images to save.");
        //    }

        //    // Process the image paths to remove the base URL
        //    var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
        //    string combinedImagePaths = string.Join(",", relativeImagePaths);

        //    // Define your connection string
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        // Begin a transaction
        //        SqlTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            // Update the ImagePath with the combined image paths for the specified ListingID
        //            string updateQuery = "UPDATE [dbo].[ClientDetail] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
        //            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
        //            {
        //                updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
        //                updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
        //                updateCommand.ExecuteNonQuery();
        //            }

        //            // Commit the transaction
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of an error
        //            transaction.Rollback();
        //            return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return Ok(new { message = "Images updated successfully"});
        //}

        [HttpPost]
        [Route("ClientDeleteImages")]
        public IActionResult ClientDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to update.");
            }

            // Process the image paths to remove the base URL
            var relativeImagePaths = request.ImagePaths.Select(path => path.Replace("https://apidev.myinteriormart.com", "")).ToList();
            string combinedImagePaths = string.Join(",", relativeImagePaths);

            // Define your connection string
            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Begin a transaction
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Retrieve the current ImagePath and ImageTitle from the database
                    string selectQuery = "SELECT ImagePath, ImageTitle FROM [dbo].[ClientDetail] WHERE ListingID = @ListingID";
                    List<string> updatedTitles = new List<string>();

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var currentImagePaths = reader["ImagePath"].ToString().Split(',').ToList();
                                var currentImageTitles = reader["ImageTitle"].ToString().Split(',').ToList();

                                // Keep only the titles of the specified images
                                foreach (var imagePath in relativeImagePaths)
                                {
                                    int index = currentImagePaths.IndexOf(imagePath);
                                    if (index >= 0 && index < currentImageTitles.Count)
                                    {
                                        updatedTitles.Add(currentImageTitles[index]);
                                    }
                                }
                            }
                        }
                    }

                    // Combine the titles to match the updated image paths
                    string combinedImageTitles = string.Join(",", updatedTitles);

                    // Update the ImagePath and ImageTitle with only the specified items
                    string updateQuery = "UPDATE [dbo].[ClientDetail] SET ImagePath = @ImagePath, ImageTitle = @ImageTitle WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
                        updateCommand.Parameters.AddWithValue("@ImageTitle", combinedImageTitles);
                        updateCommand.Parameters.AddWithValue("@ListingID", request.ListingID);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    transaction.Rollback();
                    return StatusCode(500, new { message = "An error occurred while updating the images and titles", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images and titles updated successfully" });
        }
    }
}
