using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Data;
using MyMuscleCars.Models;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MyMuscleCars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public LoginController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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

            // ✅ If matched, login successful -> generate JWT and set as HttpOnly cookie
            var token = GenerateJwtToken(existingAccount);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            };

            Response.Cookies.Append("jwtToken", token, cookieOptions);

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

        private string GenerateJwtToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
