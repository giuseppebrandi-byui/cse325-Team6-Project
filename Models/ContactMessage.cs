using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMuscleCars.Models
{
  [Table("contact_message")]
  public class ContactMessage
  {
    [Key]
    [Column("message_id")]
    public int Id { get; set; }

    [Column("message_name")]

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("message_email")]
    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please use a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Column("message_content")]
    [Required(ErrorMessage = "Please enter a message.")]
    [MinLength(10, ErrorMessage = "Your message must be at least 10 characters.")]
    public string Message { get; set; } = string.Empty;
  }
}