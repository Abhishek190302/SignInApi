using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class AreaService : IAreaService
    {
        private readonly string _connectionString;

        public AreaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimShared");
        }

        public async Task<Area> GetAreaByAreaNameAsync(string areaName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Area] WHERE Name = @Name", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", areaName);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Area
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                PincodeID = reader.GetInt32("PincodeID"),
                                LocationId = reader.GetInt32("LocationId")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task CreateAreaAsync(AreaCreateRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Area] (Name, PincodeID, LocationId) VALUES (@Name, @PincodeID, @LocationId)", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", request.AreaName);
                    cmd.Parameters.AddWithValue("@PincodeID", request.PincodeId);
                    cmd.Parameters.AddWithValue("@LocationId", request.LocalityId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Pincode> GetPincodeByIdAsync(int pincodeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [shared].[Pincode] WHERE PincodeID = @PincodeId", conn))
                {
                    cmd.Parameters.AddWithValue("@PincodeId", pincodeId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Pincode
                            {
                                PincodeID = reader.GetInt32("PincodeID"),
                                Number = reader.GetInt32("PincodeNumber"),
                                AssemblyID = reader.GetInt32("LocationId")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
