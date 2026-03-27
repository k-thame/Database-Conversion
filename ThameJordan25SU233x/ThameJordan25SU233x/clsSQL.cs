using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections;
using System.IO;
using ACS_JThameM7;



namespace ThameJordan25SU233x
{
    // SQL Connections
    internal class clsSQL
    {
        // - - - Variables

        // Private connection string
        private const string CONNECT_STRING = @"Server=3.130.26.194;" + "Database=inew233xsu25;User Id=ThameJ25Su233x;Password=hbt95Ts2";

        // Connection to database
        private static SqlConnection _cntDatabase = new SqlConnection(CONNECT_STRING);

        // Command Object
        private static SqlCommand _sqlCommand;

        // Data adapter
        private static SqlDataAdapter _daResults = new SqlDataAdapter();

        // Data table 
        private static DataTable _dtResultsTable = new DataTable();

        // String builder for detailed error display
        private static StringBuilder errorMessages = new StringBuilder();

        // Getter and Setter for DataTable(s)
        public static DataTable DTResultsTable
        {
            get { return _dtResultsTable; }
            set { _dtResultsTable = value; }
        }

        // Connection for query insert
        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(CONNECT_STRING);
            connection.Open();
            return connection;
        }

        // Method to open database and allow data access
        public static void OpenDatabase()
        {
            try
            {
                // Reinitialize if null or disposed
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
                errorMessages.Clear();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    // Specific SQL exceptions
                    errorMessages.Append("Index #" + i + "\n"
                        + "Message - " + ex.Errors[i].Message + "\n"
                        + "Line Number: " + ex.Errors[i].LineNumber + "\n"
                        + "Source: " + ex.Errors[i].Source + "\n"
                        + "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                // Error display
                MessageBox.Show(errorMessages.ToString(), "Error on OpenDatabase!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to close database and dispose all data
        public static void CloseDatabase()
        {
            // Try and Catch
            try
            {
                // Close database
                _cntDatabase.Close();
            }
            catch (SqlException ex)
            {
                // Catch errors
                if (ex is SqlException)
                {
                    // Specific SQL exceptions 
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n"
                            + "Message - " + ex.Errors[i].Message + "\n"
                            + "Line Number: " + ex.Errors[i].LineNumber + "\n"
                            + "Source: " + ex.Errors[i].Source + "\n"
                            + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    // Display errorMessages to user
                    MessageBox.Show(errorMessages.ToString(), "Error on CloseDatabase!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Error display
                    MessageBox.Show(ex.Message, "Error! Unable to fulfill CloseDatabase!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Database Command
        public static void DatabaseCommand(String query)
        {
            try
            {
                // Ensure the connection is open before command
                if (_cntDatabase.State != ConnectionState.Open)
                {
                    _cntDatabase.Open();
                }

                // Reset data adapter and datatable to NEW
                _sqlCommand = null;
                _daResults = new SqlDataAdapter();
                _dtResultsTable = new DataTable();

                // Establish command object
                _sqlCommand = new SqlCommand(query, _cntDatabase);

                // Establish data adapters
                _daResults.SelectCommand = _sqlCommand;

                // Fill data table
                _daResults.Fill(_dtResultsTable);
            }
            catch (SqlException ex)
            {
                // Specific SQL exceptions 
                errorMessages.Clear();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n"
                        + "Message - " + ex.Errors[i].Message + "\n"
                        + "Line Number: " + ex.Errors[i].LineNumber + "\n"
                        + "Source: " + ex.Errors[i].Source + "\n"
                        + "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                // Error display
                MessageBox.Show(errorMessages.ToString(), "Error on DatabaseCommand!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Dispose data
                if (_sqlCommand != null) _sqlCommand.Dispose();
                if (_daResults != null) _daResults.Dispose();
            }
        }

        // Return Command
        public static SqlConnection GetConnection()
        {
            if (_cntDatabase == null)
                _cntDatabase = new SqlConnection(CONNECT_STRING);

            if (_cntDatabase.State != ConnectionState.Open)
                _cntDatabase.Open();

            return _cntDatabase;
        }

        // frmLogon         - Validate User Credentials
        public static bool ValidateUserCredentials(string username, string password, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                OpenDatabase();
                string query = @"SELECT AccountDisabled, AccountDeleted  FROM ThameJ25Su233x.Logon WHERE LogonName = @Username AND Password = @Password";

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

        // frmLogon         - Detect users personal ID
        public static string GetUserPersonID(string personID)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Query for finding users position title followed by an SQL Command
                string query = @"SELECT PersonID FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", personID);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error retrieving users personal ID:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Unknown";
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmNewAccount        - Load Security Questions
        public static Dictionary<int, DataTable> GetSecurityQuestionSets()
        {
            // Variable for holding the security questions
            var sets = new Dictionary<int, DataTable>();

            try
            {
                // Open database
                OpenDatabase();

                // For loop for security questions sets
                for (int setId = 1; setId <= 3; setId++)
                {
                    string query = $"SELECT QuestionID, QuestionPrompt FROM ThameJ25Su233x.SecurityQuestions WHERE SetID = {setId}";
                    DatabaseCommand(query);
                    sets[setId] = DTResultsTable.Copy();
                }
            }
            catch (Exception ex)
            {
                // Error message 
                MessageBox.Show("Failed to load security questions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close database
                CloseDatabase();
            }

            // Return
            return sets;
        }

        // frmNewAccount        - Create New Account
        public static bool CreateNewAccount(Dictionary<string, object> userData, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                OpenDatabase();

                string positionTitle = "Customer";
                if (userData.TryGetValue("PositionTitle", out var posObj) && posObj != null)
                {
                    if (posObj is System.Windows.Forms.ComboBox combo && combo.SelectedItem != null)
                    {
                        positionTitle = combo.SelectedItem.ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(posObj.ToString()))
                    {
                        positionTitle = posObj.ToString();
                    }
                }

                using (SqlCommand seed = new SqlCommand(@"IF NOT EXISTS (SELECT 1 FROM ThameJ25Su233x.Position WHERE PositionTitle = @Title) INSERT INTO ThameJ25Su233x.Position (PositionTitle) VALUES (@Title);", GetConnection()))
                {
                    seed.Parameters.AddWithValue("@Title", positionTitle);
                    seed.ExecuteNonQuery();
                }

                int positionID;
                using (SqlCommand lookup = new SqlCommand(
                    "SELECT PositionID FROM ThameJ25Su233x.Position WHERE PositionTitle = @Title;",
                    GetConnection()))
                {
                    lookup.Parameters.AddWithValue("@Title", positionTitle);
                    positionID = Convert.ToInt32(lookup.ExecuteScalar());
                }

                int personID;
                using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ThameJ25Su233x.Person (
                            Title, NameFirst, NameMiddle, NameLast, Suffix,
                            Address1, Address2, Address3, City, Zipcode, State,
                            Email, PhonePrimary, PhoneSecondary, PositionID
                        )
                        VALUES (
                            @Title, @NameFirst, @NameMiddle, @NameLast, @Suffix,
                            @Address1, @Address2, @Address3, @City, @Zipcode, @State,
                            @Email, @PhonePrimary, @PhoneSecondary, @PositionID
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Title", userData["Title"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@NameFirst", userData["FirstName"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@NameMiddle", userData["MiddleName"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@NameLast", userData["LastName"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Suffix", userData["Suffix"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Address1", userData["Address1"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Address2", userData["Address2"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Address3", userData["Address3"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@City", userData["City"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Zipcode", userData["Zipcode"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@State", userData["State"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Email", userData["Email"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@PhonePrimary", userData["PhonePrimary"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@PhoneSecondary", userData["PhoneSecondary"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@PositionID", positionID);

                    personID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                object firstQ = userData.TryGetValue("FirstChallengeQuestion", out var fq) ? fq : userData.TryGetValue("Q1ID", out var q1) ? q1 : null;
                object firstA = userData.TryGetValue("FirstChallengeAnswer", out var fa) ? fa : userData.TryGetValue("Q1A", out var a1) ? a1 : null;
                object secondQ = userData.TryGetValue("SecondChallengeQuestion", out var sq) ? sq : userData.TryGetValue("Q2ID", out var q2) ? q2 : null;
                object secondA = userData.TryGetValue("SecondChallengeAnswer", out var sa) ? sa : userData.TryGetValue("Q2A", out var a2) ? a2 : null;
                object thirdQ = userData.TryGetValue("ThirdChallengeQuestion", out var tq) ? tq : userData.TryGetValue("Q3ID", out var q3) ? q3 : null;
                object thirdA = userData.TryGetValue("ThirdChallengeAnswer", out var ta) ? ta : userData.TryGetValue("Q3A", out var a3) ? a3 : null;

                using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ThameJ25Su233x.Logon (
                            PersonID, LogonName, Password, PositionTitle,
                            FirstChallengeQuestion, FirstChallengeAnswer,
                            SecondChallengeQuestion, SecondChallengeAnswer,
                            ThirdChallengeQuestion, ThirdChallengeAnswer
                        )
                        VALUES (
                            @PersonID, @LogonName, @Password, @PositionTitle,
                            @FirstQ, @FirstA, @SecondQ, @SecondA, @ThirdQ, @ThirdA
                        );", GetConnection()))
                {
                    string logonName = userData.TryGetValue("LogonName", out var ln) ? ln?.ToString()
                                      : userData.TryGetValue("Username", out var un) ? un?.ToString()
                                      : "";
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@LogonName", logonName ?? "");
                    cmd.Parameters.AddWithValue("@Password", userData["Password"]?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);

                    cmd.Parameters.AddWithValue("@FirstQ", firstQ ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstA", firstA?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@SecondQ", secondQ ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondA", secondA?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@ThirdQ", thirdQ ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ThirdA", thirdA?.ToString() ?? "");

                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Error creating account: " + ex.Message;
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmNewAccount        - Check if username has been taken
        public static bool CheckUsernameExists(string username)
        {
            // bool variable
            bool exists = false;

            try
            {
                // Open database
                OpenDatabase();

                // Query for matching user username input with database username values followed by an SQL Command
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ThameJ25Su233x.Logon WHERE LogonName = @Username", GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    exists = (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                //    // Error message
                //    MessageBox.Show("Could not check username: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Return
                return true;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }

            // Return
            return exists;
        }

        // frmPasswordReset     - Check if username exists
        public static bool UsernameExists(string username)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Query for matching user input with database values followed by an SQL Command
                string query = @"SELECT COUNT(*) FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error checking username: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmPasswordReset     - Find security question prompts associated with user
        private static string GetPromptText(int questionID)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Query for matching user input with database values followed by an SQL Command
                string query = @"SELECT QuestionPrompt FROM ThameJ25Su233x.SecurityQuestions WHERE QuestionID = @QuestionID";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionID);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "[Prompt not found]";
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error retrieving question prompt: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "[Prompt error]";
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmPasswordReset     - Gather security question prompts
        public static List<string> GetSecurityQuestionPrompts(string username)
        {
            List<string> prompts = new List<string>();

            try
            {
                // Open database
                OpenDatabase();

                // Query for matching user input with database values followed by an SQL Command
                string idQuery = @"SELECT FirstChallengeQuestion, SecondChallengeQuestion, ThirdChallengeQuestion FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmdIDs = new SqlCommand(idQuery, GetConnection()))
                {
                    cmdIDs.Parameters.AddWithValue("@Username", username);

                    // Assign prompts
                    using (SqlDataReader reader = cmdIDs.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int q1 = reader.GetInt32(0);
                            int q2 = reader.GetInt32(1);
                            int q3 = reader.GetInt32(2);
                            reader.Close();

                            prompts.Add(GetPromptText(q1));
                            prompts.Add(GetPromptText(q2));
                            prompts.Add(GetPromptText(q3));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error loading security question prompts: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // CLose database
                CloseDatabase();
            }

            // Return
            return prompts;
        }

        // frmPasswordReset     - Validate user's security question answers
        public static bool ValidateSecurityAnswers(string username, string ans1, string ans2, string ans3, out string errorMessage)
        {
            // Error message container
            errorMessage = "";

            // Check if user has entered any values for the security question answers
            if (string.IsNullOrWhiteSpace(ans1))
            {
                errorMessage = "Please enter an answer for security question 1.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ans2))
            {
                errorMessage = "Please enter an answer for security question 2.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ans3))
            {
                errorMessage = "Please enter an answer for security question 3.";
                return false;
            }

            try
            {
                // Open database
                OpenDatabase();

                // Find user's associated security question answers, then validate, following SQL Command
                string query = @"SELECT FirstChallengeAnswer, SecondChallengeAnswer, ThirdChallengeAnswer FROM ThameJ25Su233x.Logon WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Gather security question answers from database
                            string correctAns1 = reader["FirstChallengeAnswer"].ToString().Trim();
                            string correctAns2 = reader["SecondChallengeAnswer"].ToString().Trim();
                            string correctAns3 = reader["ThirdChallengeAnswer"].ToString().Trim();

                            // Match user input security question answers with database values
                            if (!string.Equals(ans1.Trim(), correctAns1, StringComparison.OrdinalIgnoreCase))
                            {
                                errorMessage = "Security answer 1 is incorrect.";
                                return false;
                            }
                            if (!string.Equals(ans2.Trim(), correctAns2, StringComparison.OrdinalIgnoreCase))
                            {
                                errorMessage = "Security answer 2 is incorrect.";
                                return false;
                            }
                            if (!string.Equals(ans3.Trim(), correctAns3, StringComparison.OrdinalIgnoreCase))
                            {
                                errorMessage = "Security answer 3 is incorrect.";
                                return false;
                            }

                            // Return success
                            return true;
                        }
                        else
                        {
                            // Alert user, then return
                            errorMessage = "User not found.";
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Error message
                errorMessage = "Error validating answers: " + ex.Message;
                return false;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmPasswordReset     - Resetting the user's password
        public static bool ResetUserPassword(string username, string newPassword)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Change the users old password with the new one, following SQL Command
                string query = @"UPDATE ThameJ25Su233x.Logon SET Password = @Password WHERE LogonName = @Username";
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Username", username);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error resetting password: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmShopping     - Populate the DGV
        private static DataTable imageDataTable;
        public static DataTable PopulateDGV(DataGridView dgv)
        {
            try
            {
                OpenDatabase();

                string query = @"SELECT InventoryID, ItemName, ItemDescription, RetailPrice, Quantity, ItemImage FROM ThameJ25Su233x.Inventory WHERE Discontinued = 0";
                SqlCommand cmd = new SqlCommand(query, GetConnection());
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Keep copy for image data
                DataTable dtCopy = dt.Copy();

                // Remove image column before binding
                dt.Columns.Remove("ItemImage");

                // Bind data to DGV
                dgv.AllowUserToAddRows = false;
                dgv.DataSource = dt;

                // Add image column
                if (!dgv.Columns.Contains("ProductImage"))
                {
                    DataGridViewImageColumn imageCol = new DataGridViewImageColumn
                    {
                        Name = "ProductImage",
                        HeaderText = "Product Image",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    dgv.Columns.Insert(0, imageCol);
                }


                // Populate images immediately
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    byte[] imgBytes = dtCopy.Rows[i]["ItemImage"] as byte[];
                    if (imgBytes != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            dgv.Rows[i].Cells["ProductImage"].Value = Image.FromStream(ms);
                        }
                    }
                }

                // Column headers
                dgv.Columns["InventoryID"].Visible = false;
                dgv.Columns["ItemName"].HeaderText = "Product Name";
                dgv.Columns["ItemDescription"].HeaderText = "Description";
                dgv.Columns["RetailPrice"].HeaderText = "Price";
                dgv.Columns["Quantity"].HeaderText = "Stock Left";

                // Layout
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.RowTemplate.Height = 65;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                return dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Unable to display products!\n\n" + ex.Message);
                return null;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmShopping     - Categories
        public DataTable GetAllCategories()
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM ThameJ25Su233x.Categories", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // frmShopping     - Searches
        public DataTable SearchAndFilterInventory(string searchText, int? categoryId)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();

                string query = @"SELECT InventoryID, ItemName, ItemDescription, RetailPrice, Quantity, ItemImage 
                FROM ThameJ25Su233x.Inventory 
                WHERE (@searchText IS NULL OR (ItemName LIKE '%' + @searchText + '%')) AND (@categoryId IS NULL OR CategoryID = @categoryId)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@searchText", string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText);
                cmd.Parameters.AddWithValue("@categoryId", categoryId == null ? (object)DBNull.Value : categoryId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // frmShopping     - Reduce inventory once user adds to cart
        public static bool DeductInventoryQuantity(string itemName, int amount)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Query for obtaining data
                string query = @"UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity - @Amount WHERE ItemName = @ItemName AND Quantity >= @Amount";

                //
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error updating inventory: " + ex.Message, "Inventory Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmShopping     - Add removed items back to inventory
        public static bool AddInventoryQuantity(string itemName, int quantity)
        {
            try
            {
                // Open database
                OpenDatabase();

                // Query for obtaining data
                string query = @"UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity + @Quantity WHERE ItemName = @ItemName";

                SqlCommand cmd = new SqlCommand(query, GetConnection());
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@ItemName", itemName);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch
            {
                // Return
                return false;
            }
            finally
            {
                // Close database
                CloseDatabase();
            }
        }

        // frmShopping     - When user clears cart
        public static void RestockItem(int itemID, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
            {
                // Query for obtaining data
                string query = "UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity + @Quantity WHERE InventoryID = @InventoryID";

                // SQL Commands
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InventoryID", itemID);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // frmShopping     - Filter/Searc DGV
        public static DataTable GetFilteredProducts(string searchTerm, int? categoryID)
        {
            using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
            {
                conn.Open();

                // Base query with joins if needed
                string sql = "SELECT * FROM Products WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    sql += " AND ItemName LIKE @SearchTerm";

                if (categoryID.HasValue && categoryID.Value > 0)
                    sql += " AND CategoryID = @CategoryID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    if (categoryID.HasValue && categoryID.Value > 0)
                        cmd.Parameters.AddWithValue("@CategoryID", categoryID.Value);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        //
        public class DiscountInfo
        {
            public int DiscountID { get; set; }
            public decimal DiscountPercentage { get; set; }
            public decimal DiscountDollarAmount { get; set; }
            public int DiscountLevel { get; set; }
        }

        // frmShopping     - Discount Codes
        public static decimal DiscountCode(string code)
        {
            // default no discount
            decimal discountRate = 0m;

            try
            {
                //
                OpenDatabase();

                //
                string query = "SELECT DiscountRate FROM ThameJ25Su233x.Discounts WHERE DiscountCode = @DiscountCode";

                //
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@DiscountCode", code.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null && decimal.TryParse(result.ToString(), out decimal rate))
                    {
                        discountRate = rate;
                    }
                }
            }
            catch (Exception ex)
            {
                discountRate = 0m;
            }
            finally
            {
                //
                CloseDatabase();
            }

            //
            return discountRate;
        }

        // Gather discount info
        public static DiscountInfo GetDiscountInfo(string code)
        {
            DiscountInfo discount = new DiscountInfo();

            try
            {
                //
                OpenDatabase();

                string query = @"SELECT DiscountID, DiscountPercentage, DiscountDollarAmount FROM ThameJ25Su233x.Discounts WHERE DiscountCode = @code";

                SqlCommand cmd = new SqlCommand(query, GetConnection());
                cmd.Parameters.AddWithValue("@code", code);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    discount.DiscountID = reader["DiscountID"] != DBNull.Value
                        ? Convert.ToInt32(reader["DiscountID"]) : 0;

                    discount.DiscountPercentage = reader["DiscountPercentage"] != DBNull.Value
                        ? Convert.ToDecimal(reader["DiscountPercentage"]) : 0;

                    discount.DiscountDollarAmount = reader["DiscountDollarAmount"] != DBNull.Value
                        ? Convert.ToDecimal(reader["DiscountDollarAmount"]) : 0;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching discount info: " + ex.Message);
            }

            return discount;
        }

        //
        public static List<clsItemDiscount> GetItemDiscounts(string code)
        {
            var discounts = new List<clsItemDiscount>();

            try
            {
                OpenDatabase();

                string query = @"SELECT InventoryID, DiscountID, DiscountPercentage, DiscountDollarAmount FROM ThameJ25Su233x.Discounts WHERE DiscountCode = @DiscountCode AND GETDATE() BETWEEN StartDate AND ExpirationDate";

                using (var cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@DiscountCode", code);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["InventoryID"] != DBNull.Value)
                            {
                                discounts.Add(new clsItemDiscount
                                {
                                    ItemID = Convert.ToInt32(reader["InventoryID"]),
                                    DiscountID = Convert.ToInt32(reader["DiscountID"]),
                                    DiscountPercentage = reader["DiscountPercentage"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountPercentage"]) : 0,
                                    DiscountDollarAmount = reader["DiscountDollarAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountDollarAmount"]) : 0
                                });
                            }
                        }
                    }
                }
            }
            finally
            {
                CloseDatabase();
            }

            return discounts;
        }

        //
        public static bool IsCartLevelDiscount(string code)
        {
            try
            {
                OpenDatabase();

                const string sql = @"SELECT COUNT(*) FROM ThameJ25Su233x.Discounts WHERE DiscountCode = @code AND DiscountLevel = 1 AND InventoryID IS NULL AND GETDATE() BETWEEN StartDate AND ExpirationDate";

                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@code", code ?? string.Empty);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        //
        public static int InsertOrder(string personID, string creditCardNumber, string expirationDate, string cvv, int? employeeId = null)
        {
            try
            {
                OpenDatabase();

                string sql =
                    employeeId.HasValue
                    ? @"INSERT INTO ThameJ25Su233x.Orders (PersonID, EmployeeID, OrderDate, CC_Number, ExpDate, CCV)
                        VALUES (@PersonID, @EmployeeID, GETDATE(), @CC_Number, @ExpDate, @CCV);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);"
                    : @"INSERT INTO ThameJ25Su233x.Orders (PersonID, OrderDate, CC_Number, ExpDate, CCV)
                        VALUES (@PersonID, GETDATE(), @CC_Number, @ExpDate, @CCV);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    if (employeeId.HasValue) cmd.Parameters.AddWithValue("@EmployeeID", employeeId.Value);
                    cmd.Parameters.AddWithValue("@CC_Number", creditCardNumber ?? "");
                    cmd.Parameters.AddWithValue("@ExpDate", expirationDate ?? "");
                    cmd.Parameters.AddWithValue("@CCV", cvv ?? "");

                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting order:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmShopping      - OrderDetails table inserts
        public static void InsertOrderDetails(int orderID, List<clsCartItem> cartItems, List<clsItemDiscount> itemDiscounts, string discountCode)
        {
            using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
            {
                conn.Open();
                try
                {
                    bool isCartLevel = IsCartLevelDiscount(discountCode);
                    int? appliedDiscountID = null;

                    if (isCartLevel)
                    {
                        appliedDiscountID = GetDiscountInfo(discountCode)?.DiscountID;
                    }

                    foreach (clsCartItem item in cartItems)
                    {
                        // Determine if discount applies to this item
                        int? itemDiscountID = null;

                        if (!isCartLevel && itemDiscounts != null)
                        {
                            var match = itemDiscounts.FirstOrDefault(d => d.ItemID == item.ItemID);
                            if (match != null)
                                itemDiscountID = match.DiscountID;
                        }

                        // Build insert query
                        string query = @"INSERT INTO ThameJ25Su233x.OrderDetails (OrderID, InventoryID, Quantity, DiscountID) VALUES (@OrderID, @InventoryID, @Quantity, @DiscountID)";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@InventoryID", item.ItemID);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                        //
                        // A
                        int? rawDiscountID = isCartLevel ? appliedDiscountID : itemDiscountID;
                        object discountValue = rawDiscountID.HasValue ? (object)rawDiscountID.Value : DBNull.Value;
                        cmd.Parameters.AddWithValue("@DiscountID", discountValue);


                        cmd.ExecuteNonQuery();

                        //  Deduct inventory
                        string deductQuery = @"UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity - @Amount WHERE InventoryID = @InventoryID AND Quantity >= @Amount";

                        using (SqlCommand deductCmd = new SqlCommand(deductQuery, conn))
                        {
                            deductCmd.Parameters.AddWithValue("@Amount", item.Quantity);
                            deductCmd.Parameters.AddWithValue("@InventoryID", item.ItemID);
                            deductCmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //
                    throw new Exception("Error inserting order details: " + ex.Message);
                }
                finally
                {
                    //
                    conn.Close();
                }
            }
        }

        // clsHTML         - Gather users details
        public static DataTable GetPersonDetails(string personID)
        {
            try
            {
                //
                OpenDatabase();

                //
                string query = @"SELECT NameFirst, NameLast, PhonePrimary FROM ThameJ25Su233x.Person WHERE PersonID = @PersonID";

                //
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                //
                MessageBox.Show("Error getting person details:\n" + ex.Message);
                return null;
            }
            finally
            {
                //
                CloseDatabase();
            }
        }

        // frmManaging       - Add to Inventory
        public static bool RestockInventory(int inventoryID, int quantityToAdd)
        {
            try
            {
                //
                OpenDatabase();

                string query = @"UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity + @Amount WHERE InventoryID = @InventoryID";

                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Amount", quantityToAdd);
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                //
                MessageBox.Show("Error restocking inventory: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                //
                CloseDatabase();
            }
        }

        // frmManaging      - Remove from Inventory
        public static bool DecreaseInventoryQuantity(int inventoryID, int amountToSubtract)
        {
            try
            {
                //
                OpenDatabase();

                string query = @"UPDATE ThameJ25Su233x.Inventory SET Quantity = Quantity - @Amount WHERE InventoryID = @InventoryID AND Quantity >= @Amount";

                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    cmd.Parameters.AddWithValue("@Amount", amountToSubtract);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decreasing inventory: " + ex.Message);
                return false;
            }
            finally
            {
                //
                CloseDatabase();
            }
        }

        // frmManaging      - Add New Item
        public static bool InsertNewInventoryItem(string itemName, string itemDescription, int categoryID, decimal retailPrice, decimal cost, int quantity, int restockThreshold, byte[] itemImage, bool discontinued)
        {
            try
            {
                string query = @"
                    INSERT INTO ThameJ25Su233x.Inventory 
                    (ItemName, ItemDescription, CategoryID, RetailPrice, Cost, Quantity, RestockThreshold, ItemImage, Discontinued)
                    VALUES (@ItemName, @ItemDescription, @CategoryID, @RetailPrice, @Cost, @Quantity, @RestockThreshold, @ItemImage, @Discontinued)";

                using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@ItemDescription", itemDescription);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);
                    cmd.Parameters.AddWithValue("@Cost", cost);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@RestockThreshold", restockThreshold);
                    cmd.Parameters.AddWithValue("@ItemImage", itemImage ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", discontinued);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //frm
        public static bool RemoveInventoryItem(int inventoryID)
        {
            try
            {
                string query = @"
                        UPDATE ThameJ25Su233x.Inventory 
                        SET Discontinued = 1 
                        WHERE InventoryID = @InventoryID";

                using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // frmManaging      - Check for duplicate items
        public static bool ItemExists(string itemName, string itemDescription)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = @"SELECT COUNT(*) 
                                 FROM ThameJ25Su233x.Inventory 
                                 WHERE ItemName = @ItemName OR ItemDescription = @ItemDescription";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@ItemDescription", itemDescription);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // frmManager       - Display Inventory (REVIEW)
        public static DataTable ManagerViewInventory(DataGridView dgvR)
        {
            try
            {
                OpenDatabase();

                string query = @"SELECT InventoryID, ItemName, ItemDescription, RetailPrice, Quantity, RestockThreshold, ItemImage, Discontinued FROM ThameJ25Su233x.Inventory";
                SqlCommand cmd = new SqlCommand(query, GetConnection());
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Keep copy for image data
                DataTable dtCopy = dt.Copy();

                // Remove image column before binding
                dt.Columns.Remove("ItemImage");

                // Bind data to DGV
                dgvR.AllowUserToAddRows = false;
                dgvR.DataSource = dt;

                // Add image column
                if (!dgvR.Columns.Contains("ProductImage"))
                {
                    DataGridViewImageColumn imageCol = new DataGridViewImageColumn
                    {
                        Name = "ProductImage",
                        HeaderText = "Product Image",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    dgvR.Columns.Insert(0, imageCol);
                }

                // Populate images immediately
                for (int i = 0; i < dgvR.Rows.Count; i++)
                {
                    byte[] imgBytes = dtCopy.Rows[i]["ItemImage"] as byte[];
                    if (imgBytes != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            dgvR.Rows[i].Cells["ProductImage"].Value = Image.FromStream(ms);
                        }
                    }
                }

                // Column headers
                dgvR.Columns["InventoryID"].Visible = false;
                dgvR.Columns["ItemName"].HeaderText = "Product Name";
                dgvR.Columns["ItemDescription"].HeaderText = "Description";
                dgvR.Columns["RetailPrice"].HeaderText = "Price";
                dgvR.Columns["Quantity"].HeaderText = "Stock Left";
                dgvR.Columns["RestockThreshold"].HeaderText = "Restock Threshold";
                dgvR.Columns["Discontinued"].HeaderText = "Discontinued";

                // Layout
                dgvR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvR.RowTemplate.Height = 65;
                dgvR.ReadOnly = true;
                dgvR.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                return dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Unable to display products!\n\n" + ex.Message);
                return null;
            }
            finally
            {
                CloseDatabase();
            }
        }

        //
        public DataTable RestockThreshold()
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = @"SELECT ItemName, Quantity, RestockThreshold FROM ThameJ25Su233x.Inventory WHERE Quantity < RestockThreshold AND Discontinued = 0";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Restock specific item
        public DataRow RestockSpecificItem(int inventoryID)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = @"SELECT ItemName, Quantity, RestockThreshold FROM ThameJ25Su233x.Inventory WHERE InventoryID = @InventoryID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
        }

        // frmUsers     - Display all accounts
        public static DataTable DisplayUsers(DataGridView dgv, string positionFilter = null)
        {
            try
            {
                OpenDatabase();

                // Select every editable Person field plus Logon fields
                string query = @"
            SELECT 
                P.PersonID,
                P.Title,
                P.NameFirst,
                P.NameMiddle,
                P.NameLast,
                P.Suffix,
                P.Address1,
                P.Address2,
                P.Address3,
                P.City,
                P.Zipcode,
                P.State,
                P.Email,
                P.PhonePrimary,
                P.PhoneSecondary,
                L.PositionTitle,
                L.LogonName,
                L.AccountDisabled,
                L.AccountDeleted
            FROM ThameJ25Su233x.Person P
            INNER JOIN ThameJ25Su233x.Logon L ON P.PersonID = L.PersonID";

                // Apply position filter if needed
                if (!string.IsNullOrEmpty(positionFilter) && !positionFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    query += " WHERE L.PositionTitle = @PositionTitle";
                }

                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    if (!string.IsNullOrEmpty(positionFilter) && !positionFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@PositionTitle", positionFilter);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Bind to DataGridView
                        dgv.AllowUserToAddRows = false;
                        dgv.DataSource = dt;

                        // Hide internal key
                        dgv.Columns["PersonID"].Visible = false;

                        // Rename headers for readability
                        dgv.Columns["Title"].HeaderText = "Title";
                        dgv.Columns["NameFirst"].HeaderText = "First Name";
                        dgv.Columns["NameMiddle"].HeaderText = "Middle Name";
                        dgv.Columns["NameLast"].HeaderText = "Last Name";
                        dgv.Columns["Suffix"].HeaderText = "Suffix";
                        dgv.Columns["Address1"].HeaderText = "Address 1";
                        dgv.Columns["Address2"].HeaderText = "Address 2";
                        dgv.Columns["Address3"].HeaderText = "Address 3";
                        dgv.Columns["City"].HeaderText = "City";
                        dgv.Columns["Zipcode"].HeaderText = "Zip Code";
                        dgv.Columns["State"].HeaderText = "State";
                        dgv.Columns["Email"].HeaderText = "Email";
                        dgv.Columns["PhonePrimary"].HeaderText = "Primary Phone";
                        dgv.Columns["PhoneSecondary"].HeaderText = "Secondary Phone";
                        dgv.Columns["PositionTitle"].HeaderText = "Role";
                        dgv.Columns["LogonName"].HeaderText = "Username";
                        dgv.Columns["AccountDisabled"].HeaderText = "Disabled?";
                        dgv.Columns["AccountDeleted"].HeaderText = "Deleted?";

                        // Layout settings
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgv.RowTemplate.Height = 35;
                        dgv.ReadOnly = true;
                        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                //
                MessageBox.Show("Error displaying users:\n\n" + ex.Message);
                return null;
            }
            finally
            {
                //
                CloseDatabase();
            }
        }

        // frmUsers     - Disable a user
        public bool DisableUser(int personID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CONNECT_STRING))
                {
                    con.Open();
                    string query = @"UPDATE ThameJ25Su233x.Logon
                             SET AccountDisabled = 1
                             WHERE PersonID = @PersonID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disabling user:\n\n" + ex.Message);
                return false;
            }
        }

        // frmUsers     - Remove a user
        public bool DeleteUser(int personID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CONNECT_STRING))
                {
                    con.Open();
                    string query = @"UPDATE ThameJ25Su233x.Logon
                             SET AccountDeleted = 1
                             WHERE PersonID = @PersonID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user:\n\n" + ex.Message);
                return false;
            }
        }

        // frmUsers     - Reactivate a user
        public bool EnableUser(int personID)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();

                string query = @"
                        UPDATE ThameJ25Su233x.Logon
                        SET AccountDisabled = 0
                        WHERE PersonID = @PersonID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PersonID", personID);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // frmUsers     - Update a user
        public static bool UpdateUser(int personID, string firstName, string lastName, string phone, string email, string positionTitle, string username = null, string password = null)
        {
            try
            {
                OpenDatabase();

                // Update Person table
                string updatePerson = @"
                            UPDATE ThameJ25Su233x.Person 
                            SET NameFirst = @FirstName,
                                NameLast = @LastName,
                                PhonePrimary = @Phone,
                                Email = @Email,
                                PositionTitle = @PositionTitle
                            WHERE PersonID = @PersonID";

                using (SqlCommand cmd = new SqlCommand(updatePerson, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.ExecuteNonQuery();
                }


                if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
                {
                    string updateLogon = @"
                                UPDATE ThameJ25Su233x.Logon
                                SET LogonName = COALESCE(NULLIF(@Username, ''), LogonName),
                                    Password   = COALESCE(NULLIF(@Password, ''), Password),
                                    PositionTitle = @PositionTitle
                                WHERE PersonID = @PersonID";

                    using (SqlCommand cmd = new SqlCommand(updateLogon, GetConnection()))
                    {
                        cmd.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Password", (object)password ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user:\n\n" + ex.Message);
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmUpdateUsers       - Gather all details
        public static DataRow GetUserDetails(int personID)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = @"
                        SELECT P.Title, P.NameFirst, P.NameMiddle, P.NameLast, P.Suffix,
                               P.Address1, P.Address2, P.Address3, P.City, P.Zipcode, P.State,
                               P.Email, P.PhonePrimary, P.PhoneSecondary,
                               L.PositionTitle
                        FROM ThameJ25Su233x.Person P
                        INNER JOIN ThameJ25Su233x.Logon L ON P.PersonID = L.PersonID
                        WHERE P.PersonID = @PersonID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return (dt.Rows.Count > 0) ? dt.Rows[0] : null;
                    }
                }
            }
        }

        // frmUpdateUsers       - Update details
        public static bool UpdateUserDetails(int personID, Dictionary<string, object> userData)
        {
            try
            {
                OpenDatabase();

                // ensure PositionTitle exists in Position table and get its ID
                string positionTitle = userData["PositionTitle"].ToString();
                using (SqlCommand seed = new SqlCommand(
                            @"IF NOT EXISTS (SELECT 1 FROM ThameJ25Su233x.Position WHERE PositionTitle = @Title)
                      BEGIN INSERT INTO ThameJ25Su233x.Position (PositionTitle) VALUES (@Title) END",
                    GetConnection()))
                {
                    seed.Parameters.AddWithValue("@Title", positionTitle);
                    seed.ExecuteNonQuery();
                }
                int positionID;
                using (SqlCommand lookup = new SqlCommand(
                    "SELECT PositionID FROM ThameJ25Su233x.Position WHERE PositionTitle = @Title",
                    GetConnection()))
                {
                    lookup.Parameters.AddWithValue("@Title", positionTitle);
                    positionID = Convert.ToInt32(lookup.ExecuteScalar());
                }

                // update Person
                string updatePerson = @"
                        UPDATE ThameJ25Su233x.Person SET
                            Title          = @Title,
                            NameFirst      = @NameFirst,
                            NameMiddle     = @NameMiddle,
                            NameLast       = @NameLast,
                            Suffix         = @Suffix,
                            Address1       = @Address1,
                            Address2       = @Address2,
                            Address3       = @Address3,
                            City           = @City,
                            Zipcode        = @Zipcode,
                            State          = @State,
                            Email          = @Email,
                            PhonePrimary   = @PhonePrimary,
                            PhoneSecondary = @PhoneSecondary, PositionID = @PositionID WHERE PersonID = @PersonID";

                using (SqlCommand cmd = new SqlCommand(updatePerson, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Title", userData["Title"]);
                    cmd.Parameters.AddWithValue("@NameFirst", userData["NameFirst"]);
                    cmd.Parameters.AddWithValue("@NameMiddle", userData["NameMiddle"]);
                    cmd.Parameters.AddWithValue("@NameLast", userData["NameLast"]);
                    cmd.Parameters.AddWithValue("@Suffix", userData["Suffix"]);
                    cmd.Parameters.AddWithValue("@Address1", userData["Address1"]);
                    cmd.Parameters.AddWithValue("@Address2", userData["Address2"]);
                    cmd.Parameters.AddWithValue("@Address3", userData["Address3"]);
                    cmd.Parameters.AddWithValue("@City", userData["City"]);
                    cmd.Parameters.AddWithValue("@Zipcode", userData["Zipcode"]);
                    cmd.Parameters.AddWithValue("@State", userData["State"]);
                    cmd.Parameters.AddWithValue("@Email", userData["Email"]);
                    cmd.Parameters.AddWithValue("@PhonePrimary", userData["PhonePrimary"]);
                    cmd.Parameters.AddWithValue("@PhoneSecondary", userData["PhoneSecondary"]);
                    cmd.Parameters.AddWithValue("@PositionID", positionID);
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.ExecuteNonQuery();
                }

                // update PositionTitle in Logon
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE ThameJ25Su233x.Logon SET PositionTitle = @PositionTitle WHERE PersonID = @PersonID",
                    GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user details:\n\n" + ex.Message);
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmUsers     - Filter accounts by PositionTitle
        public DataTable GetUsersByPosition(string positionTitle)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();

                string query = @"
                        SELECT P.PersonID, P.NameFirst, P.NameLast, P.Email, P.PhonePrimary,
                               L.LogonName, L.PositionTitle, L.AccountDisabled, L.AccountDeleted
                        FROM ThameJ25Su233x.Person P
                        INNER JOIN ThameJ25Su233x.Logon L ON P.PersonID = L.PersonID
                        WHERE (L.AccountDeleted IS NULL OR L.AccountDeleted = 0)";

                if (!string.Equals(positionTitle, "All", StringComparison.OrdinalIgnoreCase))
                {
                    query += " AND L.PositionTitle = @PositionTitle";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.Equals(positionTitle, "All", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // frmUsers     - Gather all position titles
        public static List<string> GetAllPositionTitles()
        {
            List<string> positions = new List<string>();

            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = @"SELECT DISTINCT PositionTitle 
                         FROM ThameJ25Su233x.Logon
                         WHERE PositionTitle IS NOT NULL";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        positions.Add(reader["PositionTitle"].ToString());
                    }
                }
            }

            return positions;
        }

        // frmDiscount  - Creating discounts
        //public static bool AddDiscount(string discountCode, string description, int discountLevel, int? inventoryID, int discountType, decimal discountPercentage, decimal discountDollarAmount, DateTime startDate, DateTime expirationDate)
        //{
        //    try
        //    {
        //        //
        //        OpenDatabase();

        //        string query = @"INSERT INTO ThameJ25Su233x.Discounts (DiscountCode, Description, DiscountLevel, InventoryID, DiscountType, DiscountPercentage, DiscountDollarAmount, StartDate, ExpirationDate) VALUES (@DiscountCode, @Description, @DiscountLevel, @InventoryID, @DiscountType, @DiscountPercentage, @DiscountDollarAmount, @StartDate, @ExpirationDate)";

        //        using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
        //        {
        //            cmd.Parameters.AddWithValue("@DiscountCode", discountCode);
        //            cmd.Parameters.AddWithValue("@Description", description);
        //            cmd.Parameters.AddWithValue("@DiscountLevel", discountLevel);
        //            cmd.Parameters.AddWithValue("@InventoryID", inventoryID ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@DiscountType", discountType);
        //            cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
        //            cmd.Parameters.AddWithValue("@DiscountDollarAmount", discountDollarAmount);
        //            cmd.Parameters.AddWithValue("@StartDate", startDate);
        //            cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate);

        //            int rowsAffected = cmd.ExecuteNonQuery();
        //            return rowsAffected > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //
        //        MessageBox.Show("Error adding discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return false;
        //    }
        //    finally
        //    {
        //        //
        //        CloseDatabase();
        //    }
        //}

        // frmDiscount - Edit discounts

        public static bool EditDiscount(int discountID, string discountCode, string description, int discountLevel, int? inventoryID, int discountType, decimal discountPercentage, decimal discountDollarAmount, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                //
                OpenDatabase();

                string query = @"
                        UPDATE ThameJ25Su233x.Discounts
                        SET DiscountCode = @DiscountCode,
                            Description = @Description,
                            DiscountLevel = @DiscountLevel,
                            InventoryID = @InventoryID,
                            DiscountType = @DiscountType,
                            DiscountPercentage = @DiscountPercentage,
                            DiscountDollarAmount = @DiscountDollarAmount,
                            StartDate = @StartDate, ExpirationDate = @ExpirationDate WHERE DiscountID = @DiscountID";

                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@DiscountCode", discountCode);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DiscountLevel", discountLevel);
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountType", discountType);
                    cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    cmd.Parameters.AddWithValue("@DiscountDollarAmount", discountDollarAmount);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate);
                    cmd.Parameters.AddWithValue("@DiscountID", discountID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                //
                MessageBox.Show("Error updating discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                //
                CloseDatabase();
            }
        }

        // frmDiscount - Update discounts
        public static bool UpdateDiscountFields(int discountID, Dictionary<string, object> fieldsToUpdate)
        {
            if (fieldsToUpdate == null || fieldsToUpdate.Count == 0)
                return false;

            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
                {
                    var setClauses = new List<string>();
                    foreach (var field in fieldsToUpdate.Keys)
                        setClauses.Add($"{field} = @{field}");

                    string setClause = string.Join(", ", setClauses);
                    string query = $"UPDATE ThameJ25Su233x.Discounts SET {setClause} WHERE DiscountID = @DiscountID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        foreach (var kvp in fieldsToUpdate)
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@DiscountID", discountID);

                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // frmDiscount - Delete discounts
        public static bool RemoveDiscount(int discountID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CONNECT_STRING))
                {
                    con.Open();
                    string query = "DELETE FROM ThameJ25Su233x.Discounts WHERE DiscountID = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", discountID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing discount: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // frmDiscount - Display discounts
        public static DataTable DisplayDiscounts()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
                {
                    string query = @"
                                SELECT 
                                    D.DiscountID,
                                    D.DiscountCode,
                                    D.Description,
                                    D.DiscountLevel,
                                    D.DiscountType,
                                    D.DiscountPercentage,
                                    D.DiscountDollarAmount,
                                    D.StartDate,
                                    D.ExpirationDate,
                                    D.InventoryID,
                                    I.ItemName,
                                    I.ItemImage
                                FROM ThameJ25Su233x.Discounts D
                                LEFT JOIN ThameJ25Su233x.Inventory I ON D.InventoryID = I.InventoryID
                                ORDER BY D.DiscountID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading discounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // frmDiscount - Display Items and InventoryID
        public static DataTable GetAllInventoryItems()
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = "SELECT InventoryID, ItemName FROM ThameJ25Su233x.Inventory ORDER BY ItemName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        //
        public static bool InventoryIDExists(int inventoryID)
        {
            using (var con = new SqlConnection(CONNECT_STRING))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM ThameJ25Su233x.Inventory WHERE InventoryID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", inventoryID);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static bool AddDiscount(string discountCode, string description, int discountLevel, int? inventoryID, int discountType, decimal normalizedPercentage, decimal normalizedDollar, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                OpenDatabase();

                decimal? pct = (discountType == 0 && normalizedPercentage > 0) ? Math.Round(normalizedPercentage, 2) : (decimal?)null;
                decimal? amt = (discountType == 1 && normalizedDollar > 0) ? Math.Round(normalizedDollar, 2) : (decimal?)null;

                using (var cmd = new SqlCommand(@"
                    INSERT INTO ThameJ25Su233x.Discounts
                    (
                        DiscountCode, Description, DiscountLevel, InventoryID, DiscountType,
                        DiscountPercentage, DiscountDollarAmount, StartDate, ExpirationDate
                    )
                    VALUES
                    (
                        @DiscountCode, @Description, @DiscountLevel, @InventoryID, @DiscountType,
                        @DiscountPercentage, @DiscountDollarAmount, @StartDate, @ExpirationDate
                    );", GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@DiscountCode", discountCode ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Description", description ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DiscountLevel", discountLevel);
                    cmd.Parameters.AddWithValue("@DiscountType", discountType); // 0/1

                    var pInv = cmd.Parameters.Add("@InventoryID", SqlDbType.Int);
                    pInv.Value = (object)inventoryID ?? DBNull.Value;

                    var pPct = cmd.Parameters.Add("@DiscountPercentage", SqlDbType.Decimal);
                    pPct.Precision = 3; pPct.Scale = 2;
                    pPct.Value = (object)pct ?? DBNull.Value;

                    var pAmt = cmd.Parameters.Add("@DiscountDollarAmount", SqlDbType.Decimal);
                    pAmt.Precision = 11; pAmt.Scale = 2;
                    pAmt.Value = (object)amt ?? DBNull.Value;

                    cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate.Date;
                    cmd.Parameters.Add("@ExpirationDate", SqlDbType.Date).Value = expirationDate.Date;

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmDiscounts     - Check duplicate by code
        public static bool DiscountCodeExists(string discountCode)
        {
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM ThameJ25Su233x.Discounts
                    WHERE DiscountCode = @Code;", GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Code", discountCode ?? string.Empty);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmDiscounts     - Check duplicate by full combination
        public static bool DiscountCombinationExists(int discountLevel, int? inventoryID, int discountType, decimal? discountPercentage, decimal? discountDollarAmount, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM ThameJ25Su233x.Discounts
                    WHERE DiscountLevel = @Level
                      AND ((@InventoryID IS NULL AND InventoryID IS NULL) OR InventoryID = @InventoryID)
                      AND DiscountType = @Type
                      AND ISNULL(DiscountPercentage, 0) = ISNULL(@Pct, 0)
                      AND ISNULL(DiscountDollarAmount, 0) = ISNULL(@Amt, 0)
                      AND StartDate = @Start
                      AND ExpirationDate = @End;", GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Level", discountLevel);

                    var pInv = cmd.Parameters.Add("@InventoryID", SqlDbType.Int);
                    pInv.Value = (object)inventoryID ?? DBNull.Value;

                    var pPct = cmd.Parameters.Add("@Pct", SqlDbType.Decimal);
                    pPct.Precision = 3; pPct.Scale = 2;
                    pPct.Value = (object)discountPercentage ?? DBNull.Value;

                    var pAmt = cmd.Parameters.Add("@Amt", SqlDbType.Decimal);
                    pAmt.Precision = 11; pAmt.Scale = 2;
                    pAmt.Value = (object)discountDollarAmount ?? DBNull.Value;

                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = discountType;
                    cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startDate.Date;
                    cmd.Parameters.Add("@End", SqlDbType.Date).Value = expirationDate.Date;

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // frmDiscounts     - Validate InventoryID exists
        public static bool InventoryExists(int inventoryId)
        {
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM ThameJ25Su233x.Inventory WHERE InventoryID = @id",
                    GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", inventoryId);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseDatabase();
            }
        }

        // ------------- SALES & INVENTORY REPORT QUERIES -----------------

        public static DataTable GetSalesTotalsByDateRange(DateTime startInclusive, DateTime endInclusive)
        {
            var dt = new DataTable();
            try
            {
                OpenDatabase();

                using (var cmd = new SqlCommand(@"
                        WITH R AS (
                          SELECT 
                            CAST(O.OrderDate AS date) AS [Date],
                            O.OrderID,
                            OD.Quantity,
                            I.RetailPrice,
                            ISNULL(D.DiscountPercentage, 0) AS DiscountPct,         -- EX: 0.10 for 10%
                            ISNULL(D.DiscountDollarAmount, 0) AS DiscountAmt        -- Dollar off per line
                          FROM ThameJ25Su233x.Orders O
                          INNER JOIN ThameJ25Su233x.OrderDetails OD ON O.OrderID = OD.OrderID
                          INNER JOIN ThameJ25Su233x.Inventory I     ON OD.InventoryID = I.InventoryID
                          LEFT  JOIN ThameJ25Su233x.Discounts D     ON OD.DiscountID  = D.DiscountID
                          WHERE O.OrderDate >= @Start AND O.OrderDate < DATEADD(day, 1, @End)
                        )
                        SELECT 
                          [Date],
                          COUNT(DISTINCT OrderID)                                     AS OrdersCount,
                          SUM(Quantity)                                               AS ItemsSold,
                          SUM(Quantity * RetailPrice)                                 AS GrossSales,
                          SUM(Quantity * RetailPrice * DiscountPct + DiscountAmt)     AS TotalDiscounts,
                          SUM(Quantity * RetailPrice
                              - (Quantity * RetailPrice * DiscountPct)
                              - DiscountAmt)                                          AS NetSales
                        FROM R
                        GROUP BY [Date]
                        ORDER BY [Date];
                        ", GetConnection()))
                {
                    cmd.Parameters.Add("@Start", SqlDbType.Date).Value = startInclusive.Date;
                    cmd.Parameters.Add("@End", SqlDbType.Date).Value = endInclusive.Date;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales totals:\n\n" + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseDatabase();
            }

            return dt;
        }

        public static DataTable GetInventoryAvailable()
        {
            var dt = new DataTable();
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(@"
                        SELECT 
                          InventoryID, ItemName, Cost, RetailPrice, Quantity, RestockThreshold, 
                          CASE WHEN Discontinued = 1 THEN 'Yes' ELSE 'No' END AS Discontinued
                        FROM ThameJ25Su233x.Inventory
                        WHERE Discontinued = 0 AND Quantity > 0
                        ORDER BY ItemName;", GetConnection()))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading available inventory:\n\n" + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally { CloseDatabase(); }

            return dt;
        }

        public static DataTable GetInventoryNeedingRestock()
        {
            var dt = new DataTable();
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(@"
                        SELECT 
                          InventoryID, ItemName, Cost, RetailPrice, Quantity, RestockThreshold,
                          CASE WHEN Discontinued = 1 THEN 'Yes' ELSE 'No' END AS Discontinued
                        FROM ThameJ25Su233x.Inventory
                        WHERE Discontinued = 0 AND Quantity < RestockThreshold
                        ORDER BY ItemName;", GetConnection()))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading restock-needed inventory:\n\n" + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally { CloseDatabase(); }

            return dt;
        }

        public static DataTable GetInventoryAll()
        {
            var dt = new DataTable();
            try
            {
                OpenDatabase();
                using (var cmd = new SqlCommand(@"
                        SELECT 
                          InventoryID, ItemName, Cost, RetailPrice, Quantity, RestockThreshold,
                          CASE WHEN Discontinued = 1 THEN 'Yes' ELSE 'No' END AS Discontinued
                        FROM ThameJ25Su233x.Inventory
                        ORDER BY ItemName;", GetConnection()))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading all inventory:\n\n" + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally { CloseDatabase(); }

            return dt;
        }

        public static DataTable SearchCustomers(string email, string phone, string invoiceNumber, string personId)
        {
            using (var con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();

                bool noFilters =
                    string.IsNullOrWhiteSpace(email) &&
                    string.IsNullOrWhiteSpace(phone) &&
                    string.IsNullOrWhiteSpace(invoiceNumber) &&
                    string.IsNullOrWhiteSpace(personId);

                string sql = @"
                        SELECT TOP 200
                            P.PersonID,
                            (P.NameFirst + ' ' + P.NameLast) AS FullName,
                            P.PhonePrimary,
                            L.LogonName AS Email,
                            O.OrderID AS LastOrderID,
                            O.OrderDate AS LastOrderDate
                        FROM ThameJ25Su233x.Person P
                        LEFT JOIN ThameJ25Su233x.Logon L ON L.PersonID = P.PersonID
                        OUTER APPLY (
                            SELECT TOP 1 Ord.OrderID, Ord.OrderDate
                            FROM ThameJ25Su233x.[Order] Ord
                            WHERE Ord.PersonID = P.PersonID
                            ORDER BY Ord.OrderDate DESC, Ord.OrderID DESC
                        ) O
                        WHERE
                            (@NoFilters = 1)
                            OR (@Email IS NOT NULL AND L.LogonName LIKE '%' + @Email + '%')
                            OR (@Phone IS NOT NULL AND P.PhonePrimary LIKE '%' + @Phone + '%')
                            OR (@Invoice IS NOT NULL AND CAST(O.OrderID AS NVARCHAR(50)) = @Invoice)
                            OR (@PersonID IS NOT NULL AND CAST(P.PersonID AS NVARCHAR(50)) = @PersonID)
                        ORDER BY COALESCE(O.OrderDate, '1900-01-01') DESC, P.PersonID DESC;";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@NoFilters", noFilters ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Email", (object)(string.IsNullOrWhiteSpace(email) ? null : email) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)(string.IsNullOrWhiteSpace(phone) ? null : phone) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice", (object)(string.IsNullOrWhiteSpace(invoiceNumber) ? null : invoiceNumber) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PersonID", (object)(string.IsNullOrWhiteSpace(personId) ? null : personId) ?? DBNull.Value);

                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        public static DataRow GetCustomerById(int personID)
        {
            try
            {
                OpenDatabase();
                const string sql = @"
                        SELECT TOP 1
                            P.PersonID,
                            P.NameFirst,
                            P.NameLast,
                            P.PhonePrimary
                        FROM ThameJ25Su233x.Person AS P
                        WHERE P.PersonID = @PersonID;";
                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
            finally
            {
                CloseDatabase();
            }
        }

        public static DataTable GetOrdersForCustomer(int personId)
        {
            const string sql = @"
                    SELECT
                        o.OrderID,
                        o.OrderDate,
                        -- Gross total from line items (qty * price)
                        SUM(CAST(od.Quantity AS decimal(18,2)) * inv.RetailPrice) AS TotalDue,
                        o.CC_Number,
                        o.CCV AS CCV,
                        o.ExpDate
                    FROM ThameJ25Su233x.Orders            AS o
                    LEFT JOIN ThameJ25Su233x.OrderDetails AS od   ON od.OrderID     = o.OrderID
                    LEFT JOIN ThameJ25Su233x.Inventory    AS inv  ON inv.InventoryID = od.InventoryID
                    WHERE o.PersonID = @pid
                    GROUP BY
                        o.OrderID, o.OrderDate,
                        o.CC_Number, o.CCV, o.ExpDate
                    ORDER BY o.OrderDate DESC, o.OrderID DESC;";

            using (var cn = new SqlConnection(CONNECT_STRING))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@pid", personId);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        //
        public static DataTable GetSalesTotals(DateTime start, DateTime end)
        {
            DateTime endInclusive = end.Date.AddDays(1).AddMilliseconds(-3);

            try
            {
                OpenDatabase();
                const string sql = @"
                        SELECT
                            COUNT(DISTINCT O.OrderID)                                   AS OrdersCount,
                            SUM(CAST(OD.Quantity AS decimal(18,2)))                     AS ItemsCount,
                            SUM(CAST(OD.Quantity AS decimal(18,2)) * I.RetailPrice)     AS GrossTotal
                        FROM ThameJ25Su233x.Orders       AS O
                        JOIN ThameJ25Su233x.OrderDetails AS OD ON OD.OrderID    = O.OrderID
                        JOIN ThameJ25Su233x.Inventory    AS I  ON I.InventoryID = OD.InventoryID
                        WHERE O.OrderDate >= @Start AND O.OrderDate <= @End;";
                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Start", start.Date);
                    cmd.Parameters.AddWithValue("@End", endInclusive);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            finally
            {
                CloseDatabase();
            }
        }

        public static string GetFullNameByPersonID(string personID)
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();
                string query = "SELECT NameFirst, NameLast FROM ThameJ25Su233x.Person WHERE PersonID = @PersonID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string first = reader["NameFirst"]?.ToString();
                            string last = reader["NameLast"]?.ToString();
                            return $"{first} {last}".Trim();
                        }
                    }
                }
            }
            return string.Empty;
        }

        //
        public static DataTable GetActiveDiscounts()
        {
            using (var con = new SqlConnection(CONNECT_STRING))
            using (var cmd = new SqlCommand(@"
                        SELECT 
                            d.DiscountID,
                            d.DiscountCode        AS DiscountName,         
                            d.[Description]       AS DiscountDescription,  
                            d.DiscountLevel,                                
                            d.InventoryID,                                  
                            d.DiscountType,                                 
                            d.DiscountPercentage,
                            d.DiscountDollarAmount,
                            d.StartDate,
                            d.ExpirationDate,
                            i.ItemName
                        FROM ThameJ25Su233x.Discounts d
                        LEFT JOIN ThameJ25Su233x.Inventory i 
                            ON i.InventoryID = d.InventoryID
                        WHERE 
                            d.StartDate       <= CAST(GETDATE() AS date)
                            AND d.ExpirationDate >= CAST(GETDATE() AS date)
                        ORDER BY d.DiscountLevel, d.DiscountCode;", con))
            {
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetCustomersByInvoice(string invoiceSearch)
        {
            var dt = new DataTable();
            if (invoiceSearch == null) invoiceSearch = "";

            string raw = invoiceSearch.Trim();
            if (raw.StartsWith("#")) raw = raw.Substring(1).Trim();

            int orderId;
            bool numeric = int.TryParse(raw, out orderId);
            string like = "%" + raw + "%";

            try
            {
                OpenDatabase();

                string sql = @"
                    SELECT DISTINCT 
                        p.PersonID, p.NameFirst, p.NameLast, p.Email, p.PhonePrimary, p.PhoneSecondary
                    FROM ThameJ25Su233x.Persons p
                    INNER JOIN ThameJ25Su233x.Orders o ON o.PersonID = p.PersonID
                    WHERE
                        (@OrderId IS NOT NULL AND o.OrderID = @OrderId)
                        OR (@OrderId IS NULL AND CONVERT(varchar(32), o.OrderID) LIKE @Like);";

                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    var pId = cmd.Parameters.Add("@OrderId", SqlDbType.Int);
                    pId.Value = numeric ? (object)orderId : DBNull.Value;

                    cmd.Parameters.AddWithValue("@Like", like);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            finally
            {
                CloseDatabase();
            }

            return dt;
        }

        // frmManager - 
        public static DataTable GetAllCustomers()
        {
            using (SqlConnection con = new SqlConnection(CONNECT_STRING))
            {
                con.Open();

                string query = @"
                    SELECT PersonID, NameFirst, NameLast
                    FROM ThameJ25Su233x.Person
                    WHERE PositionID = (SELECT PositionID FROM ThameJ25Su233x.Position WHERE PositionTitle = 'Customer')
                    ORDER BY NameFirst, NameLast;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // customer search for frmSelectCustomer
        public static DataTable GetCustomerLookupForPicker(string searchText = null)
        {
            var sql = @"
                    SELECT
                        p.PersonID,
                        p.NameFirst,
                        p.NameLast,
                        p.Email,
                        p.PhonePrimary,
                        p.PhoneSecondary
                    FROM ThameJ25Su233x.Person AS p
                    WHERE
                        (@q IS NULL OR @q = '')
                        OR (
                            -- numeric prefix match for PersonID
                            (CASE WHEN @isNum = 1 THEN CONVERT(varchar(32), p.PersonID) ELSE '' END) LIKE @qStarts
                            OR p.NameFirst      LIKE @qLike
                            OR p.NameLast       LIKE @qLike
                            OR p.Email          LIKE @qLike
                            OR p.PhonePrimary   LIKE @qLike
                            OR p.PhoneSecondary LIKE @qLike
                        )
                    ORDER BY p.NameLast, p.NameFirst, p.PersonID;";

            string q = searchText?.Trim() ?? string.Empty;
            bool isNum = !string.IsNullOrEmpty(q) && IsAllDigits(q);

            using (var cn = new SqlConnection(CONNECT_STRING))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@q", q);
                cmd.Parameters.AddWithValue("@isNum", isNum ? 1 : 0);
                cmd.Parameters.AddWithValue("@qStarts", q + "%");   
                cmd.Parameters.AddWithValue("@qLike", "%" + q + "%"); 

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        //
        private static bool IsAllDigits(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (!char.IsDigit(s[i])) return false;
            return s.Length > 0;
        }

        private static string InventoryBaseSelect = @"
                                                    SELECT
                                                        inv.InventoryID            AS ID,
                                                        inv.ItemName               AS Name,
                                                        inv.Cost,
                                                        inv.RetailPrice            AS Price,
                                                        inv.Quantity               AS QuantityOnHand,
                                                        inv.RestockThreshold,
                                                        CASE
                                                            WHEN inv.Discontinued = 1                 THEN 'Discontinued'
                                                            WHEN inv.Quantity <= 0                    THEN 'Out of Stock'
                                                            WHEN inv.Quantity <= inv.RestockThreshold THEN 'Needs Restock'
                                                            ELSE 'Available'
                                                        END                           AS Availability,
                                                        inv.ItemImage,
                                                        inv.ItemDescription,
                                                        inv.Discontinued
                                                    FROM ThameJ25Su233x.Inventory AS inv
";

        private static DataTable BindInventoryWithImages(DataGridView grid, string whereClause, params SqlParameter[] parms)
        {
            try
            {
                OpenDatabase();

                string sql = InventoryBaseSelect + (string.IsNullOrWhiteSpace(whereClause) ? "" : " WHERE " + whereClause);
                using (var cmd = new SqlCommand(sql, GetConnection()))
                {
                    if (parms != null)
                        cmd.Parameters.AddRange(parms);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        var dtCopy = dt.Copy();

                        if (dt.Columns.Contains("ItemImage"))
                            dt.Columns.Remove("ItemImage");

                        grid.AllowUserToAddRows = false;
                        grid.DataSource = dt;

                        if (!grid.Columns.Contains("ProductImage"))
                        {
                            var imageCol = new DataGridViewImageColumn
                            {
                                Name = "ProductImage",
                                HeaderText = "Product",
                                ImageLayout = DataGridViewImageCellLayout.Zoom
                            };
                            grid.Columns.Insert(0, imageCol);
                        }

                        for (int i = 0; i < grid.Rows.Count && i < dtCopy.Rows.Count; i++)
                        {
                            var bytes = dtCopy.Rows[i]["ItemImage"] as byte[];
                            if (bytes != null)
                            {
                                using (var ms = new MemoryStream(bytes))
                                {
                                    grid.Rows[i].Cells["ProductImage"].Value = Image.FromStream(ms);
                                }
                            }
                        }

                        string[] keepOrder = { "ID", "Name", "Cost", "Price", "QuantityOnHand", "RestockThreshold", "Availability" };

                        foreach (DataGridViewColumn c in grid.Columns)
                            if (c.Name != "ProductImage") c.Visible = false;

                        int displayIndex = 1;
                        foreach (var colName in keepOrder)
                        {
                            if (grid.Columns.Contains(colName))
                            {
                                var c = grid.Columns[colName];
                                c.Visible = true;
                                c.DisplayIndex = displayIndex++;
                            }
                        }

                        grid.Columns["ID"].HeaderText = "ID";
                        grid.Columns["Name"].HeaderText = "Name";
                        grid.Columns["Cost"].HeaderText = "Cost";
                        grid.Columns["Price"].HeaderText = "Price";
                        grid.Columns["QuantityOnHand"].HeaderText = "Quantity on Hand";
                        grid.Columns["RestockThreshold"].HeaderText = "Restock Threshold";
                        grid.Columns["Availability"].HeaderText = "Availability";

                        if (grid.Columns.Contains("Cost")) grid.Columns["Cost"].DefaultCellStyle.Format = "C2";
                        if (grid.Columns.Contains("Price")) grid.Columns["Price"].DefaultCellStyle.Format = "C2";
                        if (grid.Columns.Contains("QuantityOnHand"))
                            grid.Columns["QuantityOnHand"].DefaultCellStyle.Format = "N0";

                        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        grid.RowTemplate.Height = 65;
                        grid.ReadOnly = true;
                        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                        return dtCopy;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load inventory:\n\n" + ex.Message, "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseDatabase();
            }
        }

        public static DataTable ManagerViewInventoryAll(DataGridView grid)
        {
            return BindInventoryWithImages(grid, null);
        }

        public static DataTable ManagerViewInventoryAvailable(DataGridView grid)
        {
            return BindInventoryWithImages(grid, "inv.Quantity > 0 AND inv.Discontinued = 0");
        }

        public static DataTable ManagerViewInventoryNeedsRestock(DataGridView grid)
        {
            return BindInventoryWithImages(grid, "inv.Discontinued = 0 AND inv.Quantity <= inv.RestockThreshold");
        }
    }

    //
    public class clsItemDiscount
    {
        public int ItemID { get; set; }
        public int DiscountID { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountDollarAmount { get; set; }
    }
}