using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    public class Restaurant
    {
        public int RestaurantID { get; set; }
        public int RepresentativeUserID { get; set; }
        public string Name { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Zip { get; set; } = "";
        public string Cuisine { get; set; } = "";
        public string Description { get; set; } = "";
        public string Website { get; set; } = "";
        public string SocialMedia { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Owner { get; set; } = "";

        
        public bool LoadByID (int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand sqlCMD = new SqlCommand();
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.CommandText = "TP_GetRestaurantByID";
            sqlCMD.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(sqlCMD);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            // ToString turns NULL into ""
            DataRow row = ds.Tables[0].Rows[0];
            RestaurantID = restaurantID;
            if (row["RepresentativeUserID"] != DBNull.Value)
            {
                RepresentativeUserID = Convert.ToInt32(row["RepresentativeUserID"]);
            }

            Name = row["RestaurantName"].ToString();
            Street = row["RestaurantAddress"].ToString();
            City = row["RestaurantCity"].ToString();
            State = row["RestaurantState"].ToString();
            Zip = row["RestaurantZipCode"].ToString();
            Cuisine = row["RestaurantCuisine"].ToString();
            Description = row["RestaurantDescription"].ToString();
            Website = row["RestaurantWebsite"].ToString();
            SocialMedia = row["RestaurantSocialMedia"].ToString();
            Phone = row["RestaurantPhone"].ToString();
            Owner = row["RestaurantOwner"].ToString();

            return true;
        }

        // Week of hours, Sunday (0) -> Saturday (6). Call after LoadById
        public DataTable GetHours()
        {
            DBConnect hoursDB = new DBConnect();
            SqlCommand hoursCMD = new SqlCommand();
            hoursCMD.CommandType = CommandType.StoredProcedure;
            hoursCMD.CommandText = "TP_GetRestaurantHours";
            hoursCMD.Parameters.AddWithValue("@RestaurantID", RestaurantID);

            DataSet ds = hoursDB.GetDataSetUsingCmdObj(hoursCMD);
            return ds.Tables[0];
        }

        // One hour(s) row as text: "11:00AM - 10:00PM' or "Closed"
        public string FormatHours(DataRow row)
        {
            if (Convert.ToBoolean(row["IsClosed"]) || row["OpenTime"] == DBNull.Value || row["CloseTime"] == DBNull.Value)
            {
                return "Closed";
            }

            // For actual open/close time
            string open = DateTime.Parse(row["OpenTime"].ToString()).ToString("h:mm:tt");
            string close = DateTime.Parse(row["CloseTime"].ToString()).ToString("h:mm:tt");
            return open + " - " + close;        
        }
    }
}