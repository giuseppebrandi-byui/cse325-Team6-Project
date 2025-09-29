using System.ComponentModel.DataAnnotations;

namespace cse325_Team6_Project.Models
{
  public class ContactMessage
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please use a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a message.")]
    [MinLength(10, ErrorMessage = "Your message must be at least 10 characters.")]
    public string Message { get; set; } = string.Empty;
  }
}