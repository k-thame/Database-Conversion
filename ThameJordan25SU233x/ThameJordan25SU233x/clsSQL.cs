using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using ACS_JThameM7;

namespace ThameJordan25SU233x
{
    internal class clsSQL
    {
        // Path to the local database file
        private static readonly string DB_PATH =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmolTech", "SmolTech.db");

        // Connection string using the local file path
        private static string CONNECT_STRING => $"Data Source={DB_PATH};";

        // Opens a new connection and turns on foreign keys
        public static SqliteConnection GetOpenConnection()
        {
            var cn = new SqliteConnection(CONNECT_STRING);
            cn.Open();
            using (var p = cn.CreateCommand())
            {
                p.CommandText = "PRAGMA foreign_keys = ON;";
                p.ExecuteNonQuery();
            }
            return cn;
        }

        // Fills a DataTable from a command (replaces SqlDataAdapter)
        private static DataTable FillDataTable(SqliteCommand cmd)
        {
            var dt = new DataTable();
            using (var r = cmd.ExecuteReader())
            {
                for (int i = 0; i < r.FieldCount; i++)
                    dt.Columns.Add(r.GetName(i));
                while (r.Read())
                {
                    var row = dt.NewRow();
                    for (int i = 0; i < r.FieldCount; i++)
                        if (r.IsDBNull(i))
                            row[i] = DBNull.Value;
                        else if (r.GetDataTypeName(i).ToUpper() == "BLOB")
                        {
                            try
                            {
                                using (var ms = new MemoryStream())
                                {
                                    using (var stream = r.GetStream(i))
                                        stream.CopyTo(ms);
                                    row[i] = ms.ToArray();
                                }
                            }
                            catch { row[i] = DBNull.Value; }
                        }
                        else
                            row[i] = r.GetValue(i);
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        // Kept so existing callers still compile
        public static void OpenDatabase() { }
        public static void CloseDatabase() { }
        public static SqliteConnection GetConnection() => GetOpenConnection();

        // --- LOGIN ---

        // Checks username and password, returns true if valid
        public static bool ValidateUserCredentials(string username, string password, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (var cn = GetOpenConnection())
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
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PersonID FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteScalar()?.ToString() ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving PersonID:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Unknown";
            }
        }

        // Returns the position title for a given username
        public static string GetUserPositionTitle(string username)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PositionTitle FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteScalar()?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving PositionTitle:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        // --- ACCOUNT CREATION ---

        // Loads security questions grouped by set ID (1, 2, 3)
        public static Dictionary<int, DataTable> GetSecurityQuestionSets()
        {
            var sets = new Dictionary<int, DataTable>();
            try
            {
                using (var cn = GetOpenConnection())
                    for (int setId = 1; setId <= 3; setId++)
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT QuestionID, QuestionPrompt FROM SecurityQuestions WHERE SetID = @SetID";
                            cmd.Parameters.AddWithValue("@SetID", setId);
                            sets[setId] = FillDataTable(cmd);
                        }
            }
            catch (Exception ex) { MessageBox.Show("Failed to load security questions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return sets;
        }

        // Creates a new Person, Logon, and security answers in one transaction
        public static bool CreateNewAccount(Dictionary<string, object> userData, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                string positionTitle = "Customer";
                if (userData.TryGetValue("PositionTitle", out var posObj) && posObj != null && !string.IsNullOrWhiteSpace(posObj.ToString()))
                    positionTitle = posObj.ToString();

                using (var cn = GetOpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    // Add position if it doesn't exist
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "INSERT OR IGNORE INTO Position (PositionTitle) VALUES (@Title)";
                        cmd.Parameters.AddWithValue("@Title", positionTitle);
                        cmd.ExecuteNonQuery();
                    }

                    // Get the position ID
                    int positionID;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "SELECT PositionID FROM Position WHERE PositionTitle = @Title";
                        cmd.Parameters.AddWithValue("@Title", positionTitle);
                        positionID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insert the person record
                    long personID;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"INSERT INTO Person (Title,NameFirst,NameMiddle,NameLast,Suffix,Address1,Address2,Address3,City,Zipcode,State,Email,PhonePrimary,PhoneSecondary,PositionID)
                                            VALUES (@Title,@NameFirst,@NameMiddle,@NameLast,@Suffix,@Address1,@Address2,@Address3,@City,@Zipcode,@State,@Email,@PhonePrimary,@PhoneSecondary,@PositionID);
                                            SELECT last_insert_rowid();";
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
                        personID = (long)cmd.ExecuteScalar();
                    }

                    // Insert the login record
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"INSERT INTO Logon (PersonID,LogonName,Password,PositionTitle,FirstChallengeQuestion,FirstChallengeAnswer,SecondChallengeQuestion,SecondChallengeAnswer,ThirdChallengeQuestion,ThirdChallengeAnswer)
                                            VALUES (@PersonID,@LogonName,@Password,@PositionTitle,@FirstQ,@FirstA,@SecondQ,@SecondA,@ThirdQ,@ThirdA);";
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        cmd.Parameters.AddWithValue("@LogonName", userData.TryGetValue("Username", out var un) ? un?.ToString() ?? "" : "");
                        cmd.Parameters.AddWithValue("@Password", userData["Password"]?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                        cmd.Parameters.AddWithValue("@FirstQ", userData.TryGetValue("FirstChallengeQuestion", out var fq) ? (object)fq : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FirstA", userData.TryGetValue("FirstChallengeAnswer", out var fa) ? fa?.ToString() ?? "" : "");
                        cmd.Parameters.AddWithValue("@SecondQ", userData.TryGetValue("SecondChallengeQuestion", out var sq) ? (object)sq : DBNull.Value);
                        cmd.Parameters.AddWithValue("@SecondA", userData.TryGetValue("SecondChallengeAnswer", out var sa) ? sa?.ToString() ?? "" : "");
                        cmd.Parameters.AddWithValue("@ThirdQ", userData.TryGetValue("ThirdChallengeQuestion", out var tq) ? (object)tq : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThirdA", userData.TryGetValue("ThirdChallengeAnswer", out var ta) ? ta?.ToString() ?? "" : "");
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex) { errorMessage = "Error creating account: " + ex.Message; return false; }
        }

        // Returns true if the username is already taken
        public static bool CheckUsernameExists(string username)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
            catch { return true; }
        }

        // Alias used by password reset form
        public static bool UsernameExists(string username) => CheckUsernameExists(username);

        // --- PASSWORD RESET ---

        // Returns the three security question prompts for a user
        public static List<string> GetSecurityQuestionPrompts(string username)
        {
            var prompts = new List<string>();
            try
            {
                using (var cn = GetOpenConnection())
                {
                    int q1, q2, q3;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT FirstChallengeQuestion, SecondChallengeQuestion, ThirdChallengeQuestion FROM Logon WHERE LogonName = @Username";
                        cmd.Parameters.AddWithValue("@Username", username);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) { MessageBox.Show("No security questions found for that username.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning); return prompts; }
                            q1 = r.GetInt32(0); q2 = r.GetInt32(1); q3 = r.GetInt32(2);
                        }
                    }
                    prompts.Add(GetPromptText(cn, q1));
                    prompts.Add(GetPromptText(cn, q2));
                    prompts.Add(GetPromptText(cn, q3));
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading security question prompts: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return prompts;
        }

        // Looks up the question text for a given question ID
        private static string GetPromptText(SqliteConnection cn, int questionID)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT QuestionPrompt FROM SecurityQuestions WHERE QuestionID = @ID";
                cmd.Parameters.AddWithValue("@ID", questionID);
                return cmd.ExecuteScalar()?.ToString() ?? "[Prompt not found]";
            }
        }

        // Checks if all three security answers match what's stored
        public static bool ValidateSecurityAnswers(string username, string ans1, string ans2, string ans3, out string errorMessage)
        {
            errorMessage = "";
            if (string.IsNullOrWhiteSpace(ans1)) { errorMessage = "Please enter an answer for security question 1."; return false; }
            if (string.IsNullOrWhiteSpace(ans2)) { errorMessage = "Please enter an answer for security question 2."; return false; }
            if (string.IsNullOrWhiteSpace(ans3)) { errorMessage = "Please enter an answer for security question 3."; return false; }
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstChallengeAnswer, SecondChallengeAnswer, ThirdChallengeAnswer FROM Logon WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) { errorMessage = "User not found."; return false; }
                        if (!string.Equals(ans1.Trim(), r.GetString(0).Trim(), StringComparison.OrdinalIgnoreCase)) { errorMessage = "Security answer 1 is incorrect."; return false; }
                        if (!string.Equals(ans2.Trim(), r.GetString(1).Trim(), StringComparison.OrdinalIgnoreCase)) { errorMessage = "Security answer 2 is incorrect."; return false; }
                        if (!string.Equals(ans3.Trim(), r.GetString(2).Trim(), StringComparison.OrdinalIgnoreCase)) { errorMessage = "Security answer 3 is incorrect."; return false; }
                        return true;
                    }
                }
            }
            catch (Exception ex) { errorMessage = "Error validating answers: " + ex.Message; return false; }
        }

        // Updates the password for a given username
        public static bool ResetUserPassword(string username, string newPassword)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Logon SET Password = @Password WHERE LogonName = @Username";
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error resetting password: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // --- SHOPPING / INVENTORY ---

        private static DataTable imageDataTable;

        // Loads all active inventory items into the shopping grid
        public static DataTable PopulateDGV(DataGridView dgv)
        {
            try
            {
                using (var cn = GetOpenConnection())
                {
                    // Load everything except images first
                    DataTable dt;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT InventoryID, ItemName, ItemDescription, RetailPrice, Quantity FROM Inventory WHERE Discontinued = 0";
                        dt = FillDataTable(cmd);
                    }

                    dgv.AllowUserToAddRows = false;
                    dgv.Columns.Clear();
                    dgv.Columns.Add(new DataGridViewImageColumn { Name = "ProductImage", HeaderText = "Product Image", ImageLayout = DataGridViewImageCellLayout.Zoom });
                    dgv.DataSource = dt;

                    // Load images separately using raw binary reader
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT InventoryID, ItemImage FROM Inventory WHERE Discontinued = 0 AND ItemImage IS NOT NULL";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                int invID = r.GetInt32(0);
                                byte[] bytes = (byte[])r[1];
                                if (bytes == null || bytes.Length == 0) continue;

                                foreach (DataGridViewRow row in dgv.Rows)
                                {
                                    if (row.Cells["InventoryID"] != null &&
                                        Convert.ToInt32(row.Cells["InventoryID"].Value) == invID)
                                    {
                                        using (var ms = new MemoryStream(bytes))
                                            row.Cells["ProductImage"].Value = Image.FromStream(ms);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (dgv.Columns.Contains("InventoryID")) dgv.Columns["InventoryID"].Visible = false;
                    if (dgv.Columns.Contains("ItemName")) dgv.Columns["ItemName"].HeaderText = "Product Name";
                    if (dgv.Columns.Contains("ItemDescription")) dgv.Columns["ItemDescription"].HeaderText = "Description";
                    if (dgv.Columns.Contains("RetailPrice")) dgv.Columns["RetailPrice"].HeaderText = "Price";
                    if (dgv.Columns.Contains("Quantity")) dgv.Columns["Quantity"].HeaderText = "Stock Left";
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.RowTemplate.Height = 65;
                    dgv.ReadOnly = true;
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    return dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error! Unable to display products!\n\n" + ex.Message); return null; }
        }

        // Returns all product categories
        public DataTable GetAllCategories()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT CategoryID, CategoryName FROM Categories";
                return FillDataTable(cmd);
            }
        }

        // Searches inventory by name and/or category
        public DataTable SearchAndFilterInventory(string searchText, int? categoryId)
        {
            using (var cn = GetOpenConnection())
            {
                // Load data without images first
                DataTable dt;
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, ItemName, ItemDescription, printf('$%.2f', RetailPrice) AS RetailPrice, Quantity FROM Inventory WHERE Discontinued = 0 AND (@searchText IS NULL OR ItemName LIKE '%' || @searchText || '%') AND (@categoryId IS NULL OR CategoryID = @categoryId)"; cmd.Parameters.AddWithValue("@searchText", string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                    dt = FillDataTable(cmd);
                }

                // Add image data as a column so frmShopping can load it
                dt.Columns.Add("ItemImage", typeof(byte[]));

                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, ItemImage FROM Inventory WHERE Discontinued = 0 AND ItemImage IS NOT NULL AND (@searchText IS NULL OR ItemName LIKE '%' || @searchText || '%') AND (@categoryId IS NULL OR CategoryID = @categoryId)";
                    cmd.Parameters.AddWithValue("@searchText", string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            int invID = r.GetInt32(0);
                            byte[] bytes = (byte[])r[1];
                            foreach (DataRow row in dt.Rows)
                            {
                                if (Convert.ToInt32(row["InventoryID"]) == invID)
                                {
                                    row["ItemImage"] = bytes;
                                    break;
                                }
                            }
                        }
                    }
                }

                return dt;
            }
        }

        // Subtracts stock when an item is added to cart
        public static bool DeductInventoryQuantity(string itemName, int amount)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity - @Amount WHERE ItemName = @ItemName AND Quantity >= @Amount";
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error updating inventory: " + ex.Message, "Inventory Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Adds stock back when an item is removed from cart
        public static bool AddInventoryQuantity(string itemName, int quantity)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity + @Quantity WHERE ItemName = @ItemName";
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        // Restocks an item by its ID
        public static void RestockItem(int itemID, int quantity)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity + @Quantity WHERE InventoryID = @InventoryID";
                cmd.Parameters.AddWithValue("@InventoryID", itemID);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.ExecuteNonQuery();
            }
        }

        // --- DISCOUNTS ---

        public class DiscountInfo
        {
            public int DiscountID { get; set; }
            public decimal DiscountPercentage { get; set; }
            public decimal DiscountDollarAmount { get; set; }
            public int DiscountLevel { get; set; }
        }

        // Returns the discount rate for a given code
        public static decimal DiscountCode(string code)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DiscountPercentage FROM Discounts WHERE DiscountCode = @DiscountCode";
                    cmd.Parameters.AddWithValue("@DiscountCode", code.Trim());
                    var result = cmd.ExecuteScalar();
                    return result != null && decimal.TryParse(result.ToString(), out decimal rate) ? rate : 0m;
                }
            }
            catch { return 0m; }
        }

        // Returns discount details for a given code
        public static DiscountInfo GetDiscountInfo(string code)
        {
            var info = new DiscountInfo();
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DiscountID, DiscountPercentage, DiscountDollarAmount FROM Discounts WHERE DiscountCode = @code";
                    cmd.Parameters.AddWithValue("@code", code);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read())
                        {
                            info.DiscountID = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                            info.DiscountPercentage = r.IsDBNull(1) ? 0m : (decimal)r.GetDouble(1);
                            info.DiscountDollarAmount = r.IsDBNull(2) ? 0m : (decimal)r.GetDouble(2);
                        }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error fetching discount info: " + ex.Message); }
            return info;
        }

        // Returns item-level discounts that match the code and are currently active
        public static List<clsItemDiscount> GetItemDiscounts(string code)
        {
            var list = new List<clsItemDiscount>();
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, DiscountID, DiscountPercentage, DiscountDollarAmount FROM Discounts WHERE DiscountCode = @DiscountCode AND date('now') BETWEEN StartDate AND ExpirationDate";
                    cmd.Parameters.AddWithValue("@DiscountCode", code);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            if (!r.IsDBNull(0))
                                list.Add(new clsItemDiscount
                                {
                                    ItemID = r.GetInt32(0),
                                    DiscountID = r.GetInt32(1),
                                    DiscountPercentage = r.IsDBNull(2) ? 0m : (decimal)r.GetDouble(2),
                                    DiscountDollarAmount = r.IsDBNull(3) ? 0m : (decimal)r.GetDouble(3),
                                });
                }
            }
            catch { }
            return list;
        }

        // Returns true if the code is a cart-level discount
        public static bool IsCartLevelDiscount(string code)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Discounts WHERE DiscountCode = @code AND DiscountLevel = 1 AND InventoryID IS NULL AND date('now') BETWEEN StartDate AND ExpirationDate";
                    cmd.Parameters.AddWithValue("@code", code ?? "");
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
            catch { return false; }
        }

        // Returns all currently active discounts
        public static DataTable GetActiveDiscounts()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT d.DiscountID, d.DiscountCode AS DiscountName, d.Description AS DiscountDescription, d.DiscountLevel, d.InventoryID, d.DiscountType, d.DiscountPercentage, d.DiscountDollarAmount, d.StartDate, d.ExpirationDate, i.ItemName FROM Discounts d LEFT JOIN Inventory i ON i.InventoryID = d.InventoryID WHERE date('now') BETWEEN d.StartDate AND d.ExpirationDate ORDER BY d.DiscountLevel, d.DiscountCode";
                return FillDataTable(cmd);
            }
        }

        // Returns all discounts for the manager grid
        public static DataTable DisplayDiscounts()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT D.DiscountID, D.DiscountCode, D.Description, D.DiscountLevel, D.DiscountType, D.DiscountPercentage, D.DiscountDollarAmount, D.StartDate, D.ExpirationDate, D.InventoryID, I.ItemName FROM Discounts D LEFT JOIN Inventory I ON D.InventoryID = I.InventoryID ORDER BY D.DiscountID DESC";
                return FillDataTable(cmd);
            }
        }

        // Inserts a new discount record
        public static bool AddDiscount(string discountCode, string description, int discountLevel, int? inventoryID, int discountType, decimal normalizedPercentage, decimal normalizedDollar, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                decimal? pct = (discountType == 0 && normalizedPercentage > 0) ? Math.Round(normalizedPercentage, 2) : (decimal?)null;
                decimal? amt = (discountType == 1 && normalizedDollar > 0) ? Math.Round(normalizedDollar, 2) : (decimal?)null;
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Discounts (DiscountCode,Description,DiscountLevel,InventoryID,DiscountType,DiscountPercentage,DiscountDollarAmount,StartDate,ExpirationDate) VALUES (@DiscountCode,@Description,@DiscountLevel,@InventoryID,@DiscountType,@DiscountPercentage,@DiscountDollarAmount,@StartDate,@ExpirationDate)";
                    cmd.Parameters.AddWithValue("@DiscountCode", discountCode ?? "");
                    cmd.Parameters.AddWithValue("@Description", description ?? "");
                    cmd.Parameters.AddWithValue("@DiscountLevel", discountLevel);
                    cmd.Parameters.AddWithValue("@DiscountType", discountType);
                    cmd.Parameters.AddWithValue("@InventoryID", (object)inventoryID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountPercentage", (object)pct ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountDollarAmount", (object)amt ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error adding discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Updates an existing discount record
        public static bool EditDiscount(int discountID, string discountCode, string description, int discountLevel, int? inventoryID, int discountType, decimal discountPercentage, decimal discountDollarAmount, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Discounts SET DiscountCode=@DiscountCode, Description=@Description, DiscountLevel=@DiscountLevel, InventoryID=@InventoryID, DiscountType=@DiscountType, DiscountPercentage=@DiscountPercentage, DiscountDollarAmount=@DiscountDollarAmount, StartDate=@StartDate, ExpirationDate=@ExpirationDate WHERE DiscountID=@DiscountID";
                    cmd.Parameters.AddWithValue("@DiscountCode", discountCode);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DiscountLevel", discountLevel);
                    cmd.Parameters.AddWithValue("@InventoryID", (object)inventoryID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountType", discountType);
                    cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    cmd.Parameters.AddWithValue("@DiscountDollarAmount", discountDollarAmount);
                    cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@DiscountID", discountID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error updating discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Updates specific fields on a discount by dictionary
        public static bool UpdateDiscountFields(int discountID, Dictionary<string, object> fieldsToUpdate)
        {
            if (fieldsToUpdate == null || fieldsToUpdate.Count == 0) return false;
            try
            {
                var setClauses = fieldsToUpdate.Keys.Select(k => $"{k} = @{k}").ToList();
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Discounts SET {string.Join(", ", setClauses)} WHERE DiscountID = @DiscountID";
                    foreach (var kvp in fieldsToUpdate)
                        cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountID", discountID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error updating discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Deletes a discount by ID
        public static bool RemoveDiscount(int discountID)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Discounts WHERE DiscountID = @id";
                    cmd.Parameters.AddWithValue("@id", discountID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error removing discount: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Returns true if the discount code already exists
        public static bool DiscountCodeExists(string discountCode)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Discounts WHERE DiscountCode = @Code";
                    cmd.Parameters.AddWithValue("@Code", discountCode ?? "");
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
            catch { return false; }
        }

        // Returns true if an identical discount already exists
        public static bool DiscountCombinationExists(int discountLevel, int? inventoryID, int discountType, decimal? discountPercentage, decimal? discountDollarAmount, DateTime startDate, DateTime expirationDate)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Discounts WHERE DiscountLevel=@Level AND ((@InventoryID IS NULL AND InventoryID IS NULL) OR InventoryID=@InventoryID) AND DiscountType=@Type AND IFNULL(DiscountPercentage,0)=IFNULL(@Pct,0) AND IFNULL(DiscountDollarAmount,0)=IFNULL(@Amt,0) AND StartDate=@Start AND ExpirationDate=@End";
                    cmd.Parameters.AddWithValue("@Level", discountLevel);
                    cmd.Parameters.AddWithValue("@InventoryID", (object)inventoryID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", discountType);
                    cmd.Parameters.AddWithValue("@Pct", (object)discountPercentage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amt", (object)discountDollarAmount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Start", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@End", expirationDate.ToString("yyyy-MM-dd"));
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
            catch { return false; }
        }

        // Checks if an inventory item exists by ID
        public static bool InventoryExists(int inventoryId) => InventoryIDExists(inventoryId);
        public static bool InventoryIDExists(int inventoryID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(1) FROM Inventory WHERE InventoryID = @id";
                cmd.Parameters.AddWithValue("@id", inventoryID);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }

        // --- ORDERS ---

        // Inserts a new order and returns the new OrderID
        public static int InsertOrder(string personID, string creditCardNumber, string expirationDate, string cvv, int? employeeId = null)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    if (employeeId.HasValue)
                    {
                        cmd.CommandText = "INSERT INTO Orders (PersonID,EmployeeID,OrderDate,CC_Number,ExpDate,CCV) VALUES (@PersonID,@EmployeeID,datetime('now'),@CC_Number,@ExpDate,@CCV); SELECT last_insert_rowid();";
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId.Value);
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO Orders (PersonID,OrderDate,CC_Number,ExpDate,CCV) VALUES (@PersonID,datetime('now'),@CC_Number,@ExpDate,@CCV); SELECT last_insert_rowid();";
                    }
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    cmd.Parameters.AddWithValue("@CC_Number", creditCardNumber ?? "");
                    cmd.Parameters.AddWithValue("@ExpDate", expirationDate ?? "");
                    cmd.Parameters.AddWithValue("@CCV", cvv ?? "");
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex) { MessageBox.Show("Error inserting order:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return -1; }
        }

        // Inserts order line items and deducts stock, all in one transaction
        public static void InsertOrderDetails(int orderID, List<clsCartItem> cartItems, List<clsItemDiscount> itemDiscounts, string discountCode)
        {
            using (var cn = GetOpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    bool isCartLevel = IsCartLevelDiscount(discountCode);
                    int? appliedDiscountID = isCartLevel ? GetDiscountInfo(discountCode)?.DiscountID : (int?)null;

                    foreach (var item in cartItems)
                    {
                        int? itemDiscountID = null;
                        if (!isCartLevel && itemDiscounts != null)
                        {
                            var match = itemDiscounts.FirstOrDefault(d => d.ItemID == item.ItemID);
                            if (match != null) itemDiscountID = match.DiscountID;
                        }

                        // Insert the line item
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "INSERT INTO OrderDetails (OrderID,InventoryID,Quantity,DiscountID) VALUES (@OrderID,@InventoryID,@Quantity,@DiscountID)";
                            cmd.Parameters.AddWithValue("@OrderID", orderID);
                            cmd.Parameters.AddWithValue("@InventoryID", item.ItemID);
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            int? raw = isCartLevel ? appliedDiscountID : itemDiscountID;
                            cmd.Parameters.AddWithValue("@DiscountID", (object)raw ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        // Deduct stock for this item
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity - @Amount WHERE InventoryID = @InventoryID AND Quantity >= @Amount";
                            cmd.Parameters.AddWithValue("@Amount", item.Quantity);
                            cmd.Parameters.AddWithValue("@InventoryID", item.ItemID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (Exception ex) { tx.Rollback(); throw new Exception("Error inserting order details: " + ex.Message); }
            }
        }

        // --- INVENTORY MANAGEMENT ---

        // Adds a new product to the inventory table
        public static bool InsertNewInventoryItem(string itemName, string itemDescription, int categoryID, decimal retailPrice, decimal cost, int quantity, int restockThreshold, byte[] itemImage, bool discontinued)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Inventory (ItemName,ItemDescription,CategoryID,RetailPrice,Cost,Quantity,RestockThreshold,ItemImage,Discontinued) VALUES (@ItemName,@ItemDescription,@CategoryID,@RetailPrice,@Cost,@Quantity,@RestockThreshold,@ItemImage,@Discontinued)";
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@ItemDescription", itemDescription);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@RetailPrice", (double)retailPrice);
                    cmd.Parameters.AddWithValue("@Cost", (double)cost);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@RestockThreshold", restockThreshold);
                    cmd.Parameters.AddWithValue("@ItemImage", (object)itemImage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", discontinued ? 1 : 0);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("SQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Marks an item as discontinued instead of deleting it
        public static bool RemoveInventoryItem(int inventoryID)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Inventory SET Discontinued = 1 WHERE InventoryID = @InventoryID";
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("SQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Adds quantity to an item by its ID
        public static bool RestockInventory(int inventoryID, int quantityToAdd)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity + @Amount WHERE InventoryID = @InventoryID";
                    cmd.Parameters.AddWithValue("@Amount", quantityToAdd);
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error restocking inventory: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        // Subtracts quantity from an item by its ID
        public static bool DecreaseInventoryQuantity(int inventoryID, int amountToSubtract)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity - @Amount WHERE InventoryID = @InventoryID AND Quantity >= @Amount";
                    cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                    cmd.Parameters.AddWithValue("@Amount", amountToSubtract);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error decreasing inventory: " + ex.Message); return false; }
        }

        // Returns true if an item with the same name or description already exists
        public static bool ItemExists(string itemName, string itemDescription)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Inventory WHERE ItemName = @ItemName OR ItemDescription = @ItemDescription";
                cmd.Parameters.AddWithValue("@ItemName", itemName);
                cmd.Parameters.AddWithValue("@ItemDescription", itemDescription);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }

        // Returns a simple ID/name list of all inventory items
        public static DataTable GetAllInventoryItems()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT InventoryID, ItemName FROM Inventory ORDER BY ItemName";
                return FillDataTable(cmd);
            }
        }

        // Shared helper that loads inventory into a grid with images
        private static DataTable BindInventoryWithImages(DataGridView grid, string whereClause)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT inv.InventoryID AS ID, inv.ItemName AS Name, inv.Cost, inv.RetailPrice AS Price, inv.Quantity AS QuantityOnHand, inv.RestockThreshold, CASE WHEN inv.Discontinued=1 THEN 'Discontinued' WHEN inv.Quantity<=0 THEN 'Out of Stock' WHEN inv.Quantity<=inv.RestockThreshold THEN 'Needs Restock' ELSE 'Available' END AS Availability, inv.ItemImage, inv.ItemDescription, inv.Discontinued FROM Inventory AS inv"
                        + (string.IsNullOrWhiteSpace(whereClause) ? "" : " WHERE " + whereClause);
                    var dt = FillDataTable(cmd);
                    var dtCopy = dt.Copy();
                    if (dt.Columns.Contains("ItemImage")) dt.Columns.Remove("ItemImage");

                    grid.AllowUserToAddRows = false;
                    grid.DataSource = dt;

                    if (!grid.Columns.Contains("ProductImage"))
                        grid.Columns.Insert(0, new DataGridViewImageColumn { Name = "ProductImage", HeaderText = "Product", ImageLayout = DataGridViewImageCellLayout.Zoom });

                    for (int i = 0; i < grid.Rows.Count && i < dtCopy.Rows.Count; i++)
                    {
                        var bytes = dtCopy.Rows[i]["ItemImage"] as byte[];
                        if (bytes != null)
                            using (var ms = new MemoryStream(bytes))
                                grid.Rows[i].Cells["ProductImage"].Value = Image.FromStream(ms);
                    }

                    string[] keep = { "ID", "Name", "Cost", "Price", "QuantityOnHand", "RestockThreshold", "Availability" };
                    foreach (DataGridViewColumn c in grid.Columns)
                        if (c.Name != "ProductImage") c.Visible = false;
                    int idx = 1;
                    foreach (var col in keep)
                        if (grid.Columns.Contains(col)) { grid.Columns[col].Visible = true; grid.Columns[col].DisplayIndex = idx++; }

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    grid.RowTemplate.Height = 65;
                    grid.ReadOnly = true;
                    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    return dtCopy;
                }
            }
            catch (Exception ex) { MessageBox.Show("Unable to load inventory:\n\n" + ex.Message, "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }
        }

        // Three filtered inventory views used by the manager form
        public static DataTable ManagerViewInventoryAll(DataGridView grid) => BindInventoryWithImages(grid, null);
        public static DataTable ManagerViewInventoryAvailable(DataGridView grid) => BindInventoryWithImages(grid, "inv.Quantity > 0 AND inv.Discontinued = 0");
        public static DataTable ManagerViewInventoryNeedsRestock(DataGridView grid) => BindInventoryWithImages(grid, "inv.Discontinued = 0 AND inv.Quantity <= inv.RestockThreshold");

        // Full inventory list for the manager report grid
        public static DataTable ManagerViewInventory(DataGridView dgvR)
        {
            try
            {
                using (var cn = GetOpenConnection())
                {
                    // Load everything except the image first
                    DataTable dt;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT InventoryID, ItemName, ItemDescription, printf('$%.2f', RetailPrice) AS RetailPrice, Quantity, RestockThreshold, Discontinued FROM Inventory";
                        dt = FillDataTable(cmd);
                    }

                    dgvR.AllowUserToAddRows = false;
                    dgvR.Columns.Clear();
                    dgvR.Columns.Add(new DataGridViewImageColumn { Name = "ProductImage", HeaderText = "Product Image", ImageLayout = DataGridViewImageCellLayout.Zoom });
                    dgvR.DataSource = dt;

                    // Load images separately using raw binary reader
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT InventoryID, ItemImage FROM Inventory WHERE ItemImage IS NOT NULL";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                int invID = r.GetInt32(0);
                                byte[] bytes = (byte[])r[1];
                                if (bytes == null || bytes.Length == 0) continue;

                                // Find the matching row in the grid
                                foreach (DataGridViewRow row in dgvR.Rows)
                                {
                                    if (row.Cells["InventoryID"] != null &&
                                        Convert.ToInt32(row.Cells["InventoryID"].Value) == invID)
                                    {
                                        using (var ms = new MemoryStream(bytes))
                                            row.Cells["ProductImage"].Value = Image.FromStream(ms);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (dgvR.Columns.Contains("InventoryID")) dgvR.Columns["InventoryID"].Visible = false;
                    if (dgvR.Columns.Contains("ItemName")) dgvR.Columns["ItemName"].HeaderText = "Product Name";
                    if (dgvR.Columns.Contains("ItemDescription")) dgvR.Columns["ItemDescription"].HeaderText = "Description";
                    if (dgvR.Columns.Contains("RetailPrice")) dgvR.Columns["RetailPrice"].HeaderText = "Price";
                    if (dgvR.Columns.Contains("Quantity")) dgvR.Columns["Quantity"].HeaderText = "Stock Left";
                    if (dgvR.Columns.Contains("RestockThreshold")) dgvR.Columns["RestockThreshold"].HeaderText = "Restock Threshold";
                    if (dgvR.Columns.Contains("Discontinued")) dgvR.Columns["Discontinued"].HeaderText = "Discontinued";
                    dgvR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvR.RowTemplate.Height = 65;
                    dgvR.ReadOnly = true;
                    dgvR.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    return dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error! Unable to display products!\n\n" + ex.Message); return null; }
        }
        // Returns items below their restock threshold
        public DataTable RestockThreshold()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT ItemName, Quantity, RestockThreshold FROM Inventory WHERE Quantity < RestockThreshold AND Discontinued = 0";
                return FillDataTable(cmd);
            }
        }

        // Returns a single item's stock info by ID
        public DataRow RestockSpecificItem(int inventoryID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT ItemName, Quantity, RestockThreshold FROM Inventory WHERE InventoryID = @InventoryID";
                cmd.Parameters.AddWithValue("@InventoryID", inventoryID);
                var dt = FillDataTable(cmd);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // --- USERS ---

        // Loads all users into a grid, optionally filtered by role
        public static DataTable DisplayUsers(DataGridView dgv, string positionFilter = null)
        {
            try
            {
                string query = "SELECT P.PersonID, P.Title, P.NameFirst, P.NameMiddle, P.NameLast, P.Suffix, P.Address1, P.Address2, P.Address3, P.City, P.Zipcode, P.State, P.Email, P.PhonePrimary, P.PhoneSecondary, L.PositionTitle, L.LogonName, L.AccountDisabled, L.AccountDeleted FROM Person P INNER JOIN Logon L ON P.PersonID = L.PersonID";
                if (!string.IsNullOrEmpty(positionFilter) && !positionFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
                    query += " WHERE L.PositionTitle = @PositionTitle";

                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = query;
                    if (!string.IsNullOrEmpty(positionFilter) && !positionFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
                        cmd.Parameters.AddWithValue("@PositionTitle", positionFilter);

                    var dt = FillDataTable(cmd);
                    dgv.AllowUserToAddRows = false;
                    dgv.DataSource = dt;
                    if (dgv.Columns.Contains("PersonID")) dgv.Columns["PersonID"].Visible = false;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.RowTemplate.Height = 35;
                    dgv.ReadOnly = true;
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    return dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error displaying users:\n\n" + ex.Message); return null; }
        }

        // Returns users filtered by position title
        public DataTable GetUsersByPosition(string positionTitle)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT P.PersonID, P.NameFirst, P.NameLast, P.Email, P.PhonePrimary, L.LogonName, L.PositionTitle, L.AccountDisabled, L.AccountDeleted FROM Person P INNER JOIN Logon L ON P.PersonID = L.PersonID WHERE (L.AccountDeleted IS NULL OR L.AccountDeleted = 0)"
                    + (positionTitle.Equals("All", StringComparison.OrdinalIgnoreCase) ? "" : " AND L.PositionTitle = @PositionTitle");
                if (!positionTitle.Equals("All", StringComparison.OrdinalIgnoreCase))
                    cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                return FillDataTable(cmd);
            }
        }

        // Returns all distinct position titles for the filter dropdown
        public static List<string> GetAllPositionTitles()
        {
            var list = new List<string>();
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT DISTINCT PositionTitle FROM Logon WHERE PositionTitle IS NOT NULL";
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(r.GetString(0));
            }
            return list;
        }

        // Sets AccountDisabled = 1 for a user
        public bool DisableUser(int personID)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Logon SET AccountDisabled = 1 WHERE PersonID = @PersonID";
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error disabling user:\n\n" + ex.Message); return false; }
        }

        // Sets AccountDisabled = 0 for a user
        public bool EnableUser(int personID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Logon SET AccountDisabled = 0 WHERE PersonID = @PersonID";
                cmd.Parameters.AddWithValue("@PersonID", personID);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Sets AccountDeleted = 1 for a user (soft delete)
        public bool DeleteUser(int personID)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Logon SET AccountDeleted = 1 WHERE PersonID = @PersonID";
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error deleting user:\n\n" + ex.Message); return false; }
        }

        // Returns a single user's details as a DataRow
        public static DataRow GetUserDetails(int personID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT P.Title,P.NameFirst,P.NameMiddle,P.NameLast,P.Suffix,P.Address1,P.Address2,P.Address3,P.City,P.Zipcode,P.State,P.Email,P.PhonePrimary,P.PhoneSecondary,L.PositionTitle FROM Person P INNER JOIN Logon L ON P.PersonID=L.PersonID WHERE P.PersonID=@PersonID";
                cmd.Parameters.AddWithValue("@PersonID", personID);
                var dt = FillDataTable(cmd);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // Updates Person and Logon tables for a user
        public static bool UpdateUserDetails(int personID, Dictionary<string, object> userData)
        {
            try
            {
                string positionTitle = userData["PositionTitle"].ToString();
                using (var cn = GetOpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    // Make sure the position exists
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "INSERT OR IGNORE INTO Position (PositionTitle) VALUES (@Title)";
                        cmd.Parameters.AddWithValue("@Title", positionTitle);
                        cmd.ExecuteNonQuery();
                    }

                    int positionID;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "SELECT PositionID FROM Position WHERE PositionTitle = @Title";
                        cmd.Parameters.AddWithValue("@Title", positionTitle);
                        positionID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Update the person record
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE Person SET Title=@Title,NameFirst=@NameFirst,NameMiddle=@NameMiddle,NameLast=@NameLast,Suffix=@Suffix,Address1=@Address1,Address2=@Address2,Address3=@Address3,City=@City,Zipcode=@Zipcode,State=@State,Email=@Email,PhonePrimary=@PhonePrimary,PhoneSecondary=@PhoneSecondary,PositionID=@PositionID WHERE PersonID=@PersonID";
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

                    // Keep the position title in sync on the Logon table too
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE Logon SET PositionTitle = @PositionTitle WHERE PersonID = @PersonID";
                        cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error updating user details:\n\n" + ex.Message); return false; }
        }

        // Quick update for basic user fields and optional login credentials
        public static bool UpdateUser(int personID, string firstName, string lastName, string phone, string email, string positionTitle, string username = null, string password = null)
        {
            try
            {
                using (var cn = GetOpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE Person SET NameFirst=@FirstName,NameLast=@LastName,PhonePrimary=@Phone,Email=@Email WHERE PersonID=@PersonID";
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PersonID", personID);
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "UPDATE Logon SET LogonName=COALESCE(NULLIF(@Username,''),LogonName), Password=COALESCE(NULLIF(@Password,''),Password), PositionTitle=@PositionTitle WHERE PersonID=@PersonID";
                            cmd.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Password", (object)password ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PositionTitle", positionTitle);
                            cmd.Parameters.AddWithValue("@PersonID", personID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error updating user:\n\n" + ex.Message); return false; }
        }

        // --- PERSON LOOKUPS ---

        // Returns a customer row by PersonID
        public static DataRow GetCustomerById(int personID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT PersonID, NameFirst, NameLast, PhonePrimary FROM Person WHERE PersonID = @PersonID LIMIT 1";
                cmd.Parameters.AddWithValue("@PersonID", personID);
                var dt = FillDataTable(cmd);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // Returns name and phone for receipt generation
        public static DataTable GetPersonDetails(string personID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT NameFirst, NameLast, PhonePrimary FROM Person WHERE PersonID = @PersonID";
                cmd.Parameters.AddWithValue("@PersonID", personID);
                return FillDataTable(cmd);
            }
        }

        // Returns full name as a single string
        public static string GetFullNameByPersonID(string personID)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT NameFirst, NameLast FROM Person WHERE PersonID = @PersonID";
                cmd.Parameters.AddWithValue("@PersonID", personID);
                using (var r = cmd.ExecuteReader())
                    if (r.Read()) return $"{r.GetString(0)} {r.GetString(1)}".Trim();
            }
            return string.Empty;
        }

        // Returns all customers (PositionTitle = 'Customer')
        public static DataTable GetAllCustomers()
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT PersonID, NameFirst, NameLast FROM Person WHERE PositionID = (SELECT PositionID FROM Position WHERE PositionTitle = 'Customer') ORDER BY NameFirst, NameLast";
                return FillDataTable(cmd);
            }
        }

        // Searches customers by ID, name, email, or phone for the picker dialog
        public static DataTable GetCustomerLookupForPicker(string searchText = null)
        {
            string q = searchText?.Trim() ?? "";
            bool isNum = !string.IsNullOrEmpty(q) && q.All(char.IsDigit);
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT PersonID, NameFirst, NameLast, Email, PhonePrimary, PhoneSecondary FROM Person WHERE (@q='') OR (@isNum=1 AND CAST(PersonID AS TEXT) LIKE @qStarts) OR NameFirst LIKE @qLike OR NameLast LIKE @qLike OR Email LIKE @qLike OR PhonePrimary LIKE @qLike OR PhoneSecondary LIKE @qLike ORDER BY NameLast, NameFirst, PersonID";
                cmd.Parameters.AddWithValue("@q", q);
                cmd.Parameters.AddWithValue("@isNum", isNum ? 1 : 0);
                cmd.Parameters.AddWithValue("@qStarts", q + "%");
                cmd.Parameters.AddWithValue("@qLike", "%" + q + "%");
                return FillDataTable(cmd);
            }
        }

        // Searches customers by email, phone, invoice number, or personID
        public static DataTable SearchCustomers(string email, string phone, string invoiceNumber, string personId)
        {
            bool noFilters = string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(invoiceNumber) && string.IsNullOrWhiteSpace(personId);
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT P.PersonID, (P.NameFirst||' '||P.NameLast) AS FullName, P.PhonePrimary, L.LogonName AS Email, O.OrderID AS LastOrderID, O.OrderDate AS LastOrderDate FROM Person P LEFT JOIN Logon L ON L.PersonID=P.PersonID LEFT JOIN Orders O ON O.PersonID=P.PersonID WHERE (@NoFilters=1) OR (@Email IS NOT NULL AND L.LogonName LIKE '%'||@Email||'%') OR (@Phone IS NOT NULL AND P.PhonePrimary LIKE '%'||@Phone||'%') OR (@Invoice IS NOT NULL AND CAST(O.OrderID AS TEXT)=@Invoice) OR (@PersonID IS NOT NULL AND CAST(P.PersonID AS TEXT)=@PersonID) ORDER BY COALESCE(O.OrderDate,'1900-01-01') DESC, P.PersonID DESC LIMIT 200";
                cmd.Parameters.AddWithValue("@NoFilters", noFilters ? 1 : 0);
                cmd.Parameters.AddWithValue("@Email", (object)(string.IsNullOrWhiteSpace(email) ? null : email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)(string.IsNullOrWhiteSpace(phone) ? null : phone) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice", (object)(string.IsNullOrWhiteSpace(invoiceNumber) ? null : invoiceNumber) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PersonID", (object)(string.IsNullOrWhiteSpace(personId) ? null : personId) ?? DBNull.Value);
                return FillDataTable(cmd);
            }
        }

        // Finds customers by order/invoice number
        public static DataTable GetCustomersByInvoice(string invoiceSearch)
        {
            string raw = (invoiceSearch ?? "").Trim().TrimStart('#').Trim();
            bool numeric = int.TryParse(raw, out int orderId);
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT DISTINCT p.PersonID, p.NameFirst, p.NameLast, p.Email, p.PhonePrimary, p.PhoneSecondary FROM Person p INNER JOIN Orders o ON o.PersonID=p.PersonID WHERE (@Numeric=1 AND o.OrderID=@OrderId) OR (@Numeric=0 AND CAST(o.OrderID AS TEXT) LIKE @Like)";
                cmd.Parameters.AddWithValue("@Numeric", numeric ? 1 : 0);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@Like", "%" + raw + "%");
                return FillDataTable(cmd);
            }
        }

        // --- REPORTS ---

        // Returns all orders for a specific customer
        public static DataTable GetOrdersForCustomer(int personId)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT o.OrderID, o.OrderDate, SUM(CAST(od.Quantity AS REAL)*inv.RetailPrice) AS TotalDue, o.CC_Number, o.CCV, o.ExpDate FROM Orders o LEFT JOIN OrderDetails od ON od.OrderID=o.OrderID LEFT JOIN Inventory inv ON inv.InventoryID=od.InventoryID WHERE o.PersonID=@pid GROUP BY o.OrderID, o.OrderDate, o.CC_Number, o.CCV, o.ExpDate ORDER BY o.OrderDate DESC, o.OrderID DESC";
                cmd.Parameters.AddWithValue("@pid", personId);
                return FillDataTable(cmd);
            }
        }

        // Returns order count, items sold, and gross total for a date range
        public static DataTable GetSalesTotals(DateTime start, DateTime end)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(DISTINCT O.OrderID) AS OrdersCount, SUM(CAST(OD.Quantity AS REAL)) AS ItemsCount, SUM(CAST(OD.Quantity AS REAL)*I.RetailPrice) AS GrossTotal FROM Orders O JOIN OrderDetails OD ON OD.OrderID=O.OrderID JOIN Inventory I ON I.InventoryID=OD.InventoryID WHERE O.OrderDate >= @Start AND O.OrderDate <= @End";
                cmd.Parameters.AddWithValue("@Start", start.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@End", end.AddDays(1).ToString("yyyy-MM-dd"));
                return FillDataTable(cmd);
            }
        }

        // Returns daily sales totals broken down by date
        public static DataTable GetSalesTotalsByDateRange(DateTime startInclusive, DateTime endInclusive)
        {
            using (var cn = GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT date(O.OrderDate) AS Date, COUNT(DISTINCT O.OrderID) AS OrdersCount, SUM(OD.Quantity) AS ItemsSold, SUM(OD.Quantity*I.RetailPrice) AS GrossSales, SUM(OD.Quantity*I.RetailPrice*IFNULL(D.DiscountPercentage,0)+IFNULL(D.DiscountDollarAmount,0)) AS TotalDiscounts, SUM(OD.Quantity*I.RetailPrice-OD.Quantity*I.RetailPrice*IFNULL(D.DiscountPercentage,0)-IFNULL(D.DiscountDollarAmount,0)) AS NetSales FROM Orders O INNER JOIN OrderDetails OD ON O.OrderID=OD.OrderID INNER JOIN Inventory I ON OD.InventoryID=I.InventoryID LEFT JOIN Discounts D ON OD.DiscountID=D.DiscountID WHERE O.OrderDate >= @Start AND O.OrderDate < date(@End,'+1 day') GROUP BY date(O.OrderDate) ORDER BY date(O.OrderDate)";
                cmd.Parameters.AddWithValue("@Start", startInclusive.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@End", endInclusive.ToString("yyyy-MM-dd"));
                return FillDataTable(cmd);
            }
        }

        // Returns all available (in-stock, not discontinued) inventory
        public static DataTable GetInventoryAvailable()
        {
            using (var cn = GetOpenConnection())
            {
                DataTable dt;
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, ItemName, Cost, printf('$%.2f', RetailPrice) AS RetailPrice, Quantity, RestockThreshold, CASE WHEN Discontinued=1 THEN 'Yes' ELSE 'No' END AS Discontinued FROM Inventory WHERE Discontinued=0 AND Quantity>0 ORDER BY ItemName";
                    dt = FillDataTable(cmd);
                }
                return LoadImagesIntoTable(cn, dt);
            }
        }

        // Returns items that need to be restocked
        public static DataTable GetInventoryNeedingRestock()
        {
            using (var cn = GetOpenConnection())
            {
                DataTable dt;
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, ItemName, Cost, printf('$%.2f', RetailPrice) AS RetailPrice, Quantity, RestockThreshold, CASE WHEN Discontinued=1 THEN 'Yes' ELSE 'No' END AS Discontinued FROM Inventory WHERE Discontinued=0 AND Quantity<RestockThreshold ORDER BY ItemName";
                    dt = FillDataTable(cmd);
                }
                return LoadImagesIntoTable(cn, dt);
            }
        }

        // Helper that adds image bytes to a DataTable by InventoryID
        private static DataTable LoadImagesIntoTable(SqliteConnection cn, DataTable dt)
        {
            if (!dt.Columns.Contains("ItemImage"))
                dt.Columns.Add("ItemImage", typeof(byte[]));

            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT InventoryID, ItemImage FROM Inventory WHERE ItemImage IS NOT NULL";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int invID = r.GetInt32(0);
                        byte[] bytes = (byte[])r[1];
                        foreach (DataRow row in dt.Rows)
                        {
                            if (Convert.ToInt32(row["InventoryID"]) == invID)
                            {
                                row["ItemImage"] = bytes;
                                break;
                            }
                        }
                    }
                }
            }
            return dt;
        }

        // Returns every inventory item regardless of status
        public static DataTable GetInventoryAll()
        {
            using (var cn = GetOpenConnection())
            {
                DataTable dt;
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT InventoryID, ItemName, Cost, printf('$%.2f', RetailPrice) AS RetailPrice, Quantity, RestockThreshold, CASE WHEN Discontinued=1 THEN 'Yes' ELSE 'No' END AS Discontinued FROM Inventory ORDER BY ItemName";
                    dt = FillDataTable(cmd);
                }
                return LoadImagesIntoTable(cn, dt);
            }
        }
    }

    // Holds discount info for a single cart item
    public class clsItemDiscount
    {
        public int ItemID { get; set; }
        public int DiscountID { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountDollarAmount { get; set; }
    }
}
