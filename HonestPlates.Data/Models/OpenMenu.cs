using System.Text.Json.Serialization;

namespace HonestPlates.Data.Models
{
    // C# classes shaped like the OpenMenu API's JSON, so System.Text.Json can turn its replies into objects.
    // [JsonPropertyName] maps OpenMenu's snake_case names (restaurant_name) to our C# names (RestaurantName).
    // These are only for SHOWING OpenMenu data (reference mode); it is never saved (OpenMenu API terms 3.2.1).

    // One restaurant from location.php (result.restaurants[])
    public class OpenMenuRestaurant
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("restaurant_name")]
        public string RestaurantName { get; set; }

        [JsonPropertyName("address_1")]
        public string Address { get; set; }

        [JsonPropertyName("city_town")]
        public string City { get; set; }

        [JsonPropertyName("state_province")]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }

        [JsonPropertyName("cuisine_type_primary")]
        public string Cuisine { get; set; }
    }

    // One menu from restaurant.php (result.menus[]), e.g. "Lunch"
    public class OpenMenuMenu
    {
        [JsonPropertyName("menu_name")]
        public string MenuName { get; set; }

        [JsonPropertyName("menu_groups")]
        public List<OpenMenuGroup> Groups { get; set; } = new List<OpenMenuGroup>();
    }

    // One group inside a menu, e.g. "Burgers"
    public class OpenMenuGroup
    {
        [JsonPropertyName("group_name")]
        public string GroupName { get; set; }

        [JsonPropertyName("menu_items")]
        public List<OpenMenuItem> Items { get; set; } = new List<OpenMenuItem>();
    }

    // One dish. Price is often missing (null or ""), so it stays a string.
    public class OpenMenuItem
    {
        [JsonPropertyName("menu_item_name")]
        public string Name { get; set; }

        [JsonPropertyName("menu_item_description")]
        public string Description { get; set; }

        [JsonPropertyName("menu_item_price")]
        public string Price { get; set; }

        // "$5.99", or "" when OpenMenu has no price
        public string PriceText()
        {
            if (Price == null || Price == "")
            {
                return "";
            }
            return "$" + Price;
        }
    }
}