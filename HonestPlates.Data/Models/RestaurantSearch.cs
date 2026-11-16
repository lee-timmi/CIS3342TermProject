using System.Data;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // Search form logic: fills the cuisine checkboxes and the city/state dropdowns,
    // and runs the search. Every database call goes through a TP_ stored procedure.
    public class RestaurantSearch
    {
        public List<string> GetCuisines()
        {
            return GetList("TP_GetCuisines", "RestaurantCuisine");
        }

        public DataTable GetLocations()
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCommand = new SqlCommand();
            objCommand.CommandType = CommandType.StoredProcedure;
            objCommand.CommandText = "TP_GetLocations";

            DataSet ds = objDB.GetDataSetUsingCmdObj(objCommand);
            return ds.Tables[0];
        }

        // Runs a procedure that returns one column, and puts each value in a list
        private List<string> GetList(string procedureName, string columnName)
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCommand = new SqlCommand();
            objCommand.CommandType = CommandType.StoredProcedure;
            objCommand.CommandText = procedureName;

            DataSet ds = objDB.GetDataSetUsingCmdObj(objCommand);

            List<string> values = new List<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                values.Add(row[columnName].ToString());
            }
            return values;
        }

        // cuisines is a comma list like "Italian,Greek"; an empty value means "any"
        public DataTable Search(string cuisines, string city, string state)
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCommand = new SqlCommand();
            objCommand.CommandType = CommandType.StoredProcedure;
            objCommand.CommandText = "TP_SearchRestaurants";
            objCommand.Parameters.AddWithValue("@Cuisines", cuisines);
            objCommand.Parameters.AddWithValue("@City", city);
            objCommand.Parameters.AddWithValue("@State", state);

            DataSet ds = objDB.GetDataSetUsingCmdObj(objCommand);
            return ds.Tables[0];
        }

        // Adds a "RatingText" column to the search results, e.g. "★ 4.2 overall · Food 4.5 · Service 4.0 · $$",
        // or "No reviews yet". One database call for all the restaurants found.
        // Came from Project 2 (ASP.NET 4.6 book, scanned, no page): AddCardColumns + the Rating class.
        public void AddRatings(DataTable results)
        {
            results.Columns.Add("RatingText");
            if (results.Rows.Count == 0)
            {
                return;
            }

            // IDs the search found, as "3,7,12"
            List<string> ids = new List<String>();
            foreach (DataRow row in results.Rows)
            {
                ids.Add(row["RestaurantID"].ToString());
            }

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetSearchRatings";
            cmd.Parameters.AddWithValue("@RestaurantIDs", string.Join(",", ids));
            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            // Match each result to it's ratings row by ID
            foreach (DataRow result in results.Rows)
            {
                result["RatingText"] = "No reviews yet.";
                foreach (DataRow ratingRow in ds.Tables[0].Rows)
                {
                    if (ratingRow["RestaurantID"].ToString() == result["RestaurantID"].ToString())
                    {
                        Rating rating = new Rating(ratingRow);
                        result["RatingText"] = rating.SummaryText();
                        break;
                    }
                }
            }
        }
    }
}