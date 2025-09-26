using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMuscleCars.Models
{
    [Table("inventory")]
    public class Inventory
    {
        [Key]
        [Column("inv_id")]
        public int InvId { get; set; }

        [Column("inv_make")]
        public string InvMake { get; set; } = string.Empty;

        [Column("inv_model")]
        public string InvModel { get; set; } = string.Empty;

        [Column("inv_year")]
        public string InvYear { get; set; } = string.Empty;

        [Column("inv_description")]
        public string InvDescription { get; set; } = string.Empty;

        [Column("inv_image")]
        public string InvImage { get; set; } = string.Empty;

        [Column("inv_thumbnail")]
        public string InvThumbnail { get; set; } = string.Empty;

        [Column("inv_price")]
        public decimal InvPrice { get; set; }

        [Column("inv_miles")]
        public int InvMiles { get; set; }

        [Column("inv_color")]
        public string InvColor { get; set; } = string.Empty;

        [Column("make_id")]
        public int MakeId { get; set; }

        public Make? MakeRef { get; set; }  // Navigation property
    }
}
