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
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            specialisation = new Specialisation
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                AcceptTenderWork = Convert.ToBoolean(row["AcceptTenderWork"]),
                                Bank = Convert.ToBoolean(row["Banks"]),
                                BeautyParlors = Convert.ToBoolean(row["BeautyParlors"]),
                                Bungalow = Convert.ToBoolean(row["Bungalow"]),
                                CallCenter = Convert.ToBoolean(row["CallCenter"]),
                                Church = Convert.ToBoolean(row["Church"]),
                                Company = Convert.ToBoolean(row["Company"]),
                                ComputerInstitute = Convert.ToBoolean(row["ComputerInstitute"]),
                                Dispensary = Convert.ToBoolean(row["Dispensary"]),
                                ExhibitionStall = Convert.ToBoolean(row["ExhibitionStall"]),
                                Factory = Convert.ToBoolean(row["Factory"]),
                                Farmhouse = Convert.ToBoolean(row["Farmhouse"]),
                                Gurudwara = Convert.ToBoolean(row["Gurudwara"]),
                                Gym = Convert.ToBoolean(row["Gym"]),
                                HealthClub = Convert.ToBoolean(row["HealthClub"]),
                                Home = Convert.ToBoolean(row["Home"]),
                                Hospital = Convert.ToBoolean(row["Hospital"]),
                                Hotel = Convert.ToBoolean(row["Hotel"]),
                                Laboratory = Convert.ToBoolean(row["Laboratory"]),
                                Mandir = Convert.ToBoolean(row["Mandir"]),
                                Mosque = Convert.ToBoolean(row["Mosque"]),
                                Office = Convert.ToBoolean(row["Office"]),
                                Plazas = Convert.ToBoolean(row["Plazas"]),
                                ResidentialSociety = Convert.ToBoolean(row["ResidentialSociety"]),
                                Resorts = Convert.ToBoolean(row["Resorts"]),
                                Restaurants = Convert.ToBoolean(row["Restaurants"]),
                                Salons = Convert.ToBoolean(row["Salons"]),
                                Shop = Convert.ToBoolean(row["Shop"]),
                                ShoppingMall = Convert.ToBoolean(row["ShoppingMall"]),
                                Showroom = Convert.ToBoolean(row["Showroom"]),
                                Warehouse = Convert.ToBoolean(row["Warehouse"])
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
