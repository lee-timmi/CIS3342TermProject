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

        // PUT api/Restaurants/{id}/Form (form data): saves changes to the rep's restaurant. Returns true if saved.
        // This is our "form data" version; AddRestaurant above is the JSON version.
        public bool UpdateRestaurant(Restaurant restaurant)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("RepresentativeUserID", restaurant.RepresentativeUserID.ToString());
            fields.Add("Name", Text(restaurant.Name));
            fields.Add("Street", Text(restaurant.Street));
            fields.Add("City", Text(restaurant.City));
            fields.Add("State", Text(restaurant.State));
            fields.Add("Zip", Text(restaurant.Zip));
            fields.Add("Cuisine", Text(restaurant.Cuisine));
            fields.Add("Description", Text(restaurant.Description));
            fields.Add("Phone", Text(restaurant.Phone));
            fields.Add("Website", Text(restaurant.Website));
            fields.Add("SocialMedia", Text(restaurant.SocialMedia));
            fields.Add("Owner", Text(restaurant.Owner));
            FormUrlEncodedContent body = new FormUrlEncodedContent(fields);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PutAsync(baseUrl + restaurant.RestaurantID + "/Form", body).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<bool>(content);
        }

        // PUT api/Restaurants/{id}/Hours (JSON body): saves all 7 days. Returns true if saved. 
        public bool SaveHours(int restaurantID, RestaurantHours hours)
        {
            string json = JsonSerializer.Serialize(hours);
            StringContent body = new StringContent(json);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PutAsync(baseUrl + restaurantID + "/Hours", body).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<bool>(content);
        }

        // DELETE api/Restaurants/{id}/Photos/{imageID}?representativeUserID=...: for Tri's photo manager.
        public bool DeletePhoto(int restaurantID, int imageID, int representativeUserID)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.DeleteAsync(baseUrl + restaurantID + "/Photos/" + imageID +
                                                              "?representativeUserID=" + representativeUserID).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("The Restaurants API answered " + (int)response.StatusCode);
            }

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<bool>(content);
        }

        // A blank instead of null, so every form field is sent
        private string Text(string value)
        {
            if (value == null)
            {
                return "";
            }
            return value;
        }
    }
}