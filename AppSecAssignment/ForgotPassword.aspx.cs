using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MyDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDatabase"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        protected void changePasswordBtn_Click(object sender, EventArgs e)
        {
            try
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


                string email = email_tb.Text.ToString().Trim();

                SqlConnection conn = new SqlConnection(MyDBConnectionString);
                string select = "UPDATE Accounts SET PasswordHash=@PasswordHash, PasswordSalt=@PasswordSalt FROM Accounts WHERE Email=@Email";

                conn.Open();

                SqlCommand cmd = new SqlCommand(select, conn);
                cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email));
                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", salt);

                cmd.ExecuteNonQuery();

                message_lbl.Text = "Password successfully changed.";
                message_lbl.ForeColor = Color.Green;
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
    }
}