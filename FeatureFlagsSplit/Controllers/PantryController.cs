using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlagsSplit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PantryController : Controller
    {
        private readonly AppDbContext _context;

        public PantryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => await _context.Products.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) => await _context.Products.FindAsync(id) ?? (ActionResult<Product>) NotFound();

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(Product product)
        { 
            var entityProduct = await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return entityProduct.Entity.Id;
        }

        [SplitFeatureGate(SplitFeatureFlags.DeleteMeal)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ProductExists(int id) => _context.Products.Any(meal => meal.Id == id);
    }
}