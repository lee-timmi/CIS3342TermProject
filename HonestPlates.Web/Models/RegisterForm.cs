using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    public class RegisterForm
    {
        // Came from CoreMVC textbook, Chapter 2 Page 71 on Validation Attributes

        [Required(ErrorMessage = "Choose Reviewer or Restaurant Representative.")]
        public string? UserType { get; set; }

        [Required(ErrorMessage = "Enter your first name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Ener your last name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email, like name@exmaple.com")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number, like 123-456-7890")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Choose a username.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Usernames are 4 to 50 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Choose a password.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Passwords are at least 8 characters.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Type your password again.")]
        [Compare("Password", ErrorMessage = "The password don't match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Pick security question 1.")]
        public string? Question1 { get; set; }

        [Required(ErrorMessage = "Answer security question 1.")]
        [StringLength(100)]
        public string? Answer1 { get; set; }

        [Required(ErrorMessage = "Pick security question 2.")]
        public string? Question2 { get; set; }

        [Required(ErrorMessage = "Answer security question 2.")]
        [StringLength(100)]
        public string? Answer2 { get; set; }

        [Required(ErrorMessage = "Pick security question 3.")]
        public string? Question3 { get; set; }

        [Required(ErrorMessage = "Answer security question 3.")]
        [StringLength(100)]
        public string? Answer3 { get; set; }

        // Checkbox for Remember Username (saves in cookie)
        public bool RememberUsername { get; set; }
    }
}
