using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    internal class clsLogon
    {
        // Connection string
        private const string CONNECT_STRING = @"Server=3.130.26.194;Database=inew233xsu25;User Id=ThameJ25Su233x;Password=hbt95Ts2";
        private static SqlConnection _cntDatabase = new SqlConnection(CONNECT_STRING);

        // Connection helper
        private static SqlConnection GetConnection()
        {
            if (_cntDatabase == null)
                _cntDatabase = new SqlConnection(CONNECT_STRING);

            if (_cntDatabase.State != ConnectionState.Open)
                _cntDatabase.Open();

            return _cntDatabase;
        }

        public static void OpenDatabase()
        {
            try
            {
                if (_cntDatabase == null || _cntDatabase.State == ConnectionState.Closed)
                {
                    _cntDatabase = new SqlConnection(CONNECT_STRING);
                }
                if (_cntDatabase.State != ConnectionState.Open)
                {
                    _cntDatabase.Open();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error opening database: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CloseDatabase()
        {
            try
            {
                _cntDatabase.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error closing database: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Validate user credentials for login
        public static bool ValidateUserCredentials(string username, string password, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                OpenDatabase();
                string query = @"SELECT AccountDisabled, AccountDeleted FROM ThameJ25Su233x.Logon WHERE LogonName = @Username AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool disabled = reader["AccountDisabled"] != DBNull.Value && (bool)reader["AccountDisabled"];
                            bool deleted = reader["AccountDeleted"] != DBNull.Value && (bool)reader["AccountDeleted"];

                            if (deleted)
                            {
                                errorMessage = "Your account has been deleted.";
                                return false;
                            }
                            if (disabled)
                            {
                                errorMessage = "Your account has been disabled.";
                                return false;
                            }
                            return true;
                        }
                        errorMessage = "Invalid username or password.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error validating credentials:\n" + ex.Message;
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // Get PersonID by username 
        public static string GetUserPersonID(string username)
        {
            try
            {
                OpenDatabase();
                string query = @"SELECT PersonID FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving user PersonID:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Unknown";
            }
            finally
            {
                CloseDatabase();
            }
        }

        public static string GetUserPositionTitle(string username)
        {
            try
            {
                OpenDatabase();
                string query = @"SELECT PositionTitle FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving position title:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            finally
            {
                CloseDatabase();
            }
        }
    }
}