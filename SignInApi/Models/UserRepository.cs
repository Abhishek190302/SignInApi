using System.Data;
using System.Data.SqlClient;
using Twilio.Types;

namespace SignInApi.Models
{
    public class UserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[AspNetUsers] WHERE PhoneNumber = @UserName", conn);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    await conn.OpenAsync();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow row = dt.Rows[0];
                        return new ApplicationUser
                        {
                            Id = Guid.Parse(row["Id"].ToString()),
                            UserName = (string)row["UserName"],
                            IsVendor = (bool)row["IsVendor"],
                            PhoneNumber = (string)row["PhoneNumber"],
                            Email = (string)row["Email"]
                            // Map other properties as needed
                        };
                    }
                    return null;
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
