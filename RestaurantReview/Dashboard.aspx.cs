using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only logged-in users can see the dashboard
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            string firstName = Session["FirstName"].ToString();
            string userType = Session["UserType"].ToString();

            // "Chris T.": first name + the last name's first letter (Session["LastName"] is set at login)
            string displayName = firstName;
            if (Session["LastName"] != null && Session["LastName"].ToString() != "")
            {
                displayName = firstName + " " + Session["LastName"].ToString().Substring(0, 1).ToUpper() + ".";
            }
            lblFirstName.Text = Server.HtmlEncode(displayName);

            // Show the tools for this user type
            if (userType == "Representative")
            {
                lblUserType.Text = "Restaurant Representative";
                pnlRepresentative.Visible = true;
            }
            else
            {
                lblUserType.Text = "Reviewer";
                pnlReviewer.Visible = true;
            }
        }
    }
}
