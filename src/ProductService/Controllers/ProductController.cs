using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductController(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint= publishEndpoint;
            _context = context;
        }

        // Get: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            return await _context.Products
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(Guid id)
        {
            var product = await _context.Products
            .Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            })
            .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) { return NotFound(); }

            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, ProductDTO productDTO)
        {
            if (id != productDTO.Id)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.StockQuantity = productDTO.StockQuantity;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(CreateProductDTO createProductDTO)
        {
                var product = new Product
            {
                Id = Guid.NewGuid(),  // Generate a new GUID for each product
                Name = createProductDTO.Name,
                Description = createProductDTO.Description,
                Price = createProductDTO.Price,
                StockQuantity = createProductDTO.StockQuantity
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new ProductCreated(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity
            ));


            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTO);
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)  
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
