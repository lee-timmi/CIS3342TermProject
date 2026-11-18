using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // Reviews. For now: listing a restaurant's reviews and loading one review's photo.
    // (Add, edit, delete, votes and responses come back with their own issues.)
    public class Review
    {
        public int ReviewID { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; } = "";
        public int UserID { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = "";
        public string ReviewText { get; set; } = "";
        public int FoodRating { get; set; }
        public int ServiceRating { get; set; }
        public int AtmosphereRating { get; set; }
        public int PriceRating { get; set; }
        public DateTime VisitDateTime { get; set; }
        public bool HasPhoto { get; set; }
        public bool RemovePhoto { get; set; }

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

        // Loads one review for the Edit page. Returns false if it doesn't exist or this user didn't write it.
        // Came from Project 2: GetReviewForEdit.
        public bool LoadForEdit(int reviewID, int userID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetReviewForEdit";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);
            cmd.Parameters.AddWithValue("@UserID", userID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            ReviewID = reviewID;
            UserID = userID;
            RestaurantID = Convert.ToInt32(row["RestaurantID"]);
            RestaurantName = row["RestaurantName"].ToString();
            ReviewText = row["ReviewText"].ToString();
            FoodRating = Convert.ToInt32(row["FoodRating"]);
            ServiceRating = Convert.ToInt32(row["ServiceRating"]);
            PriceRating = Convert.ToInt32(row["PriceRating"]);
            HasPhoto = Convert.ToInt32(row["HasPhoto"]) == 1;

            // For older reviews in project 2. 
            if (row["AtmosphereRating"] != DBNull.Value)
            {
                AtmosphereRating = Convert.ToInt32(row["AtmosphereRating"]);
            }
            if (row["VisitDateTime"] != DBNull.Value)
            {
                VisitDateTime = Convert.ToDateTime(row["VisitDateTime"]);
            }

            return true;
        }

        // Saves the edited review (ReviewID + UserID must be set). Returns false if nothing changed.
        public bool UpdateReview()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_UpdateReview";
            cmd.Parameters.AddWithValue("@ReviewID", ReviewID);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@ReviewText", ReviewText);
            cmd.Parameters.AddWithValue("@FoodRating", FoodRating);
            cmd.Parameters.AddWithValue("@ServiceRating", ServiceRating);
            cmd.Parameters.AddWithValue("@AtmosphereRating", AtmosphereRating);
            cmd.Parameters.AddWithValue("@PriceRating", PriceRating);
            cmd.Parameters.AddWithValue("@VisitDateTime", VisitDateTime);
            cmd.Parameters.AddWithValue("@RemovePhoto", RemovePhoto);

            return db.DoUpdateUsingCmdObj(cmd) > 0;
        }

        // Deletes one review, only if this user wrote it. Returns false if nothing was deleted.
        public bool DeleteReview(int reviewID, int userID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_DeleteReview";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);
            cmd.Parameters.AddWithValue("@UserID", userID);

            return db.DoUpdateUsingCmdObj(cmd) > 0;
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
            ImageType = row["ReviewImageType"].ToString();
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