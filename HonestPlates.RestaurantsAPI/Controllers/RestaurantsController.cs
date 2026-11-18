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
    }

}
