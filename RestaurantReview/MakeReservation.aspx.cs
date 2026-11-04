using System;
using System.Data;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // Anyone (Reviewer, Representative, or a visitor without an account) can request a reservation:
    //   MakeReservation.aspx?RestaurantID=6
    public partial class MakeReservation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    FillLists();
                    if (!ShowRestaurant())
                    {
                        pnlForm.Visible = false;
                        ShowMessage("That restaurant wasn't found. <a href='Home.aspx'>Search for a restaurant</a>", false);
                    }
                    else
                    {
                        ShowHoursNote();
                    }
                }
                catch (Exception)
                {
                    pnlForm.Visible = false;
                    ShowMessage("Sorry, we couldn't load this page. Please try again later.", false);
                }
            }
        }

        protected void btnReserve_Click(object sender, EventArgs e)
        {
            int restaurantID;
            if (!int.TryParse(Request.QueryString["RestaurantID"], out restaurantID))
            {
                ShowMessage("That restaurant wasn't found.", false);
                return;
            }

            try
            {
                // 1. Check the input before saving (inside the try: the "closed that day" check reads the hours)
                string errors = ValidateForm(restaurantID);
                if (errors != "")
                {
                    ShowMessage("Please fix the following:<br />" + errors, false);
                    return;
                }

                Reservation reservation = new Reservation();
                DataRow contact = reservation.GetRestaurantContact(restaurantID);
                if (contact == null)
                {
                    ShowMessage("That restaurant wasn't found.", false);
                    return;
                }

                // 2. Copy the form into the object and save
                reservation.RestaurantID = restaurantID;
                reservation.RestaurantName = contact["RestaurantName"].ToString();
                if (Session["UserID"] != null)
                {
                    reservation.UserID = Convert.ToInt32(Session["UserID"]);
                }
                reservation.ContactName = txtName.Text.Trim();
                reservation.ContactPhone = txtPhone.Text.Trim();
                reservation.ContactEmail = txtEmail.Text.Trim();
                reservation.ReservationDateTime = GetReservationDateTime();
                reservation.PartySize = Convert.ToInt32(ddlPartySize.SelectedValue);
                reservation.SpecialRequests = txtRequests.Text.Trim();
                reservation.Status = "Pending";

                if (!reservation.AddReservation())
                {
                    ShowMessage("Sorry, your reservation couldn't be saved. Please try again.", false);
                    return;
                }

                // 3. Email the rep. The reservation is already saved, so an email problem is only a warning.
                string emailNote;
                if (contact["RepEmail"] == DBNull.Value)
                {
                    emailNote = "This restaurant doesn't have a representative on Honest Plates yet, so please call to confirm.";
                }
                else
                {
                    try
                    {
                        reservation.SendNewReservationEmail(contact["RepEmail"].ToString(), contact["RepFirstName"].ToString());
                        emailNote = "We emailed the restaurant your request. They'll confirm it with you by email.";
                    }
                    catch (Exception)
                    {
                        emailNote = "Your request was saved, but we couldn't email the restaurant right now. It will show on their dashboard.";
                    }
                }

                // 4. Show the summary instead of the form
                pnlForm.Visible = false;
                lblMessage.Visible = false;
                litSummary.Text = "<p>Your request for <b>" + reservation.PartySize + "</b> on <b>"
                    + reservation.ReservationDateTime.ToString("dddd, MMMM d, yyyy 'at' h:mm tt") + "</b> at <b>"
                    + Server.HtmlEncode(reservation.RestaurantName) + "</b> was received (status: Pending).</p>"
                    + "<p>" + emailNote + "</p>";
                pnlDone.Visible = true;
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        // Picking a date or a time re-checks the hours right away, before the form is sent
        protected void calDate_SelectionChanged(object sender, EventArgs e)
        {
            ShowHoursNote();
        }

        protected void ddlTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHoursNote();
        }

        // Shows that day's hours under the time list, or a warning if the restaurant isn't open
        // at the chosen date and time. Older restaurants with no hours rows show nothing.
        private void ShowHoursNote()
        {
            lblHours.Visible = false;
            int restaurantID;
            if (!int.TryParse(Request.QueryString["RestaurantID"], out restaurantID) || calDate.SelectedDate == DateTime.MinValue)
            {
                return;
            }

            try
            {
                Restaurant restaurant = new Restaurant();
                restaurant.RestaurantID = restaurantID;
                string hoursText = restaurant.GetHoursText(calDate.SelectedDate);
                if (hoursText == "")
                {
                    return;
                }

                string problem = restaurant.GetHoursProblem(GetReservationDateTime());
                if (problem != "")
                {
                    lblHours.Text = problem;
                    lblHours.CssClass = "d-block alert alert-warning small mt-2 mb-0 py-2";
                }
                else
                {
                    lblHours.Text = "Open " + calDate.SelectedDate.ToString("dddd") + ": " + hoursText;
                    lblHours.CssClass = "d-block form-text";
                }
                lblHours.Visible = true;
            }
            catch (Exception)
            {
                // The note is only a hint; the hours are checked again when the form is sent
            }
        }

        // Returns the problems with the form, one per line ("" = everything is OK)
        private string ValidateForm(int restaurantID)
        {
            string errors = "";

            string name = txtName.Text.Trim();
            if (name == "") errors += "Enter your name.<br />";

            string phone = txtPhone.Text.Trim();
            if (Validation.CountDigits(phone) != 10) errors += "Phone must have 10 digits, e.g. 215-555-0123.<br />";

            if (!Validation.IsEmail(txtEmail.Text.Trim()))
            {
                errors += "Enter a valid email address, e.g. you@example.com.<br />";
            }

            if (calDate.SelectedDate == DateTime.MinValue)
            {
                errors += "Choose a date on the calendar.<br />";
            }
            else
            {
                if (GetReservationDateTime() <= DateTime.Now)
                {
                    errors += "The reservation has to be in the future.<br />";
                }
                else
                {
                    Restaurant restaurant = new Restaurant();
                    restaurant.RestaurantID = restaurantID;
                    string hoursProblem = restaurant.GetHoursProblem(GetReservationDateTime());
                    if (hoursProblem != "")
                    {
                        errors += hoursProblem + "<br />";
                    }
                }
            }

            if (txtRequests.Text.Trim().Length > 500) errors += "Special requests can be at most 500 characters.<br />";

            // No < or > anywhere (the message uses &lt; &gt; because lblMessage shows its text as HTML)
            TextBox[] textBoxes = { txtName, txtPhone, txtEmail, txtRequests };
            foreach (TextBox box in textBoxes)
            {
                if (Validation.HasHtml(box.Text))
                {
                    errors += "Text can't contain &lt; or &gt; (HTML isn't allowed).<br />";
                    break;
                }
            }

            return errors;
        }

        // The calendar's date + the time list's time ("19:30")
        private DateTime GetReservationDateTime()
        {
            return calDate.SelectedDate.Date + TimeSpan.Parse(ddlTime.SelectedValue);
        }

        // Fills the time and party size lists, and starts the calendar on tomorrow (first page load only)
        private void FillLists()
        {
            ListHelper.FillTimes(ddlTime);
            ddlTime.SelectedValue = "19:00";

            ListHelper.FillPartySizes(ddlPartySize);
            ddlPartySize.SelectedValue = "2";

            calDate.SelectedDate = DateTime.Today.AddDays(1);
            calDate.VisibleDate = DateTime.Today.AddDays(1);
        }

        // Shows the restaurant's name and sets the links back to it. Returns false if the ID isn't valid.
        private bool ShowRestaurant()
        {
            int restaurantID;
            if (!int.TryParse(Request.QueryString["RestaurantID"], out restaurantID))
            {
                return false;
            }

            Reservation reservation = new Reservation();
            DataRow contact = reservation.GetRestaurantContact(restaurantID);
            if (contact == null)
            {
                return false;
            }

            lnkRestaurant.Text = Server.HtmlEncode(contact["RestaurantName"].ToString());
            lnkRestaurant.NavigateUrl = "RestaurantProfile.aspx?id=" + restaurantID;
            lblLocation.Text = " &middot; " + Server.HtmlEncode(contact["RestaurantCity"] + ", " + contact["RestaurantState"]);
            lnkCancel.NavigateUrl = lnkRestaurant.NavigateUrl;
            lnkBack.NavigateUrl = lnkRestaurant.NavigateUrl;
            return true;
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
