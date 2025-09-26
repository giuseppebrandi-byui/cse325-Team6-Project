using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMuscleCars.Models
{
    [Table("account")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        public int Id { get; set; }

        [Column("account_firstname")]
        public string FirstName { get; set; } = string.Empty;

        [Column("account_lastname")]
        public string LastName { get; set; } = string.Empty;

        [Column("account_email")]
        public string Email { get; set; } = string.Empty;

        [Column("account_password")]
        public string Password { get; set; } = string.Empty;

        [Column("account_type")]
        public string AccountType { get; set; } = "Client";
    }
}
