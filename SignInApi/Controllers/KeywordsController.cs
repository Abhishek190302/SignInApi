using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly KeywordRepository _keywordRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private static List<Keyword> keywordList = new List<Keyword>();
        private static List<Keyword> deleteKeywordsList = new List<Keyword>();
        public KeywordsController(KeywordRepository keywordRepository, UserService userService, CompanyDetailsRepository companyDetailsRepository)
        {
            _userService= userService;
            _keywordRepository = keywordRepository;
            _companydetailsRepository = companyDetailsRepository;
        }

        [HttpPost("ManageKeywords")]
        public async Task<IActionResult> ManageKeywords([FromBody] KeywordActionRequest request)
        {
            var applicationUser = await _userService.GetUserByUserName("web@jeb.com");
            if (applicationUser != null)
            {
                try
                {
                    string currentUserGuid = applicationUser.Id.ToString();
                    var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                    if (listing != null)
                    {
                        switch (request.Action.ToLower())
                        {
                            case "add":
                                if (string.IsNullOrWhiteSpace(request.Keyword))
                                {
                                    return BadRequest("Keyword cannot be empty");
                                }

                                var keywordToAdd = new Keyword
                                {
                                    ListingID = listing.Listingid, // Replace with actual ListingId
                                    OwnerGuid = currentUserGuid, // Replace with actual CurrentUserGuid
                                    SeoKeyword = request.Keyword
                                };

                                if (await _keywordRepository.KeywordExists(keywordToAdd.SeoKeyword))
                                {
                                    return Conflict($"{request.Keyword} already exists in the listing");
                                }

                                // Add keyword to the list
                                keywordList.Add(keywordToAdd);
                                return Ok("Keyword added to the list successfully");

                            case "remove":
                                if (string.IsNullOrWhiteSpace(request.Keyword))
                                {
                                    return BadRequest("Keyword cannot be empty");
                                }

                                // Remove keyword from the list
                                var keywordToRemove = keywordList.Find(k => k.SeoKeyword == request.Keyword);
                                if (keywordToRemove != null)
                                {
                                    keywordList.Remove(keywordToRemove);
                                    deleteKeywordsList.Add(keywordToRemove);
                                }
                                return Ok("Keyword removed from the list successfully");

                            case "save":
                                if (keywordList.Count == 0)
                                {
                                    return BadRequest("Keywords list is empty");
                                }

                                // Save all keywords in the list to the database
                                await _keywordRepository.SaveKeywordsAsync(keywordList);
                                keywordList.Clear(); // Clear the list after saving

                                // Remove all deleted keywords from the database
                                await _keywordRepository.RemoveKeywordsAsync(deleteKeywordsList);
                                deleteKeywordsList.Clear(); // Clear the delete list after saving

                                // Retrieve the updated list of keywords from the database
                                var updatedKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listing.Listingid);

                                return Ok(new
                                {
                                    Message = "Keywords saved successfully",
                                    Keywords = updatedKeywords
                                });

                            default:
                                return BadRequest("Invalid action specified");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception if you have a logging mechanism
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            return NotFound("User not found");
        }
    }
}
