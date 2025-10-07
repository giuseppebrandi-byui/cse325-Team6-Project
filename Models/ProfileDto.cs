using System.Text.Json.Serialization;

namespace MyMuscleCars.Models
{
    // DTO used to serialize/deserialize account profile data between server and client
    public class ProfileDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("accountType")]
        public string? AccountType { get; set; }
    }
}
