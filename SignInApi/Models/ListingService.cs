using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Twilio.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using EllipticCurve.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace SignInApi.Models
{
    public class ListingService : IListingService
    {
        private readonly string _connectionString;
        private readonly string _connectionStringCat;
        private readonly string _connectionStringMimshared;
        private readonly string _connectionStringAudit;
        private readonly ILogger<ListingService> _logger;
        public ListingService(IConfiguration configuration, ILogger<ListingService> logger)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionStringCat = configuration.GetConnectionString("MimCategories");
            _connectionStringAudit = configuration.GetConnectionString("AuditTrail");
            _connectionStringMimshared = configuration.GetConnectionString("MimShared");
            _logger = logger;
        }

        public async Task<List<ListingResult>> GetListings(int pageNumber, int pageSize, int subCategoryid, string cityName = null)
        {
            var listings = new List<ListingResult>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    int offset = (pageNumber - 1) * pageSize;

                    // Fetch listings
                    //var listingCmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE ListingID IN (SELECT ListingID FROM [listing].[Categories] WHERE SecondCategoryID = @SubCategoryId) ORDER BY ListingID  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn);

                    //var listingCmd = new SqlCommand("SELECT l.*, a.City FROM[listing].[Listing] l " +
                    //    "JOIN[listing].[Categories] c ON l.ListingID = c.ListingID JOIN[listing].[Address] a " +
                    //    "ON l.ListingID = a.ListingID JOIN [MimShared].[shared].[City] city ON a.City = city.CityID " +
                    //    "WHERE c.SecondCategoryID = @SubCategoryId  AND(@CityName IS NULL OR city.Name = @CityName) " +
                    //    "ORDER BY l.ListingID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY; ", conn);

                    var listingCmd = new SqlCommand("SELECT l.*, a.City FROM[listing].[Listing] l " +
                       "JOIN[listing].[Categories] c ON l.ListingID = c.ListingID JOIN[listing].[Address] a " +
                       "ON l.ListingID = a.ListingID JOIN [MimShared_Api].[shared].[City] city ON a.City = city.CityID " +
                       "WHERE c.SecondCategoryID = @SubCategoryId  AND(@CityName IS NULL OR city.Name = @CityName) " +
                       "ORDER BY l.ListingID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY; ", conn);



                    listingCmd.Parameters.AddWithValue("@Offset", offset);
                    listingCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    listingCmd.Parameters.AddWithValue("@SubCategoryId", subCategoryid);
                    listingCmd.Parameters.AddWithValue("@CityName", cityName);

                    using (SqlDataReader reader = await listingCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int listingId = reader.GetInt32(reader.GetOrdinal("ListingID"));
                            string userGuid = reader.GetString(reader.GetOrdinal("OwnerGuid"));
                            var listing = new ListingResult
                            {
                                ListingId = listingId,
                                OwnerId = userGuid,
                                Id = reader.GetGuid(reader.GetOrdinal("Id")).ToString(),
                                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                                Url = reader.GetString(reader.GetOrdinal("ListingURL")),
                                ListingUrl = reader.GetString(reader.GetOrdinal("ListingURL")),
                                ListingKeyword = reader.GetString(reader.GetOrdinal("BusinessCategory")),
                                SelfCreated = reader.GetBoolean(reader.GetOrdinal("SelfCreated")),
                                ClaimedListing = reader.GetBoolean(reader.GetOrdinal("ClaimedListing")),
                                //GSTNumber = reader.GetString(reader.GetOrdinal("GSTNumber")),
                                GSTNumber = reader.IsDBNull(reader.GetOrdinal("GSTNumber")) ? null : reader.GetString(reader.GetOrdinal("GSTNumber")),

                            };

                            // Calculate BusinessYear
                            DateTime yearOfEstablishment = reader.GetDateTime(reader.GetOrdinal("YearOfEstablishment"));
                            listing.BusinessYear = DateTime.Now.Year - yearOfEstablishment.Year;

                            //// Fetch logo image
                            listing.LogoImage = await GetLogoImageByListingId(listingId);

                            // Fetch business working hours
                            listing.BusinessWorking = await IsBusinessOpen(listingId);

                            // Fetch workingHours
                            listing.Workingtime = await GetWorkingHoursByListingId(listingId);

                            // Fetch NumberOfEmployees details
                            listing.NumberOfEmployees = await GetNumberofEmployee(listingId);

                            // Fetch Turnover details
                            listing.Turnover = await GetTurnover(listingId);

                            // Fetch category details
                            listing.SubCategory = await GetSubcategory(listingId);

                            // Fetch keyword details
                            listing.Keyword = await GetKeywordsAsync(listingId);

                            // Fetch Descreption details
                            listing.Description = await GetDescription(listingId);

                            // Fetch YearOfEstablishment details
                            listing.YearOfEstablishment = await GetYearOfEstablishment(listingId);

                            // Fetch ratings
                            var (RatingCount, RatingAverage) = await GetRating(listingId);
                            listing.RatingCount = RatingCount;
                            listing.RatingAverage = RatingAverage;

                            // Fetch communication details
                            var (Telephone, Whatsapp, Mobile, Email, Website, Languges, Tollfree, RegisterNumber) = await Getcommunication(listingId);
                            listing.Telephone = Telephone;
                            listing.Whatsapp = Whatsapp;
                            listing.Mobile = Mobile;
                            listing.Email = Email;
                            listing.Website = Website;
                            listing.Languges = Languges;
                            listing.TollFree = Tollfree;
                            listing.RegisterMobile = RegisterNumber;

                            // Fetch address details
                            var (City, Locality, Area, pincode, state, country, FullAddress) = await GetAddress(listingId);
                            listing.City = City;
                            listing.Locality = Locality;
                            listing.Area = Area;
                            listing.FullAddress = FullAddress;


                            // Fetch Review Details
                            listing.Reviews = await GetReviews(listingId);

                            listings.Add(listing);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching listings");
                throw;
            }

            return listings;
        }


        public async Task<List<ListingResult>> GetListingsid(int subCategoryid, string cityName, string Keywords)
        {
            var listings = new List<ListingResult>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    // Fetch listings
                    //var listingCmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE ListingID IN (SELECT ListingID FROM [listing].[Categories] WHERE SecondCategoryID = @SubCategoryId) ORDER BY ListingID  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn);

                    //var listingCmd = new SqlCommand("SELECT l.*, a.City FROM [listing].[Listing] l " +
                    //    "JOIN [listing].[Categories] c ON l.ListingID = c.ListingID " +
                    //    "JOIN [listing].[Address] a ON l.ListingID = a.ListingID " +
                    //    "JOIN [MimShared].[shared].[City] city ON a.City = city.CityID " +
                    //    "JOIN [dbo].[Keyword] k ON l.ListingID = k.ListingID " +
                    //    "WHERE k.SeoKeyword = @Keywords AND c.SecondCategoryID = @SubCategoryId AND (@CityName IS NULL OR city.Name = @CityName) " +
                    //    "ORDER BY l.ListingID;", conn);

                    var listingCmd = new SqlCommand("SELECT l.*, a.City FROM [listing].[Listing] l " +
                       "JOIN [listing].[Categories] c ON l.ListingID = c.ListingID " +
                       "JOIN [listing].[Address] a ON l.ListingID = a.ListingID " +
                       "JOIN [MimShared_Api].[shared].[City] city ON a.City = city.CityID " +
                       "JOIN [dbo].[Keyword] k ON l.ListingID = k.ListingID " +
                       "WHERE k.SeoKeyword = @Keywords AND c.SecondCategoryID = @SubCategoryId AND (@CityName IS NULL OR city.Name = @CityName) " +
                       "ORDER BY l.ListingID;", conn);

                    listingCmd.Parameters.AddWithValue("@Keywords", Keywords);
                    listingCmd.Parameters.AddWithValue("@SubCategoryId", subCategoryid);
                    listingCmd.Parameters.AddWithValue("@CityName", cityName);

                    using (SqlDataReader reader = await listingCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int listingId = reader.GetInt32(reader.GetOrdinal("ListingID"));
                            string userGuid = reader.GetString(reader.GetOrdinal("OwnerGuid"));
                            var listing = new ListingResult
                            {
                                ListingId = listingId,
                                OwnerId = userGuid,
                                Id = reader.GetGuid(reader.GetOrdinal("Id")).ToString(),
                                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                                Url = reader.GetString(reader.GetOrdinal("ListingURL")),
                                ListingUrl = reader.GetString(reader.GetOrdinal("ListingURL")),
                                ListingKeyword = reader.GetString(reader.GetOrdinal("BusinessCategory")),
                                SelfCreated = reader.GetBoolean(reader.GetOrdinal("SelfCreated")),
                                ClaimedListing = reader.GetBoolean(reader.GetOrdinal("ClaimedListing")),
                                GSTNumber = reader.IsDBNull(reader.GetOrdinal("GSTNumber")) ? null : reader.GetString(reader.GetOrdinal("GSTNumber")),
                            };

                            // Calculate BusinessYear
                            DateTime yearOfEstablishment = reader.GetDateTime(reader.GetOrdinal("YearOfEstablishment"));
                            listing.BusinessYear = DateTime.Now.Year - yearOfEstablishment.Year;

                            //// Fetch logo image
                            listing.LogoImage = await GetLogoImageByListingId(listingId);

                            // Fetch business working hours
                            listing.BusinessWorking = await IsBusinessOpen(listingId);

                            // Fetch workingHours
                            listing.Workingtime = await GetWorkingHoursByListingId(listingId);

                            // Fetch NumberOfEmployees details
                            listing.NumberOfEmployees = await GetNumberofEmployee(listingId);

                            // Fetch Turnover details
                            listing.Turnover = await GetTurnover(listingId);

                            // Fetch keyword details
                            listing.Keyword = await GetKeywordsAsync(listingId);

                            // Fetch category details
                            listing.SubCategory = await GetSubcategory(listingId);

                            // Fetch Descreption details
                            listing.Description = await GetDescription(listingId);

                            // Fetch YearOfEstablishment details
                            listing.YearOfEstablishment = await GetYearOfEstablishment(listingId);

                            // Fetch ratings
                            var (RatingCount, RatingAverage) = await GetRating(listingId);
                            listing.RatingCount = RatingCount;
                            listing.RatingAverage = RatingAverage;

                            // Fetch communication details
                            var (Telephone, Whatsapp, Mobile, Email, Website, Languges, TollFree, RegisterNumber) = await Getcommunication(listingId);
                            listing.Telephone = Telephone;
                            listing.Whatsapp = Whatsapp;
                            listing.Mobile = Mobile;
                            listing.Email = Email;
                            listing.Website = Website;
                            listing.Languges = Languges;
                            listing.TollFree = TollFree;
                            listing.RegisterMobile = RegisterNumber;

                            // Fetch address details
                            var (City, Locality, Area, pincode, state, country, FullAddress) = await GetAddress(listingId);
                            listing.City = City;
                            listing.Locality = Locality;
                            listing.Area = Area;
                            listing.FullAddress = FullAddress;


                            // Fetch Review Details
                            listing.Reviews = await GetReviews(listingId);

                            listings.Add(listing);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching listings");
                throw;
            }

            return listings;
        }


        private async Task<string> GetPincodeById(int pincodeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetPincodeById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PincodeID", pincodeId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<string> GetStateById(int stateId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetStateById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StateID", stateId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<string> GetCountryById(int countryId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetCountryById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryID", countryId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<string> GetCityById(int cityId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetCityById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CityID", cityId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<string> GetLocalityById(int localityId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetLocalityById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocalityID", localityId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<string> GetAreaById(int areaId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringMimshared))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("Usp_GetAreaById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AreaID", areaId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        private async Task<List<SubCategory>> GetSubcategory(int listingId)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
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

        private async Task<List<Review>> GetReviews(int listingId)
        {
            var reviews = new List<Review>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var command = new SqlCommand("Usp_GetReviews", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        reviews.Add(new Review
                        {
                            Id = Convert.ToInt32(row["RatingID"]),
                            UserName = row["Name"].ToString(),
                            UserImage = row["ImagePath"].ToString(),
                            Ratings = Convert.ToInt32(row["Ratings"]),
                            Comment = row["Comment"].ToString(),
                            Date = Convert.ToDateTime(row["Date"]),
                            RatingReplyMessage = row["Message"].ToString()

                        });
                    }
                }
            }
          
            return reviews;
        }


        private async Task<string> GetTurnover(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT Turnover FROM [listing].[Listing] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var Turnover = dt.Rows[0]["Turnover"].ToString();
                    return Turnover;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<(string City, string Locality, string Area, string Pincode, string State, string Country, string FullAddress)> GetAddress(int listingId)
        {
            DataTable addressTable = new DataTable();
            string City = string.Empty;
            string Locality = string.Empty;
            string Area = string.Empty;
            string Pincode = string.Empty;
            string State = string.Empty;
            string Country = string.Empty;
            string FullAddress = string.Empty;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Address] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addressTable);
                if (addressTable.Rows.Count > 0)
                {
                    DataRow addressRow = addressTable.Rows[0];
                    string localAddress = addressRow["LocalAddress"].ToString();
                    int cityId = Convert.ToInt32(addressRow["City"]);
                    int localityId = Convert.ToInt32(addressRow["AssemblyID"]);
                    int areaId = Convert.ToInt32(addressRow["LocalityID"]);
                    int pincodeId = Convert.ToInt32(addressRow["PincodeID"]);
                    int stateId = Convert.ToInt32(addressRow["StateID"]);
                    int countryId = Convert.ToInt32(addressRow["CountryID"]);

                    City = await GetCityById(cityId);
                    Locality = await GetLocalityById(localityId);
                    Area = await GetAreaById(areaId);
                    Pincode = await GetPincodeById(pincodeId);
                    State = await GetStateById(stateId);
                    Country = await GetCountryById(countryId);
                    FullAddress = $"{localAddress}, {Area}, {Locality}, {City} - {Pincode}, {State}, {Country}";
                }

                return (City, Locality, Area, Pincode, State, Country, FullAddress);
            } 
        }

        private async Task<(string Telephone, string Whatsapp, string Mobile, string Email, string Website, string Languages, string Tollfree, string RegisterNumber)> Getcommunication(int listingId)
        {
            string telephone = string.Empty;
            string whatsapp = string.Empty;
            string mobile = string.Empty;
            string email = string.Empty;
            string website = string.Empty;
            string languages = string.Empty;
            string tollfree = string.Empty;
            string registernumber = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand commCmd = new SqlCommand("SELECT * FROM [listing].[Communication] WHERE ListingID = @ListingId", conn);
                commCmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter adapter = new SqlDataAdapter(commCmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    telephone = GetColumnValueOrNull(row, "Telephone");
                    whatsapp = GetColumnValueOrNull(row, "Whatsapp");
                    mobile = GetColumnValueOrNull(row, "Mobile");
                    email = GetColumnValueOrNull(row, "Email");
                    website = GetColumnValueOrNull(row, "Website");
                    languages = GetColumnValueOrNull(row, "Language");
                    tollfree = GetColumnValueOrNull(row, "TollFree");
                    registernumber = GetColumnValueOrNull(row, "TelephoneSecond");
                }
            }

            return (telephone, whatsapp, mobile, email, website, languages, tollfree, registernumber);
        }

        private string GetColumnValueOrNull(DataRow row, string columnName)
        {
            return row[columnName] != DBNull.Value ? row[columnName].ToString() : null;
        }

        private async Task<string> GetYearOfEstablishment(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT YearOfEstablishment FROM [listing].[Listing] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DateTime yearOfEstablishment = (DateTime)dt.Rows[0]["YearOfEstablishment"];
                    string yearString = yearOfEstablishment.ToString("yyyy");
                    return yearString;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<string> GetDescription(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT Description FROM [listing].[Listing] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var Description = dt.Rows[0]["Description"].ToString();
                    return Description;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<string> GetNumberofEmployee(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT NumberOfEmployees FROM [listing].[Listing] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var NumberOfEmployees = dt.Rows[0]["NumberOfEmployees"].ToString();
                    return NumberOfEmployees;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<List<SeoKeyword>> GetKeywordsAsync(int listingId)
        {
            var keywords = new List<SeoKeyword>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT KeywordID, SeoKeyword FROM [dbo].[Keyword] WHERE ListingID = @ListingId", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingId", listingId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            keywords.Add(new SeoKeyword
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("KeywordID")),
                                Name = reader.GetString(reader.GetOrdinal("SeoKeyword"))
                            });
                        }
                    }
                }
            }

            return keywords;
        }

        private async Task<(int RatingCount, double RatingAverage)> GetRating(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand ratingCmd = new SqlCommand("SELECT * FROM [listing].[Rating] WHERE ListingID = @ListingId", conn);
                ratingCmd.Parameters.AddWithValue("@ListingId", listingId);
                SqlDataAdapter adapter = new SqlDataAdapter(ratingCmd);
                DataTable ratingsTable = new DataTable();
                adapter.Fill(ratingsTable);
                List<int> ratings = new List<int>();
                if (ratingsTable.Rows.Count > 0)
                {
                    var rating = Convert.ToInt32(ratingsTable.Rows[0]["Ratings"]);
                    ratings.Add(rating);
                }

                var RatingCount = ratings.Count;
                var RatingAverage = ratings.Any() ? ratings.Average() : 0;
                return (RatingCount, RatingAverage);
            }
        }

        private async Task<LogoImage> GetLogoImageByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT ImagePath FROM [dbo].[LogoImage] WHERE ListingID = @ListingID AND Status = '1';", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new LogoImage { ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")) };
                    }
                }
            }
            return null;
        }

        private async Task<WorkingTime> GetWorkingHoursByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM [listing].[WorkingHours] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new WorkingTime
                        {
                            MondayFrom = reader.GetDateTime(reader.GetOrdinal("MondayFrom")).TimeOfDay,
                            MondayTo = reader.GetDateTime(reader.GetOrdinal("MondayTo")).TimeOfDay,
                            TuesdayFrom = reader.GetDateTime(reader.GetOrdinal("TuesdayFrom")).TimeOfDay,
                            TuesdayTo = reader.GetDateTime(reader.GetOrdinal("TuesdayTo")).TimeOfDay,
                            WednesdayFrom = reader.GetDateTime(reader.GetOrdinal("WednesdayFrom")).TimeOfDay,
                            WednesdayTo = reader.GetDateTime(reader.GetOrdinal("WednesdayTo")).TimeOfDay,
                            ThursdayFrom = reader.GetDateTime(reader.GetOrdinal("ThursdayFrom")).TimeOfDay,
                            ThursdayTo = reader.GetDateTime(reader.GetOrdinal("ThursdayTo")).TimeOfDay,
                            FridayFrom = reader.GetDateTime(reader.GetOrdinal("FridayFrom")).TimeOfDay,
                            FridayTo = reader.GetDateTime(reader.GetOrdinal("FridayTo")).TimeOfDay,
                            SaturdayFrom = reader.GetDateTime(reader.GetOrdinal("SaturdayFrom")).TimeOfDay,
                            SaturdayTo = reader.GetDateTime(reader.GetOrdinal("SaturdayTo")).TimeOfDay,
                            SundayFrom = reader.GetDateTime(reader.GetOrdinal("SundayFrom")).TimeOfDay,
                            SundayTo = reader.GetDateTime(reader.GetOrdinal("SundayTo")).TimeOfDay,
                            SaturdayHoliday = reader.GetBoolean(reader.GetOrdinal("SaturdayHoliday")),
                            SundayHoliday = reader.GetBoolean(reader.GetOrdinal("SundayHoliday")),
                        };
                    }
                }
            }
            return null;
        }

        public async Task<BusinessWorkingHour> IsBusinessOpen(int listingId)
        {
            var workingTime = await GetWorkingHoursByListingId(listingId);
            BusinessWorkingHour businessWorking = new BusinessWorkingHour();

            if (workingTime == null)
            {
                businessWorking.IsBusinessOpen = true;
                return businessWorking;
            }

            DateTime timeZoneDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            string day = timeZoneDate.ToString("dddd");

            if (day == "Saturday" && workingTime.SaturdayHoliday)
            {
                businessWorking.OpenDay = workingTime.SundayHoliday ? "Monday" : "Sunday";
                businessWorking.OpenTime = (workingTime.SundayHoliday ? workingTime.MondayFrom : workingTime.SundayFrom).ToString(@"hh\:mm");
                return businessWorking;
            }
            else if (day == "Sunday" && workingTime.SundayHoliday)
            {
                businessWorking.OpenDay = "Monday";
                businessWorking.OpenTime = workingTime.MondayFrom.ToString(@"hh\:mm");
                return businessWorking;
            }

            DateTime currentTime = DateTime.Parse(timeZoneDate.ToString("hh:mm tt"), System.Globalization.CultureInfo.CurrentCulture);
            DateTime openTime, closeTime;

            switch (day)
            {
                case "Monday":
                    openTime = DateTime.Parse(workingTime.MondayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.MondayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = "Tuesday";
                    break;
                case "Tuesday":
                    openTime = DateTime.Parse(workingTime.TuesdayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.TuesdayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = "Wednesday";
                    break;
                case "Wednesday":
                    openTime = DateTime.Parse(workingTime.WednesdayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.WednesdayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = "Thursday";
                    break;
                case "Thursday":
                    openTime = DateTime.Parse(workingTime.ThursdayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.ThursdayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = "Friday";
                    break;
                case "Friday":
                    openTime = DateTime.Parse(workingTime.FridayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.FridayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = workingTime.SaturdayHoliday ? (workingTime.SundayHoliday ? "Monday" : "Sunday") : "Saturday";
                    break;
                case "Saturday":
                    openTime = DateTime.Parse(workingTime.SaturdayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.SaturdayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = workingTime.SundayHoliday ? "Monday" : "Sunday";
                    break;
                default:
                    openTime = DateTime.Parse(workingTime.SundayFrom.ToString(@"hh\:mm"));
                    closeTime = DateTime.Parse(workingTime.SundayTo.ToString(@"hh\:mm"));
                    businessWorking.OpenDay = "Monday";
                    break;
            }

            businessWorking.OpenTime = openTime.ToString("hh:mm tt");
            businessWorking.CloseTime = closeTime.ToString("hh:mm tt");
            businessWorking.IsBusinessOpen = currentTime > openTime && currentTime < closeTime;
            return businessWorking;
        }

        #region BookMark
        public async Task<Bookmarks> GetBookmarkByListingAndUserIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Bookmarks] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    return new Bookmarks
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"), 
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"), 
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        Bookmark = row.Field<bool>("Bookmark")
                    };
                }

                return null;
            }
        }

        public async Task AddAsync(Bookmarks bookmark)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[Bookmarks] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, Bookmark) " + " VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Bookmark)", conn);
                cmd.Parameters.AddWithValue("@ListingID", bookmark.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", bookmark.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", bookmark.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", bookmark.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", bookmark.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", bookmark.VisitTime);
                cmd.Parameters.AddWithValue("@Bookmark", bookmark.Bookmark);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(Bookmarks bookmark)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[Bookmarks] SET Bookmark = @Bookmark WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", bookmark.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", bookmark.UserGuid);
                cmd.Parameters.AddWithValue("@Bookmark", bookmark.Bookmark);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        #endregion

        #region Like and Dislike
        public async Task<LikeDislike> GetLikeDislikeByListingAndUserIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[LikeDislike] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    return new LikeDislike
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"),
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"),
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        LikeandDislike = row.Field<bool>("Like")
                    };
                }
                return null;
            }
        }

        public async Task AddLikeDislikeAsync(LikeDislike like)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[LikeDislike] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, [Like]) " + "VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Like)", conn);
                cmd.Parameters.AddWithValue("@ListingID", like.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", like.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", like.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", like.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", like.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", like.VisitTime);
                cmd.Parameters.AddWithValue("@Like", like.LikeandDislike);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateLikeDislikeAsync(LikeDislike like)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[LikeDislike] SET [Like] = @Like WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", like.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", like.UserGuid);
                cmd.Parameters.AddWithValue("@Like", like.LikeandDislike);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        #endregion

        #region Subscribe
        public async Task<Subscribes> GetSubscribeByListingAndUserIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Subscribes] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Subscribes
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"),
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"),
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        Subscribe = row.Field<bool>("Subscribe")
                    };
                }
                return null;
            }
        }

        public async Task AddSubscribeAsync(Subscribes subscribe)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[Subscribes] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, Subscribe) " + " VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Subscribe)", conn);
                cmd.Parameters.AddWithValue("@ListingID", subscribe.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", subscribe.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", subscribe.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", subscribe.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", subscribe.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", subscribe.VisitTime);
                cmd.Parameters.AddWithValue("@Subscribe", subscribe.Subscribe);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateSubscribeAsync(Subscribes subscribe)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStringAudit))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[Subscribes] SET Subscribe = @Subscribe WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", subscribe.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", subscribe.UserGuid);
                cmd.Parameters.AddWithValue("@Subscribe", subscribe.Subscribe);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        #endregion
    }
}
