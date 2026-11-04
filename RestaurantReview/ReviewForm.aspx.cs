using System;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // One page for both writing and editing a review:
    //   ReviewForm.aspx?RestaurantID=6   -> new review for restaurant 6
    //   ReviewForm.aspx?ReviewID=80      -> edit review 80 (only if it's this reviewer's)
    public partial class ReviewForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Logged-in Reviewers and Representatives can write reviews (a rep, just not for their own restaurant)
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            string userType = Session["UserType"].ToString();
            if (userType != "Reviewer" && userType != "Representative")
            {
                Response.Redirect("Dashboard.aspx");
            }

            if (!IsPostBack)
            {
                try
                {
                    FillLists();
                    LoadPage();
                }
                catch (Exception)
                {
                    ShowError("Sorry, we couldn't load this page. Please try again later.");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Check the input before touching the database
            string errors = ValidateForm();
            if (errors != "")
            {
                ShowMessage("Please fix the following:<br />" + errors, false);
                return;
            }

            bool saved = false;
            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                Review review = new Review();
                bool editing = Request.QueryString["ReviewID"] != null;

                // 2. Editing: look the review up again with this user's ID (never trust the page)
                if (editing)
                {
                    int reviewID;
                    if (!int.TryParse(Request.QueryString["ReviewID"], out reviewID) || !review.LoadForEdit(reviewID, userID))
                    {
                        ShowError("That review wasn't found in your reviews.");
                        return;
                    }
                }
                else
                {
                    // New review: the restaurant must exist and not be this user's own
                    Restaurant restaurant = new Restaurant();
                    int restaurantID;
                    if (!int.TryParse(Request.QueryString["RestaurantID"], out restaurantID) || !restaurant.LoadByID(restaurantID) || restaurant.IsRunBy(userID))
                    {
                        ShowError("You can't review this restaurant.");
                        return;
                    }
                    review.RestaurantID = restaurantID;
                }

                // 3. Copy the form into the object
                review.ReviewUserID = userID;
                review.FoodRating = Convert.ToInt32(rblFood.SelectedValue);
                review.ServiceRating = Convert.ToInt32(rblService.SelectedValue);
                review.AtmosphereRating = Convert.ToInt32(rblAtmosphere.SelectedValue);
                review.PriceRating = Convert.ToInt32(rblPrice.SelectedValue);
                review.VisitDateTime = GetVisitDateTime();
                review.ReviewText = txtReviewText.Text.Trim();

                // Read the uploaded photo into a byte[] (same as ProductImageLoader)
                if (fuPhoto.HasFile)
                {
                    int imageSize = fuPhoto.PostedFile.ContentLength;
                    byte[] imageData = new byte[imageSize];
                    fuPhoto.PostedFile.InputStream.Read(imageData, 0, imageSize);
                    review.ImageData = imageData;
                    review.ImageType = fuPhoto.PostedFile.ContentType;
                }

                // 4. Save
                if (editing)
                {
                    saved = review.UpdateReview(chkRemovePhoto.Checked);
                }
                else
                {
                    saved = review.AddReview();
                }

                if (!saved)
                {
                    ShowMessage("Sorry, your review couldn't be saved. Please try again.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }

            // Outside the try: Response.Redirect throws a ThreadAbortException (same as Login.aspx)
            if (saved)
            {
                Response.Redirect("MyReviews.aspx?saved=1");
            }
        }

        // Returns the problems with the form, one per line ("" = everything is OK)
        private string ValidateForm()
        {
            string errors = "";

            if (rblFood.SelectedIndex == -1) errors += "Rate the food.<br />";
            if (rblService.SelectedIndex == -1) errors += "Rate the service.<br />";
            if (rblAtmosphere.SelectedIndex == -1) errors += "Rate the atmosphere.<br />";
            if (rblPrice.SelectedIndex == -1) errors += "Choose a price level.<br />";

            if (calVisit.SelectedDate == DateTime.MinValue)
            {
                errors += "Choose the date of your visit on the calendar.<br />";
            }
            else if (calVisit.SelectedDate.Date > DateTime.Today)
            {
                errors += "The visit date can't be after today.<br />";
            }

            string text = txtReviewText.Text.Trim();
            if (text == "") errors += "Write a few words about your visit.<br />";
            if (text.Length > 4000) errors += "Your review can be at most 4,000 characters.<br />";

            // No < or > (the message uses &lt; &gt; because lblMessage shows its text as HTML)
            if (Validation.HasHtml(text))
            {
                errors += "Your review can't contain &lt; or &gt; (HTML isn't allowed).<br />";
            }

            // Photo checks, the same way as the restaurant photos (ManageRestaurant.aspx)
            if (fuPhoto.HasFile)
            {
                if (fuPhoto.PostedFile.ContentLength > Review.MaxImageBytes)
                {
                    errors += "The photo is larger than 1 MB. Please choose a smaller one.<br />";
                }

                if (!Validation.IsImageFile(fuPhoto.PostedFile.FileName))
                {
                    errors += "Only JPG, PNG, or GIF photos can be uploaded.<br />";
                }
            }

            return errors;
        }

        // The calendar's date + the time list's time ("19:30")
        private DateTime GetVisitDateTime()
        {
            TimeSpan time = TimeSpan.Parse(ddlVisitTime.SelectedValue);
            return calVisit.SelectedDate.Date + time;
        }

        // Fills the rating lists and the time list (first page load only)
        private void FillLists()
        {
            string[] stars = { "1 &#9733;", "2 &#9733;", "3 &#9733;", "4 &#9733;", "5 &#9733;" };
            string[] dollars = { "$", "$$", "$$$", "$$$$", "$$$$$" };
            for (int i = 0; i < 5; i++)
            {
                string value = (i + 1).ToString();
                rblFood.Items.Add(new ListItem(stars[i], value));
                rblService.Items.Add(new ListItem(stars[i], value));
                rblAtmosphere.Items.Add(new ListItem(stars[i], value));
                rblPrice.Items.Add(new ListItem(dollars[i], value));
            }

            // Every half hour (same lists as ManageRestaurant and the reservation pages)
            ListHelper.FillTimes(ddlVisitTime);
            ddlVisitTime.SelectedValue = "19:00";

            calVisit.SelectedDate = DateTime.Today;
            calVisit.VisibleDate = DateTime.Today;
        }

        // New review: shows the restaurant. Edit: fills the form with the saved review.
        private void LoadPage()
        {
            int userID = Convert.ToInt32(Session["UserID"]);

            if (Request.QueryString["ReviewID"] != null)
            {
                Review review = new Review();
                int reviewID;
                if (!int.TryParse(Request.QueryString["ReviewID"], out reviewID) || !review.LoadForEdit(reviewID, userID))
                {
                    ShowError("That review wasn't found in your reviews.");
                    return;
                }

                lblHeading.Text = "Edit Your Review";
                btnSave.Text = "Save Changes";
                Page.Title = "Edit Review | Honest Plates";
                ShowRestaurant(review.RestaurantID, review.RestaurantName, "");

                ListHelper.SelectValue(rblFood, review.FoodRating.ToString());
                ListHelper.SelectValue(rblService, review.ServiceRating.ToString());
                ListHelper.SelectValue(rblAtmosphere, review.AtmosphereRating.ToString());
                ListHelper.SelectValue(rblPrice, review.PriceRating.ToString());
                calVisit.SelectedDate = review.VisitDateTime.Date;
                calVisit.VisibleDate = review.VisitDateTime.Date;
                ListHelper.SelectValue(ddlVisitTime, review.VisitDateTime.ToString("HH:mm"));
                txtReviewText.Text = review.ReviewText;

                if (review.HasImage)
                {
                    imgCurrentPhoto.ImageUrl = "ImageGrab.aspx?ReviewID=" + review.ReviewID;
                    pnlCurrentPhoto.Visible = true;
                }
            }
            else
            {
                Restaurant restaurant = new Restaurant();
                int restaurantID;
                if (!int.TryParse(Request.QueryString["RestaurantID"], out restaurantID) || !restaurant.LoadByID(restaurantID))
                {
                    ShowError("That restaurant wasn't found. Pick one from the search page.");
                    return;
                }
                if (restaurant.IsRunBy(userID))
                {
                    ShowError("You can't review your own restaurant.");
                    return;
                }
                ShowRestaurant(restaurant.RestaurantID, restaurant.Name, " &middot; " + restaurant.City + ", " + restaurant.State);
            }
        }

        private void ShowRestaurant(int restaurantID, string name, string location)
        {
            lnkRestaurant.Text = Server.HtmlEncode(name);
            lnkRestaurant.NavigateUrl = "RestaurantProfile.aspx?id=" + restaurantID;
            lblLocation.Text = location;
        }

        // An error that stops the whole form (bad link): hide the form, show the message
        private void ShowError(string message)
        {
            pnlForm.Visible = false;
            ShowMessage(message + " <a href='Dashboard.aspx'>Back to Dashboard</a>", false);
        }

        // Shows lblMessage as a green (success) or red (error) Bootstrap alert
        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = success ? "d-block alert alert-success" : "d-block alert alert-danger";
            lblMessage.Visible = true;
        }
    }
}
