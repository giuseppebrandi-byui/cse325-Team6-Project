using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cse325_Team6_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            _logger.LogInformation("[AuthController] /api/auth/me requested. IsAuthenticated={IsAuth}", User?.Identity?.IsAuthenticated);
            if (User?.Identity?.IsAuthenticated == true)
            {
                var email = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                _logger.LogInformation("[AuthController] returning user email={Email}", email);
                return Ok(new { email });
            }
            _logger.LogInformation("[AuthController] user not authenticated - returning 401");
            return Unauthorized();
        }
    }
}
