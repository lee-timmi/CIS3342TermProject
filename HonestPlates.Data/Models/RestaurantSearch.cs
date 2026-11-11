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
    }
}