using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    public class ReviewForm
    {
        public int ReviewID { get; set; }

        // Shown on the page only ("Edit your review of ..."); kept in a hidden field for post-backs
        public string RestaurantName { get; set; }

        [Required(ErrorMessage = "Write your review.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Reviews are 10 to 2000 characters.")]
        public string ReviewText { get; set; }

        // Each rating is 1 to 5. No choice posts nothing, so it stays 0 and fails the range.
        [Range(1, 5, ErrorMessage = "Rate the food.")]
        public int FoodRating { get; set; }

        [Range(1, 5, ErrorMessage = "Rate the service.")]
        public int ServiceRating { get; set; }

        [Range(1, 5, ErrorMessage = "Rate the atmosphere.")]
        public int AtmosphereRating { get; set; }

        [Range(1, 5, ErrorMessage = "Rate the price.")]
        public int PriceRating { get; set; }

        // "yyyy-MM-ddTHH:mm" from a date-and-time box. The controller turns it into a DateTime and checks it isn't in the future.
        [Required(ErrorMessage = "Enter the date you visited.")]
        public string VisitDate { get; set; }

        // Photo: HasPhoto only decides if the "Remove photo" box shows
        public bool HasPhoto { get; set; }
        public bool RemovePhoto { get; set; }
    }
}