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
    }
}
