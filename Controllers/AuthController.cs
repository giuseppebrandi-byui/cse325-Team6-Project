using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Auth Controller handles authentication status and logout
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
        // GET api/auth/me
        // Returns the email of the currently authenticated user, or 401 if not authenticated
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            _logger.LogInformation("[AuthController] /api/auth/me requested. IsAuthenticated={IsAuth}", User?.Identity?.IsAuthenticated);
            try
            {
                if (Request?.Cookies?.Count > 0)
                {
                    foreach (var c in Request.Cookies)
                    {
                        var val = c.Value ?? string.Empty;
                        var display = val.Length > 8 ? val.Substring(0, 6) + "..." : val;
                        _logger.LogInformation("[AuthController] Cookie: {Name}={ValuePreview}", c.Key, display);
                    }
                }
                else
                {
                    _logger.LogInformation("[AuthController] No request cookies present");
                }

                // Also log Authorization header if present
                if (Request?.Headers != null && Request.Headers.ContainsKey("Authorization"))
                {
                    var auth = Request.Headers["Authorization"].ToString();
                    _logger.LogInformation("[AuthController] Authorization header present: {AuthPreview}", auth.Length > 16 ? auth.Substring(0, 16) + "..." : auth);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[AuthController] failed to log request cookies/headers");
            }
            if (User?.Identity?.IsAuthenticated == true)
            {
                var email = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                _logger.LogInformation("[AuthController] returning user email={Email}", email);
                return Ok(new { email });
            }
            _logger.LogInformation("[AuthController] user not authenticated - returning 401");
            return Unauthorized();
        }
        // POST api/auth/logout
        // Logs out the current user by clearing the jwtToken cookie
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("[AuthController] logout requested");
            try
            {
                // Overwrite the cookie with an expired cookie to instruct the browser to remove it
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    Path = "/",
                };

                Response.Cookies.Append("jwtToken", string.Empty, cookieOptions);
                _logger.LogInformation("[AuthController] cleared jwtToken cookie");
                return Ok(new { message = "Logged out" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController] error during logout");
                return StatusCode(500, "Failed to logout");
            }
        }
        // GET api/auth/logout
        // Logs out the current user by clearing the jwtToken cookie and redirects to the specified URL (default: /)
        [HttpGet("logout")]
        public IActionResult LogoutGet([FromQuery] string? redirect = "/")
        {
            _logger.LogInformation("[AuthController] logout (GET) requested, redirect={Redirect}", redirect);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    Path = "/",
                };

                Response.Cookies.Append("jwtToken", string.Empty, cookieOptions);
                _logger.LogInformation("[AuthController] cleared jwtToken cookie (GET)");

                // Redirect the browser to the provided redirect location (default: home)
                return Redirect(redirect ?? "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController] error during logout (GET)");
                return StatusCode(500, "Failed to logout");
            }
        }
    }
}
