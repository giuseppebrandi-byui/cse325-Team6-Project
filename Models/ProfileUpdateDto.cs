using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyMuscleCars.Models
{
    public class ProfileUpdateDto
    {
        [JsonPropertyName("firstName")]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("accountType")]
        public string? AccountType { get; set; }
    }
}
