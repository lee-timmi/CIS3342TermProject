using System;
using System.Data;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // The user's own reviews (Reviewer or Representative), with Edit / Delete. The list always comes from
    // Session["UserID"], so no one can see or change someone else's reviews here.
    public partial class MyReviews : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Logged-in Reviewers and Representatives (reps can review other restaurants)
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
                // ReviewForm.aspx sends ?saved=1 after a successful save
                if (Request.QueryString["saved"] == "1")
                {
                    ShowMessage("Your review was saved.", true);
                }
                LoadReviews();
            }
        }

        // Runs when a Delete button inside rptMyReviews is clicked (same idea as Repeater Example 3)
        protected void rptMyReviews_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteReview")
            {
                return;
            }

            try
            {
                // Find the hidden ReviewID label in the card whose button was clicked
                int rowIndex = e.Item.ItemIndex;
                Label lblReviewID = (Label)rptMyReviews.Items[rowIndex].FindControl("lblReviewID");
                int reviewID = Convert.ToInt32(lblReviewID.Text);

                // DeleteReview also checks the user, so only this reviewer's own review can be deleted
                Review review = new Review();
                if (review.DeleteReview(reviewID, Convert.ToInt32(Session["UserID"])))
                {
                    ShowMessage("Your review was deleted.", true);
                }
                else
                {
                    ShowMessage("Sorry, that review couldn't be deleted.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
            LoadReviews();
        }

        // Shows this reviewer's reviews in the Repeater (or the "no reviews yet" message)
        private void LoadReviews()
        {
            try
            {
                Review review = new Review();
                DataTable reviews = review.GetReviewsByUser(Convert.ToInt32(Session["UserID"]));
                rptMyReviews.DataSource = reviews;
                rptMyReviews.DataBind();
                lblNoReviews.Visible = (reviews.Rows.Count == 0);
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't load your reviews. Please try again later.", false);
            }
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
