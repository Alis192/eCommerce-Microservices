using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchService.Data;
using SearchService.Models;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchDbContext _dbContext;

        public SearchController(SearchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/search?query=example&minPrice=10&maxPrice=100
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] double? minPrice, [FromQuery] double? maxPrice)
        {
            var productsQuery = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await productsQuery.ToListAsync();

            return Ok(products);
        }
    }
}
