using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // 3 forms: what you forgot, security questions, and new password
    // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 401 on validation attributes.

    // Page 1: What did user forget, and email (username path) or username (password path)
    public class ForgotForm
    {
        [Required(ErrorMessage = "Choose whether you forgot username or password.")]
        public string? Mode { get; set; } // username or password

        [Required(ErrorMessage = "Enter your email or username.")]
        [StringLength(100)]
        public string? Lookup { get; set; } // email (forgot username), or username (forgot password)
    }

    // Page 2: Answer randomly selected security question
    public class QuestionForm
    {
        public string? QuestionText { get; set; } // shown on the page; not used for checking

        [Required(ErrorMessage = "Enter your answer.")]
        [StringLength(100)]
        public string? Answer { get; set; }
    }

    // Page 3 (forgot password): new password, typed twice.
    public class ResetPasswordForm
    {
        [Required(ErrorMessage = "Choose a new password.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Passwords are at least 8-characters.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Type your password again.")]
        [Compare("NewPassword", ErrorMessage = "The passwords don't match.")]
        public string? ConfirmPassword { get; set; }
    }
}