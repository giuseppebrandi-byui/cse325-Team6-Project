using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Data;
using MyMuscleCars.Models; // <- Needed to use Inventory model

namespace MyMuscleCars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
        {
            var inventories = await _context.Inventories
                .Include(i => i.MakeRef) // eager-load Make relationship
                .ToListAsync();

            return Ok(inventories);
        }

        // GET: api/inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.InvMake)
                .FirstOrDefaultAsync(i => i.InvId == id);

            if (inventory == null) 
                return NotFound();

            return Ok(inventory);
        }
    }
}
