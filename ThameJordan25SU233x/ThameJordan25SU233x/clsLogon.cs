using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace ThameJordan25SU233x
{
    internal class clsLogon
    {
        // All DB access goes through clsSQL so the connection string lives in one place
        public static void OpenDatabase() { }
        public static void CloseDatabase() { }

        // Checks username and password, returns true if valid
        public static bool ValidateUserCredentials(string username, string password, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (var cn = clsSQL.GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT AccountDisabled, AccountDeleted FROM Logon WHERE LogonName = @Username AND Password = @Password";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            bool disabled = !r.IsDBNull(0) && r.GetInt64(0) == 1;
                            bool deleted = !r.IsDBNull(1) && r.GetInt64(1) == 1;
                            if (deleted) { errorMessage = "Your account has been deleted."; return false; }
                            if (disabled) { errorMessage = "Your account has been disabled."; return false; }
                            return true;
                        }
                        errorMessage = "Invalid username or password.";
                        return false;
                    }
                }
            }
            catch (Exception ex) { errorMessage = "Error validating credentials:\n" + ex.Message; return false; }
        }

        // Returns the PersonID for a given username
        public static string GetUserPersonID(string username)
        {
            try
            {
                using (var cn = clsSQL.GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PersonID FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown";
                }
            }
            catch (Exception ex) { MessageBox.Show("Error retrieving user PersonID:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return "Unknown"; }
        }

        // Returns the position title for a given username
        public static string GetUserPositionTitle(string username)
        {
            try
            {
                using (var cn = clsSQL.GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PositionTitle FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteScalar()?.ToString() ?? "";
                }
            }
            catch (Exception ex) { MessageBox.Show("Error retrieving position title:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return ""; }
        }
    }
}
