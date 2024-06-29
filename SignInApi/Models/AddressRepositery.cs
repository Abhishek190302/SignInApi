using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SignInApi.Models
{
    public class AddressRepositery : IAddressRepositery
    {
        private readonly string _connectionString;
        private readonly string _AddressListingconnectionString;
        public AddressRepositery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimShared");
            _AddressListingconnectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<List<Country>> GetAddressDetails()
        {
            try
            {
                var countries = new List<Country>();

                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    // Retrieve countries
                    var countryCmd = new SqlCommand("Usp_Countrysall", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var countryDa = new SqlDataAdapter(countryCmd);
                    var countryDt = new DataTable();
                    countryDa.Fill(countryDt);
                    foreach (DataRow row in countryDt.Rows)
                    {
                        var country = new Country
                        {
                            CountryID = (int)row["CountryID"],
                            Name = (string)row["Name"],
                            States = new List<State>()
                        };
                        countries.Add(country);

                        // Retrieve states for the current country
                        var stateCmd = new SqlCommand("Usp_Statesall", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        stateCmd.Parameters.AddWithValue("@CountryID", country.CountryID);
                        var stateDa = new SqlDataAdapter(stateCmd);
                        var stateDt = new DataTable();
                        stateDa.Fill(stateDt);
                        foreach (DataRow stateRow in stateDt.Rows)
                        {
                            var state = new State
                            {
                                StateID = (int)stateRow["StateID"],
                                Name = (string)stateRow["Name"],
                                CountryID = (int)stateRow["CountryID"],
                                Cities = new List<City>()
                            };
                            country.States.Add(state);

                            // Retrieve cities for the current state
                            var cityCmd = new SqlCommand("Usp_Citysall", con)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            cityCmd.Parameters.AddWithValue("@StateID", state.StateID);
                            var cityDa = new SqlDataAdapter(cityCmd);
                            var cityDt = new DataTable();
                            cityDa.Fill(cityDt);
                            foreach (DataRow cityRow in cityDt.Rows)
                            {
                                var city = new City
                                {
                                    CityID = (int)cityRow["CityID"],
                                    Name = (string)cityRow["Name"],
                                    StateID = (int)cityRow["StateID"],
                                    Assemblies = new List<Assembly>()
                                };
                                state.Cities.Add(city);

                                // Retrieve assemblies for the current city
                                var assemblyCmd = new SqlCommand("Usp_Locationsall", con)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                assemblyCmd.Parameters.AddWithValue("@CityID", city.CityID);
                                var assemblyDa = new SqlDataAdapter(assemblyCmd);
                                var assemblyDt = new DataTable();
                                assemblyDa.Fill(assemblyDt);
                                foreach (DataRow assemblyRow in assemblyDt.Rows)
                                {
                                    var assembly = new Assembly
                                    {
                                        AssemblyID = (int)assemblyRow["ID"],
                                        Name = (string)assemblyRow["Name"],
                                        CityID = (int)assemblyRow["CityID"],
                                        Pincodes = new List<Pincode>()
                                    };
                                    city.Assemblies.Add(assembly);

                                    // Retrieve pincodes for the current assembly
                                    var pincodeCmd = new SqlCommand("Usp_Pincodesall", con)
                                    {
                                        CommandType = CommandType.StoredProcedure
                                    };
                                    pincodeCmd.Parameters.AddWithValue("@LocationId", assembly.AssemblyID);
                                    var pincodeDa = new SqlDataAdapter(pincodeCmd);
                                    var pincodeDt = new DataTable();
                                    pincodeDa.Fill(pincodeDt);
                                    foreach (DataRow pincodeRow in pincodeDt.Rows)
                                    {
                                        var pincode = new Pincode
                                        {
                                            PincodeID = (int)pincodeRow["PincodeID"],
                                            Number = (int)pincodeRow["PincodeNumber"],
                                            AssemblyID = (int)pincodeRow["LocationId"],
                                            Localities = new List<Locality>()
                                        };
                                        assembly.Pincodes.Add(pincode);

                                        // Retrieve localities for the current pincode
                                        var localityCmd = new SqlCommand("Usp_Areasall", con)
                                        {
                                            CommandType = CommandType.StoredProcedure
                                        };
                                        localityCmd.Parameters.AddWithValue("@PincodeID", pincode.PincodeID);
                                        var localityDa = new SqlDataAdapter(localityCmd);
                                        var localityDt = new DataTable();
                                        localityDa.Fill(localityDt);
                                        foreach (DataRow localityRow in localityDt.Rows)
                                        {
                                            var locality = new Locality
                                            {
                                                LocalityID = (int)localityRow["Id"],
                                                Name = (string)localityRow["Name"],
                                                PincodeID = (int)localityRow["PincodeID"]
                                            };
                                            pincode.Localities.Add(locality);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await con.CloseAsync();
                }

                return countries;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Address> GetAddressByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_AddressListingconnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Address] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Address
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,                        
                        CountryID = row.Field<int?>("CountryID") ?? 0,
                        StateID = row.Field<int?>("StateID") ?? 0,
                        CityID = row.Field<int?>("City") ?? 0,
                        AssemblyID = row.Field<int?>("AssemblyID") ?? 0,
                        PincodeID = row.Field<int?>("PincodeID") ?? 0,
                        LocalityID = row.Field<int?>("LocalityID") ?? 0,
                        LocalAddress = row.Field<string>("LocalAddress") ?? string.Empty,

                    };
                }
                return null;
            }
        }

        public async Task CreateAddress([FromBody] Address address)
        {
            using (SqlConnection conn = new SqlConnection(_AddressListingconnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_CreateAddress", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", address.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", address.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", address.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryID", address.CountryID);
                    cmd.Parameters.AddWithValue("@StateID", address.StateID);
                    cmd.Parameters.AddWithValue("@CityID", address.CityID);
                    cmd.Parameters.AddWithValue("@AssemblyID", address.AssemblyID);
                    cmd.Parameters.AddWithValue("@PincodeID", address.PincodeID);
                    cmd.Parameters.AddWithValue("@LocalityID", address.LocalityID);
                    cmd.Parameters.AddWithValue("@LocalAddress", address.LocalAddress);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }      
        }

        public async Task UpdateAddress([FromBody] Address address)
        {
            using (SqlConnection conn = new SqlConnection(_AddressListingconnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_UpdateAddress", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", address.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", address.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", address.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryID", address.CountryID);
                    cmd.Parameters.AddWithValue("@StateID", address.StateID);
                    cmd.Parameters.AddWithValue("@CityID", address.CityID);
                    cmd.Parameters.AddWithValue("@AssemblyID", address.AssemblyID);
                    cmd.Parameters.AddWithValue("@PincodeID", address.PincodeID);
                    cmd.Parameters.AddWithValue("@LocalityID", address.LocalityID);
                    cmd.Parameters.AddWithValue("@LocalAddress", address.LocalAddress ?? (object)DBNull.Value);
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
