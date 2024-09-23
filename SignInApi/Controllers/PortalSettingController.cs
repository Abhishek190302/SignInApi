using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortalSettingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PortalSettingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetPortalSetting")]
        public async Task<IActionResult> GetPortalSetting()
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            List<portalsetting> portalSettings = new List<portalsetting>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[PortalSettings]", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                portalsetting portalSetting = new portalsetting
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Logopath = reader["LogoPath"].ToString(),
                                    Feviconpath = reader["FeviconPath"].ToString(),
                                    Facebook = reader["Facebook"].ToString(),
                                    Instagram = reader["Instagram"].ToString(),
                                    Whatsapp = reader["Whatsup"].ToString(),
                                    Linkedin = reader["LinkdenIn"].ToString(),
                                    Twitter = reader["Twitter"].ToString(),
                                    Youtube = reader["Youtube"].ToString(),
                                    Contactinformation = reader["ContactInformation"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Ownerguid = reader["OwnerGuid"].ToString()
                                };

                                portalSettings.Add(portalSetting);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error");
                }

            }

            var response = new
            {
                PortalSettings = portalSettings
            };

            return Ok(response);
        }
    }
}
