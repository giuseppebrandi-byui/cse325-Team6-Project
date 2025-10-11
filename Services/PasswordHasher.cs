using System.Security.Cryptography;
using System.Text;

namespace MyMuscleCars.Services
{
    public static class PasswordHasher
    {   
        // Simple SHA256 hash for password
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password ?? string.Empty);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
