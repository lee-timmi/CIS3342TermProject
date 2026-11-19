using System.ComponentModel.DataAnnotations;

namespace HonestPlates.Web.Models
{
    // What the "Add item" form on the Menu page posts back. Lengths match TP_MenuItems.
    public class MenuItemForm
    {
        [Required(ErrorMessage = "Enter a section, like Burgers or Drinks.")]
        [StringLength(100, ErrorMessage = "The section can be up to 100 characters.")]
        public string Section { get; set; }

        [Required(ErrorMessage = "Enter the item's name.")]
        [StringLength(150, ErrorMessage = "The name can be up to 150 characters.")]
        public string ItemName { get; set; }

        [StringLength(500, ErrorMessage = "The description can be up to 500 characters.")]
        public string Description { get; set; }

        // Optional. A string so a blank box is allowed; the controller checks it's a real price.
        public string Price { get; set; }
    }
}