using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAllFromDropdownController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CategoryRepository _categoryRepository;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public CategoryAllFromDropdownController(CategoryRepository categoryRepository , CompanyDetailsRepository companydetailsRepository, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetAllCategoriesfromFirstandSecond")]
        public async Task<IActionResult> GetAllCategoriesfromFirstandSecond()
        {
            var categories = await _categoryRepository.GetFirstCategoriesAsync();
            object response = new { AllCategories = categories };

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
                            var category = await _categoryRepository.GetCategoryByListingIdAsync(listing.Listingid);
                            bool recordNotFound = category == null;

                            var selectedFirstCategory = categories.FirstOrDefault();

                            // Change to get a single second category ID instead of a comma-separated string
                            var singleSecondCategoryId = selectedFirstCategory?.SecondCategories.FirstOrDefault()?.SecondCategoryId;

                            // Other categories remain comma-separated
                            var thirdCategoryIds = selectedFirstCategory?.SecondCategories.SelectMany(sc => sc.ThirdCategories).Select(tc => tc.ThirdCategoryId.ToString());
                            var thirdCategoryIdsString = string.Join(",", thirdCategoryIds);

                            var fourthCategoryIds = selectedFirstCategory?.SecondCategories.SelectMany(sc => sc.FourthCategories).Select(fc => fc.FourthCategoryId.ToString());
                            var fourthCategoryIdsString = string.Join(",", fourthCategoryIds);

                            var fifthCategoryIds = selectedFirstCategory?.SecondCategories.SelectMany(sc => sc.FifthCategories).Select(fc => fc.FifthCategoryId.ToString());
                            var fifthCategoryIdsString = string.Join(",", fifthCategoryIds);

                            var sixthCategoryIds = selectedFirstCategory?.SecondCategories.SelectMany(sc => sc.SixthCategories).Select(sc => sc.SixthCategoryId.ToString());
                            var sixthCategoryIdsString = string.Join(",", sixthCategoryIds);

                            if (recordNotFound)
                            {
                                category = new Categories
                                {
                                    OwnerGuid = currentUserGuid,
                                    ListingID = listing.Listingid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                    FirstCategoryID = selectedFirstCategory?.FirstCategoryID ?? 0,
                                    SecondCategoryID = singleSecondCategoryId ?? 0,
                                    ThirdCategoryID = thirdCategoryIdsString,
                                    FourthCategoryID = fourthCategoryIdsString,
                                    FifthCategoryID = fifthCategoryIdsString,
                                    SixthCategoryID = sixthCategoryIdsString
                                };
                            }
                            else
                            {
                                category.FirstCategoryID = selectedFirstCategory?.FirstCategoryID ?? category.FirstCategoryID;
                                category.SecondCategoryID = singleSecondCategoryId ?? category.SecondCategoryID;
                                category.ThirdCategoryID = thirdCategoryIdsString ?? category.ThirdCategoryID;
                                category.FourthCategoryID = fourthCategoryIdsString ?? category.FourthCategoryID;
                                category.FifthCategoryID = fifthCategoryIdsString ?? category.FifthCategoryID;
                                category.SixthCategoryID = sixthCategoryIdsString ?? category.SixthCategoryID;
                            }

                            if (recordNotFound)
                            {
                                await _categoryRepository.CreateCategories(category);
                                response = new { Message = "Category Details created successfully", Category = category, AllCategories = categories };
                            }
                            else
                            {
                                await _categoryRepository.UpdateCategories(category);
                                response = new { Message = "Category Details Updated successfully", Category = category, AllCategories = categories };
                            }
                        }

                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, "Internal server error");
                    }
                }
                return NotFound("User not found");

            }
            return Unauthorized();
        }
    }
}
