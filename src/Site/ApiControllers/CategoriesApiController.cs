﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Site.ApiControllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesApiController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async  Task<IActionResult> Get()
        {
            var model = await _categoryService.GetAll();
            
            return Ok(model);
        }
    }
}
