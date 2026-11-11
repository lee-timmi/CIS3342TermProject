using System;
using Utilities;
using System.Data.SqlClient;
using System.Data;

namespace RestaurantReview
{
    public class RestaurantImage
    {
        // 1 MB = 1,048,576 bytes (the spec limit)
        public const int MaxImageBytes = 1048576;

        public int ImageID { get; set; }
        public int RestaurantID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
        public string Caption { get; set; }
        public string Category { get; set; }   // Logo, Profile, or Gallery

        // Saves this photo. Returns true if it worked.
        public bool AddImage()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddRestaurantImage";
            cmd.Parameters.AddWithValue("@RestaurantID", this.RestaurantID);
            cmd.Parameters.AddWithValue("@ImageData", this.ImageData);
            cmd.Parameters.AddWithValue("@ImageContentType", this.ContentType);

            if (string.IsNullOrEmpty(this.Caption))
            {
                cmd.Parameters.AddWithValue("@Caption", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Caption", this.Caption);
            }

            cmd.Parameters.AddWithValue("@ImageCategory", this.Category);

            // Replacing a Logo/Profile photo deletes the old row too, so this can be 2
            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected > 0;
        }

        // Returns the photo list for one restaurant (ImageID, Caption, ImageCategory, UploadDate)
        public DataTable GetImagesForRestaurant(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRestaurantImages";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Finds the first photo of one category (Logo, Profile, or Gallery) in a list from
        // GetImagesForRestaurant. Returns its ImageID, or 0 if there's no photo of that category.
        public int FindImageID(DataTable photos, string category)
        {
            foreach (DataRow row in photos.Rows)
            {
                if (row["ImageCategory"].ToString() == category)
                {
                    return Convert.ToInt32(row["ImageID"]);
                }
            }
            return 0;
        }

        // Loads one photo's bytes and type. Returns false if the ID doesn't exist.
        public bool LoadImage(int imageID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetImageByID";
            cmd.Parameters.AddWithValue("@ImageID", imageID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            DataRow row = ds.Tables[0].Rows[0];
            this.ImageID = imageID;
            this.ImageData = (byte[])row["ImageData"];
            this.ContentType = row["ImageContentType"].ToString();
            return true;
        }

        // Deletes a photo if it belongs to this restaurant. Returns true if it was deleted.
        public bool DeleteImage(int imageID, int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteRestaurantImage";
            cmd.Parameters.AddWithValue("@ImageID", imageID);
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmd);
            return rowsAffected == 1;
        }
    }
}
