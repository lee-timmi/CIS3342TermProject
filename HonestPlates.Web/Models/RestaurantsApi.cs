using System.Net.Http.Headers;
using System.Text.Json;
using HonestPlates.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace HonestPlates.Web.Models
{
    // Calls our Restaurants Web API from server-side code (no JavaScript), as the spec requires.
    // Every method throws when the API doesn't answer OK, so the controllers' try/catch shows a friendly message.
    public class RestaurantsApi
    {
        // COMMENT OUT after publishing.
        private string baseUrl = "http://localhost:5256/api/Restaurants/";

        // The API sends "restaurantID"; this lets it fill RestaurantID (capital letters don't matter).
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // GET api/Restaurants/{id}: one restaurant, or RestaurantID 0 if not found.
        public Restaurant GetRestaurant(int id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(baseUrl + id).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Restaurant>(content, jsonOptions);
        }

        // GET api/Restaurants/{id}: one restaurant, or RestaurantID 0 if not found.
        // Came from Prof. Pascucci's CIS3342 site: TeamWebAPI_Example btnFindTeam_Click.
        public List<Restaurant> Search (string cuisines, string city, string state)
        {
            string url = baseUrl + "Search?cuisines=" + Uri.EscapeDataString(cuisines) +
                         "&city=" + Uri.EscapeDataString(city) +
                         "&state=" + Uri.EscapeDataString(state);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<List<Restaurant>>(content, jsonOptions);
        }

        // POST api/Restaurants (JSON body): adds a restaurant and returns it with its new ID (0 = not added).
        // Came from Prof. Pascucci's CIS3342 site: TeamWebAPI_Example2 btnStoreTeam_Click.
        public Restaurant AddRestaurant(Restaurant restaurant)
        {
            string json = JsonSerializer.Serialize(restaurant);
            StringContent body = new StringContent(json);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsync(baseUrl, body).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Restaurant>(content, jsonOptions);
        }

    }
}