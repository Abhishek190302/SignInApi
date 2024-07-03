using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class PaymentModeRepository
    {
        private readonly string _connectionString;
        public PaymentModeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<PaymentMode> GetPaymentModeByListingId(int listingId)
        {
            PaymentMode paymentmode = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[PaymentMode] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            paymentmode = new PaymentMode
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                Cash = Convert.ToBoolean(row["Cash"]),
                                Cheque = Convert.ToBoolean(row["Cheque"]),
                                RtgsNeft = Convert.ToBoolean(row["RtgsNeft"]),
                                DebitCard = Convert.ToBoolean(row["DebitCard"]),
                                CreditCard = Convert.ToBoolean(row["CreditCard"]),
                                NetBanking = Convert.ToBoolean(row["NetBanking"]),                                                           
                            };
                        }
                    }
                }
            }
            return paymentmode;
        }

        public async Task AddAsync(PaymentMode paymentmode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_PaymentMode", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", paymentmode.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", paymentmode.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", paymentmode.IPAddress);
                    cmd.Parameters.AddWithValue("@Cash", paymentmode.Cash);
                    cmd.Parameters.AddWithValue("@Cheque", paymentmode.Cheque);
                    cmd.Parameters.AddWithValue("@RtgsNeft", paymentmode.RtgsNeft);
                    cmd.Parameters.AddWithValue("@DebitCard", paymentmode.DebitCard);
                    cmd.Parameters.AddWithValue("@CreditCard", paymentmode.CreditCard);
                    cmd.Parameters.AddWithValue("@NetBanking", paymentmode.NetBanking);
                    cmd.Parameters.AddWithValue("@PayTM", paymentmode.PayTM);
                    cmd.Parameters.AddWithValue("@PhonePay", paymentmode.PhonePay);
                    cmd.Parameters.AddWithValue("@Paypal", paymentmode.Paypal);


                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(PaymentMode paymentmode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_UpdatePaymentMode", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", paymentmode.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", paymentmode.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", paymentmode.IPAddress);
                    cmd.Parameters.AddWithValue("@Cash", paymentmode.Cash);
                    cmd.Parameters.AddWithValue("@Cheque", paymentmode.Cheque);
                    cmd.Parameters.AddWithValue("@RtgsNeft", paymentmode.RtgsNeft);
                    cmd.Parameters.AddWithValue("@DebitCard", paymentmode.DebitCard);
                    cmd.Parameters.AddWithValue("@CreditCard", paymentmode.CreditCard);
                    cmd.Parameters.AddWithValue("@NetBanking", paymentmode.NetBanking);
                    cmd.Parameters.AddWithValue("@PayTM", paymentmode.PayTM);
                    cmd.Parameters.AddWithValue("@PhonePay", paymentmode.PhonePay);
                    cmd.Parameters.AddWithValue("@Paypal", paymentmode.Paypal);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
