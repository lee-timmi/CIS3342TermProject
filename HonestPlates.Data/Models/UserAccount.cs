using System.Data;
using Microsoft.Data.SqlClient;
using System.Net;

namespace HonestPlates.Data.Models
{
    // One user account. Register fills the properties and calls Register();
    // every database call goes through a TP_ stored procedure.
    // (Login methods are added with the Login page.)
    public class UserAccount
    {
        // The questions users pick from when they register (3 different ones)
        public static readonly string[] QuestionChoices =
        {
            "What was the name of your first pet?",
            "What city were you born in?",
            "What is your mother's maiden name?",
            "What was the name of your elementary school?",
            "What was the make of your first car?",
            "What is your favorite food?",
            "What was your childhood nickname?",
            "In what city did your parents meet?"
        };

        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string UserType { get; set; } = "";

        public string VerificationCode { get; set; } = "";
        public string Question1 { get; set; } = "";
        public string Answer1 { get; set; } = "";
        public string Question2 { get; set; } = "";
        public string Answer2 { get; set; } = "";
        public string Question3 { get; set; } = "";
        public string Answer3 { get; set; } = "";

        // True if another account already has this username
        public bool UsernameExists(string username)
        {
            DBConnect usernameDB = new DBConnect();
            SqlCommand usernameCMD = new SqlCommand();
            usernameCMD.CommandType = CommandType.StoredProcedure;
            usernameCMD.CommandText = "TP_CheckUsernameExists";
            usernameCMD.Parameters.AddWithValue("@Username", username);

            DataSet ds = usernameDB.GetDataSetUsingCmdObj(usernameCMD);
            return Convert.ToInt32(ds.Tables[0].Rows[0]["UsernameCount"]) > 0;
        }

        // Saves the user and their 3 security ques
        // Returns true and sets UserID when it worked.
        public bool Register()
        {
            DBConnect registerDB = new DBConnect();
            SqlCommand registerCMD = new SqlCommand();
            registerCMD.CommandType = CommandType.StoredProcedure;
            registerCMD.CommandText = "TP_AddUser";
            registerCMD.Parameters.AddWithValue("@Username", Username);
            registerCMD.Parameters.AddWithValue("@Password", Password);
            registerCMD.Parameters.AddWithValue("@FirstName", FirstName);
            registerCMD.Parameters.AddWithValue("@LastName", LastName);
            registerCMD.Parameters.AddWithValue("@Email", Email);

            // Phone is optional: send a real NULL
            if (Phone == "")
                registerCMD.Parameters.AddWithValue("@Phone", DBNull.Value);
            else
                registerCMD.Parameters.AddWithValue("@Phone", Phone);

            registerCMD.Parameters.AddWithValue("@UserType", UserType);
            registerCMD.Parameters.AddWithValue("@VerificationCode", VerificationCode);
            registerCMD.Parameters.AddWithValue("@Question1", Question1);
            registerCMD.Parameters.AddWithValue("@Answer1", Answer1);
            registerCMD.Parameters.AddWithValue("@Question2", Question2);
            registerCMD.Parameters.AddWithValue("@Answer2", Answer2);
            registerCMD.Parameters.AddWithValue("@Question3", Question3);
            registerCMD.Parameters.AddWithValue("@Answer3", Answer3);

            UserID = Convert.ToInt32(registerDB.ExecuteScalarFunction(registerCMD));
            return UserID > 0;
        }

        // Makes a random 6-digit code, e.g. "482913"
        // Came from Murach's ASP.NET Core MVC, Chapter 20 Page 741 on the Random class.
        public void MakeVerificationCode()
        {
            Random random = new Random();
            VerificationCode = random.Next(100000, 1000000).ToString();
        }

        // Emails the code and the verify link. Throws an exception if the email can't be sent.
        // Came from Project 2 (Prof. Pascucci's Email class); not in the textbooks.
        public void SendVerificationEmail(string verifyLink)
        {
            string body = "<p>Hi " + WebUtility.HtmlEncode(FirstName) + ",</p>"
                + "<p>Welcome to Honest Plates! Your verification code is <b>" + VerificationCode + "</b>.</p>"
                + "<p><a href=\"" + verifyLink + "\">Click here to verify your account</a>, "
                + "or enter the code on the Verify page.</p>";

            HonestPlates.Data.Email objEmail = new HonestPlates.Data.Email();
            objEmail.SendMail(Email, "tut54751@temple.edu", "Verify your Honest Plates account", body);
        }

        // Marks the account verified. Returns false when the username/code don't match
        // (or it was already verified).
        // Came from Project 2 (ASP.NET 4.6 book, scanned, no page): DoUpdateUsingCmdObj returns the rows changed.
        public bool Verify(string username, string code)
        {
            DBConnect db = new DBConnect();
            SqlCommand sqlCMD = new SqlCommand();
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.CommandText = "TP_VerifyAccount";
            sqlCMD.Parameters.AddWithValue("@Username", username);
            sqlCMD.Parameters.AddWithValue("@Code", code);

            return db.DoUpdateUsingCmdObj(sqlCMD) == 1;
        }
    }
}