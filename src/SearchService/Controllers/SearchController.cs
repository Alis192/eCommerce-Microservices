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
            public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] double? minPrice, [FromQuery] double? maxPrice, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
            {
                // Start building query
                var productsQuery = _dbContext.Products.AsQueryable();

                // Apply search filters if provided
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

                // Apply pagination
                var pagedResult = await productsQuery
                    .Skip((pageNumber - 1) * pageSize)  // Apply skip for pagination
                    .Take(pageSize)                    // Limit to page size
                    .ToListAsync();

                return Ok(pagedResult);  // Return paged result
            }
        }
    }
