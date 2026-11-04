using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Came from a restaurant's "write a review" link: keep the RestaurantID on the Register link too
            int restaurantID = Validation.ParseID(Request.QueryString["RestaurantID"]);
            if (restaurantID > 0)
            {
                lnkRegister.NavigateUrl = "Register.aspx?RestaurantID=" + restaurantID;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // 1. Check the input before touching the database
            if (username == "" || password == "")
            {
                ShowMessage("Please enter your username and password.", false);
                return;
            }

            // 2. Ask the database if this username + password match
            UserAccount account = new UserAccount();
            account.Username = username;
            account.Password = password;

            bool loggedIn = false;
            string nextPage = "Dashboard.aspx";

            try
            {
                if (account.LoginValidate())
                {
                    // 3. Remember who is logged in
                    Session["UserID"] = account.UserID;
                    Session["FirstName"] = account.FirstName;
                    Session["LastName"] = account.LastName;
                    Session["UserType"] = account.UserType;
                    loggedIn = true;

                    // Came from a restaurant's "write a review" link: go to its review form,
                    // unless this is the restaurant's own rep (they can't review it)
                    int restaurantID = Validation.ParseID(Request.QueryString["RestaurantID"]);
                    Restaurant restaurant = new Restaurant();
                    if (restaurantID > 0 && restaurant.LoadByID(restaurantID) && !restaurant.IsRunBy(account.UserID))
                    {
                        nextPage = "ReviewForm.aspx?RestaurantID=" + restaurantID;
                    }
                }
                else
                {
                    ShowMessage("Incorrect username or password.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }

            // 4. Go to the review form or the dashboard (outside the try, see below)
            if (loggedIn)
            {
                Response.Redirect(nextPage);
            }
        }

        // Shows lblMessage in green (success) or red (error)
        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = success ? "d-block mt-3 text-success" : "d-block mt-3 text-danger";
            lblMessage.Visible = true;
        }
    }
}