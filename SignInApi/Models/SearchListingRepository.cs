using SendGrid.Helpers.Mail;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SignInApi.Models
{
    public class SearchListingRepository
    {
        private readonly string _connectionShared;
        private readonly string _connectionListing;
        private readonly string _connectionStringCat;

        public SearchListingRepository(IConfiguration configuration)
        {
            _connectionShared = configuration.GetConnectionString("MimShared");
            _connectionListing = configuration.GetConnectionString("MimListing");
            _connectionStringCat = configuration.GetConnectionString("MimCategories");
        }

        public async Task<IList<ListingSearch>> GetApprovedListings()
        {
            var listings = new List<ListingSearch>();

            using (SqlConnection conn = new SqlConnection(_connectionListing))
            {
                await conn.OpenAsync();
                string query = @"
                SELECT l.ListingID, l.CompanyName, l.ListingURL,l.BusinessCategory, a.AssemblyID 
                FROM [listing].[Listing] l
                JOIN [listing].[Address] a ON l.ListingID = a.ListingID
                WHERE l.Status = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    listings.Add(new ListingSearch
                    {
                        ListingId = row["ListingID"].ToString(),
                        CompanyName = row["CompanyName"].ToString(),
                        ListingURL = row["ListingURL"].ToString(),
                        Keyword = row["BusinessCategory"].ToString(),
                        Address = new AddressSearch { AssemblyID = row["AssemblyID"].ToString() }
                    });
                }
            }

            return listings;
        }


        public async Task<IList<ListingSearch>> GetUnapprovedListings()
        {
            var listings = new List<ListingSearch>();

            using (SqlConnection conn = new SqlConnection(_connectionListing))
            {
                await conn.OpenAsync();
                string query = @"
                SELECT l.ListingID, l.CompanyName, l.ListingURL,l.BusinessCategory, a.AssemblyID 
                FROM [listing].[Listing] l
                JOIN [listing].[Address] a ON l.ListingID = a.ListingID
                WHERE l.Status = 0";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    listings.Add(new ListingSearch
                    {
                        ListingId = row["ListingID"].ToString(),
                        CompanyName = row["CompanyName"].ToString(),
                        ListingURL = row["ListingURL"].ToString(),
                        Keyword = row["BusinessCategory"].ToString(),
                        Address = new AddressSearch { AssemblyID = row["AssemblyID"].ToString() }
                    });
                }
            }

            return listings;
        }



        public async Task<bool> IsListingInCategory(int listingId, string searchText)
        {
            var subCategories = await GetSubcategory(listingId);
            return subCategories.Any(sub => sub.Name.ToLower().Contains(searchText));
        }

        public async Task<List<string>> GetSubcategoryNames(int listingId)
        {
            var subCategories = await GetSubcategory(listingId);
            return subCategories.Select(sub => sub.Name).ToList();
        }

        public async Task<List<SubCategory>> GetSubcategory(int listingId)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            using (SqlConnection conn = new SqlConnection(_connectionListing))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Categories] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                await conn.OpenAsync();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int subCategoryId = Convert.ToInt32(row["SecondCategoryID"]);
                        string subCategoryName = await GetSecondCategoryById(subCategoryId);
                        if (!string.IsNullOrEmpty(subCategoryName))
                        {
                            subCategories.Add(new SubCategory
                            {
                                Id = subCategoryId,
                                Name = subCategoryName
                            });
                        }
                    }
                }
            }

            return subCategories;
        }

        private async Task<string> GetSecondCategoryById(int categoryId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringCat))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Name FROM [cat].[SecondCategory] WHERE SecondCategoryID = @SecondCategoryID", conn);
                cmd.Parameters.AddWithValue("@SecondCategoryID", categoryId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }



        public async Task<IList<LocalitySearch>> GetLocalitiesByLocalityIds(string[] localityIds)
        {
            try
            {
                var localities = new List<LocalitySearch>();

                using (SqlConnection conn = new SqlConnection(_connectionShared))
                {
                    await conn.OpenAsync();
                    var queryBuilder = new StringBuilder();
                    queryBuilder.Append(@"
                    SELECT l.Id, l.Name, c.Name as CityName 
                    FROM [dbo].[Location] l 
                    JOIN [shared].[City] c ON l.CityID = c.CityID 
                    WHERE l.Id IN (");

                    var parameters = new List<SqlParameter>();

                    for (int i = 0; i < localityIds.Length; i++)
                    {
                        var paramName = "@localityId" + i;
                        if (i > 0)
                        {
                            queryBuilder.Append(",");
                        }
                        queryBuilder.Append(paramName);
                        parameters.Add(new SqlParameter(paramName, localityIds[i]));
                    }

                    queryBuilder.Append(")");

                    SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn);
                    cmd.Parameters.AddRange(parameters.ToArray());
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        localities.Add(new LocalitySearch
                        {
                            Id = row["Id"].ToString(),
                            Name = row["Name"].ToString(),
                            City = new CitySearch { Name = row["CityName"].ToString() }
                        });
                    }
                }

                return localities;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionStringCat))
            {
                // Open the connection
                await conn.OpenAsync();

                // SQL query to retrieve all categories, optionally ordered by name
                var query = "SELECT * FROM [cat].[SecondCategory] ORDER BY Name";

                using (var command = new SqlCommand(query, conn))
                {
                    // Use SqlDataAdapter to fill a DataTable with the results
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();

                        // Fill the DataTable with the result from the query
                        adapter.Fill(dataTable);

                        // Extract the categories from the DataTable
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var category = new Category
                            {
                                Id = Convert.ToInt32(row["SecondCategoryID"]),
                                Name = row["Name"].ToString()
                            };

                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }

        public async Task<bool> IsCategoryMatching(string searchText)
        {
            using (SqlConnection connection = new SqlConnection(_connectionStringCat))
            {
                using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM [cat].[SecondCategory] WHERE LOWER(Name) = @searchText", connection))
                {
                    // Add the parameter with wildcard characters
                    command.Parameters.AddWithValue("@searchText", "%" + searchText.ToLower() + "%");
                    await connection.OpenAsync();
                    int matchCount = (int)await command.ExecuteScalarAsync();
                    return matchCount > 0;
                }
            }
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            Category category = null;

            using (var connection = new SqlConnection(_connectionStringCat))
            {
                var query = "SELECT TOP 1 SecondCategoryID, Name FROM [cat].[SecondCategory] WHERE LOWER(Name) LIKE @CategoryName";

                using (var command = new SqlCommand(query, connection))
                {
                    // Add the parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@CategoryName", $"%{categoryName.ToLower()}%");

                    // Open the connection
                    await connection.OpenAsync();

                    // Execute the query and load the results into a DataTable
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Check if a row is returned
                        if (dataTable.Rows.Count > 0)
                        {
                            var row = dataTable.Rows[0];
                            category = new Category
                            {
                                Id = Convert.ToInt32(row["SecondCategoryID"]),
                                Name = row["Name"].ToString()
                            };
                        }
                    }
                }
            }

            return category;
        }
    }
}
