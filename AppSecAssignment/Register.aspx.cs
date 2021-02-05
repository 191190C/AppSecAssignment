using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions;
using System.Drawing;

using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace AppSecAssignment
{
    public partial class Register : System.Web.UI.Page
    {
        // Connection string for SQL
        string MyDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDatabase"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private bool emailCheck()
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "Select * from Accounts where Email=@Email";

            SqlCommand cmd = new SqlCommand(select, conn);

            cmd.Parameters.AddWithValue("@Email", email_tb.Text);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Make sure that email has a row in the database (exists)
                if (dr.HasRows == true)
                {
                    error_lbl.Text = "Email already exists.";
                    error_lbl.ForeColor = Color.Red;
                    return true;
                }
            }
            return false;
        }

        // Server-side scoring system for password strength
        private int checkPasssword(string password)
        {
            int score = 0;

            if (password.Length < 8) // Score 0: Extremely Weak
            {
                return 1;
            }

            else
            {
                score = 1; // Score 1: Very Weak
            }

            // Score 2: Weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            // Score 3: Medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            // Score 4: Strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            // Score 5: Excellent
            if (Regex.IsMatch(password, ".!@#$%^&*()_+-="))
            {
                score++;
            }

            return score;
        }

        // Server-side password validation
        protected void checkPasswordBtn_Click(object sender, EventArgs e)
        {
            int scores = checkPasssword(password_tb.Text);
            string status = "";

            switch (scores)
            {
                case 1:
                    status = "Very weak";
                    break;

                case 2:
                    status = "Weak";
                    break;

                case 3:
                    status = "Medium";
                    break;

                case 4:
                    status = "Strong";
                    break;

                case 5:
                    status = "Excellent";
                    break;
            }

            pwdRating_lbl.Text = "Status: " + status;
            if (scores < 4)
            {
                pwdRating_lbl.ForeColor = Color.Red;
                return;
            }

            else
            {
                pwdRating_lbl.ForeColor = Color.Green;
            }
        }

        // Creates a password hash and salt and creates an account
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            if (!emailCheck())
            {
                // Get value from textbox
                string pwd = password_tb.Text.ToString().Trim();

                // Generate random salt
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                // Fill array of bytes with a cryptographically strong sequence of random values
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                createAccount();
            }
            else
            {
                error_lbl.Text = "Email already exists.";
                error_lbl.ForeColor = Color.Red;
            }           
        }

        // Adds a new entry to the Accounts table
        public void createAccount()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(MyDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Accounts VALUES(@FirstName, @LastName, @CCNo, @Email, @PasswordHash, @PasswordSalt, @DOB, @IV, @Key, @LockoutCount, @StartTime)"))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            if (!emailCheck())
                            {
                                // Parameterize each value to prevent SQLi and perform HtmlEncode to prevent XSS
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(fname_tb.Text.Trim()));
                                cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(lname_tb.Text.Trim()));
                                cmd.Parameters.AddWithValue("@CCNo", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(ccno_tb.Text.Trim()))));
                                cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_tb.Text.Trim()));
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(dob_tb.Text.Trim()));
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                                cmd.Parameters.AddWithValue("@LockoutCount", 0);
                                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                                cmd.Connection = conn;

                                try
                                {
                                    conn.Open();
                                    cmd.ExecuteNonQuery();

                                    error_lbl.Text = "Account successfully registered.";
                                    error_lbl.ForeColor = Color.Green;
                                }

                                catch (Exception ex)
                                {
                                    error_lbl.Text = "Registration failed. Please try again.";
                                }

                                finally
                                {
                                    conn.Close();
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Encrypts the given data
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            { 

            }

            return cipherText;
        }

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