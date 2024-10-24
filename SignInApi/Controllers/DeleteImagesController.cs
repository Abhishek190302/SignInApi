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
                            string updateQuery = "UPDATE [dbo].[LogoImage] SET ImagePath = '' WHERE ListingID = @ListingID";
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

        [HttpPost]
        [Route("OwnerDeleteImages")]
        public IActionResult OwnerDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to save.");
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
                    // Update the ImagePath with the combined image paths for the specified ListingID
                    string updateQuery = "UPDATE [dbo].[OwnerImage] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
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
                    return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images updated successfully" });
        }

        [HttpPost]
        [Route("GalleryDeleteImages")]
        public IActionResult GalleryDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to save.");
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
                    // Update the ImagePath with the combined image paths for the specified ListingID
                    string updateQuery = "UPDATE [dbo].[GalleryImage] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
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
                    return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images updated successfully" });
        }

        //[HttpPost]
        //[Route("BannerDeleteImage")]
        //public IActionResult BannerDeleteImage([FromBody] LogoDeleteRequest model)
        //{
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            string updateQuery = "UPDATE [dbo].[BannerDetail] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
        //            using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
        //            {
        //                string defaultImagePath = "/images/logo/default-logo.jpg";

        //                if (string.IsNullOrEmpty(model.ImagePaths))
        //                {
        //                    updateCommand.Parameters.AddWithValue("@ImagePath", defaultImagePath);
        //                }
        //                else
        //                {
        //                    // Remove the base URL and store the relative path in the database
        //                    string relativeImagePath = model.ImagePaths.Replace("https://apidev.myinteriormart.com", "");
        //                    updateCommand.Parameters.AddWithValue("@ImagePath", relativeImagePath);
        //                }

        //                updateCommand.Parameters.AddWithValue("@ListingID", model.ListingID);
        //                updateCommand.ExecuteNonQuery();
        //            }

        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of an error
        //            transaction.Rollback();
        //            return StatusCode(500, "An error occurred while saving the images: " + ex.Message);
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return Ok(new { message = "Images updated successfully" });
        //}

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
                            string updateQuery = "UPDATE [dbo].[BannerDetail] SET ImagePath = '' WHERE ListingID = @ListingID";
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

        [HttpPost]
        [Route("CertificateDeleteImages")]
        public IActionResult CertificateDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to save.");
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
                    // Update the ImagePath with the combined image paths for the specified ListingID
                    string updateQuery = "UPDATE [dbo].[CertificationDetail] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
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
                    return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images updated successfully" });
        }

        [HttpPost]
        [Route("ClientDeleteImages")]
        public IActionResult ClientDeleteImages([FromBody] GalleryDeleteRequest request)
        {
            if (request == null || request.ImagePaths == null || !request.ImagePaths.Any())
            {
                return BadRequest("No images to save.");
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
                    // Update the ImagePath with the combined image paths for the specified ListingID
                    string updateQuery = "UPDATE [dbo].[ClientDetail] SET ImagePath = @ImagePath WHERE ListingID = @ListingID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ImagePath", combinedImagePaths);
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
                    return StatusCode(500, new { message = "An error occurred while saving the images", error = ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }

            return Ok(new { message = "Images updated successfully"});
        }
    }
}
