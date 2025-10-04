// Controllers/CarsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Data;
using MyMuscleCars.Models;

namespace MyMuscleCars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CarsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> Get()
        {
            return await _db.Cars.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Car>> Get(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return car;
        }

        [HttpPost]
        public async Task<ActionResult<Car>> Create(Car car)
        {
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Car car)
        {
            if (id != car.Id) return BadRequest();
            _db.Entry(car).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Cars.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var found = await _db.Cars.FindAsync(id);
            if (found == null) return NotFound();
            _db.Cars.Remove(found);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
