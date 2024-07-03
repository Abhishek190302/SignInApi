using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class WorkingHoursRepository
    {
        private readonly string _connectionString;
        public WorkingHoursRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<WorkingHours> GetWorkingHoursByListingId(int listingId)
        {
            WorkingHours workinghours = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[WorkingHours] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            workinghours = new WorkingHours
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                MondayFrom = row.Field<DateTime>("MondayFrom"),
                                MondayTo = row.Field<DateTime>("MondayTo"),
                                TuesdayFrom = row.Field<DateTime>("TuesdayFrom"),
                                TuesdayTo = row.Field<DateTime>("TuesdayTo"),
                                WednesdayFrom = row.Field<DateTime>("WednesdayFrom"),
                                WednesdayTo = row.Field<DateTime>("WednesdayTo"),
                                ThursdayFrom = row.Field<DateTime>("ThursdayFrom"),
                                ThursdayTo = row.Field<DateTime>("ThursdayTo"),
                                FridayFrom = row.Field<DateTime>("FridayFrom"),
                                FridayTo = row.Field<DateTime>("FridayTo"),
                                SaturdayFrom = row.Field<DateTime>("SaturdayFrom"),
                                SaturdayTo = row.Field<DateTime>("SaturdayTo"),
                                SundayFrom = row.Field<DateTime>("SundayFrom"),
                                SundayTo = row.Field<DateTime>("SundayTo"),
                                SaturdayHoliday = Convert.ToBoolean(row["SaturdayHoliday"]),
                                SundayHoliday = Convert.ToBoolean(row["SundayHoliday"]),
                            };
                        }
                    }
                }
            }
            return workinghours;
        }

        public async Task WorkingHoursAddAsync(WorkingHours workinghours)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_WorkingHours", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", workinghours.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", workinghours.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", workinghours.IPAddress);
                    cmd.Parameters.AddWithValue("@MondayFrom", workinghours.MondayFrom);
                    cmd.Parameters.AddWithValue("@MondayTo", workinghours.MondayTo);
                    cmd.Parameters.AddWithValue("@TuesdayFrom", workinghours.TuesdayFrom);
                    cmd.Parameters.AddWithValue("@TuesdayTo", workinghours.TuesdayTo);
                    cmd.Parameters.AddWithValue("@WednesdayFrom", workinghours.WednesdayFrom);
                    cmd.Parameters.AddWithValue("@WednesdayTo", workinghours.WednesdayTo);
                    cmd.Parameters.AddWithValue("@ThursdayFrom", workinghours.ThursdayFrom);
                    cmd.Parameters.AddWithValue("@ThursdayTo", workinghours.ThursdayTo);
                    cmd.Parameters.AddWithValue("@FridayFrom", workinghours.FridayFrom);
                    cmd.Parameters.AddWithValue("@FridayTo", workinghours.FridayTo);
                    cmd.Parameters.AddWithValue("@SaturdayFrom", workinghours.SaturdayFrom);
                    cmd.Parameters.AddWithValue("@SaturdayTo", workinghours.SaturdayTo);
                    cmd.Parameters.AddWithValue("@SundayFrom", workinghours.SundayFrom);
                    cmd.Parameters.AddWithValue("@SundayTo", workinghours.SundayTo);
                    cmd.Parameters.AddWithValue("@SaturdayHoliday", workinghours.SaturdayHoliday);
                    cmd.Parameters.AddWithValue("@SundayHoliday", workinghours.SundayHoliday);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task WorkingHoursUpdateAsync(WorkingHours workinghours)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_UpdateWorkingHours", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", workinghours.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", workinghours.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", workinghours.IPAddress);
                    cmd.Parameters.AddWithValue("@MondayFrom", workinghours.MondayFrom);
                    cmd.Parameters.AddWithValue("@MondayTo", workinghours.MondayTo);
                    cmd.Parameters.AddWithValue("@TuesdayFrom", workinghours.TuesdayFrom);
                    cmd.Parameters.AddWithValue("@TuesdayTo", workinghours.TuesdayTo);
                    cmd.Parameters.AddWithValue("@WednesdayFrom", workinghours.WednesdayFrom);
                    cmd.Parameters.AddWithValue("@WednesdayTo", workinghours.WednesdayTo);
                    cmd.Parameters.AddWithValue("@ThursdayFrom", workinghours.ThursdayFrom);
                    cmd.Parameters.AddWithValue("@ThursdayTo", workinghours.ThursdayTo);
                    cmd.Parameters.AddWithValue("@FridayFrom", workinghours.FridayFrom);
                    cmd.Parameters.AddWithValue("@FridayTo", workinghours.FridayTo);
                    cmd.Parameters.AddWithValue("@SaturdayFrom", workinghours.SaturdayFrom);
                    cmd.Parameters.AddWithValue("@SaturdayTo", workinghours.SaturdayTo);
                    cmd.Parameters.AddWithValue("@SundayFrom", workinghours.SundayFrom);
                    cmd.Parameters.AddWithValue("@SundayTo", workinghours.SundayTo);
                    cmd.Parameters.AddWithValue("@SaturdayHoliday", workinghours.SaturdayHoliday);
                    cmd.Parameters.AddWithValue("@SundayHoliday", workinghours.SundayHoliday);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
