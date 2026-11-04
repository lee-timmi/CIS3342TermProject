using System;

namespace RestaurantReview
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Logged in: swap Log In / Create Account for Dashboard / greeting / Log Out
            if (Session["UserID"] != null)
            {
                phLoggedOut.Visible = false;
                phLoggedIn.Visible = true;
                lblGreeting.Text = "Hi, " + Server.HtmlEncode(Session["FirstName"].ToString());
            }
        }
    }
}