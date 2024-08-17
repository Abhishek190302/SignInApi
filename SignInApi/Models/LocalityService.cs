using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class LocalityService : ILocalityService
    {
        private readonly string _connectionString;

        public LocalityService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimShared");
        }

        public async Task<Location> GetLocalityByLocalityNameAsync(string localityName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Location] WHERE Name = @Name", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", localityName);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Location
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                CityID = reader.GetInt32("CityID")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task CreateLocalityAsync(LocalityCreateRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Location] (Name, CityID) VALUES (@Name, @CityID)", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", request.LocalityName);
                    cmd.Parameters.AddWithValue("@CityID", request.CityId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [shared].[City] WHERE CityID = @CityID", conn))
                {
                    cmd.Parameters.AddWithValue("@CityID", cityId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new City
                            {
                                CityID = reader.GetInt32("CityID"),
                                Name = reader.GetString("Name")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
