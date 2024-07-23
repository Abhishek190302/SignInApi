using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace SignInApi.Models
{
    public class CategoryRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionStringListing;
        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimCategories");
            _connectionStringListing = configuration.GetConnectionString("MimListing");
        }

        public async Task<IEnumerable<FirstCategories>> GetFirstCategoriesAsync()
        {
            var firstCategories = new List<FirstCategories>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var firstCatCmd = new SqlCommand("Usp_Getfirstcategories", connection);
                firstCatCmd.CommandType = CommandType.StoredProcedure;
                var firstDa = new SqlDataAdapter(firstCatCmd);
                var firstDt = new DataTable();
                firstDa.Fill(firstDt);
                foreach (DataRow firstRow in firstDt.Rows)
                {
                    var firstCategory = new FirstCategories
                    {
                        FirstCategoryID = (int)firstRow["FirstCategoryID"],
                        FirstCategoryName = (string)firstRow["Name"]
                    };

                    var secondCatCmd = new SqlCommand("Usp_Getsecondcategories", connection);
                    secondCatCmd.CommandType = CommandType.StoredProcedure;
                    secondCatCmd.Parameters.AddWithValue("@FirstCategoryId", firstCategory.FirstCategoryID);
                    var secondDa = new SqlDataAdapter(secondCatCmd);
                    var secondDt = new DataTable();
                    secondDa.Fill(secondDt);
                    foreach (DataRow secondRow in secondDt.Rows)
                    {
                        var secondCategory = new SecondCategories
                        {
                            SecondCategoryId = (int)secondRow["SecondCategoryID"],
                            SecondCategoryName = (string)secondRow["Name"],
                            FirstCategoryId = firstCategory.FirstCategoryID
                        };

                        var thirdCmd = new SqlCommand("Usp_Getthirdcategories", connection);
                        thirdCmd.CommandType = CommandType.StoredProcedure;
                        thirdCmd.Parameters.AddWithValue("@SecondCategoryId", secondCategory.SecondCategoryId);
                        var thirdDa = new SqlDataAdapter(thirdCmd);
                        var thirdDt = new DataTable();
                        thirdDa.Fill(thirdDt);
                        foreach (DataRow thirdRow in thirdDt.Rows)
                        {
                            var thirdCategory = new ThirdCategories
                            {
                                ThirdCategoryId = (int)thirdRow["ThirdCategoryID"],
                                ThirdCategoryName = (string)thirdRow["Name"]
                            };
                            secondCategory.ThirdCategories.Add(thirdCategory);
                        }

                        var fourthCmd = new SqlCommand("Usp_Getfourthcategories", connection);
                        fourthCmd.CommandType = CommandType.StoredProcedure;
                        fourthCmd.Parameters.AddWithValue("@SecondCategoryId", secondCategory.SecondCategoryId);
                        var fourthDa = new SqlDataAdapter(fourthCmd);
                        var fourthDt = new DataTable();
                        fourthDa.Fill(fourthDt);
                        foreach (DataRow fourthRow in fourthDt.Rows)
                        {
                            var fourthCategory = new FourthCategories
                            {
                                FourthCategoryId = (int)fourthRow["FourthCategoryID"],
                                FourthCategoryName = (string)fourthRow["Name"]
                            };
                            secondCategory.FourthCategories.Add(fourthCategory);
                        }

                        var fifthCmd = new SqlCommand("Usp_Getfifthcategories", connection);
                        fifthCmd.CommandType = CommandType.StoredProcedure;
                        fifthCmd.Parameters.AddWithValue("@SecondCategoryId", secondCategory.SecondCategoryId);
                        var fifthDa = new SqlDataAdapter(fifthCmd);
                        var fifthDt = new DataTable();
                        fifthDa.Fill(fifthDt);
                        foreach (DataRow fifthRow in fifthDt.Rows)
                        {
                            var fifthCategory = new FifthCategories
                            {
                                FifthCategoryId = (int)fifthRow["FifthCategoryID"],
                                FifthCategoryName = (string)fifthRow["Name"]
                            };
                            secondCategory.FifthCategories.Add(fifthCategory);
                        }

                        var sixthCmd = new SqlCommand("Usp_Getsixthcategories", connection);
                        sixthCmd.CommandType = CommandType.StoredProcedure;
                        sixthCmd.Parameters.AddWithValue("@SecondCategoryId", secondCategory.SecondCategoryId);
                        var sixthDa = new SqlDataAdapter(sixthCmd);
                        var sixthDt = new DataTable();
                        sixthDa.Fill(sixthDt);
                        foreach (DataRow sixthRow in sixthDt.Rows)
                        {
                            var sixthCategory = new SixthCategories
                            {
                                SixthCategoryId = (int)sixthRow["SixthCategoryID"],
                                SixthCategoryName = (string)sixthRow["Name"]
                            };
                            secondCategory.SixthCategories.Add(sixthCategory);
                        }

                        firstCategory.SecondCategories.Add(secondCategory);
                    }

                    firstCategories.Add(firstCategory);
                }
            }

            return firstCategories;
        }


        public async Task<Categories> GetCategoryByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringListing))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Categories] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
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

        public async Task CreateCategories([FromBody] Categories categories)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringListing))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_CreateCategory", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", categories.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", categories.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", categories.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstCategoryID", categories.FirstCategoryID);
                    cmd.Parameters.AddWithValue("@SecondCategoryID", categories.SecondCategoryID);
                    cmd.Parameters.AddWithValue("@ThirdCategoryID", categories.ThirdCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FourthCategoryID", categories.FourthCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FifthCategoryID", categories.FifthCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SixthCategoryID", categories.SixthCategoryID ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task UpdateCategories([FromBody] Categories categories)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringListing))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_UpdateCategory", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", categories.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", categories.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", categories.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstCategoryID", categories.FirstCategoryID);
                    cmd.Parameters.AddWithValue("@SecondCategoryID", categories.SecondCategoryID);
                    cmd.Parameters.AddWithValue("@ThirdCategoryID", categories.ThirdCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FourthCategoryID", categories.FourthCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FifthCategoryID", categories.FifthCategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SixthCategoryID", categories.SixthCategoryID ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
