using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // One item on a restaurant's menu, typed in by its rep (TP_MenuItems).
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public int RestaurantID { get; set; }
        public string Section { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public bool HasPrice { get; set; }

        // A restaurant's whole menu, ordered by section then name
        public DataTable GetMenuItems(int restaurantID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetMenuItems";
            cmd.Parameters.AddWithValue("@RestaurantID", restaurantID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

        // Adds this item for RestaurantID if the rep owns it. Sets MenuItemID; returns false if not added.
        public bool AddMenuItem(int representativeUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_AddMenuItem";
            cmd.Parameters.AddWithValue("@RestaurantID", RestaurantID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", representativeUserID);
            cmd.Parameters.AddWithValue("@Section", Section);
            cmd.Parameters.AddWithValue("@ItemName", ItemName);

            if (Description == null || Description == "")
                cmd.Parameters.AddWithValue("@Description", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@Description", Description);

            if (HasPrice)
                cmd.Parameters.AddWithValue("@Price", Price);
            else
                cmd.Parameters.AddWithValue("@Price", DBNull.Value);

            MenuItemID = Convert.ToInt32(db.ExecuteScalarFunction(cmd));
            return MenuItemID > 0;
        }

        // Deletes one item if the rep owns its restaurant. Returns false if nothing was deleted.
        public bool DeleteMenuItem(int menuItemID, int representativeUserID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_DeleteMenuItem";
            cmd.Parameters.AddWithValue("@MenuItemID", menuItemID);
            cmd.Parameters.AddWithValue("@RepresentativeUserID", representativeUserID);

            return db.DoUpdateUsingCmdObj(cmd) > 0;
        }
    }
}