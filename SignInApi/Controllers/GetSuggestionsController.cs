using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSuggestionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public GetSuggestionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetSuggestions")]
        public async Task<IActionResult> GetSuggestions(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword cannot be empty.");
            var suggestions = new List<string>();

            string connectionString = _configuration.GetConnectionString("MimListing");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT k.SeoKeyword, l.Name AS LocationName,lst.ListingId
                FROM [dbo].[Keyword] k
                INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                INNER JOIN [MimShared].[dbo].[Location] l ON a.AssemblyID = l.Id
                INNER JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                WHERE k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var keywordName = reader["SeoKeyword"].ToString();
                            var assemblyName = reader["LocationName"].ToString();
                            int listingid = Convert.ToInt32(reader["ListingID"]);
                            suggestions.Add($"{keywordName} in {assemblyName}");
                        }
                    }
                }
            }

            return Ok(suggestions);
        }
    }
}
