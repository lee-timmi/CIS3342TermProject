using System;
using System.Data;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // The rep sees every review of their own restaurant and posts a public response to any of them.
    // The list and every save go through Session["UserID"], so a rep can only answer their own restaurant's reviews.
    public partial class RespondToReviews : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only logged-in Representatives
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (Session["UserType"].ToString() != "Representative")
            {
                Response.Redirect("Dashboard.aspx");
            }

            if (!IsPostBack)
            {
                LoadReviews();
            }
        }

        // Runs when a Save Response button inside rptReviews is clicked (same idea as Repeater Example 3)
        protected void rptReviews_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SaveResponse")
            {
                return;
            }

            // Find the hidden ReviewID label and the response box in the card whose button was clicked
            int rowIndex = e.Item.ItemIndex;
            Label lblReviewID = (Label)rptReviews.Items[rowIndex].FindControl("lblReviewID");
            TextBox txtResponse = (TextBox)rptReviews.Items[rowIndex].FindControl("txtResponse");
            string responseText = txtResponse.Text.Trim();

            // Check the input (the box keeps what was typed, so nothing has to be re-entered)
            if (responseText.Length > 2000)
            {
                ShowMessage("A response can be at most 2,000 characters.", false);
                return;
            }
            if (Validation.HasHtml(responseText))
            {
                ShowMessage("A response can't contain &lt; or &gt; (HTML isn't allowed).", false);
                return;
            }

            try
            {
                Review review = new Review();
                if (review.SaveResponse(Convert.ToInt32(lblReviewID.Text), Convert.ToInt32(Session["UserID"]), responseText))
                {
                    if (responseText == "")
                    {
                        ShowMessage("The response was removed.", true);
                    }
                    else
                    {
                        ShowMessage("Your response was posted. It now shows under the review on your public page.", true);
                    }
                    LoadReviews();
                }
                else
                {
                    ShowMessage("Sorry, the response couldn't be saved.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        // Shows the reviews of this rep's restaurant (or a message)
        private void LoadReviews()
        {
            try
            {
                int repUserID = Convert.ToInt32(Session["UserID"]);

                Restaurant restaurant = new Restaurant();
                if (!restaurant.LoadByRepresentative(repUserID))
                {
                    lblNoReviews.Text = "Add your restaurant first (Dashboard → Manage my restaurant). Its reviews will show up here.";
                    lblNoReviews.Visible = true;
                    return;
                }
                lnkPublicPage.NavigateUrl = "RestaurantProfile.aspx?id=" + restaurant.RestaurantID;
                lnkPublicPage.Visible = true;

                Review review = new Review();
                DataTable reviews = review.GetReviewsForRep(repUserID);
                rptReviews.DataSource = reviews;
                rptReviews.DataBind();
                lblNoReviews.Visible = (reviews.Rows.Count == 0);
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't load the reviews. Please try again later.", false);
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
