using System;
using System.Data;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // The rep's reservations: view all, Confirm, Edit, Delete. Every change emails the customer.
    // Every lookup goes through Session["UserID"], so a rep only ever sees their own restaurant's reservations.
    public partial class ManageReservations : System.Web.UI.Page
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
                FillLists();
                LoadReservations();
            }
        }

        // Runs when Confirm / Edit / Delete inside rptReservations is clicked (same idea as Repeater Example 3)
        protected void rptReservations_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            try
            {
                // Find the hidden ReservationID label in the card whose button was clicked
                int rowIndex = e.Item.ItemIndex;
                Label lblReservationID = (Label)rptReservations.Items[rowIndex].FindControl("lblReservationID");
                int reservationID = Convert.ToInt32(lblReservationID.Text);
                int repUserID = Convert.ToInt32(Session["UserID"]);

                // Load it again with the rep's ID (it must be for their restaurant)
                Reservation reservation = new Reservation();
                if (!reservation.LoadForRep(reservationID, repUserID))
                {
                    ShowMessage("That reservation wasn't found.", false);
                    LoadReservations();
                    return;
                }

                if (e.CommandName == "ConfirmReservation")
                {
                    reservation.Status = "Confirmed";
                    if (reservation.UpdateReservation(repUserID))
                    {
                        ShowMessage("The reservation was confirmed. " + EmailCustomer(reservation, "confirmed"), true);
                    }
                    else
                    {
                        ShowMessage("Sorry, the reservation couldn't be confirmed.", false);
                    }
                }
                else if (e.CommandName == "EditReservation")
                {
                    ShowEditForm(reservation);
                }
                else if (e.CommandName == "DeleteReservation")
                {
                    if (reservation.DeleteReservation(reservationID, repUserID))
                    {
                        ShowMessage("The reservation was cancelled. " + EmailCustomer(reservation, "cancelled"), true);
                        pnlEdit.Visible = false;
                    }
                    else
                    {
                        ShowMessage("Sorry, the reservation couldn't be cancelled.", false);
                    }
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
            LoadReservations();
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // 1. Check the input
            string errors = "";
            if (calEditDate.SelectedDate == DateTime.MinValue) errors += "Choose a date on the calendar.<br />";
            if (txtEditRequests.Text.Trim().Length > 500) errors += "Special requests can be at most 500 characters.<br />";
            if (Validation.HasHtml(txtEditRequests.Text))
            {
                errors += "Special requests can't contain &lt; or &gt; (HTML isn't allowed).<br />";
            }
            if (errors != "")
            {
                ShowMessage("Please fix the following:<br />" + errors, false);
                return;
            }

            try
            {
                // 2. Load it again with the rep's ID, then copy the form in
                int repUserID = Convert.ToInt32(Session["UserID"]);
                Reservation reservation = new Reservation();
                if (!reservation.LoadForRep(Convert.ToInt32(lblEditID.Text), repUserID))
                {
                    ShowMessage("That reservation wasn't found.", false);
                    pnlEdit.Visible = false;
                    LoadReservations();
                    return;
                }

                reservation.ReservationDateTime = calEditDate.SelectedDate.Date + TimeSpan.Parse(ddlEditTime.SelectedValue);

                // The new time has to be in the future (same rule as MakeReservation) ...
                if (reservation.ReservationDateTime <= DateTime.Now)
                {
                    ShowMessage("Please fix the following:<br />The reservation has to be in the future.", false);
                    return;
                }

                // ... and inside the restaurant's hours
                Restaurant restaurant = new Restaurant();
                restaurant.RestaurantID = reservation.RestaurantID;
                string hoursProblem = restaurant.GetHoursProblem(reservation.ReservationDateTime);
                if (hoursProblem != "")
                {
                    ShowMessage("Please fix the following:<br />" + hoursProblem, false);
                    return;
                }

                reservation.PartySize = Convert.ToInt32(ddlEditPartySize.SelectedValue);
                reservation.SpecialRequests = txtEditRequests.Text.Trim();
                reservation.Status = rblEditStatus.SelectedValue;

                // 3. Save + email the customer
                if (reservation.UpdateReservation(repUserID))
                {
                    ShowMessage("The reservation was changed. " + EmailCustomer(reservation, "changed"), true);
                    pnlEdit.Visible = false;
                    LoadReservations();
                }
                else
                {
                    ShowMessage("Sorry, the reservation couldn't be changed.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
        }

        // Emails the customer and returns a sentence for the on-screen message.
        // The database change already happened, so an email problem is only a warning.
        private string EmailCustomer(Reservation reservation, string action)
        {
            try
            {
                reservation.SendCustomerEmail(action);
                return "We emailed " + Server.HtmlEncode(reservation.ContactEmail) + ".";
            }
            catch (Exception)
            {
                return "(The email to " + Server.HtmlEncode(reservation.ContactEmail) + " couldn't be sent right now.)";
            }
        }

        // Fills the edit form with one reservation and shows it
        private void ShowEditForm(Reservation reservation)
        {
            lblEditID.Text = reservation.ReservationID.ToString();
            lblEditName.Text = Server.HtmlEncode(reservation.ContactName);
            calEditDate.SelectedDate = reservation.ReservationDateTime.Date;
            calEditDate.VisibleDate = reservation.ReservationDateTime.Date;
            ListHelper.SelectValue(ddlEditTime, reservation.ReservationDateTime.ToString("HH:mm"));
            ListHelper.SelectValue(ddlEditPartySize, reservation.PartySize.ToString());
            ListHelper.SelectValue(rblEditStatus, reservation.Status);
            txtEditRequests.Text = reservation.SpecialRequests;
            pnlEdit.Visible = true;
            lblMessage.Visible = false;
        }

        // Shows the rep's reservations in the Repeater (or a message)
        private void LoadReservations()
        {
            try
            {
                int repUserID = Convert.ToInt32(Session["UserID"]);

                Restaurant restaurant = new Restaurant();
                if (!restaurant.LoadByRepresentative(repUserID))
                {
                    lblNoReservations.Text = "Add your restaurant first (Dashboard → Manage my restaurant). Its reservations will show up here.";
                    lblNoReservations.Visible = true;
                    return;
                }

                Reservation reservation = new Reservation();
                DataTable reservations = reservation.GetReservationsForRep(repUserID);
                rptReservations.DataSource = reservations;
                rptReservations.DataBind();
                lblNoReservations.Visible = (reservations.Rows.Count == 0);
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't load your reservations. Please try again later.", false);
            }
        }

        // Fills the edit form's time and party size lists (first page load only)
        private void FillLists()
        {
            ListHelper.FillTimes(ddlEditTime);
            ListHelper.FillPartySizes(ddlEditPartySize);
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
