using System;
using Utilities;
using System.Data.SqlClient;
using System.Data;

namespace RestaurantReview
{
    public class Restaurant
    {
        // Properties (match the RestaurantTable columns the form edits)
        public int RestaurantID { get; set; }
        public int RepresentativeUserID { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Cuisine { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string SocialMedia { get; set; }
        public string Phone { get; set; }
        public string Owner { get; set; }
        public string OldImageUrl { get; set; }     // Project 1 image link (used when no Profile photo was uploaded)
        public string OldHours { get; set; }        // Project 1 hours text (used when there are no RestaurantHours rows)

        // Loads the restaurant this rep manages. Returns false if the rep hasn't added one yet.
        public bool LoadByRepresentative(int userID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantByRepID";
            cmd.Parameters.AddWithValue("@RepresentativeUserID", userID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            // ToString() turns a NULL column into ""
            DataRow row = ds.Tables[0].Rows[0];
            this.RestaurantID = Convert.ToInt32(row["RestaurantID"]);
            this.RepresentativeUserID = userID;
            this.Name = row["RestaurantName"].ToString();
            this.Street = row["RestaurantAddress"].ToString();
            this.City = row["RestaurantCity"].ToString();
            this.State = row["RestaurantState"].ToString();
            this.Zip = row["RestaurantZipCode"].ToString();
            this.Cuisine = row["RestaurantCuisine"].ToString();
            this.Description = row["RestaurantDescription"].ToString();
            this.Website = row["RestaurantWebsite"].ToString();
            this.SocialMedia = row["RestaurantSocialMedia"].ToString();
            this.Phone = row["RestaurantPhone"].ToString();
            this.Owner = row["RestaurantOwner"].ToString();
            return true;
        }

        // Loads any restaurant by its ID (RestaurantProfile.aspx, ReviewForm.aspx).
        // Returns false if the ID doesn't exist.
        public bool LoadByID(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantByID";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            // ToString() turns a NULL column into ""
            DataRow row = ds.Tables[0].Rows[0];
            this.RestaurantID = restaurantID;
            // Old restaurants have no rep (NULL), so they get 0
            if (row["RepresentativeUserID"] != DBNull.Value)
            {
                this.RepresentativeUserID = Convert.ToInt32(row["RepresentativeUserID"]);
            }
            this.Name = row["RestaurantName"].ToString();
            this.Street = row["RestaurantAddress"].ToString();
            this.City = row["RestaurantCity"].ToString();
            this.State = row["RestaurantState"].ToString();
            this.Zip = row["RestaurantZipCode"].ToString();
            this.Cuisine = row["RestaurantCuisine"].ToString();
            this.Description = row["RestaurantDescription"].ToString();
            this.Website = row["RestaurantWebsite"].ToString();
            this.SocialMedia = row["RestaurantSocialMedia"].ToString();
            this.Phone = row["RestaurantPhone"].ToString();
            this.Owner = row["RestaurantOwner"].ToString();
            this.OldImageUrl = row["RestaurantImage"].ToString();
            this.OldHours = row["RestaurantHoursOfOperation"].ToString();
            return true;
        }

        // True if this user is the restaurant's rep (call after LoadByID). A rep can't review their own restaurant.
        public bool IsRunBy(int userID)
        {
            return this.RepresentativeUserID == userID;
        }

        // Searches by any number of cuisines ("Italian,Greek", "" = any) and a city/state ("" = any).
        // Returns the result rows from SearchRestaurants (name, logo ID, review count, averages).
        public DataTable Search(string cuisineList, string location)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SearchRestaurants";
            cmd.Parameters.AddWithValue("@Cuisines", cuisineList);
            cmd.Parameters.AddWithValue("@Location", location);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // The best-rated restaurants in one city (only ones with reviews), for the Home page's "Top rated" cards.
        // Returns the same columns as Search, so the same cards can show them.
        public DataTable GetTopRated(string city, string state, int count)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetTopRatedRestaurants";
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);
            cmd.Parameters.AddWithValue("@Count", count);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // True if this restaurant's hours mark that day as Closed.
        // Older restaurants with no hours rows are never "closed" here.
        public bool IsClosedOn(DateTime date)
        {
            DataTable hours = GetHours();
            foreach (DataRow row in hours.Rows)
            {
                // DayOfWeek: 0 = Sunday, the same numbering as .NET's DayOfWeek
                if (Convert.ToInt32(row["DayOfWeek"]) == (int)date.DayOfWeek)
                {
                    return Convert.ToBoolean(row["IsClosed"]);
                }
            }
            return false;
        }

        // True if the restaurant is open at that date and time (used for reservations).
        // A CloseTime earlier than the OpenTime means it closes after midnight, so 1:00 AM Saturday
        // can still be inside Friday's hours. Older restaurants with no hours rows are always "open" here.
        public bool IsOpenAt(DateTime when)
        {
            DataTable hours = GetHours();
            if (hours.Rows.Count == 0)
            {
                return true;
            }

            TimeSpan time = when.TimeOfDay;
            int today = (int)when.DayOfWeek;
            int yesterday = (today + 6) % 7;
            foreach (DataRow row in hours.Rows)
            {
                if (Convert.ToBoolean(row["IsClosed"]) || row["OpenTime"] == DBNull.Value || row["CloseTime"] == DBNull.Value)
                {
                    continue;
                }
                int day = Convert.ToInt32(row["DayOfWeek"]);
                TimeSpan open = TimeSpan.Parse(row["OpenTime"].ToString());
                TimeSpan close = TimeSpan.Parse(row["CloseTime"].ToString());
                bool overnight = close <= open;

                // Inside that day's own hours (an overnight day runs from OpenTime to midnight)
                if (day == today && time >= open && (overnight || time < close))
                {
                    return true;
                }
                // After midnight, inside the day before's overnight hours
                if (day == yesterday && overnight && time < close)
                {
                    return true;
                }
            }
            return false;
        }

        // That day's hours as text, e.g. "11:00 AM &ndash; 10:00 PM" or "Closed".
        // "" if the restaurant has no hours rows for that day.
        public string GetHoursText(DateTime date)
        {
            DataTable hours = GetHours();
            foreach (DataRow row in hours.Rows)
            {
                if (Convert.ToInt32(row["DayOfWeek"]) == (int)date.DayOfWeek)
                {
                    return FormatHours(row);
                }
            }
            return "";
        }

        // One RestaurantHours row as text: "11:00 AM &ndash; 10:00 PM" or "Closed"
        // (used here and by the profile's hours table)
        public string FormatHours(DataRow row)
        {
            if (Convert.ToBoolean(row["IsClosed"]) || row["OpenTime"] == DBNull.Value || row["CloseTime"] == DBNull.Value)
            {
                return "Closed";
            }
            // "17:00" -> "5:00 PM"
            string open = DateTime.Parse(row["OpenTime"].ToString()).ToString("h:mm tt");
            string close = DateTime.Parse(row["CloseTime"].ToString()).ToString("h:mm tt");
            return open + " &ndash; " + close;
        }

        // Returns why a reservation can't be at that time ("" = the restaurant is open then)
        public string GetHoursProblem(DateTime when)
        {
            if (IsOpenAt(when))
            {
                return "";
            }
            if (IsClosedOn(when))
            {
                return "The restaurant is closed on " + when.ToString("dddd") + "s. Please pick another day.";
            }
            return "The restaurant isn't open at " + when.ToString("h:mm tt") + " on " + when.ToString("dddd")
                + "s (hours: " + GetHoursText(when) + "). Please pick another time.";
        }

        // Saves a new restaurant. Returns true and sets RestaurantID if it worked.
        public bool AddRestaurant()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddRestaurant";

            // The procedure RETURNs the new ID; a ReturnValue parameter catches it
            SqlParameter returnParameter = new SqlParameter("returnValue", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnParameter);
            AddProfileParameters(cmd);

            db.DoUpdateUsingCmdObj(cmd);

            // 0 = this rep already has a restaurant. If the database call failed,
            // Value is null, and Convert.ToInt32(null) is also 0.
            int newID = Convert.ToInt32(returnParameter.Value);
            if (newID == 0)
            {
                return false;
            }
            this.RestaurantID = newID;
            return true;
        }

        // Saves changes to this rep's restaurant. Returns false if nothing was updated.
        public bool UpdateRestaurant()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateRestaurant";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);
            AddProfileParameters(cmd);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Saves one day's hours (dayOfWeek: 0 = Sunday ... 6 = Saturday). Times are "HH:mm".
        public bool SaveHours(int dayOfWeek, bool isClosed, string openTime, string closeTime)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SaveRestaurantHours";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);
            cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
            cmd.Parameters.AddWithValue("@IsClosed", isClosed);

            // A closed day has no times
            if (isClosed)
            {
                cmd.Parameters.AddWithValue("@OpenTime", DBNull.Value);
                cmd.Parameters.AddWithValue("@CloseTime", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@OpenTime", openTime);
                cmd.Parameters.AddWithValue("@CloseTime", closeTime);
            }

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Returns this restaurant's hours (DayOfWeek, IsClosed, OpenTime, CloseTime), Sunday first.
        public DataTable GetHours()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantHours";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Returns this restaurant's rating summary (review count + Food, Service, Atmosphere, Price, Overall averages)
        public Rating GetRatings()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantRatings";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return new Rating(ds.Tables[0].Rows[0]);
        }

        // Returns every cuisine already in the database (for the cuisine DropDownList).
        public DataTable GetCuisineList()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCuisines";

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // The profile parameters AddRestaurant and UpdateRestaurant both send
        private void AddProfileParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@RepresentativeUserID", this.RepresentativeUserID);
            cmd.Parameters.AddWithValue("@RestaurantName", this.Name);
            cmd.Parameters.AddWithValue("@RestaurantAddress", this.Street);
            cmd.Parameters.AddWithValue("@RestaurantCity", this.City);
            cmd.Parameters.AddWithValue("@RestaurantState", this.State);
            cmd.Parameters.AddWithValue("@RestaurantZipCode", this.Zip);
            cmd.Parameters.AddWithValue("@RestaurantCuisine", this.Cuisine);
            AddOptionalParameter(cmd, "@RestaurantDescription", this.Description);
            AddOptionalParameter(cmd, "@RestaurantWebsite", this.Website);
            AddOptionalParameter(cmd, "@RestaurantSocialMedia", this.SocialMedia);
            AddOptionalParameter(cmd, "@RestaurantPhone", this.Phone);
            AddOptionalParameter(cmd, "@RestaurantOwner", this.Owner);
        }

        // Optional fields are stored as NULL when left blank (same idea as Phone in UserAccount)
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
