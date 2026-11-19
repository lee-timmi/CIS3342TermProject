using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HonestPlates.Web.Models;
using HonestPlates.Data.Models;

using System.Data;
using Microsoft.Data.SqlClient;
using HonestPlates.Data;
using System.Security.Cryptography.X509Certificates;

namespace HonestPlates.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        LoadSearchLists();
        LoadTopRated();
        LoadHistory();
        return View();
    }

    public IActionResult Search(List<string> cuisines, string location)
    {
        if (cuisines == null) cuisines = new List<string>();
        if (location == null) location = "";

        // location looks like "Philadelphia|PA" (one city) or "|PA" (the whole state)
        string city = "";
        string state = "";
        if (location != "")
        {
            string[] parts = location.Split('|');
            if (parts.Length == 2)
            {
                city = parts[0];
                state = parts[1];
            }
        }

        // Keep what the user picked, so the form doesn't reset
        ViewBag.SelectedCuisines = cuisines;
        ViewBag.SelectedLocation = location;
        LoadSearchLists();
        LoadHistory();

        if (cuisines.Count == 0 && city == "" && state == "")
        {
            ViewBag.Error = "Please pick at least one cuisine or a location.";
            return View("Index");
        }

        try
        {
            // The search runs through our Restaurants Web API (GET api/Restaurants/Search).
            RestaurantsApi api = new RestaurantsApi();
            List<Restaurant> results = api.Search(string.Join(",", cuisines), city, state);

            if (results.Count == 0)
            {
                ViewBag.Error = "No restaurants found. Try different filters.";
            }
            else
            {
                ViewBag.Results = results;
            }
        }
        catch (Exception)
        {
            ViewBag.Error = "The search isn't working right now. Please try again later.";
        }

        return View("Index");
    }

    // Fills the cuisine checkboxes and the city/state dropdowns
    private void LoadSearchLists()
    {
        try
        {
            RestaurantSearch search = new RestaurantSearch();
            ViewBag.Cuisines = search.GetCuisines();
            ViewBag.Locations = search.GetLocations();
        }
        catch (Exception)
        {
            ViewBag.Cuisines = new List<String>();
            ViewBag.Location = new DataTable();
            ViewBag.Error = "Unable to load the search options. Please try again later.";
        }
    }

    private void LoadTopRated()
    {
        try
        {
            RestaurantSearch search = new RestaurantSearch();

            DataTable topRated = search.GetTopRated("Philadelphia", "PA", 6);
            List<Restaurant> cards = new List<Restaurant>();
            foreach (DataRow row in topRated.Rows)
            {
                cards.Add(Restaurant.FromCardRow(row));
            }
            ViewBag.TopRated = cards;
        }
        catch (Exception)
        {

        }
    }

    private void LoadHistory()
    {
        if (HttpContext.Session.GetInt32("UserID") == null)
        {
            return;
        }

        try
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            ViewBag.History = UserHistory.Load(userID);
        }
        catch (Exception)
        {
            // No history section if it can't load; the rest of the page still works
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Saves the current search to the user's history (serialization), then goes back to the same results.
    [HttpPost]
    public IActionResult SaveSearch(List<string> cuisines, string location)
    {
        if (HttpContext.Session.GetInt32("UserID") == null)
        {
            TempData["Message"] = "Log in to save searches.";
            return RedirectToAction("Login", "Account");
        }
        if (cuisines == null) cuisines = new List<string>();
        if (location == null) location = "";

        try
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            UserHistory history = UserHistory.Load(userID);
            string error = history.AddSearch(cuisines, location);

            if (error == "")
            {
                history.Save(userID);
                TempData["SaveSearchMessage"] = "Search saved. You'll find it on the home page.";
            }
            else
            {
                TempData["SaveSearchMessage"] = error;
            }
        }
        catch (Exception)
        {
            TempData["SaveSearchMessage"] = "We couldn't save your search right now. Please try again later.";
        } 
        
        // Back to the same results
        SavedSearch current = new SavedSearch();
        current.Cuisines = cuisines;
        current.Location = location;
        return Redirect(Url.Action("Search") + current.QueryString());
    }

    // Removes one saved search (by its position in the list), then back to the home page.
    [HttpPost]
    public IActionResult RemoveSearch(int index)
    {
        if (HttpContext.Session.GetInt32("UserID") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            UserHistory history = UserHistory.Load(userID);
            if (history.RemoveSearch(index))
            {
                history.Save(userID);
                TempData["HistoryMessage"] = "Saved search removed.";
            }
            else
            {
                TempData["HistoryMessage"] = "That saved search doesn't exist anymore.";
            }
        }
        catch (Exception)
        {
            TempData["HistoryMessage"] = "We couldn't remove that search right now. Please try again later.";
        }

        return RedirectToAction("Index");
    }
}
