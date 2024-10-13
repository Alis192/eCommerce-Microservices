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
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] double? minPrice, [FromQuery] double? maxPrice)
        {
            // Fetch all products if no filters are applied
            if (string.IsNullOrEmpty(query) && !minPrice.HasValue && !maxPrice.HasValue)
            {
                var allProducts = await _dbContext.Products.ToListAsync();
                return Ok(allProducts);
            }

            // Start building query
            var productsQuery = _dbContext.Products.AsQueryable();

            // Filter by query if provided
            if (!string.IsNullOrEmpty(query))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }

            // Filter by minPrice if provided
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            // Filter by maxPrice if provided
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Execute query and return results
            var products = await productsQuery.ToListAsync();
            return Ok(products);
        }
    }
}
