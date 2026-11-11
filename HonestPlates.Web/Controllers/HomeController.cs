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
        return View();
    }

    public IActionResult Privacy()
    { 
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

        if (cuisines.Count == 0 && city == "" && state == "")
        {
            ViewBag.Error = "Please pick at least one cuisine or a location.";
            return View("Index");
        }

        try
        {
            RestaurantSearch search = new RestaurantSearch();
            DataTable results = search.Search(string.Join(",", cuisines), city, state);

            if (results.Rows.Count == 0)
                ViewBag.Error = "No restaurants found. Try different filters.";
            else
                ViewBag.Results = results;
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
