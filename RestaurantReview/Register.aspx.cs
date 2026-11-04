using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Came from a restaurant's "write a review" link: keep the RestaurantID on the Log in link too
            int restaurantID = Validation.ParseID(Request.QueryString["RestaurantID"]);
            if (restaurantID > 0)
            {
                lnkLogin.NavigateUrl = "Login.aspx?RestaurantID=" + restaurantID;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string userType = rblUserType.SelectedValue;

            // 1. Check the input before touching the database
            if (userType == "")
            {
                ShowMessage("Please choose Reviewer or Restaurant Representative.", false);
                return;
            }
            if (firstName == "" || lastName == "" || email == "" || username == "" || password == "")
            {
                ShowMessage("Please fill in every field except Phone.", false);
                return;
            }
            if (!Validation.IsEmail(email))
            {
                ShowMessage("Enter a valid email address, e.g. you@example.com.", false);
                return;
            }
            if (password != txtConfirmPassword.Text)
            {
                ShowMessage("The two passwords don't match.", false);
                return;
            }
            // No < or > (passwords aren't shown anywhere, so they can have them)
            TextBox[] textBoxes = { txtFirstName, txtLastName, txtEmail, txtPhone, txtUsername };
            foreach (TextBox box in textBoxes)
            {
                if (Validation.HasHtml(box.Text))
                {
                    ShowMessage("Text can't contain &lt; or &gt; (HTML isn't allowed).", false);
                    return;
                }
            }

            // 2. Fill in the account and save it
            UserAccount account = new UserAccount();
            account.Username = username;
            account.Password = password;
            account.FirstName = firstName;
            account.LastName = lastName;
            account.Email = email;
            account.Phone = phone;
            account.UserType = userType;

            // A new account that came from a restaurant's profile is logged in and sent to its review form
            // (a brand-new rep has no restaurant yet, so they can review any of them)
            int restaurantID = Validation.ParseID(Request.QueryString["RestaurantID"]);
            bool goToReview = false;

            try
            {
                if (account.UsernameExists(username))
                {
                    ShowMessage("That username is already taken. Please choose another one.", false);
                    return;
                }

                if (account.RegisterUser())
                {
                    if (restaurantID > 0 && account.LoginValidate())
                    {
                        // Same Session values Login.aspx sets
                        Session["UserID"] = account.UserID;
                        Session["FirstName"] = account.FirstName;
                        Session["LastName"] = account.LastName;
                        Session["UserType"] = account.UserType;
                        goToReview = true;
                    }
                    else
                    {
                        ShowMessage("Your account was created. You can now log in.", true);
                        ClearForm();
                    }
                }
                else
                {
                    ShowMessage("Sorry, your account couldn't be created. Please try again.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }

            // Outside the try: Response.Redirect throws a ThreadAbortException (same as Login.aspx)
            if (goToReview)
            {
                Response.Redirect("ReviewForm.aspx?RestaurantID=" + restaurantID);
            }
        }

        // Shows lblMessage in green (success) or red (error)
        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = success ? "d-block mt-3 text-success" : "d-block mt-3 text-danger";
            lblMessage.Visible = true;
        }

        private void ClearForm()
        {
            rblUserType.ClearSelection();
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtUsername.Text = "";
        }
    }
}