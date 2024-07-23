using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class UserProfileService: IUserProfileService
    {
        private readonly string _connectionString;

        public UserProfileService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        public async Task<ProfileInfo> GetProfileInfo(string userId)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OwnerGuid", userId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    profileInfo.UserProfile = new UserprofileUpdateVM
                    {
                        CountryID = row.Field<int>("CountryID"),
                        StateID = row.Field<int>("StateID"),
                        CityID = row.Field<int>("CityID"),
                        AssemblyID = row.Field<int>("AssemblyID"),
                        PincodeID = row.Field<int>("PincodeID"),
                        LocalityID = row.Field<int>("LocalityID"),
                        Address = row.Field<string>("Address"),
                        IsProfileCompleted = row.Field<bool>("IsProfileCompleted"),
                        DateOfBirth = row.IsNull("DateOfBirth") ? (DateTime?)null : row.Field<DateTime>("DateOfBirth"),
                        MaritalStatus = row.Field<string>("MaritalStatus"),
                        QualificationId = row.Field<int>("QualificationId")
                    };
                }
            }
            return profileInfo;
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT Id, UserName, IsVendor FROM [dbo].[AspNetUsers] WHERE UserName = @UserName", conn);              
                cmd.Parameters.AddWithValue("@UserName", userName);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new ApplicationUser
                    {
                        Id = row.Field<Guid>("Id"),
                        UserName = row.Field<string>("UserName"),
                        IsVendor = row.Field<bool>("IsVendor")
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task UpdateUserProfile(UserprofileUpdateVM userProfile,string ownerguid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "UPDATE [dbo].[UserProfile] SET CountryID = @CountryID, StateID = @StateID, CityID = @CityID, AssemblyID = @AssemblyID, " +
                               "PincodeID = @PincodeID, LocalityID = @LocalityID, Address = @Address, IsProfileCompleted = @IsProfileCompleted, " +
                               "DateOfBirth = @DateOfBirth, MaritalStatus = @MaritalStatus, QualificationId = @QualificationId WHERE OwnerGuid = @OwnerGuid";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerGuid", ownerguid);
                    cmd.Parameters.AddWithValue("@CountryID", userProfile.CountryID);
                    cmd.Parameters.AddWithValue("@StateID", userProfile.StateID);
                    cmd.Parameters.AddWithValue("@CityID", userProfile.CityID);
                    cmd.Parameters.AddWithValue("@AssemblyID", userProfile.AssemblyID);
                    cmd.Parameters.AddWithValue("@PincodeID", userProfile.PincodeID);
                    cmd.Parameters.AddWithValue("@LocalityID", userProfile.LocalityID);
                    cmd.Parameters.AddWithValue("@Address", userProfile.Address);
                    cmd.Parameters.AddWithValue("@IsProfileCompleted", userProfile.IsProfileCompleted);
                    cmd.Parameters.AddWithValue("@DateOfBirth", userProfile.DateOfBirth);
                    cmd.Parameters.AddWithValue("@MaritalStatus", userProfile.MaritalStatus);
                    cmd.Parameters.AddWithValue("@QualificationId", userProfile.QualificationId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
