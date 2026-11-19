using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // What the Make a reservation page posts back. Lengths match TP_Reservations.
    public class ReservationForm
    {
        public int RestaurantID { get; set; }

        // Shown on the page only ("Reserve a table at ..."); kept in a hidden field for post-backs
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Enter your name.")]
        [StringLength(100, ErrorMessage = "Your name can be up to 100 characters.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Enter a phone number so the restaurant can reach you.")]
        [Phone(ErrorMessage = "Enter a valid phone number, like 215-555-1234.")]
        [StringLength(20)]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Enter your email for the confirmation.")]
        [EmailAddress(ErrorMessage = "Enter a valid email, like name@example.com.")]
        [StringLength(100)]
        public string ContactEmail { get; set; }

        // "yyyy-MM-dd" from a date box and "HH:mm" from a time box.
        // The controller joins them and checks the time is in the future and the restaurant is open that day.
        [Required(ErrorMessage = "Pick a date.")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Pick a time.")]
        public string Time { get; set; }

        // Came from Murach's ASP.NET Core MVC, Chapter 2 Page 71 on the Range attribute.
        [Range(1, 20, ErrorMessage = "Party size is 1 to 20. For bigger groups, call the restaurant.")]
        public int PartySize { get; set; } = 2;

        [StringLength(500, ErrorMessage = "Special requests can be up to 500 characters.")]
        public string SpecialRequests { get; set; }
    }
}