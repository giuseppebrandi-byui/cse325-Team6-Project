using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyMuscleCars.Models
{
    public class ChangePasswordDto
    {
        [JsonPropertyName("currentPassword")]
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [JsonPropertyName("newPassword")]
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
