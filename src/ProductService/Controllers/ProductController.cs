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

            try
            {
                return await _context.Products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                }).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500, "An error occured while fetching the products");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(Guid id)
        {

            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500, "An error occured while fetching the product");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, [FromBody] ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (id != productDTO.Id)
                {
                    return BadRequest("Product ID in the URL does not match the ID in the body.");
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

                // Publish the ProductUpdated event
                await _publishEndpoint.Publish(new ProductUpdated(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.StockQuantity
                ));

                return NoContent();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500, "An error occured while updating the product");
            }
        }


        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(CreateProductDTO createProductDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)  
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Publish the ProductDeleted event
                await _publishEndpoint.Publish(new ProductDeleted(product.Id));

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }
    }
}
