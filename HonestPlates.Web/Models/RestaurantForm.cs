using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // What the Add / Edit restaurant page posts back.
    // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 401 on validation attributes.
    public class RestaurantForm
    {
        // 0 = adding a new restaurant; otherwise the restaurant being edited
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Enter the restaurant's name.")]
        [StringLength(100, ErrorMessage = "The name can be up to 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the street address.")]
        [StringLength(200, ErrorMessage = "The address can be up to 200 characters.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Enter the city.")]
        [StringLength(100, ErrorMessage = "The city can be up to 100 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter the state.")]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "Use the 2-letter state, like PA.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Enter the ZIP code.")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Enter a 5-digit ZIP code, like 19122.")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Enter the cuisine, like Italian or Thai.")]
        [StringLength(50, ErrorMessage = "The cuisine can be up to 50 characters.")]
        public string Cuisine { get; set; }

        [Required(ErrorMessage = "Describe the restaurant.")]
        [StringLength(2000, ErrorMessage = "The description can be up to 2000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter the restaurant's phone number.")]
        [Phone(ErrorMessage = "Enter a valid phone number, like 215-555-1234.")]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200, ErrorMessage = "The website can be up to 200 characters.")]
        public string Website { get; set; }

        [StringLength(200, ErrorMessage = "Social media can be up to 200 characters.")]
        public string SocialMedia { get; set; }

        [StringLength(100, ErrorMessage = "The owner's name can be up to 100 characters.")]
        public string Owner { get; set; }

        // Weekly hours: index 0 = Sunday ... 6 = Saturday, like TP_RestaurantHours.DayOfWeek.
        // Came from Murach's ASP.NET Core MVC, Chapter 10 Page 373 on posting an array to an action method.
        public bool[] Closed { get; set; } = new bool[7];
        public string[] OpenTime { get; set; } = new string[7];
        public static string[] DayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        public string[] CloseTime { get; set; } = new string[7];
    }
}