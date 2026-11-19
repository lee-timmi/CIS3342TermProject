using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // Restaurant photos (Logo, Profile, Gallery).
    // Photo Manager will handle the upload/deleting
    public class RestaurantImage
    {
        public const int MaxImageBytes = 1048576; // 1 MB

        public int ImageID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; } = "";

        // Restaurant photo (imageid, caption, imagecategory)
        public DataTable GetImagesForRestaurant(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetPhotos";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Image ID of first image of one kind ("logo" or "profile"), 0 if none.
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

        // Fills ImageData and ContentType. Return false if no photo has the correct id
        public bool LoadImage(int imageID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetPhoto";
            cmd.Parameters.AddWithValue("@ImageID", imageID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            DataRow row = ds.Tables[0].Rows[0];
            ImageID = imageID;
            ImageData = (byte[])row["ImageData"];
            ContentType = row["ImageContentType"].ToString();
            return true;
        }

        // Deletes one photo, only if it's this restaurant's and this rep owns the restaurant.
        // Returns false if nothing was deleted.
        public bool DeletePhoto(int imageID, int restaurantID, int representativeUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_DeleteRestaurantPhoto";
            cmd.Parameters.AddWithValue("@ImageID", imageID);
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", representativeUserID);

            return db.DoUpdateUsingCmdObj(cmd) > 0;
        }
    }
}