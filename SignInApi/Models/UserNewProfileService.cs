using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class UserNewProfileService: IUserNewProfileService
    {
        public string TimeZoneOfCountry { get; set; } = "India Standard Time";
        private readonly string _connectionString;
        public UserNewProfileService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        public async Task<UserNewProfile> GetProfileByOwnerGuid(string ownerGuid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", connection))
                {
                    command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            return null;
                        }

                        var row = dataTable.Rows[0];
                        var userProfile = new UserNewProfile
                        {
                            OwnerGuid = row["OwnerGuid"].ToString(),
                            IPAddress = row["IPAddress"].ToString(),
                            FirstName = row["Name"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Gender = row["Gender"].ToString(),
                            CreatedDate = DateTime.Parse(row["CreatedDate"].ToString()),
                            UpdatedDate = DateTime.Parse(row["UpdatedDate"].ToString()),
                            TimeZoneOfCountry = TimeZoneOfCountry,
                            ImageUrl = row["ImageUrl"].ToString(),
                        };

                        return userProfile;
                    }
                }
            }
        }

        public async Task AddUserProfile(UserNewProfile userProfile,string imageURL)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO [dbo].[UserProfile] (OwnerGuid, IPAddress, Name, LastName, Gender, CreatedDate, TimeZoneOfCountry, ImageUrl) " + "VALUES (@OwnerGuid, @IPAddress, @Name, @LastName, @Gender, GETDATE(), @TimeZoneOfCountry, @ImageUrl)", connection);
                command.Parameters.AddWithValue("@OwnerGuid", userProfile.OwnerGuid);
                command.Parameters.AddWithValue("@IPAddress", userProfile.IPAddress);
                command.Parameters.AddWithValue("@Name", userProfile.FirstName);
                command.Parameters.AddWithValue("@LastName", userProfile.LastName);
                command.Parameters.AddWithValue("@Gender", userProfile.Gender);
                command.Parameters.AddWithValue("@CreatedDate", userProfile.CreatedDate);
                command.Parameters.AddWithValue("@TimeZoneOfCountry", userProfile.TimeZoneOfCountry);
                command.Parameters.AddWithValue("@ImageUrl", imageURL);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateUserProfile(UserNewProfile userProfile, string imageURL)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE [dbo].[UserProfile] SET Name = @Name, LastName = @LastName, Gender = @Gender, UpdatedDate = GETDATE(), ImageUrl = @ImageUrl " + " WHERE OwnerGuid = @OwnerGuid" , connection);
                command.Parameters.AddWithValue("@OwnerGuid", userProfile.OwnerGuid);
                command.Parameters.AddWithValue("@IPAddress", userProfile.IPAddress);
                command.Parameters.AddWithValue("@Name", userProfile.FirstName);
                command.Parameters.AddWithValue("@LastName", userProfile.LastName);
                command.Parameters.AddWithValue("@Gender", userProfile.Gender);
                command.Parameters.AddWithValue("@UpdatedDate", userProfile.UpdatedDate);
                command.Parameters.AddWithValue("@TimeZoneOfCountry", userProfile.TimeZoneOfCountry);
                command.Parameters.AddWithValue("@ImageUrl", userProfile.ImageUrl);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<string> UploadProfileImage(IFormFile file, string ownerGuid)
        {
            // Implement logic to upload image and return URL
            // For example, save the file to a directory and return the file path
            var filePath = Path.Combine("uploads", ownerGuid.ToString(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }

        //public async Task<string> MoveProfileImage(UserNewProfile userProfile)
        //{
        //    // Implement logic to move image and return new URL
        //    var newPath = Path.Combine("wwwroot/images/logos/", userProfile.OwnerGuid.ToString(), userProfile.ImageUrl);
        //    // Assume some logic to move the image
        //    return newPath;
        //}
    }
}
