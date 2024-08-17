using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class PincodeService : IPincodeService
    {
        private readonly string _connectionString;

        public PincodeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimShared");
        }

        public async Task<Pincode> GetPincodeByPinNumberAsync(int pinNumber)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [shared].[Pincode] WHERE PincodeNumber = @PincodeNumber", conn))
                {
                    cmd.Parameters.AddWithValue("@PincodeNumber", pinNumber);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Pincode
                            {
                                PincodeID = reader.GetInt32("PincodeID"),
                                AssemblyID = reader.GetInt32("LocationId"),
                                Number = reader.GetInt32("PincodeNumber")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task CreatePincodeAsync(PincodeCreateRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [shared].[Pincode] (LocationId, PincodeNumber) VALUES (@LocationId, @PincodeNumber)", conn))
                {
                    cmd.Parameters.AddWithValue("@LocationId", request.LocalityId);
                    cmd.Parameters.AddWithValue("@PincodeNumber", request.PinNumber);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Location> GetLocalityByIdAsync(int localityId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Location] WHERE Id = @LocalityId", conn))
                {
                    cmd.Parameters.AddWithValue("@LocalityId", localityId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Location
                            {
                                Id = reader.GetInt32("Id"),
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
