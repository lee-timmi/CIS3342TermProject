using System;
using System.Web;
using Utilities;
using System.Data.SqlClient;
using System.Data;

namespace RestaurantReview
{
    public class Reservation
    {
        // The "From" address on every email the site sends (Temple's mail server needs a temple.edu sender)
        public const string SiteEmail = "tut54751@temple.edu";

        // Properties (match the Reservations columns)
        public int ReservationID { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public int UserID { get; set; }                 // 0 = visitor without an account
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int PartySize { get; set; }
        public string SpecialRequests { get; set; }
        public string Status { get; set; }              // Pending or Confirmed

        // Saves a new reservation. Returns true and sets ReservationID if it worked.
        public bool AddReservation()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddReservation";

            // The procedure RETURNs the new ID (same as AddRestaurant / IdentityFieldExample)
            SqlParameter returnParameter = new SqlParameter("returnValue", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnParameter);

            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);
            if (this.UserID == 0)
            {
                cmd.Parameters.AddWithValue("@UserID", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@UserID", this.UserID);
            }
            cmd.Parameters.AddWithValue("@ContactName", this.ContactName);
            cmd.Parameters.AddWithValue("@ContactPhone", this.ContactPhone);
            cmd.Parameters.AddWithValue("@ContactEmail", this.ContactEmail);
            cmd.Parameters.AddWithValue("@ReservationDateTime", this.ReservationDateTime);
            cmd.Parameters.AddWithValue("@PartySize", this.PartySize);
            AddOptionalParameter(cmd, "@SpecialRequests", this.SpecialRequests);

            db.DoUpdateUsingCmdObj(cmd);

            int newID = Convert.ToInt32(returnParameter.Value);
            if (newID == 0)
            {
                return false;
            }
            this.ReservationID = newID;
            return true;
        }

        // Loads one reservation, but only if it's for this rep's restaurant. Returns false otherwise.
        public bool LoadForRep(int reservationID, int repUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReservationForRep";
            cmd.Parameters.AddWithValue("@ReservationID", reservationID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            this.ReservationID = reservationID;
            this.RestaurantID = Convert.ToInt32(row["RestaurantID"]);
            this.RestaurantName = row["RestaurantName"].ToString();
            this.ContactName = row["ContactName"].ToString();
            this.ContactPhone = row["ContactPhone"].ToString();
            this.ContactEmail = row["ContactEmail"].ToString();
            this.ReservationDateTime = Convert.ToDateTime(row["ReservationDateTime"]);
            this.PartySize = Convert.ToInt32(row["PartySize"]);
            this.SpecialRequests = row["SpecialRequests"].ToString();
            this.Status = row["Status"].ToString();
            return true;
        }

        // Saves the rep's changes (date/time, party size, requests, status). Returns false if nothing was updated.
        public bool UpdateReservation(int repUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateReservation";
            cmd.Parameters.AddWithValue("@ReservationID", this.ReservationID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);
            cmd.Parameters.AddWithValue("@ReservationDateTime", this.ReservationDateTime);
            cmd.Parameters.AddWithValue("@PartySize", this.PartySize);
            AddOptionalParameter(cmd, "@SpecialRequests", this.SpecialRequests);
            cmd.Parameters.AddWithValue("@Status", this.Status);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Deletes a reservation for this rep's restaurant. Returns true if it was deleted.
        public bool DeleteReservation(int reservationID, int repUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteReservation";
            cmd.Parameters.AddWithValue("@ReservationID", reservationID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Returns every reservation for the rep's restaurant (upcoming first)
        public DataTable GetReservationsForRep(int repUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReservationsForRep";
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Returns the restaurant's name/location and its rep's first name + email (null row = no such restaurant)
        public DataRow GetRestaurantContact(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantContact";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Rows[0];
        }

        // Emails the restaurant's rep about this new reservation (all the details).
        // Throws an exception if the email can't be sent (same as the professor's Email class).
        public void SendNewReservationEmail(string repEmail, string repFirstName)
        {
            string body = "<p>Hi " + Encode(repFirstName) + ",</p>"
                + "<p>You have a new reservation request at <b>" + Encode(this.RestaurantName) + "</b>:</p>"
                + DetailsHtml()
                + "<p>Log in to Honest Plates and open <b>Reservations</b> to confirm, change, or delete it.</p>";

            Email objEmail = new Email();
            objEmail.SendMail(repEmail, SiteEmail, "New reservation: " + this.ContactName + ", party of " + this.PartySize, body);
        }

        // Emails the customer when the rep confirms, changes, or deletes their reservation.
        // action: "confirmed", "changed", or "cancelled". Throws an exception if the email can't be sent.
        public void SendCustomerEmail(string action)
        {
            string body = "<p>Hi " + Encode(this.ContactName) + ",</p>"
                + "<p>Your reservation at <b>" + Encode(this.RestaurantName) + "</b> has been <b>" + action + "</b>.</p>"
                + DetailsHtml()
                + "<p>Thank you for using Honest Plates.</p>";

            Email objEmail = new Email();
            objEmail.SendMail(this.ContactEmail, SiteEmail, "Your reservation at " + this.RestaurantName + " was " + action, body);
        }

        // The reservation details as an HTML list (used by both emails)
        private string DetailsHtml()
        {
            string requests = this.SpecialRequests;
            if (string.IsNullOrEmpty(requests))
            {
                requests = "(none)";
            }

            return "<ul>"
                + "<li><b>Name:</b> " + Encode(this.ContactName) + "</li>"
                + "<li><b>Phone:</b> " + Encode(this.ContactPhone) + "</li>"
                + "<li><b>Email:</b> " + Encode(this.ContactEmail) + "</li>"
                + "<li><b>Date &amp; time:</b> " + this.ReservationDateTime.ToString("dddd, MMMM d, yyyy 'at' h:mm tt") + "</li>"
                + "<li><b>Party size:</b> " + this.PartySize + "</li>"
                + "<li><b>Special requests:</b> " + Encode(requests) + "</li>"
                + "<li><b>Status:</b> " + Encode(this.Status) + "</li>"
                + "</ul>";
        }

        // Text typed by visitors is HTML-encoded before it goes into an HTML email
        private string Encode(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        // Optional fields are stored as NULL when left blank
        private void AddOptionalParameter(SqlCommand cmd, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cmd.Parameters.AddWithValue(name, DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue(name, value);
            }
        }
    }
}
