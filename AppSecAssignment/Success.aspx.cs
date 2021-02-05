using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class Success : System.Web.UI.Page
    {
        string MyDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDatabase"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] CCNo = null;
        string userID = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Checks session for matching user ID (email), AuthToken and cookies
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                // Checks if AuthToken in session matches AuthToken in cookies
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    // Returns user back to home page if AuthTokens do not match
                    Response.Redirect("Home.aspx", false);
                }

                else
                {
                    // Successful login
                    userID = (string)Session["Email"];
                    message_lbl.Text = "You are currently logged in.";
                    message_lbl.ForeColor = System.Drawing.Color.Green;
                    displayUserProfile(userID);
                }
            }

            else
            {
                // Returns user back to home page if any of the sessions are null
                Response.Redirect("Home.aspx", false);
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                // Creates a decryptor to perform the stream transform
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Reads the decrypted bytes from the decrypting stream and places them in a string
                            plainText = srDecrypt.ReadToEnd();
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
                
            }

            return plainText;
        }

        // Displays user data on success page
        protected void displayUserProfile (string email)
        {
            SqlConnection conn = new SqlConnection(MyDBConnectionString);
            string select = "SELECT * FROM Accounts WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            email_lbl.Text = reader["Email"].ToString();
                        }

                        if (reader["CCNo"] != DBNull.Value)
                        {
                            // Convert Base64 in DB to byte[]
                            CCNo = Convert.FromBase64String(reader["CCNo"].ToString());
                        }

                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }

                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
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
        }

        // Logout function
        protected void logOutBtn_Click(object sender, EventArgs e)
        {
            // Clears session and redirects user back to home page
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["Email"] != null)
            {
                Response.Cookies["Email"].Value = string.Empty;
                Response.Cookies["Email"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            Response.Redirect("Home.aspx", false);
        }
    }
}