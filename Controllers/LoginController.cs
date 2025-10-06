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
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> LoginUser([FromBody] LoginModel login)
        {
            // ✅ Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Find user by email
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == login.Email);

            if (existingAccount == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // ✅ Hash entered password to compare
            var hashedPassword = HashPassword(login.Password);

            if (existingAccount.Password != hashedPassword)
                return Unauthorized(new { message = "Invalid email or password." });

            // ✅ If matched, login successful
            return Ok(new
            {
                message = "Login successful!",
                user = new
                {
                    existingAccount.Id,
                    existingAccount.FirstName,
                    existingAccount.LastName,
                    existingAccount.Email
                }
            });
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
