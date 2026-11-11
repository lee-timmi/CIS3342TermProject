using System;
using Utilities;
using System.Data.SqlClient;
using System.Data;

namespace RestaurantReview
{
    public class Review
    {
        // Same photo limit as the restaurant photos (1 MB)
        public const int MaxImageBytes = RestaurantImage.MaxImageBytes;

        // Properties (match the ReviewsTable columns the review form edits)
        public int ReviewID { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public int ReviewUserID { get; set; }
        public string ReviewText { get; set; }
        public int FoodRating { get; set; }
        public int ServiceRating { get; set; }
        public int AtmosphereRating { get; set; }
        public int PriceRating { get; set; }        // 1 = $ ... 5 = $$$$$ (most expensive)
        public DateTime VisitDateTime { get; set; }
        public bool HasImage { get; set; }
        public byte[] ImageData { get; set; }       // null = no new photo
        public string ImageType { get; set; }

        // Saves a new review. Returns true if it worked (false = a rep reviewing their own restaurant, or a database error).
        public bool AddReview()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddReview";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);
            AddReviewParameters(cmd);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Saves changes to this reviewer's review. removePhoto = true deletes the attached photo.
        // Returns false if nothing was updated (not this reviewer's review, or a database error).
        public bool UpdateReview(bool removePhoto)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateReview";
            cmd.Parameters.AddWithValue("@ReviewID", this.ReviewID);
            AddReviewParameters(cmd);
            cmd.Parameters.AddWithValue("@RemovePhoto", removePhoto);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Deletes a review if it belongs to this reviewer. Returns true if it was deleted.
        public bool DeleteReview(int reviewID, int reviewUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteReview";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);
            cmd.Parameters.AddWithValue("@ReviewUserID", reviewUserID);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Loads one review for editing. Returns false if it doesn't exist or isn't this reviewer's.
        public bool LoadForEdit(int reviewID, int reviewUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReviewForEdit";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);
            cmd.Parameters.AddWithValue("@ReviewUserID", reviewUserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            this.ReviewID = reviewID;
            this.ReviewUserID = reviewUserID;
            this.RestaurantID = Convert.ToInt32(row["RestaurantID"]);
            this.RestaurantName = row["RestaurantName"].ToString();
            this.ReviewText = row["ReviewText"].ToString();
            this.FoodRating = Convert.ToInt32(row["FoodRating"]);
            this.ServiceRating = Convert.ToInt32(row["ServiceRating"]);
            this.PriceRating = Convert.ToInt32(row["PriceRating"]);
            this.HasImage = Convert.ToInt32(row["HasImage"]) == 1;

            // Older reviews have no Atmosphere rating or visit date (0 / today = nothing selected yet)
            if (row["AtmosphereRating"] != DBNull.Value)
            {
                this.AtmosphereRating = Convert.ToInt32(row["AtmosphereRating"]);
            }
            if (row["VisitDateTime"] != DBNull.Value)
            {
                this.VisitDateTime = Convert.ToDateTime(row["VisitDateTime"]);
            }
            else
            {
                this.VisitDateTime = DateTime.Today;
            }
            return true;
        }

        // Returns every review for one restaurant, newest first (for RestaurantProfile.aspx)
        public DataTable GetReviewsForRestaurant(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReviewsByRestaurantID";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Adds a Helpful (helpful = true) or Not Helpful vote to a review. Returns true if it was counted.
        public bool AddVote(int reviewID, bool helpful)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (helpful)
            {
                cmd.CommandText = "AddHelpfulVote";
            }
            else
            {
                cmd.CommandText = "AddNotHelpfulVote";
            }
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Returns one reviewer's reviews, newest first (for MyReviews.aspx)
        public DataTable GetReviewsByUser(int reviewUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReviewsByUserID";
            cmd.Parameters.AddWithValue("@ReviewUserID", reviewUserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Returns every review of the rep's restaurant (ones without a response first), for RespondToReviews.aspx
        public DataTable GetReviewsForRep(int repUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReviewsForRep";
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Posts or changes the rep's public response ("" removes it).
        // Returns false if the review isn't for this rep's restaurant (or a database error).
        public bool SaveResponse(int reviewID, int repUserID, string responseText)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SaveReviewResponse";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", repUserID);
            cmd.Parameters.AddWithValue("@ResponseText", responseText);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }

        // Loads a review's photo (for ImageGrab.aspx). Returns false if the review has no photo.
        public bool LoadImage(int reviewID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReviewImage";
            cmd.Parameters.AddWithValue("@ReviewID", reviewID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            this.ReviewID = reviewID;
            this.ImageData = (byte[])row["ImageData"];
            this.ImageType = row["ImageContentType"].ToString();
            return true;
        }

        // The parameters AddReview and UpdateReview both send
        private void AddReviewParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ReviewUserID", this.ReviewUserID);
            cmd.Parameters.AddWithValue("@ReviewText", this.ReviewText);
            cmd.Parameters.AddWithValue("@FoodRating", this.FoodRating);
            cmd.Parameters.AddWithValue("@ServiceRating", this.ServiceRating);
            cmd.Parameters.AddWithValue("@AtmosphereRating", this.AtmosphereRating);
            cmd.Parameters.AddWithValue("@PriceRating", this.PriceRating);
            cmd.Parameters.AddWithValue("@VisitDateTime", this.VisitDateTime);

            // No photo: the parameter needs its type set (VarBinary), because SQL Server
            // won't convert an untyped NULL into VARBINARY(MAX)
            SqlParameter imageParameter = new SqlParameter("@ReviewImage", SqlDbType.VarBinary, -1);
            SqlParameter typeParameter = new SqlParameter("@ReviewImageType", SqlDbType.VarChar, 50);
            if (this.ImageData == null)
            {
                imageParameter.Value = DBNull.Value;
                typeParameter.Value = DBNull.Value;
            }
            else
            {
                imageParameter.Value = this.ImageData;
                typeParameter.Value = this.ImageType;
            }
            cmd.Parameters.Add(imageParameter);
            cmd.Parameters.Add(typeParameter);
        }
    }
}
