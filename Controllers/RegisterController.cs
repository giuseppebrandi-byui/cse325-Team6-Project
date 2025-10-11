using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyMuscleCars.Data;
using MyMuscleCars.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// Register Controller handles user registration
namespace MyMuscleCars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public RegisterController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        // POST api/register
        // Registers a new user and returns a JWT token in an HttpOnly cookie
        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] RegistrationModel registration)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email exists
            if (await _context.Accounts.AnyAsync(a => a.Email == registration.Email))
                return Conflict(new { message = "Email already exists" });

            // Hash password
            var hashedPassword = HashPassword(registration.Password);

            var newAccount = new Account
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Email = registration.Email,
                Password = hashedPassword
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // Generate JWT
            var token = GenerateJwtToken(newAccount);

            // Set token as an HttpOnly cookie so client-side JS cannot read it.
            // Use Secure=true when the request is over HTTPS.
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            };

            Response.Cookies.Append("jwtToken", token, cookieOptions);

            // Return minimal user info (token is in httpOnly cookie)
            return Ok(new { user = newAccount.Email });
        }
        // Simple SHA256 hash for password
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        // Generate JWT token for the given account
        private string GenerateJwtToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim("email", account.Email),
                new Claim(ClaimTypes.Name, string.IsNullOrEmpty(account.FirstName) && string.IsNullOrEmpty(account.LastName) ? account.Email : (account.FirstName + " " + account.LastName)),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
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
