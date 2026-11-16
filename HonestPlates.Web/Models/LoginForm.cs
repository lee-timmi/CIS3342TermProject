using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // What the Login form collects.
    // Came from Murach's ASP.NET Core MVC, Chapter 16 Page 675 on the Login view model.
    public class LoginForm
    {
        [Required(ErrorMessage = "Enter your username.")]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [StringLength(100)]
        public string? Password { get; set; }

        // "Remember Me": keeps the username in an encrypted cookie for next time
        public bool RememberMe { get; set; }
    }
}
