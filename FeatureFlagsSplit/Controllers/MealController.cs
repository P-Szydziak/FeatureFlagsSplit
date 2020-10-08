using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlagsSplit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : Controller
    {
        private readonly AppDbContext _context;

        public MealController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meal>>> GetMeals() => await _context.Meal.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Meal>> GetMeal(int id) => await _context.Meal.FindAsync(id) ?? (ActionResult<Meal>) NotFound();

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeal(int id, Meal meal)
        {
            if (id != meal.Id)
                return BadRequest();

            _context.Entry(meal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MealExists(id))
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
        public async Task<ActionResult<int>> PostMeal(Meal meal)
        { 
            var entityMeal = await _context.Meal.AddAsync(meal);
            await _context.SaveChangesAsync();

            return entityMeal.Entity.Id;
        }

        [SplitFeatureGate(SplitFeatureFlags.DeleteMeal)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMeal(int id)
        {
            var meal = await _context.Meal.FindAsync(id);
            if (meal == null)
            {
                return NotFound();
            }

            _context.Meal.Remove(meal);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool MealExists(int id) => _context.Meal.Any(meal => meal.Id == id);
    }
}