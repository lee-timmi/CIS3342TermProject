using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using System.Data;

namespace HonestPlates.RestaurantsAPI.Controllers
{
    /*
     * The Restaurants Web API: one job, restaurant data (Tri's Reviews API does reviews).
       Every URL starts with api/Restaurants.
       Came from Prof. Pascucci's CIS3342 site: ASP.NET Core Web API Example (TeamsController, CustomerServiceController).
    */
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class RestaurantsController : Controller
    {
        // GET api/Restaurants/36: one restaurant's details.
        // Returns an empty Restaurant (RestaurantID 0) when there's no such restaurant, like the Teams example.
        // Came from Prof. Pascucci's CIS3342 site: TeamsController Get(int id).

        [HttpGet("{id}")]
        public Restaurant GetRestaurant(int id)
        {
            Restaurant restaurant = new Restaurant();
            restaurant.LoadByID(id);
            return restaurant;
        }

        // Every restaurant that matches, with its rating text and logo, as a list.
        // A blank cuisines/city/state means "any", like the home page search.
        [HttpGet("Search")]
        public List<Restaurant> Search(string cuisines, string city, string state)
        {
            if (cuisines == null)
            {
                cuisines = "";
            }
            if (city == null)
            {
                city = "";
            }
            if (state == null)
            {
                state = "";
            }

            RestaurantSearch search = new RestaurantSearch();
            DataTable results = search.Search(cuisines, city, state);
            search.AddRatings(results);

            List<Restaurant> restaurants = new List<Restaurant>();
            foreach (DataRow row in results.Rows)
            {
                restaurants.Add(Restaurant.FromCardRow(row));
            }

            return restaurants;
        }

        // ------- POST method --------------

        // POST api/Restaurants (JSON body): adds a restaurant for RepresentativeUserID.
        // Returns the restaurant with its new RestaurantID, or RestaurantID 0 if it wasn't added
        // (missing fields, or this rep already has a restaurant).
        [HttpPost]
        public Restaurant AddRestaurant([FromBody] Restaurant restaurant)
        {
            return saveNewRestaurant(restaurant);
        }

        // POST api/Restaurants/Form (form data): the same as above, for callers that send form fields.
        [HttpPost("Form")]
        public Restaurant AddRestaurantForm([FromForm] Restaurant restaurant)
        {
            return saveNewRestaurant(restaurant);
        }

        // ---------- PUT Methods -------------

        // PUT api/Restaurants/36 (JSON body): saves changes to a restaurant.
        // Returns true if it was saved; false if it isn't this rep's restaurant, fields are missing, or nothing changed
        [HttpPost("{id}")]
        public Boolean UpdateRestaurant(int id, [FromBody] Restaurant restaurant)
        {
            return SaveChanges(id, restaurant);
        }

        // PUT api/Restaurants/36/Form (form data): the same as above, for callers that send form fields.
        [HttpPut("{id}/Form")]
        public Boolean UpdateRestaurantForm(int id, [FromForm] Restaurant restaurant)
        {
            return SaveChanges(id, restaurant);
        }

        // PUT api/Restaurants/36/Hours (JSON body): saves all 7 days of hours, only if this rep owns the restaurant.
        [HttpPut("{id}/Hours")]
        public Boolean SaveHours(int id, [FromBody] RestaurantHours hours)
        {
            if (hours == null)
            {
                return false;
            }

            Restaurant restaurant = new Restaurant();
            if (!restaurant.LoadByID(id) || restaurant.RepresentativeUserID != hours.RepresentativeUserID)
            {
                return false;
            }

            for (int i = 0; i < 7; i++)
            {
                restaurant.SaveHours(i, hours.Closed[i], hours.OpenTime[i], hours.CloseTime[i]);
            }
            return true;
        }

        // DELETE api/Restaurants/36/Photos/9?representativeUserID=4: deletes one of the restaurant's photos.
        // Returns true if it was deleted; false if it isn't this rep's restaurant or photo.
        [HttpDelete("{id}/Photos/{imageID}")]
        public Boolean DeletePhoto(int id, int imageID, int representativeUserID)
        {
            RestaurantImage image = new RestaurantImage();
            return image.DeletePhoto(imageID, id, representativeUserID);
        }

        // ------- Methods ---------
        private Restaurant saveNewRestaurant(Restaurant restaurant)
        {
            if (restaurant == null)
            {
                return new Restaurant();
            }

            if (restaurant.RepresentativeUserID <= 0 || IsBlank(restaurant.Name) || IsBlank(restaurant.Street) ||
               IsBlank(restaurant.City) || IsBlank(restaurant.State) || IsBlank(restaurant.Zip) ||
               IsBlank(restaurant.Cuisine) || IsBlank(restaurant.Description) || IsBlank(restaurant.Phone))
            {
                restaurant.RestaurantID = 0;
                return restaurant;
            }

            restaurant.AddRestaurant();    // sets RestaurantID to the new ID, or 0 if the rep already has one
            return restaurant;
        }

        private bool IsBlank(string value)
        {
            return value == null || value.Trim() == "";
        }

        private Boolean SaveChanges(int id, Restaurant restaurant)
        {
            if (restaurant == null || restaurant.RepresentativeUserID <= 0 || IsBlank(restaurant.Name) ||
                IsBlank(restaurant.Street) || IsBlank(restaurant.City) || IsBlank(restaurant.State) ||
                IsBlank(restaurant.Zip) || IsBlank(restaurant.Cuisine) || IsBlank(restaurant.Description) ||
                IsBlank(restaurant.Phone))
            {
                return false;
            }

            restaurant.RestaurantID = id;
            return restaurant.UpdateRestaurant();    // TP_UpdateRestaurant only changes it if RepresentativeUserID owns it
        }
    }

}
