using InventoryService.Data;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/inventory/{productId}
        [HttpGet("{productId}")]
        public async Task<ActionResult<InventoryItem>> GetStock(Guid productId)
        {
            var inventoryItem = await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventoryItem == null)
            {
                return NotFound("Product not found in inventory.");
            }

            return Ok(inventoryItem);
        }

        // PUT: api/inventory/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] int newStockQuantity)
        {
            var inventoryItem = await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventoryItem == null)
            {
                return NotFound("Product not found in inventory.");
            }

            inventoryItem.StockQuantity = newStockQuantity;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
