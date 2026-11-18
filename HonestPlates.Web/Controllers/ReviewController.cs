using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using HonestPlates.Web.Models;

namespace HonestPlates.Web.Controllers
{
    public class ReviewController : Controller
    {
        // Pages for logged-in users call this first. It returns null when someone is logged in;
        // otherwise it sends them to Log In (so the page can't be opened by typing its URL).
        public IActionResult CheckLoggedIn()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                TempData["Message"] = "Please log in to manage your reviews.";
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        // ---- GET methods ------
        [HttpGet] // Edit form; filled in with user's review
        public IActionResult Edit (int id)
        {
            IActionResult blocked = CheckLoggedIn();
            if (blocked != null)
            {
                return blocked;
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));

            try
            {
                Review review = new Review();
                if (!review.LoadForEdit(id, userID))
                {
                    TempData["Message"] = "We couldn't find that review.";
                    return RedirectToAction("Dashboard", "Action");
                }

                ReviewForm form = new ReviewForm();
                form.ReviewID = review.ReviewID;
                form.RestaurantName = review.RestaurantName;
                form.ReviewText = review.ReviewText;
                form.FoodRating = review.FoodRating;
                form.ServiceRating = review.ServiceRating;
                form.AtmosphereRating = review.AtmosphereRating;
                form.PriceRating = review.PriceRating;
                form.HasPhoto = review.HasPhoto;

                // A date-and-time box wants "yyyy-MM-ddTHH:mm"; older reviews with no visit date stay blank
                if (review.VisitDateTime != DateTime.MinValue)
                {
                    form.VisitDate = review.VisitDateTime.ToString("yyyy-MM-ddTHH:mm");
                }

                return View(form);
            }
            catch (Exception)
            {
                TempData["Message"] = "Unable to load that review right now. Please try again later.";
                return RedirectToAction("Dashboard", "Account");
            }
        }

        // ---- POST methods ----
        [HttpPost] // edit form; check and save review (only if user typed it)
        public IActionResult Edit (ReviewForm form)
        {
            IActionResult blocked = CheckLoggedIn();
            if (blocked != null)
            {
                return blocked;
            }

            // Rules the attributes can't check: a real date, not in the future.
            DateTime visitDate = DateTime.MinValue;
            if (form.VisitDate != null && form.VisitDate != "")
            {
                if (!DateTime.TryParse(form.VisitDate, out visitDate))
                {
                    ModelState.AddModelError("VisitDate", "Enter a real date.");
                }
                else if (visitDate > DateTime.Now)
                {
                    ModelState.AddModelError("VisitDate", "The visit date can't be in the future.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                Review review = new Review();
                review.ReviewID = form.ReviewID;
                review.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
                review.ReviewText = form.ReviewText;
                review.FoodRating = form.FoodRating;
                review.ServiceRating = form.ServiceRating;
                review.AtmosphereRating = form.AtmosphereRating;
                review.PriceRating = form.PriceRating;
                review.VisitDateTime = visitDate;
                review.RemovePhoto = form.RemovePhoto;

                if (!review.UpdateReview())
                {
                    ModelState.AddModelError("", "We couldn't save your review. Please try again.");
                    return View(form);
                }

                TempData["Message"] = "Your review of " + form.RestaurantName + " was updated.";
                return RedirectToAction("Dashboard", "Account");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't save your review right now. Please try again later.");
                return View(form);
            }
        }

        [HttpPost] // Delete review; return to dashboard after
        public IActionResult Delete(int id)
        {
            IActionResult blocked = CheckLoggedIn();
            if (blocked != null)
            {
                return blocked;
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));

            try
            {
                Review review = new Review();
                if (review.DeleteReview(id, userID))
                {
                    TempData["Message"] = "Your review was deleted.";
                }
                else
                {
                    TempData["Message"] = "We couldn't find that review.";
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "We couldn't delete your review right now. Please try again later.";
            }

            return RedirectToAction("Dashboard", "Account");
        }
    }
}
