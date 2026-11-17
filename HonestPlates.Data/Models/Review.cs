using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // Reviews. For now: listing a restaurant's reviews and loading one review's photo.
    // (Add, edit, delete, votes and responses come back with their own issues.)
    public class Review
    {
        public int ReviewID { get; set; }
        public byte[]? ImageData { get; set; }
        public string ImageType { get; set; } = "";

        // Every review for one restaurant, newest first (TP_GetReviews)
        public DataTable GetReviewsForRestaurant(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetReviews";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Every review one user wrote, newest first, with the restaurant's name (TP_GetMyReviews).
        // The userID comes from Session (who is logged in), never from the URL.
        public DataTable GetReviewsByUser(int userID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetMyReviews";
            cmd.Parameters.AddWithValue("@UserID", userID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }



        // Fill ImageData and ImageType. Return false when review has no photo
        public bool LoadImage(int reviewID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetReviewPhoto";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            ReviewID = reviewID;
            ImageData = (byte[])row["ReviewImage"];
            ImageType = row["ReviewImage"].ToString();
            return true;
        }

        // Name on review. 
        public static string ReviewerName(DataRow row)
        {
            if (row["FirstName"] != DBNull.Value)
            {
                string last = row["LastName"].ToString();
                if (last.Length > 0)
                {
                    return row["FirstName"] + " " + last.Substring(0, 1) + ".";
                }
                return row["FirstName"].ToString();
            }

            if (row["ReviewerName"] != DBNull.Value && row["ReviewerName"].ToString() != "")
            {
                return row["ReviewerName"].ToString();
            }

            return "Anonymous";
        }

    }
}