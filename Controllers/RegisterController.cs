using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Data;
using MyMuscleCars.Models;
using System.Security.Cryptography;
using System.Text;

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

        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] RegistrationModel registration)
        {
            // ✅ Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Check if email already exists
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == registration.Email);

            if (existingAccount != null)
                return Conflict(new { message = "An account with that email already exists." });

            // ✅ Hash password
            var hashedPassword = HashPassword(registration.Password);

            // ✅ Create new account record
            var newAccount = new Account
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Email = registration.Email,
                Password = hashedPassword,
                AccountType = "Client"
            };

            // ✅ Save to database
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Account created successfully!" });
        }

        // 🔐 Secure password hashing (SHA-256)
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
