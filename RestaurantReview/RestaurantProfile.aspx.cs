using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class RestaurantProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Retrieve the restaurant ID from the query string
                string idParam = Request.QueryString["id"];

                // Validate the restaurant ID
                int restaurantId;
                if (string.IsNullOrEmpty(idParam) || !int.TryParse(idParam, out restaurantId))
                {
                    Response.Redirect("Home.aspx");
                    return;
                }

                // Load restaurant details
                Restaurant restaurant = new Restaurant();
                bool found;
                try
                {
                    found = restaurant.LoadByID(restaurantId);
                }
                catch (Exception)
                {
                    lblError.Text = "Unable to load restaurant details. Please try again later.";
                    lblError.Visible = true;
                    return;
                }

                // Redirect outside the try (Response.Redirect throws a ThreadAbortException)
                if (!found)
                {
                    Response.Redirect("Home.aspx");
                    return;
                }

                try
                {
                    // Populate the restaurant details
                    lblName.Text = restaurant.Name;
                    lblAddress.Text = restaurant.Street + ", " + restaurant.City + ", " +
                                      restaurant.State + " " + restaurant.Zip;
                    lblCuisine.Text = restaurant.Cuisine;
                    lblPhone.Text = restaurant.Phone;
                    litHours.Text = BuildHoursText(restaurant);
                    lblOwner.Text = restaurant.Owner;

                    // Phone and Owner are optional; hide the heading when there's nothing under it
                    pnlPhone.Visible = restaurant.Phone.Trim() != "";
                    pnlOwner.Visible = restaurant.Owner.Trim() != "";
                    lblDescription.Text = restaurant.Description;
                    ShowLinks(restaurant.Website, restaurant.SocialMedia);
                    ShowPhotos(restaurantId, restaurant.OldImageUrl);
                    ShowWriteReview(restaurant);
                    lnkReserve.NavigateUrl = "MakeReservation.aspx?RestaurantID=" + restaurantId;
                }
                catch (Exception)
                {
                    lblError.Text = "Unable to load restaurant details. Please try again later.";
                    lblError.Visible = true;
                    return;
                }

                try
                {
                    // Load reviews for this restaurant
                    Review review = new Review();
                    DataTable reviews = review.GetReviewsForRestaurant(restaurantId);

                    if (reviews.Rows.Count == 0)
                    {
                        lblNoReviews.Visible = true;
                    }
                    else
                    {
                        ShowRatings(restaurantId);
                    }

                    rptReviews.DataSource = reviews;
                    rptReviews.DataBind();
                }
                catch (Exception)
                {
                    lblError.Text = "Unable to load reviews. Please try again later.";
                    lblError.Visible = true;
                }
            }
        }

        // "Write a Review": a log-in link for visitors, a button for Reviewers and Representatives,
        // nothing for the rep of this restaurant (they can't review their own)
        private void ShowWriteReview(Restaurant restaurant)
        {
            int restaurantId = restaurant.RestaurantID;
            if (Session["UserID"] == null)
            {
                // The RestaurantID rides along so Login / Register can send a reviewer straight to the review form
                lnkLoginToReview.NavigateUrl = "Login.aspx?RestaurantID=" + restaurantId;
                lnkRegisterToReview.NavigateUrl = "Register.aspx?RestaurantID=" + restaurantId;
                pnlLoginToReview.Visible = true;
            }
            else if (!restaurant.IsRunBy(Convert.ToInt32(Session["UserID"])))
            {
                lnkWriteReview.NavigateUrl = "ReviewForm.aspx?RestaurantID=" + restaurantId;
                pnlWriteReview.Visible = true;
            }
        }

        // Shows the rating summary (the Rating class does the averages and formatting)
        private void ShowRatings(int restaurantId)
        {
            Restaurant restaurant = new Restaurant();
            restaurant.RestaurantID = restaurantId;
            Rating rating = restaurant.GetRatings();

            lblReviewCount.Text = rating.ReviewCount.ToString();
            lblAvgOverall.Text = rating.Format(rating.Overall, rating.HasReviews());
            lblAvgFood.Text = rating.Format(rating.Food, rating.HasReviews());
            lblAvgService.Text = rating.Format(rating.Service, rating.HasReviews());
            lblAvgAtmosphere.Text = rating.Format(rating.Atmosphere, rating.HasAtmosphere);
            lblAvgPrice.Text = rating.PriceSigns();

            pnlRatingSummary.Visible = true;
        }

        // Shows the Website / Social Media links. Only full http:// or https:// links are shown
        // (ManageRestaurant.aspx only saves links in that form).
        private void ShowLinks(string website, string socialMedia)
        {
            if (Validation.IsLink(website))
            {
                lnkWebsite.NavigateUrl = website;
                pnlWebsite.Visible = true;
            }
            if (Validation.IsLink(socialMedia))
            {
                lnkSocialMedia.NavigateUrl = socialMedia;
                pnlSocialMedia.Visible = true;
            }
        }

        // Shows the photos the rep uploaded (RestaurantImages):
        // - Profile photo -> the big hero image (older restaurants keep their old image link)
        // - Logo -> shown in the hero next to the name
        // - Gallery photos -> HTML built as a string and put in divGallery
        //   (the same technique as Prof. Pascucci's Dynamic Display Example 1)
        private void ShowPhotos(int restaurantId, string oldImageUrl)
        {
            RestaurantImage image = new RestaurantImage();
            DataTable photos = image.GetImagesForRestaurant(restaurantId);

            int profileID = image.FindImageID(photos, "Profile");
            if (profileID > 0)
            {
                imgRestaurant.ImageUrl = "ImageGrab.aspx?ID=" + profileID;
            }
            else
            {
                imgRestaurant.ImageUrl = oldImageUrl;
            }

            int logoID = image.FindImageID(photos, "Logo");
            if (logoID > 0)
            {
                imgLogo.ImageUrl = "ImageGrab.aspx?ID=" + logoID;
                imgLogo.AlternateText = lblName.Text + " logo";
                imgLogo.Visible = true;
            }

            string strHTML = "";
            foreach (DataRow row in photos.Rows)
            {
                if (row["ImageCategory"].ToString() == "Gallery")
                {
                    // Captions are typed by the rep, so they're HTML-encoded before going into the page
                    string caption = Server.HtmlEncode(row["Caption"].ToString());

                    strHTML += "<figure class='gallery-item'>";
                    strHTML += "<img src='ImageGrab.aspx?ID=" + row["ImageID"] + "' alt='" + caption + "' />";
                    if (caption != "")
                    {
                        strHTML += "<figcaption>" + caption + "</figcaption>";
                    }
                    strHTML += "</figure>";
                }
            }

            if (strHTML != "")
            {
                divGallery.InnerHtml = strHTML;
                pnlGallery.Visible = true;
            }
        }

        // Builds the Hours table (like Yelp's): one row per day from RestaurantHours (set by the rep on
        // ManageRestaurant.aspx), the short day name in the first column and the hours in the second.
        // Today's row is marked so it can be shown in bold.
        // Older restaurants have no rows there, so they fall back to the old RestaurantHoursOfOperation text.
        private string BuildHoursText(Restaurant restaurant)
        {
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            DataTable hours = restaurant.GetHours();

            if (hours.Rows.Count == 0)
            {
                // Old format: split on " / " or newlines so each day range is on its own line
                string[] hoursSegments = restaurant.OldHours.Trim().Split(
                    new string[] { " / ", "/", "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                string oldText = "";
                foreach (string segment in hoursSegments)
                {
                    oldText += segment.Trim() + "<br />";
                }
                return oldText;
            }

            int today = (int)DateTime.Now.DayOfWeek;     // 0 = Sunday, the same numbering as DayOfWeek
            string text = "<table class='hours-table'>";
            foreach (DataRow row in hours.Rows)
            {
                int dayOfWeek = Convert.ToInt32(row["DayOfWeek"]);
                string day = dayNames[dayOfWeek];
                string time = restaurant.FormatHours(row);
                if (dayOfWeek == today)
                {
                    text += "<tr class='hours-today'>";
                }
                else
                {
                    text += "<tr>";
                }
                text += "<th scope='row'>" + day + "</th><td>" + time + "</td></tr>";
            }
            text += "</table>";
            return text;
        }

        protected void rptReviews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Get the ReviewID from the hidden label in the clicked row
            Label lblReviewID = (Label)e.Item.FindControl("lblReviewID");
            int reviewId = int.Parse(lblReviewID.Text);

            Review review = new Review();
            if (!review.AddVote(reviewId, e.CommandName == "Helpful"))
            {
                lblError.Text = "Unable to record vote. Please try again.";
                lblError.Visible = true;
                return;
            }

            // Reload the reviews so the updated count shows immediately
            try
            {
                int restaurantId = int.Parse(Request.QueryString["id"]);
                rptReviews.DataSource = review.GetReviewsForRestaurant(restaurantId);
                rptReviews.DataBind();
            }
            catch (Exception)
            {
                lblError.Text = "Vote recorded, but unable to refresh reviews. Please reload the page.";
                lblError.Visible = true;
            }
        }
    }
}