using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using System.Data;

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
        public IActionResult Photo (int id)
        {
            try
            {
                RestaurantImage image = new RestaurantImage();
                if (image.LoadImage(id))
                {
                    return File(image.ImageData, image.ContentType);
                }
            } catch (Exception)
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
    }
}