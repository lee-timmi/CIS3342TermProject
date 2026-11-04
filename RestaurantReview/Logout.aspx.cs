using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Forget who was logged in, then go back to the home page
            Session.Abandon();
            Response.Redirect("Home.aspx");
        }
    }
}