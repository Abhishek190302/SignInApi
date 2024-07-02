using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class SpecialisationRepository
    {
        private readonly string _connectionString;
        public SpecialisationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<Specialisation> GetSpecialisationByListingId(int listingId)
        {
            Specialisation specialisation = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Specialisation] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            specialisation = new Specialisation
                            {
                                ListingID = reader.GetInt32(reader.GetOrdinal("ListingID")),
                                OwnerGuid = reader.GetString(reader.GetOrdinal("OwnerGuid")),
                                IPAddress = reader.GetString(reader.GetOrdinal("IPAddress")),
                                AcceptTenderWork = reader.GetBoolean(reader.GetOrdinal("AcceptTenderWork")),
                                Bank = reader.GetBoolean(reader.GetOrdinal("Banks")),
                                BeautyParlors = reader.GetBoolean(reader.GetOrdinal("BeautyParlors")),
                                Bungalow = reader.GetBoolean(reader.GetOrdinal("Bungalow")),
                                CallCenter = reader.GetBoolean(reader.GetOrdinal("CallCenter")),
                                Church = reader.GetBoolean(reader.GetOrdinal("Church")),
                                Company = reader.GetBoolean(reader.GetOrdinal("Company")),
                                ComputerInstitute = reader.GetBoolean(reader.GetOrdinal("ComputerInstitute")),
                                Dispensary = reader.GetBoolean(reader.GetOrdinal("Dispensary")),
                                ExhibitionStall = reader.GetBoolean(reader.GetOrdinal("ExhibitionStall")),
                                Factory = reader.GetBoolean(reader.GetOrdinal("Factory")),
                                Farmhouse = reader.GetBoolean(reader.GetOrdinal("Farmhouse")),
                                Gurudwara = reader.GetBoolean(reader.GetOrdinal("Gurudwara")),
                                Gym = reader.GetBoolean(reader.GetOrdinal("Gym")),
                                HealthClub = reader.GetBoolean(reader.GetOrdinal("HealthClub")),
                                Home = reader.GetBoolean(reader.GetOrdinal("Home")),
                                Hospital = reader.GetBoolean(reader.GetOrdinal("Hospital")),
                                Hotel = reader.GetBoolean(reader.GetOrdinal("Hotel")),
                                Laboratory = reader.GetBoolean(reader.GetOrdinal("Laboratory")),
                                Mandir = reader.GetBoolean(reader.GetOrdinal("Mandir")),
                                Mosque = reader.GetBoolean(reader.GetOrdinal("Mosque")),
                                Office = reader.GetBoolean(reader.GetOrdinal("Office")),
                                Plazas = reader.GetBoolean(reader.GetOrdinal("Plazas")),
                                ResidentialSociety = reader.GetBoolean(reader.GetOrdinal("ResidentialSociety")),
                                Resorts = reader.GetBoolean(reader.GetOrdinal("Resorts")),
                                Restaurants = reader.GetBoolean(reader.GetOrdinal("Restaurants")),
                                Salons = reader.GetBoolean(reader.GetOrdinal("Salons")),
                                Shop = reader.GetBoolean(reader.GetOrdinal("Shop")),
                                ShoppingMall = reader.GetBoolean(reader.GetOrdinal("ShoppingMall")),
                                Showroom = reader.GetBoolean(reader.GetOrdinal("Showroom")),
                                Warehouse = reader.GetBoolean(reader.GetOrdinal("Warehouse"))
                            };
                        }
                    }
                }
            }

            return specialisation;
        }

        public async Task AddAsync(Specialisation specialisation)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Specialisation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ListingID", specialisation.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", specialisation.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", specialisation.IPAddress);
                    cmd.Parameters.AddWithValue("@AcceptTenderWork", specialisation.AcceptTenderWork);
                    cmd.Parameters.AddWithValue("@Bank", specialisation.Bank);
                    cmd.Parameters.AddWithValue("@BeautyParlors", specialisation.BeautyParlors);
                    cmd.Parameters.AddWithValue("@Bungalow", specialisation.Bungalow);
                    cmd.Parameters.AddWithValue("@CallCenter", specialisation.CallCenter);
                    cmd.Parameters.AddWithValue("@Church", specialisation.Church);
                    cmd.Parameters.AddWithValue("@Company", specialisation.Company);
                    cmd.Parameters.AddWithValue("@ComputerInstitute", specialisation.ComputerInstitute);
                    cmd.Parameters.AddWithValue("@Dispensary", specialisation.Dispensary);
                    cmd.Parameters.AddWithValue("@ExhibitionStall", specialisation.ExhibitionStall);
                    cmd.Parameters.AddWithValue("@Factory", specialisation.Factory);
                    cmd.Parameters.AddWithValue("@Farmhouse", specialisation.Farmhouse);
                    cmd.Parameters.AddWithValue("@Gurudwara", specialisation.Gurudwara);
                    cmd.Parameters.AddWithValue("@Gym", specialisation.Gym);
                    cmd.Parameters.AddWithValue("@HealthClub", specialisation.HealthClub);
                    cmd.Parameters.AddWithValue("@Home", specialisation.Home);
                    cmd.Parameters.AddWithValue("@Hospital", specialisation.Hospital);
                    cmd.Parameters.AddWithValue("@Hotel", specialisation.Hotel);
                    cmd.Parameters.AddWithValue("@Laboratory", specialisation.Laboratory);
                    cmd.Parameters.AddWithValue("@Mandir", specialisation.Mandir);
                    cmd.Parameters.AddWithValue("@Mosque", specialisation.Mosque);
                    cmd.Parameters.AddWithValue("@Office", specialisation.Office);
                    cmd.Parameters.AddWithValue("@Plazas", specialisation.Plazas);
                    cmd.Parameters.AddWithValue("@ResidentialSociety", specialisation.ResidentialSociety);
                    cmd.Parameters.AddWithValue("@Resorts", specialisation.Resorts);
                    cmd.Parameters.AddWithValue("@Restaurants", specialisation.Restaurants);
                    cmd.Parameters.AddWithValue("@Salons", specialisation.Salons);
                    cmd.Parameters.AddWithValue("@Shop", specialisation.Shop);
                    cmd.Parameters.AddWithValue("@ShoppingMall", specialisation.ShoppingMall);
                    cmd.Parameters.AddWithValue("@Showroom", specialisation.Showroom);
                    cmd.Parameters.AddWithValue("@Warehouse", specialisation.Warehouse);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(Specialisation specialisation)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Usp_UpdateSpecialisation", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ListingID", specialisation.ListingID);
                cmd.Parameters.AddWithValue("@OwnerGuid", specialisation.OwnerGuid);
                cmd.Parameters.AddWithValue("@IPAddress", specialisation.IPAddress);
                cmd.Parameters.AddWithValue("@AcceptTenderWork", specialisation.AcceptTenderWork);
                cmd.Parameters.AddWithValue("@Bank", specialisation.Bank);
                cmd.Parameters.AddWithValue("@BeautyParlors", specialisation.BeautyParlors);
                cmd.Parameters.AddWithValue("@Bungalow", specialisation.Bungalow);
                cmd.Parameters.AddWithValue("@CallCenter", specialisation.CallCenter);
                cmd.Parameters.AddWithValue("@Church", specialisation.Church);
                cmd.Parameters.AddWithValue("@Company", specialisation.Company);
                cmd.Parameters.AddWithValue("@ComputerInstitute", specialisation.ComputerInstitute);
                cmd.Parameters.AddWithValue("@Dispensary", specialisation.Dispensary);
                cmd.Parameters.AddWithValue("@ExhibitionStall", specialisation.ExhibitionStall);
                cmd.Parameters.AddWithValue("@Factory", specialisation.Factory);
                cmd.Parameters.AddWithValue("@Farmhouse", specialisation.Farmhouse);
                cmd.Parameters.AddWithValue("@Gurudwara", specialisation.Gurudwara);
                cmd.Parameters.AddWithValue("@Gym", specialisation.Gym);
                cmd.Parameters.AddWithValue("@HealthClub", specialisation.HealthClub);
                cmd.Parameters.AddWithValue("@Home", specialisation.Home);
                cmd.Parameters.AddWithValue("@Hospital", specialisation.Hospital);
                cmd.Parameters.AddWithValue("@Hotel", specialisation.Hotel);
                cmd.Parameters.AddWithValue("@Laboratory", specialisation.Laboratory);
                cmd.Parameters.AddWithValue("@Mandir", specialisation.Mandir);
                cmd.Parameters.AddWithValue("@Mosque", specialisation.Mosque);
                cmd.Parameters.AddWithValue("@Office", specialisation.Office);
                cmd.Parameters.AddWithValue("@Plazas", specialisation.Plazas);
                cmd.Parameters.AddWithValue("@ResidentialSociety", specialisation.ResidentialSociety);
                cmd.Parameters.AddWithValue("@Resorts", specialisation.Resorts);
                cmd.Parameters.AddWithValue("@Restaurants", specialisation.Restaurants);
                cmd.Parameters.AddWithValue("@Salons", specialisation.Salons);
                cmd.Parameters.AddWithValue("@Shop", specialisation.Shop);
                cmd.Parameters.AddWithValue("@ShoppingMall", specialisation.ShoppingMall);
                cmd.Parameters.AddWithValue("@Showroom", specialisation.Showroom);
                cmd.Parameters.AddWithValue("@Warehouse", specialisation.Warehouse);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
