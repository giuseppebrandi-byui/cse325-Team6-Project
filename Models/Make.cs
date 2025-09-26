using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyMuscleCars.Models
{
    [Table("make")]
    public class Make
    {
        [Key]
        [Column("make_id")]
        public int Id { get; set; }

        [Column("make_name")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
