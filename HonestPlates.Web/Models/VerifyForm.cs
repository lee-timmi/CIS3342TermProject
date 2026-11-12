using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // What the Verify form collects.
    // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 401 on validation attributes.
    public class VerifyForm
    {
        [Required(ErrorMessage = "Enter your username.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Enter the 6-digit code from your email.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The code is 6 digits.")]
        public string? Code { get; set; }
    }
}