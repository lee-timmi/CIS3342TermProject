using System;
using Utilities;
using System.Data.SqlClient;
using System.Data;

namespace RestaurantReview
{
    public class UserAccount
    {
        // Properties {UserID, Username, Password, First & Last Name, Email, Phone, UserType}
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }

        // Returns true if the username exists
        public bool UsernameExists(string username)
        {
            DBConnect dbConnect = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CheckUsernameExists";
            cmd.Parameters.AddWithValue("@Username", username);

            DataSet dsCheck = dbConnect.GetDataSetUsingCmdObj(cmd);
            int usernameCount = Convert.ToInt32(dsCheck.Tables[0].Rows[0]["UsernameCount"]);
            return usernameCount > 0;
        }

        // Saves the user account to the database. Returns true if successful, false otherwise.
        public bool RegisterUser()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmdAdd = new SqlCommand();
            cmdAdd.CommandType = CommandType.StoredProcedure;
            cmdAdd.CommandText = "AddUser";
            cmdAdd.Parameters.AddWithValue("@Username", this.Username);
            cmdAdd.Parameters.AddWithValue("@Password", this.Password);
            cmdAdd.Parameters.AddWithValue("@FirstName", this.FirstName);
            cmdAdd.Parameters.AddWithValue("@LastName", this.LastName);
            cmdAdd.Parameters.AddWithValue("@Email", this.Email);

            // Phone is optional, an empty string will be stored if not provided
            if (string.IsNullOrEmpty(Phone))
            {
                cmdAdd.Parameters.AddWithValue("@Phone", DBNull.Value);
            } else
            {
                cmdAdd.Parameters.AddWithValue("@Phone", this.Phone);
            }

            cmdAdd.Parameters.AddWithValue("@UserType", this.UserType);

            int rowsAffected = db.DoUpdateUsingCmdObj(cmdAdd);
            return rowsAffected == 1;
        }

        // Validates the user login credentials. Returns true if valid, false otherwise.
        public bool LoginValidate()
        {
            DBConnect dbLogin = new DBConnect();
            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.CommandType = CommandType.StoredProcedure;
            cmdLogin.CommandText = "LoginUser";
            cmdLogin.Parameters.AddWithValue("@Username", this.Username);
            cmdLogin.Parameters.AddWithValue("@Password", this.Password);

            DataSet dsLogin = dbLogin.GetDataSetUsingCmdObj(cmdLogin);

            // If the dataset has no rows, the login is invalid
            if (dsLogin.Tables[0].Rows.Count == 0)
            {
                return false;
            }
           
            DataRow row = dsLogin.Tables[0].Rows[0];
            this.UserID = Convert.ToInt32(row["UserID"]);
            this.FirstName = row["FirstName"].ToString();
            this.LastName = row["LastName"].ToString();
            this.UserType = row["UserType"].ToString();
            return true;
        }
    }
}