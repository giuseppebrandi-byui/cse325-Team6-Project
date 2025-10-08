using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMuscleCars.Data;
using MyMuscleCars.Models;

namespace cse325_Team6_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            // Log all claims for debugging
            try
            {
                foreach (var c in User?.Claims ?? Enumerable.Empty<System.Security.Claims.Claim>())
                {
                    _logger.LogInformation("[AccountController] Claim: {Type} = {ValuePreview}", c.Type, (c.Value ?? string.Empty).Length > 64 ? (c.Value ?? string.Empty).Substring(0, 64) + "..." : c.Value);
                }
            }
            catch { }

            // Resolve email from common claim types. Don't assume Identity.Name is the email (it may be full name).
            var email = User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                        ?? User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("[AccountController] profile requested but no email claim found");
                return Unauthorized();
            }

            _logger.LogInformation("[AccountController] returning profile for {Email}", email);

            var acct = _db.Accounts.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
            if (acct == null)
            {
                _logger.LogInformation("[AccountController] no account row matched email={Email}", email);
                return NotFound();
            }

            _logger.LogInformation("[AccountController] account found: Id={Id}, FirstName={FirstName}, LastName={LastName}, Email={Email}, AccountType={AccountType}",
                acct.Id, acct.FirstName, acct.LastName, acct.Email, acct.AccountType);

            return Ok(new
            {
                firstName = acct.FirstName,
                lastName = acct.LastName,
                email = acct.Email,
                accountType = acct.AccountType
            });
        }

        [HttpPost("profile")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] MyMuscleCars.Models.ProfileUpdateDto update)
        {
            if (update == null) return BadRequest("Invalid payload");

            // Resolve email as in GetProfile
            var email = User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                        ?? User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var acct = _db.Accounts.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
            if (acct == null) return NotFound();

            // If email changed, ensure it's not already taken
            var newEmail = (update.Email ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(newEmail) && !newEmail.Equals(acct.Email, StringComparison.OrdinalIgnoreCase))
            {
                var exists = _db.Accounts.Any(a => a.Email.ToLower() == newEmail.ToLower());
                if (exists)
                {
                    return Conflict(new { message = "Email already in use." });
                }
                acct.Email = newEmail;
            }

            // Allow account type update but restrict to allowed values
            if (!string.IsNullOrEmpty(update.AccountType))
            {
                var allowed = new[] { "Client", "Admin" };
                if (!allowed.Contains(update.AccountType))
                {
                    return BadRequest(new { message = "Invalid account type." });
                }
                acct.AccountType = update.AccountType;
            }

            acct.FirstName = update.FirstName ?? acct.FirstName;
            acct.LastName = update.LastName ?? acct.LastName;

            _db.SaveChanges();

            // If email changed, regenerate JWT cookie with updated claims so user's session reflects new email
            try
            {
                var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                if (config != null)
                {
                    var loginCtrl = new MyMuscleCars.Controllers.LoginController(_db, config);
                    // We cannot call private GenerateJwtToken, so instead generate inline here
                    var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty));
                    var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, acct.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, acct.Email),
                        new System.Security.Claims.Claim("email", acct.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, string.IsNullOrEmpty(acct.FirstName) && string.IsNullOrEmpty(acct.LastName) ? acct.Email : (acct.FirstName + " " + acct.LastName)),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, acct.Id.ToString()),
                        new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString())
                    };

                    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                        issuer: config["Jwt:Issuer"],
                        audience: config["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(2),
                        signingCredentials: creds
                    );

                    var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = HttpContext.Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    };

                    Response.Cookies.Append("jwtToken", tokenString, cookieOptions);
                }
            }
            catch
            {
                // If token regen fails, ignore — user will still be able to use existing cookie until expiry.
            }

            return Ok(new { message = "Profile updated" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] MyMuscleCars.Models.ChangePasswordDto payload)
        {
            if (payload == null) return BadRequest("Invalid payload");

            var email = User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                        ?? User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var acct = _db.Accounts.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
            if (acct == null) return NotFound();

            // Verify current password
            var currentHash = MyMuscleCars.Services.PasswordHasher.HashPassword(payload.CurrentPassword);
            if (acct.Password != currentHash)
            {
                return Unauthorized(new { message = "Current password is incorrect." });
            }

            // Update to new password
            acct.Password = MyMuscleCars.Services.PasswordHasher.HashPassword(payload.NewPassword);
            _db.SaveChanges();

            return Ok(new { message = "Password changed" });
        }
        [HttpPost("delete")]
        [Authorize]
        public IActionResult DeleteAccount()
        {
            var email = User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? User?.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                        ?? User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var acct = _db.Accounts.FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
            if (acct == null) return NotFound();

            // Delete the account
            _db.Accounts.Remove(acct);
            _db.SaveChanges();

            // Clear the JWT cookie to log the user out
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "/",
            };

            Response.Cookies.Append("jwtToken", string.Empty, cookieOptions);

            return Ok(new { message = "Account deleted" });
        }
    }

}
