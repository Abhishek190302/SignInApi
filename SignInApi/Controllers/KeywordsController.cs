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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly KeywordRepository _keywordRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private static List<Keyword> keywordList = new List<Keyword>();
        private static List<Keyword> deleteKeywordsList = new List<Keyword>();
        private readonly BinddetailsManagelistingRepository _binddetailsListing;
        public KeywordsController(KeywordRepository keywordRepository, UserService userService, CompanyDetailsRepository companyDetailsRepository , IHttpContextAccessor httpContextAccessor, BinddetailsManagelistingRepository binddetailsListing)
        {
            _userService = userService;
            _keywordRepository = keywordRepository;
            _companydetailsRepository = companyDetailsRepository;
            _httpContextAccessor = httpContextAccessor;
            _binddetailsListing = binddetailsListing;

        }

        [HttpPost]
        [Route("ManageKeywords")]
        public async Task<IActionResult> ManageKeywords([FromBody] KeywordActionRequest request)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            // Initialize currentKeywords with existing keywords from the database for this listing
                            var currentKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listing.Listingid);

                            switch (request.Action.ToLower())
                            {
                                case "add":
                                    if (string.IsNullOrWhiteSpace(request.Keyword))
                                    {
                                        return BadRequest("Keyword cannot be empty");
                                    }

                                    if (currentKeywords.Any(k => k.SeoKeyword == request.Keyword) || keywordList.Any(k => k.SeoKeyword == request.Keyword))
                                    {
                                        return Conflict($"{request.Keyword} already exists in the Keyword listing");
                                    }

                                    var keywordToAdd = new Keyword
                                    {
                                        ListingID = listing.Listingid,
                                        OwnerGuid = currentUserGuid,
                                        SeoKeyword = request.Keyword
                                    };

                                    // Add keyword to the static keyword list
                                    keywordList.Add(keywordToAdd);
                                    return Ok("Keyword added to the list successfully");

                                case "remove":
                                    if (string.IsNullOrWhiteSpace(request.Keyword))
                                    {
                                        return BadRequest("Keyword cannot be empty");
                                    }

                                    // Find the keyword in the current list
                                    var keywordToRemove = currentKeywords.FirstOrDefault(k => k.SeoKeyword == request.Keyword);
                                    if (keywordToRemove == null)
                                    {
                                        // Also check the static keyword list
                                        keywordToRemove = keywordList.FirstOrDefault(k => k.SeoKeyword == request.Keyword);
                                        if (keywordToRemove == null)
                                        {
                                            return NotFound($"Keyword '{request.Keyword}' not found");
                                        }

                                        // Remove the keyword from the static list
                                        keywordList.Remove(keywordToRemove);
                                    }
                                    else
                                    {
                                        // Remove the keyword from the database
                                        await _keywordRepository.RemoveKeywordsAsync(new List<Keyword> { keywordToRemove });
                                    }

                                    return Ok(new
                                    {
                                        Message = "Keyword removed successfully",
                                        Keywords = currentKeywords
                                    });

                                case "save":
                                    if (keywordList.Count == 0)
                                    {
                                        return BadRequest("No new keywords to save");
                                    }

                                    // Save only new keywords to the database
                                    await _keywordRepository.SaveKeywordsAsync(keywordList);
                                    keywordList.Clear(); // Clear the list after saving

                                    // Retrieve the updated list of keywords from the database
                                    var updatedKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listing.Listingid);

                                    return Ok(new
                                    {
                                        Message = "Keywords saved successfully",
                                        Keywords = updatedKeywords
                                    });

                                case "fetch":
                                    var fetchedKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listing.Listingid);
                                    return Ok(new
                                    {
                                        Message = "Keywords fetched successfully",
                                        Keywords = fetchedKeywords
                                    });

                                default:
                                    return BadRequest("Invalid action specified");
                            }
                        }
                        else
                        {
                            var phoneNumber = applicationUser.PhoneNumber;
                            dynamic listingId = await _binddetailsListing.GetListingIdByPhoneNumberAsync(phoneNumber);


                            // Initialize currentKeywords with existing keywords from the database for this listing
                            var currentKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listingId);

                            switch (request.Action.ToLower())
                            {
                                case "add":
                                    if (string.IsNullOrWhiteSpace(request.Keyword))
                                    {
                                        return BadRequest("Keyword cannot be empty");
                                    }

                                    if (((List<Keyword>)currentKeywords).Any(k => k.SeoKeyword == request.Keyword) || ((List<Keyword>)keywordList).Any(k => k.SeoKeyword == request.Keyword))
                                    // if (currentKeywords.Any(k => k.SeoKeyword == request.Keyword) || keywordList.Any(k => k.SeoKeyword == request.Keyword))
                                    {
                                        return Conflict($"{request.Keyword} already exists in the Keyword listing");
                                    }

                                    var keywordToAdd = new Keyword
                                    {
                                        ListingID = listingId,
                                        OwnerGuid = currentUserGuid,
                                        SeoKeyword = request.Keyword
                                    };

                                    // Add keyword to the static keyword list
                                    keywordList.Add(keywordToAdd);
                                    return Ok("Keyword added to the list successfully");

                                case "remove":
                                    if (string.IsNullOrWhiteSpace(request.Keyword))
                                    {
                                        return BadRequest("Keyword cannot be empty");
                                    }

                                    // Find the keyword in the current list
                                    var keywordToRemove = ((List<Keyword>)currentKeywords).FirstOrDefault(k => k.SeoKeyword == request.Keyword);
                                    //var keywordToRemove = currentKeywords.FirstOrDefault((Func<dynamic, bool>)(k => k.SeoKeyword == request.Keyword));
                                    //var keywordToRemove = currentKeywords.FirstOrDefault(k => k.SeoKeyword == request.Keyword);
                                    if (keywordToRemove == null)
                                    {
                                        // Also check the static keyword list
                                        keywordToRemove = keywordList.FirstOrDefault(k => k.SeoKeyword == request.Keyword);
                                        if (keywordToRemove == null)
                                        {
                                            return NotFound($"Keyword '{request.Keyword}' not found");
                                        }

                                        // Remove the keyword from the static list
                                        keywordList.Remove(keywordToRemove);
                                    }
                                    else
                                    {
                                        // Remove the keyword from the database
                                        await _keywordRepository.RemoveKeywordsAsync(new List<Keyword> { keywordToRemove });
                                    }

                                    return Ok(new
                                    {
                                        Message = "Keyword removed successfully",
                                        Keywords = currentKeywords
                                    });

                                case "save":
                                    if (keywordList.Count == 0)
                                    {
                                        return BadRequest("No new keywords to save");
                                    }

                                    // Save only new keywords to the database
                                    await _keywordRepository.SaveKeywordsAsync(keywordList);
                                    keywordList.Clear(); // Clear the list after saving

                                    // Retrieve the updated list of keywords from the database
                                    var updatedKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listingId);

                                    return Ok(new
                                    {
                                        Message = "Keywords saved successfully",
                                        Keywords = updatedKeywords
                                    });

                                case "fetch":
                                    var fetchedKeywords = await _keywordRepository.GetKeywordsByListingIdAsync(listingId);
                                    return Ok(new
                                    {
                                        Message = "Keywords fetched successfully",
                                        Keywords = fetchedKeywords
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
            return Unauthorized();
        }
    }
}
