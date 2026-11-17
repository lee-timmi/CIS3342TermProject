using System.Data;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Formats.Asn1;
using System.Transactions;

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
        public bool IsVerified { get; set; }
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
            // Passwords and answers are saved encrypted (Encryption.cs, AES-256 with a random IV).
            registerCMD.Parameters.AddWithValue("@Password", Encryption.Encrypt(Password));
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
            registerCMD.Parameters.AddWithValue("@Answer1", Encryption.Encrypt(CleanAnswer(Answer1)));
            registerCMD.Parameters.AddWithValue("@Question2", Question2);
            registerCMD.Parameters.AddWithValue("@Answer2", Encryption.Encrypt(CleanAnswer(Answer2)));
            registerCMD.Parameters.AddWithValue("@Question3", Question3);
            registerCMD.Parameters.AddWithValue("@Answer3", Encryption.Encrypt(CleanAnswer(Answer3)));

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

        // Answers compared without extra spaces or capitals, so "Fluffy" = "fluffy"
        public static string CleanAnswer(string answer)
        {
            return answer.Trim().ToLower();
        }

        // For login page: true when typed password MATCHES the encrypted one.
        public static bool CheckPassword(string savedPassword, string typedPassword)
        {
            try
            {
                return Encryption.Decrypt(savedPassword) == typedPassword;
            }
            catch
            {
                // Saved value != encrypted text. No match.
                return false;
            }
        }

        // For forgot username / reset password
        public static bool CheckAnswer(string savedAnswer, string typedAnswer)
        {
            try
            {
                return Encryption.Decrypt(savedAnswer) == CleanAnswer(typedAnswer);
            }
            catch
            {
                return false;
            }
        }

        // ONE-TIME: encrypts every password and answer that's still plain text.
        // Safe to run twice: values that are already encrypted are skipped. Returns how many were changed.
        // Came from Project 2 (ASP.NET 4.6 book, scanned, no page): looping a DataSet and running updates.
        public static int EncryptExistingAccounts()
        {
            int count = 0;
            DBConnect db = new DBConnect();

            // Password
            SqlCommand getPasswords = new SqlCommand();
            getPasswords.CommandType = CommandType.StoredProcedure;
            getPasswords.CommandText = "TP_GetAllPasswords";
            DataSet passwords = db.GetDataSetUsingCmdObj(getPasswords);

            foreach (DataRow passRow in passwords.Tables[0].Rows)
            {
                string saved = passRow["Password"].ToString();
                if (!IsEncrypted(saved))
                {
                    SqlCommand update = new SqlCommand();
                    update.CommandType = CommandType.StoredProcedure;
                    update.CommandText = "TP_UpdatePassword";
                    update.Parameters.AddWithValue("@UserID", passRow["UserID"]);
                    update.Parameters.AddWithValue("@Password", Encryption.Encrypt(saved));

                    if (db.DoUpdateUsingCmdObj(update) == 1)
                    {
                        count++;

                    }
                }
            }

            // Security answers (cleaned first, like new registration)
            SqlCommand getAnswers = new SqlCommand();
            getAnswers.CommandType = CommandType.StoredProcedure;
            getAnswers.CommandText = "TP_GetAllAnswers";
            DataSet answers = db.GetDataSetUsingCmdObj(getAnswers);

            foreach (DataRow ansRow in answers.Tables[0].Rows)
            {
                string savedAnswer = ansRow["Answer"].ToString();
                if (!IsEncrypted(savedAnswer))
                {
                    SqlCommand updateAnswer = new SqlCommand();
                    updateAnswer.CommandType = CommandType.StoredProcedure;
                    updateAnswer.CommandText = "TP_UpdateAnswer";
                    updateAnswer.Parameters.AddWithValue("@QuestionID", ansRow["QuestionID"]);
                    updateAnswer.Parameters.AddWithValue("@Answer", Encryption.Encrypt(CleanAnswer(savedAnswer)));

                    if (db.DoUpdateUsingCmdObj(updateAnswer) == 1)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool IsEncrypted(string value)
        {
            try
            {
                Encryption.Decrypt(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Checks the username and password. Returns true and fills UserID, FirstName,
        // UserType and IsVerified when they match; false when either one is wrong.
        // Came from Project 2 (ASP.NET 4.6 book, scanned, no page): LoginValidate with Rows.Count before Rows[0].
        public bool Login (string username, string typedPassword)
        {
            DBConnect loginDB = new DBConnect();
            SqlCommand loginCMD = new SqlCommand();
            loginCMD.CommandType = CommandType.StoredProcedure;
            loginCMD.CommandText = "TP_GetLoginInfo";
            loginCMD.Parameters.AddWithValue("@Username", username);

            DataSet ds = loginDB.GetDataSetUsingCmdObj(loginCMD);

            // No such username
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            // Wrong password (saved is encrypted; CheckPassword decrypts and compares)
            DataRow row = ds.Tables[0].Rows[0];
            if (!CheckPassword(row["Password"].ToString(), typedPassword))
            {
                return false;
            }

            UserID = Convert.ToInt32(row["UserID"]);
            Username = username;
            FirstName = row["FirstName"].ToString();
            UserType = row["UserType"].ToString();
            IsVerified = Convert.ToBoolean(row["IsVerified"]);
            return true;
        }

        // -------------- FORGOT username / RESET password --------------

        // Finds the account that uses this email. Fills UserID and Username; false if none.
        public bool FindByEmail(string email)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetUserByEmail";
            cmd.Parameters.AddWithValue("@Email", email);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
            Username = ds.Tables[0].Rows[0]["Username"].ToString();
            return true;
        }

        // Finds the account with this username (reuses TP_GetLoginInfo). Fills UserID and Username; false if none.
        public bool FindByUsername(string username)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetLoginInfo";
            cmd.Parameters.AddWithValue("@Username", username);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
            Username = username;
            return true;
        }

        // One of the user's security questions. Picked at random.
        // Returns the row (questionID, questionText), or null if no questions exists for user.
        // Came from Murach's ASP.NET Core MVC, Appendix B Page 741 (a Random example).
        public DataRow GetRandomQuestion()
        {
            DataTable questions = GetSecurityQuestions();
            if (questions.Rows.Count == 0)
            {
                return null;
            }
            Random random = new Random();
            return questions.Rows[random.Next(questions.Rows.Count)];
        }

        // True when the typed answer matches the saved (encrypted) answer to that question.
        // The question is looked up again here, so the answer never has to be kept in Session.
        public bool CheckQuestionAnswer(int questionID, string typedAnswer)
        {
            DataTable questions = GetSecurityQuestions();
            foreach (DataRow row in questions.Rows)
            {
                if (Convert.ToInt32(row["QuestionID"]) == questionID)
                {
                    return CheckAnswer(row["Answer"].ToString(), typedAnswer);
                }
            }
            return false;
        }

        // Saves a new password for this user, encrypted (reuses TP_UpdatePassword). True when it saved.
        public bool UpdatePassword(string newPassword)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_UpdatePassword";
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@Password", Encryption.Encrypt(newPassword));

            return db.DoUpdateUsingCmdObj(cmd) == 1;
        }

        // The user's 3 security questions
        private DataTable GetSecurityQuestions()
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetSecurityQuestions";
            cmd.Parameters.AddWithValue("@UserID", UserID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables[0];
        }

    }
}