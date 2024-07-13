using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class CategoryController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryController(ICategoryService categoryService, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetCategories")]
        public async Task<ActionResult<IndexVM>> GetCategories()
        {
            var indexVM = new IndexVM();
            //var user = _httpContextAccessor.HttpContext.User;
            //if (user.Identity.IsAuthenticated)
            //{
            //    var userName = user.Identity.Name;

            //    var applicationUser = await _userService.GetUserByUserName(userName);
            //    if (applicationUser != null)
            //    {
                    await _categoryService.GetCategoriesForIndexPageAsync(indexVM);                    
            //    }
            //}
            return Ok(indexVM);
        }
    }
}
