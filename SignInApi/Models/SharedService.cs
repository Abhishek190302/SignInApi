using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class SharedService : ISharedService
    {
        private readonly string _connectionString;

        public SharedService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimShared");
        }

        public async Task<List<Qualification>> GetQualifications()
        {
            List<Qualification> qualifications = new List<Qualification>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Qualification]", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    qualifications.Add(new Qualification
                    {
                        Id = row.Field<int>("Id"),
                        Name = row.Field<string>("Name")
                    });
                }               
            }
            return qualifications;
        }

        public async Task<List<Country>> GetCountries()
        {
            List<Country> countries = new List<Country>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [shared].[Country]";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    countries.Add(new Country
                    {
                        CountryID = row.Field<int>("CountryID"),
                        Name = row.Field<string>("Name")
                    });
                } 
            }
            return countries;
        }

        public async Task<List<State>> GetStatesByCountryId(int countryId)
        {
            List<State> states = new List<State>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [shared].[State] WHERE CountryID = @CountryID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CountryID", countryId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);     
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    states.Add(new State
                    {
                        StateID = row.Field<int>("StateID"),
                        Name = row.Field<string>("Name"),
                        CountryID = row.Field<int>("CountryID")
                    });
                } 
            }
            return states;
        }

        public async Task<List<City>> GetCitiesByStateId(int stateId)
        {
            List<City> cities = new List<City>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [shared].[City] WHERE StateID = @StateID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StateID", stateId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);  
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    cities.Add(new City
                    {
                        CityID = row.Field<int>("CityID"),
                        Name = row.Field<string>("Name"),
                        StateID = row.Field<int>("StateID")
                    });
                } 
            }
            return cities;
        }

        public async Task<List<Assembly>> GetLocalitiesByCityId(int cityId)
        {
            List<Assembly> location = new List<Assembly>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [dbo].[Location] WHERE CityID = @CityID";
                SqlCommand cmd = new SqlCommand(query, conn);    
                cmd.Parameters.AddWithValue("@CityID", cityId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    location.Add(new Assembly
                    {
                        AssemblyID = row.Field<int>("Id"),
                        Name = row.Field<string>("Name"),
                        CityID = row.Field<int>("CityID")
                    });
                }
            }
            return location;
        }

        public async Task<List<Pincode>> GetPincodesByLocalityId(int assemblyId)
        {
            List<Pincode> pincodes = new List<Pincode>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [shared].[Pincode] WHERE LocationId = @AssemblyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssemblyID", assemblyId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);    
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    pincodes.Add(new Pincode
                    {
                        PincodeID = row.Field<int>("PincodeID"),
                        Number = row.Field<int>("PincodeNumber"),
                        AssemblyID = row.Field<int>("LocationId")
                    });
                }
            }
            return pincodes;
        }

        public async Task<List<Locality>> GetAreasByPincodeId(int pincodeId)
        {
            List<Locality> area = new List<Locality>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [dbo].[Area] WHERE PincodeId = @pincodeId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pincodeId", pincodeId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    area.Add(new Locality
                    {
                        LocalityID = row.Field<int>("Id"),
                        Name = row.Field<string>("Name"),
                        PincodeID = row.Field<int>("PincodeID")
                    });
                } 
            }
            return area;
        }
    }
}
