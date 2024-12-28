using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class TrackUserService: ITrackUserService
    {
        private readonly string _connectionString;

        public TrackUserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        public void TrackActiveUser(ActiveUserModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    bool isNewUser = false;
                    var checkCommand = new SqlCommand("SELECT COUNT(*) FROM [dbo].[ActiveUsers] WHERE UserId = @UserId", connection);
                    checkCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { Value = model.UserId });
                    int userExists = (int)checkCommand.ExecuteScalar();
                    if (userExists == 0)
                    {
                        isNewUser = true;
                    }


                    // SQL command to insert user activity
                    var command = new SqlCommand("INSERT INTO [dbo].[ActiveUsers] (UserId, LoginTime, IsNewUser, CreationDate) VALUES (@UserId, @LoginTime, @IsNewUser,GETDATE())", connection);
                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { Value = model.UserId });
                    command.Parameters.Add(new SqlParameter("@LoginTime", SqlDbType.DateTime) { Value = model.LoginTime });
                    command.Parameters.Add(new SqlParameter("@IsNewUser", SqlDbType.Bit) { Value = isNewUser });
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Handle exception (e.g., log it, rethrow, etc.)
                    throw new Exception("Error tracking user activity", ex);
                }
            }
        }
    }
}
