using System.Text.Json;
using HonestPlates.Data;
using HonestPlates.Data.Models;

namespace HonestPlates.Web.Models
{
    // Calls the OpenMenu API (our third-party API) from server-side code. Reference mode only:
    // results are shown on the page and never saved (OpenMenu API terms 3.2.1).
    public class OpenMenuApi
    {
        // true = OpenMenu's free "sample" restaurant: no key, and no calls used from our ~100 per month.
        // Set to false only for the final real check and the demo.
        public static bool UseSandbox = true;

        private string baseUrl = "https://www.openmenu.com/api/v2/";

        // Restaurants on OpenMenu that match a name in a ZIP code (location.php -> result.restaurants)
        public List<OpenMenuRestaurant> FindRestaurants(string name, string zip)
        {
            string url = baseUrl + "location.php?s=sample";
            if (!UseSandbox)
            {
                url = baseUrl + "location.php?key=" + Uri.EscapeDataString(ApiKeys.OpenMenu) +
                      "&s=" + Uri.EscapeDataString(name) +
                      "&postal_code=" + Uri.EscapeDataString(zip) + "&country=US";
            }

            List<OpenMenuRestaurant> restaurants = new List<OpenMenuRestaurant>();
            JsonElement result = GetResult(url);
            if (result.ValueKind == JsonValueKind.Object && result.TryGetProperty("restaurants", out JsonElement list))
            {
                restaurants = list.Deserialize<List<OpenMenuRestaurant>>();
            }
            return restaurants;
        }

        // One restaurant's menus, groups and dishes (restaurant.php -> result.menus)
        public List<OpenMenuMenu> GetMenu(string openMenuID)
        {
            string url = baseUrl + "restaurant.php?id=sample";
            if (!UseSandbox)
            {
                url = baseUrl + "restaurant.php?key=" + Uri.EscapeDataString(ApiKeys.OpenMenu) +
                      "&id=" + Uri.EscapeDataString(openMenuID);
            }

            List<OpenMenuMenu> menus = new List<OpenMenuMenu>();
            JsonElement result = GetResult(url);
            if (result.ValueKind == JsonValueKind.Object && result.TryGetProperty("menus", out JsonElement list))
            {
                menus = list.Deserialize<List<OpenMenuMenu>>();
            }
            return menus;
        }

        // Sends the GET and returns the "result" part of OpenMenu's reply: {"response":{"api":{...},"result":{...}}}.
        // An empty result (status 204) comes back as an empty element; errors throw, so the controller's try/catch shows a message.
        private JsonElement GetResult(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("OpenMenu answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            JsonDocument document = JsonDocument.Parse(content);
            JsonElement reply = document.RootElement.GetProperty("response");

            int status = reply.GetProperty("api").GetProperty("status").GetInt32();
            if (status == 204)
            {
                return new JsonElement();     // no matches
            }
            if (status == 429)
            {
                throw new Exception("OpenMenu's free call limit has been used up.");
            }
            if (status != 200)
            {
                throw new Exception("OpenMenu answered status " + status);
            }

            return reply.GetProperty("result");
        }
    }
}