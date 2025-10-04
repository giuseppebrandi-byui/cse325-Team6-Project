using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyMuscleCars.Data;
using MyMuscleCars.Models;
using System.Text.Json;

namespace MyMuscleCars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            // ✅ Model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var json = JsonSerializer.Serialize(model);
            Console.WriteLine($"Register request JSON: {json}");

            Console.WriteLine($"Register request: FirstName={model.FirstName}, LastName={model.LastName}, Email={model.Email}");

            // ✅ Check if email is already registered
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == model.Email);

            if (existingAccount != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // ✅ Hash the password
            var hasher = new PasswordHasher<Account>();
            var newAccount = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = string.Empty,
                AccountType = "Client"
            };

            newAccount.Password = hasher.HashPassword(newAccount, model.Password);

            // ✅ Save to database
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // ✅ Return success
            return Ok(new { message = "Account created successfully." });
        }
    }
}
