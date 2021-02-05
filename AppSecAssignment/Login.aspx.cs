using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Drawing;

namespace AppSecAssignment
{
    public partial class Login1 : System.Web.UI.Page
    {
        string MyDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDatabase"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string getAllEmails(string email)
        {
            string e = null;
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT Email FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    e = reader["Email"].ToString();
                }
            }

            return e;
        }

        protected int getLockoutCount(string email)
        {
            int l = 0;
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT LockoutCount, Email FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    l = Int32.Parse(reader["LockoutCount"].ToString());
                }
            }

            return l;
        }

        protected void updateLockoutCount(string email, int count)
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "UPDATE Accounts SET LockoutCount=@Count FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Count", count);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        // Resets lockout count to zero after login 10 minute lock expires
        protected void resetLockoutCount(string email)
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "UPDATE Accounts SET LockoutCount=0 FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        protected void startLockoutTime(string email)
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "UPDATE Accounts SET StartTime=@StartTime FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        protected void resetTime(string email)
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "UPDATE Accounts SET StartTime=@StartTime FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);

            conn.Open();

            cmd.ExecuteNonQuery();
        }

        protected TimeSpan getTimeSpan(string email)
        {
            string st = null;
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT StartTime, Email FROM Accounts WHERE Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    st = reader["StartTime"].ToString();
                }
            }

            TimeSpan timePassed = DateTime.Now - Convert.ToDateTime(st);

            return timePassed;
        }

        // Login function; Verifies hash and salt
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            string email_field = email_tb.Text.ToString().Trim();
            string emailCheck = getAllEmails(email_field);

            string email = HttpUtility.HtmlEncode(email_tb.Text.ToString().Trim());
            string pwd = HttpUtility.HtmlEncode(password_tb.Text.ToString().Trim());

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);

            int lockCount = getLockoutCount(email);

            TimeSpan timePassed = getTimeSpan(email);

            try
            {
                // Check if captcha is correct
                if (ValidateCaptcha())
                {
                    // Check whether email is correct
                    if (0 == 0)
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            // Check for matching hash (correct password)
                            if (userHash.Equals(dbHash))
                            {
                                // If user has less than 3 login failures
                                if (lockCount < 3)
                                {
                                    // Resets lockout count to 0
                                    resetLockoutCount(email);

                                    // Erases start time of lockout
                                    resetTime(email);

                                    // User ID (email) session
                                    Session["Email"] = email;

                                    // Create a new GUID and save into the session
                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    // Now creates a new cookie with this GUID value
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Success.aspx", false);
                                }

                                else
                                {
                                    // Checks for 10 minute account recovery
                                    if (timePassed.Minutes >= 10)
                                    {
                                        // Resets lockout count to 0
                                        resetLockoutCount(email);

                                        // Erases start time of lockout
                                        resetTime(email);

                                        // User ID (email) session
                                        Session["Email"] = email;

                                        // Create a new GUID and save into the session
                                        string guid = Guid.NewGuid().ToString();
                                        Session["AuthToken"] = guid;

                                        // Now creates a new cookie with this GUID value
                                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                        Response.Redirect("Success.aspx", false);
                                    }

                                    else
                                    {
                                        message_lbl.Text = "Your account is currently locked for " + (10 - timePassed.Minutes) + " minutes due to too many login failure. Please try again.";
                                        message_lbl.ForeColor = Color.Red;
                                    }
                                }
                            }

                            // If user enters incorrect email or password
                            else
                            {
                                if (lockCount < 3)
                                {
                                    message_lbl.Text = "Email or password is not valid. Please try again.";
                                    message_lbl.ForeColor = Color.Red;

                                    // Adds one count to account logout max
                                    lockCount += 1;

                                    updateLockoutCount(email, lockCount);
                                }

                                else
                                {
                                    startLockoutTime(email);
                                    message_lbl.Text = "Your account is currently locked for " + (10 - timePassed.Minutes) + " minutes due to too many login failures. Please try again.";
                                    message_lbl.ForeColor = Color.Red;
                                }
                            }
                        }
                    }

                    else
                    {
                        message_lbl.Text = "Email does not exist. Please try again.";
                        message_lbl.ForeColor = Color.Red;
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {

            }
        }

        protected string getDBHash(string email)
        {
            string h = null;

            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT PasswordHash FROM Accounts WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                conn.Close();
            }

            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT PasswordSalt FROM Accounts WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                conn.Close();
            }

            return s;
        }

        // Captcha object
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        // Google captcha v3 validation
        public bool ValidateCaptcha()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter
            // captchaResponse conists of user click pattern
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // To send a GET request to Google along with the response and secret key
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdegD4aAAAAAM1R4WGnsdzo4T5Z1o5z3tSNni6X &response=" + captchaResponse);

            try
            {
                // Codes to receive the response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // Create jsonObject to handle the response (e.g. success/error) and deserialize JSON
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        // Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }

                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
    }
}