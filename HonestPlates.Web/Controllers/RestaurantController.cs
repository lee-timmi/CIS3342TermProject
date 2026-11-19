using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using HonestPlates.Web.Models;
using System.Data;

namespace HonestPlates.Web.Controllers
{
    public class RestaurantController : Controller
    {
        /* 
           Rep-only pages call this first. It returns null when a Representative is logged in;
           otherwise it returns where to send the user (so the page can't be opened by typing its URL).
           Came from Murach's ASP.NET Core MVC, Chapter 9 Page 327 on working with session state.
           Came from Murach's ASP.NET Core MVC, Chapter 8 Page 306 on using TempData.
        */
        private IActionResult CheckRepresentative()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                TempData["Message"] = "Please login as a Restaurant Representative to open that page.";
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("UserType") != "Representative")
            {
                TempData["Message"] = "Only restaurant representative can open that page.";
                return RedirectToAction("Dashboard", "Account");
            }

            return null;
        }

        /* 
         * Manage (GET): Add (rep has no restaurant yet) or Edit (fills in their restaurant + hours).
                  (POST): Checks the form, then adds or updates the rep's restaurant and saves all 7 days.
         */
        [HttpGet]
        public IActionResult Manage()
        {
            IActionResult blocked = CheckRepresentative();
            if (blocked != null)
            {
                return blocked;
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            RestaurantForm form = new RestaurantForm();

            try
            {
                Restaurant restaurant = new Restaurant();
                DataTable mine = restaurant.GetRestaurantByRep(userID);

                if (mine.Rows.Count > 0)
                {
                    restaurant.LoadByID(Convert.ToInt32(mine.Rows[0]["RestaurantID"]));
                    form.RestaurantID = restaurant.RestaurantID;
                    form.Name = restaurant.Name;
                    form.Street = restaurant.Street;
                    form.City = restaurant.City;
                    form.State = restaurant.State;
                    form.Zip = restaurant.Zip;
                    form.Cuisine = restaurant.Cuisine;
                    form.Description = restaurant.Description;
                    form.Phone = restaurant.Phone;
                    form.Website = restaurant.Website;
                    form.SocialMedia = restaurant.SocialMedia;
                    form.Owner = restaurant.Owner;

                    // TIME columns come back as "11:00:00"; a time box wants "11:00"
                    foreach (DataRow row in restaurant.GetHours().Rows)
                    {
                        int day = Convert.ToInt32(row["DayOfWeek"]);
                        form.Closed[day] = Convert.ToBoolean(row["IsClosed"]);
                        if (row["OpenTime"] != DBNull.Value && row["CloseTime"] != DBNull.Value)
                        {
                            form.OpenTime[day] = DateTime.Parse(row["OpenTime"].ToString()).ToString("HH:mm");
                            form.CloseTime[day] = DateTime.Parse(row["CloseTime"].ToString()).ToString("HH:mm");
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load your restaurant right now. Please try again later.";
            }

            return View(form);
        }

        [HttpPost]
        public IActionResult Manage(RestaurantForm form)
        {
            IActionResult blocked = CheckRepresentative();
            if (blocked != null)
            {
                return blocked;
            }

            // Rules the attributes can't check: an open day needs both times.
            // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 407 on setting validation messages in code.
            for (int i = 0; i < 7; i++)
            {
                if (!form.Closed[i] && (form.OpenTime[i] == null || form.OpenTime[i] == "" ||
                    form.CloseTime[i] == null || form.CloseTime[i] == ""))
                {
                    ModelState.AddModelError("", RestaurantForm.DayNames[i] + " needs an open and close time, or check Closed.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));

            try
            {
                Restaurant restaurant = new Restaurant();
                restaurant.RepresentativeUserID = userID;
                restaurant.Name = form.Name;
                restaurant.Street = form.Street;
                restaurant.City = form.City;
                restaurant.State = form.State.ToUpper();
                restaurant.Zip = form.Zip;
                restaurant.Cuisine = form.Cuisine;
                restaurant.Description = form.Description;
                restaurant.Phone = form.Phone;
                restaurant.Website = form.Website;
                restaurant.SocialMedia = form.SocialMedia;
                restaurant.Owner = form.Owner;

                // double check the representative id
                DataTable mine = restaurant.GetRestaurantByRep(userID);

                // Saving goes through our Restaurants Web API: PUT (form data) to edit, POST (JSON) to add.
                RestaurantsApi api = new RestaurantsApi();
                bool saved;
                if (mine.Rows.Count > 0)
                {
                    restaurant.RestaurantID = Convert.ToInt32(mine.Rows[0]["RestaurantID"]);
                    saved = api.UpdateRestaurant(restaurant);
                }
                else
                {
                    // The API sends the restaurant back with its new RestaurantID (0 = not added).
                    // Came from Prof. Pascucci's CIS3342 site: TeamWebAPI_Example2 (POST an object as JSON).
                    Restaurant added = api.AddRestaurant(restaurant);
                    restaurant.RestaurantID = added.RestaurantID;
                    saved = added.RestaurantID > 0;
                }

                if (!saved)
                {
                    ModelState.AddModelError("", "We couldn't save your restaurant. Please try again.");
                    return View(form);
                }

                // All 7 days in one API call (PUT api/Restaurants/{id}/Hours, JSON)
                RestaurantHours hours = new RestaurantHours();
                hours.RepresentativeUserID = userID;
                hours.Closed = form.Closed;
                hours.OpenTime = form.OpenTime;
                hours.CloseTime = form.CloseTime;

                if (!api.SaveHours(restaurant.RestaurantID, hours))
                {
                    ModelState.AddModelError("", "Your details were saved, but the hours weren't. Please try again.");
                    return View(form);
                }

                TempData["Message"] = restaurant.Name + " was saved.";
                return RedirectToAction("Dashboard", "Account");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't save your restaurant right now. Please try again later.");
                return View(form);
            }
        }

        // GET: the rep's own menu (left) and an OpenMenu lookup to use as a reference (right).
        // Nothing from OpenMenu is saved (OpenMenu API terms 3.2.1).
        [HttpGet]
        public IActionResult Menu(string search, string zip, string openMenuID)
        {
            IActionResult blocked = CheckRepresentative();
            if (blocked != null)
            {
                return blocked;
            }

            try
            {
                if (!LoadMenuPage(search, zip, openMenuID))
                {
                    TempData["Message"] = "Add your restaurant first, then you can enter its menu.";
                    return RedirectToAction("Manage");
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load your menu right now. Please try again later.";
            }

            return View(new MenuItemForm());
        }

        // POST /Restaurant/AddMenuItem: adds one item the rep typed in.
        [HttpPost]
        public IActionResult AddMenuItem(MenuItemForm form)
        {
            IActionResult blocked = CheckRepresentative();
            if (blocked != null)
            {
                return blocked;
            }

            // Price is optional; if given it must fit DECIMAL(6,2).
            // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 407 on setting validation messages in code.
            decimal price = 0;
            bool hasPrice = false;
            if (form.Price != null && form.Price.Trim() != "")
            {
                if (!decimal.TryParse(form.Price.Trim().Replace("$", ""), out price) || price < 0 || price > 9999.99m)
                {
                    ModelState.AddModelError("Price", "Enter a price like 7.49 (up to 9999.99), or leave it blank.");
                }
                else
                {
                    hasPrice = true;
                }
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));

            try
            {
                if (!ModelState.IsValid)
                {
                    LoadMenuPage(null, null, null);
                    return View("Menu", form);
                }

                Restaurant restaurant = new Restaurant();
                DataTable mine = restaurant.GetRestaurantByRep(userID);
                if (mine.Rows.Count == 0)
                {
                    return RedirectToAction("Manage");
                }

                MenuItem item = new MenuItem();
                item.RestaurantID = Convert.ToInt32(mine.Rows[0]["RestaurantID"]);
                item.Section = form.Section.Trim();
                item.ItemName = form.ItemName.Trim();
                item.Description = form.Description;
                item.Price = price;
                item.HasPrice = hasPrice;

                if (item.AddMenuItem(userID))
                {
                    TempData["Message"] = item.ItemName + " was added to your menu.";
                }
                else
                {
                    TempData["Message"] = "We couldn't add that item. Please try again.";
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "We couldn't add that item right now. Please try again later.";
            }

            return RedirectToAction("Menu");
        }

        // POST /Restaurant/DeleteMenuItem/12: removes one item (only if it's on this rep's menu).
        [HttpPost]
        public IActionResult DeleteMenuItem(int id)
        {
            IActionResult blocked = CheckRepresentative();
            if (blocked != null)
            {
                return blocked;
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            try
            {
                MenuItem item = new MenuItem();
                if (item.DeleteMenuItem(id, userID))
                {
                    TempData["Message"] = "The item was removed from your menu.";
                }
                else
                {
                    TempData["Message"] = "We couldn't find that item.";
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "We couldn't remove that item right now. Please try again later.";
            }

            return RedirectToAction("Menu");
        }


        // Restaurant Profile 5 represents "Restaurant #5"
        public IActionResult Profile(int id)
        {
            // Missing or non-number IDs arrives as 0
            if (id <= 0)
            {
                ViewBag.Error = "That restaurant link is invalid. Please search again.";
                return View();
            }

            Restaurant restaurant = null;
            try
            {
                // The restaurant's details come from our Restaurants Web API (GET api/Restaurants/{id}).
                RestaurantsApi api = new RestaurantsApi();
                restaurant = api.GetRestaurant(id);

                if (restaurant.RestaurantID == 0)
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

            // Reviews + the rating summary. Separate try, so if they fail the details still show.
            // Came from Project 2: RestaurantProfile.aspx.cs loading reviews in its own try/catch.
            try
            {
                ViewBag.Rating = restaurant.GetRatings();
                Review review = new Review();
                ViewBag.Reviews = review.GetReviewsForRestaurant(id);
            }
            catch (Exception)
            {
                ViewBag.ReviewsError = "Unable to load reviews right now. Please try again later.";
            }

            // Photos: the gallery, plus the Profile photo for the banner and the Logo.
            // Came from Project 2: ShowPhotos with RestaurantImage.FindImageID.
            try
            {
                RestaurantImage images = new RestaurantImage();
                DataTable photos = images.GetImagesForRestaurant(id);
                ViewBag.Photos = photos;
                ViewBag.ProfilePhotoID = images.FindImageID(photos, "Profile");
                ViewBag.LogoID = images.FindImageID(photos, "Logo");
            }
            catch (Exception)
            {

            }

            return View(restaurant);
        }

        // /Restaurant/Photo/12 sends restaurant photo #12 as an image (logos, the profile photo, the gallery).
        // Came from Murach's ASP.NET Core MVC, Chapter 8 Page 283 on returning ActionResult objects (FileContentResult).
        public IActionResult Photo(int id)
        {
            try
            {
                RestaurantImage image = new RestaurantImage();
                if (image.LoadImage(id))
                {
                    return File(image.ImageData, image.ContentType);
                }
            }
            catch (Exception)
            {
                // Fall through not found.
            }
            return NotFound();
        }

        // /Restaurant/ReviewPhoto/40 sends review #40's photo as an image.
        // Came from Murach's ASP.NET Core MVC, Chapter 8 Page 283 on returning ActionResult objects (FileContentResult).
        public IActionResult ReviewPhoto(int id)
        {
            try
            {
                Review review = new Review();
                if (review.LoadImage(id))
                {
                    return File(review.ImageData, review.ImageType);
                }
            }
            catch (Exception)
            {
                // Fall through not found.
            }
            return NotFound();
        }

        private bool LoadMenuPage(string search, string zip, string openMenuID)
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
            Restaurant restaurant = new Restaurant();
            DataTable mine = restaurant.GetRestaurantByRep(userID);
            if (mine.Rows.Count == 0)
            {
                return false;
            }

            restaurant.LoadByID(Convert.ToInt32(mine.Rows[0]["RestaurantID"]));
            ViewBag.RestaurantName = restaurant.Name;

            MenuItem menu = new MenuItem();
            ViewBag.MenuItems = menu.GetMenuItems(restaurant.RestaurantID);

            // The lookup boxes start with the rep's own restaurant name and ZIP
            if (search == null)
            {
                ViewBag.Search = restaurant.Name;
            }
            else
            {
                ViewBag.Search = search;
            }
            if (zip == null)
            {
                ViewBag.Zip = restaurant.Zip;
            }
            else
            {
                ViewBag.Zip = zip;
            }
            ViewBag.Sandbox = OpenMenuApi.UseSandbox;

            // OpenMenu lookup, only when the rep asked for it. Its own try/catch, so the rest of the page still loads.
            OpenMenuApi openMenu = new OpenMenuApi();
            try
            {
                if (search != null)
                {
                    ViewBag.Found = openMenu.FindRestaurants(search, ViewBag.Zip);
                }
                if (openMenuID != null)
                {
                    ViewBag.OpenMenu = openMenu.GetMenu(openMenuID);
                }
            }
            catch (Exception ex)
            {
                ViewBag.LookupError = "The OpenMenu lookup isn't working right now. " + ex.Message;
            }

            return true;
        }
    }
}