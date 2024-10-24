using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessFirstCategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public BusinessFirstCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("FirstCategory")]
        public async Task<IActionResult> FirstCategory()
        {
            string connectionString = _configuration.GetConnectionString("MimCategories");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand("SELECT FirstCategoryID, Name FROM [cat].[FirstCategory] WHERE IsActive = '1'", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        List<object> categories = new List<object>();
                        foreach (DataRow row in dt.Rows)
                        {
                            categories.Add(new
                            {
                                FirstCategoryID = row["FirstCategoryID"].ToString(),
                                Name = row["Name"].ToString()
                            });
                        }

                        return Ok(new
                        {
                            categories = categories
                        });
                    }
                    else
                    {
                        return NotFound(new { message = "No categories found" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Internal server error", error = ex.Message });
                }
            }
        }
    }
}
