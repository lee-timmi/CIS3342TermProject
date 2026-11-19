using System.Data;
using System.Net;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // One table reservation (TP_Reservations). Anyone can make one; the restaurant's rep is emailed about it.
    public class Reservation
    {
        // The "From" address on every email (Temple's mail server needs a temple.edu sender)
        public const string SiteEmail = "tut54751@temple.edu";

        public int ReservationID { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = "";
        public int UserID { get; set; }                  // 0 = visitor without an account
        public string ContactName { get; set; } = "";
        public string ContactPhone { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public DateTime ReservationDateTime { get; set; }
        public int PartySize { get; set; }
        public string SpecialRequests { get; set; } = "";
        public string Status { get; set; } = "Pending";

        // Saves a new reservation (TP_AddReservation). Sets ReservationID; returns false if it wasn't saved.
        public bool AddReservation()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_AddReservation";
            cmd.Parameters.AddWithValue("@RestaurantID", RestaurantID);

            if (UserID == 0)
                cmd.Parameters.AddWithValue("@UserID", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@UserID", UserID);

            cmd.Parameters.AddWithValue("@ContactName", ContactName);
            cmd.Parameters.AddWithValue("@ContactPhone", ContactPhone);
            cmd.Parameters.AddWithValue("@ContactEmail", ContactEmail);
            cmd.Parameters.AddWithValue("@ReservationDateTime", ReservationDateTime);
            cmd.Parameters.AddWithValue("@PartySize", PartySize);

            if (SpecialRequests == null || SpecialRequests.Trim() == "")
                cmd.Parameters.AddWithValue("@SpecialRequests", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@SpecialRequests", SpecialRequests.Trim());

            ReservationID = Convert.ToInt32(db.ExecuteScalarFunction(cmd));
            return ReservationID > 0;
        }

        // The restaurant's name + its rep's first name and email (TP_GetRestaurantContact). null = no such restaurant.
        public DataRow GetRestaurantContact(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetRestaurantContact";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Rows[0];
        }

        // Emails the rep about this new reservation. Throws if the email can't be sent.
        public void SendNewReservationEmail(string repEmail, string repFirstName)
        {
            string body = "<p>Hi " + Encode(repFirstName) + ",</p>"
                + "<p>You have a new reservation request at <b>" + Encode(RestaurantName) + "</b>:</p>"
                + DetailsHtml()
                + "<p>Log in to Honest Plates and open <b>Reservations</b> on your dashboard to confirm or change it.</p>";

            Email objEmail = new Email();
            objEmail.SendMail(repEmail, SiteEmail, "New reservation: " + ContactName + ", party of " + PartySize, body);
        }

        // The reservation details as an HTML list
        private string DetailsHtml()
        {
            string requests = SpecialRequests;
            if (requests == null || requests.Trim() == "")
            {
                requests = "(none)";
            }

            return "<ul>"
                + "<li><b>Name:</b> " + Encode(ContactName) + "</li>"
                + "<li><b>Phone:</b> " + Encode(ContactPhone) + "</li>"
                + "<li><b>Email:</b> " + Encode(ContactEmail) + "</li>"
                + "<li><b>Date &amp; time:</b> " + ReservationDateTime.ToString("dddd, MMMM d, yyyy 'at' h:mm tt") + "</li>"
                + "<li><b>Party size:</b> " + PartySize + "</li>"
                + "<li><b>Special requests:</b> " + Encode(requests) + "</li>"
                + "<li><b>Status:</b> " + Encode(Status) + "</li>"
                + "</ul>";
        }

        // Text typed by visitors is HTML-encoded before it goes into an HTML email
        private string Encode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }
    }
}