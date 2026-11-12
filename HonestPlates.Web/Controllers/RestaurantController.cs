using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;

namespace HonestPlates.Web.Controllers
{
    public class RestaurantController : Controller
    {
        // Restaurant Profile 5 represents "Restaurant #5"
        public IActionResult Profile(int id)
        {
            // Missing or non-number IDs arrives as 0
            if (id <= 0)
            {
                ViewBag.Error = "That restaurant link is invalid. Please search again.";
                return View();
            }

            Restaurant restaurant = new Restaurant();
            try
            {
                if (!restaurant.LoadByID(id))
                {
                    ViewBag.Error = "Unable to find restaurant. It may have been removed.";
                    return View();
                }
                ViewBag.Hours = restaurant.GetHours();
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load restaurant. Try again later.";
                return View();
            }

            return View(restaurant);
        }
    }
}