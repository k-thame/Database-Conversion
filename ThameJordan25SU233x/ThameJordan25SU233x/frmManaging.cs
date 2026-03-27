using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ACS_JThameM7;

namespace ThameJordan25SU233x
{
    public partial class frmManaging : Form
    {
        private readonly clsSQL db = new clsSQL();
        private int? _selectedCustomerId = null;

        // Guards to prevent frmUsers from relaunching
        private bool _usersDialogOpen = false;
        private DateTime _suppressUsersRelaunchUntil = DateTime.MinValue;

        // From login
        public int? ManagerEmployeeID { get; set; } = null;
        public string ManagerDisplayName { get; set; } = "";

        public frmManaging()
        {
            InitializeComponent();
        }

        private void frmManaging_Load(object sender, EventArgs e)
        {
            this.Shown -= frmManaging_Shown;
            this.Activated -= frmManaging_Activated;
            this.Shown += frmManaging_Shown;
            this.Activated += frmManaging_Activated;

            // Restock threshold alerts
            DataTable lowStock = db.RestockThreshold();
            if (lowStock.Rows.Count > 0)
            {
                string message = "Low Stock Alerts:\n\n";
                foreach (DataRow row in lowStock.Rows)
                    message += $"- {row["ItemName"]} (Qty: {row["Quantity"]}, Threshold: {row["RestockThreshold"]})\n";
                MessageBox.Show(message, "Restock Needed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RefreshInventoryDGV();
            UpdateSelectionUI();

            btnMyProfile.Click -= btnMyProfile_Click;
            btnMyProfile.Click += btnMyProfile_Click;
        }

        private void frmManaging_Shown(object sender, EventArgs e) { }

        // Never auto-open frmUsers on activation
        private void frmManaging_Activated(object sender, EventArgs e)
        {
            if (DateTime.UtcNow < _suppressUsersRelaunchUntil) return;
        }

        private void btnShopForTheCustomer_Click(object sender, EventArgs e)
        {
            StartShoppingForSelectedCustomer();
        }

        // Refresh the inventory grid
        private void RefreshInventoryDGV()
        {
            clsSQL.ManagerViewInventory(dgvInventory);

            if (dgvInventory.Columns.Contains("ItemDescription"))
            {
                dgvInventory.Columns["ItemDescription"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvInventory.Columns["ItemDescription"].Width = 300;
            }
            if (dgvInventory.Columns.Contains("ProductImage"))
                dgvInventory.Columns["ProductImage"].Width = 55;
            if (dgvInventory.Columns.Contains("ItemName"))
                dgvInventory.Columns["ItemName"].Width = 120;
            if (dgvInventory.Columns.Contains("RetailPrice"))
                dgvInventory.Columns["RetailPrice"].Width = 60;
            if (dgvInventory.Columns.Contains("Quantity"))
                dgvInventory.Columns["Quantity"].Width = 60;
            if (dgvInventory.Columns.Contains("RestockThreshold"))
                dgvInventory.Columns["RestockThreshold"].Width = 60;
            if (dgvInventory.Columns.Contains("Discontinued"))
                dgvInventory.Columns["Discontinued"].Width = 60;

            dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void dgvInventory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow != null)
            {
                int stock = Convert.ToInt32(dgvInventory.CurrentRow.Cells["Quantity"].Value);
                nudRemoveFromStock.Minimum = 1;
                nudRemoveFromStock.Maximum = stock;
            }
        }

        private void nudRestock_ValueChanged(object sender, EventArgs e)
        {
            if (nudRestock.Value > 5000)
            {
                MessageBox.Show("You cannot restock more than 5000 units.", "Exceeds Maximum Restock Quantity",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudRestock.Value = 5000;
            }
        }

        // Restock selected item
        private void btnRestock_Click(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow == null)
            {
                MessageBox.Show("Please select an item to restock.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int inventoryID = Convert.ToInt32(dgvInventory.CurrentRow.Cells["InventoryID"].Value);
            string itemName = dgvInventory.CurrentRow.Cells["ItemName"].Value.ToString();
            int quantityToAdd = (int)nudRestock.Value;

            if (quantityToAdd <= 0)
            {
                MessageBox.Show("Enter a quantity greater than zero.", "Invalid Quantity",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = clsSQL.RestockInventory(inventoryID, quantityToAdd);

            if (success)
            {
                MessageBox.Show($"{quantityToAdd} units of \"{itemName}\" successfully restocked.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshInventoryDGV();

                DataRow itemInfo = db.RestockSpecificItem(inventoryID);
                int quantity = Convert.ToInt32(itemInfo["Quantity"]);
                int threshold = Convert.ToInt32(itemInfo["RestockThreshold"]);

                if (quantity < threshold)
                    MessageBox.Show($"'{itemName}' is still below its restock threshold.\nQty: {quantity} | Threshold: {threshold}",
                        "Low Stock Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                nudRestock.Value = 0;
            }
            else
            {
                MessageBox.Show("Failed to restock inventory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Decrease stock of selected item
        private void btnDecreaseQty_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.CurrentRow == null)
                {
                    MessageBox.Show("Please select an item to decrease.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int inventoryID = Convert.ToInt32(dgvInventory.CurrentRow.Cells["InventoryID"].Value);
                string itemName = dgvInventory.CurrentRow.Cells["ItemName"].Value.ToString();
                int currentQty = Convert.ToInt32(dgvInventory.CurrentRow.Cells["Quantity"].Value);
                int decreaseAmount = (int)nudRemoveFromStock.Value;

                if (decreaseAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity to decrease.", "Invalid Quantity",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (decreaseAmount > currentQty)
                {
                    MessageBox.Show("You cannot decrease more than what's in stock.", "Too Much",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentQty - decreaseAmount == 0)
                {
                    var confirmResult = MessageBox.Show(
                        $"Are you sure you want to decrease the stock of \"{itemName}\" to 0?",
                        "Empty Stock Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.No) return;
                }

                bool success = clsSQL.DecreaseInventoryQuantity(inventoryID, decreaseAmount);

                if (success)
                {
                    MessageBox.Show($"{decreaseAmount} units of \"{itemName}\" successfully removed from inventory.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefreshInventoryDGV();

                    DataRow itemInfo = db.RestockSpecificItem(inventoryID);
                    int quantity = Convert.ToInt32(itemInfo["Quantity"]);
                    int threshold = Convert.ToInt32(itemInfo["RestockThreshold"]);

                    if (quantity < threshold)
                        MessageBox.Show($"'{itemName}' is still below its restock threshold.\nQty: {quantity} | Threshold: {threshold}",
                            "Low Stock Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    nudRemoveFromStock.Value = 0;
                }
                else
                {
                    MessageBox.Show("Failed to decrease quantity.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message);
            }
        }

        // Open add item form
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                using (var newItem = new frmAddItem())
                {
                    newItem.StartPosition = FormStartPosition.CenterParent;
                    newItem.ShowDialog(this);
                }
                RefreshInventoryDGV();
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        // Discontinue selected item
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.CurrentRow == null)
                {
                    MessageBox.Show("Please select an item to discontinue.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var discontinuedObj = dgvInventory.CurrentRow.Cells["Discontinued"].Value;
                bool isDiscontinued = discontinuedObj != DBNull.Value && Convert.ToInt32(discontinuedObj) == 1;
                if (isDiscontinued)
                {
                    MessageBox.Show("This item has already been discontinued.", "Already Discontinued",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you wish to discontinue this item? Customers will no longer be able to purchase this.",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;

                int inventoryID = Convert.ToInt32(dgvInventory.CurrentRow.Cells["InventoryID"].Value);
                bool success = clsSQL.RemoveInventoryItem(inventoryID);

                if (success)
                {
                    MessageBox.Show("Inventory successfully discontinued.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshInventoryDGV();
                }
                else
                {
                    MessageBox.Show("Failed to discontinue selected item.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed to discontinue selected item.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Open user management form
        private void btnModifyUser_Click(object sender, EventArgs e)
        {
            if (_usersDialogOpen) return;

            _usersDialogOpen = true;
            this.Hide();
            try
            {
                using (var modifyUser = new frmUsers())
                {
                    modifyUser.StartPosition = FormStartPosition.CenterParent;
                    modifyUser.ShowInTaskbar = false;
                    modifyUser.MinimizeBox = false;
                    modifyUser.MaximizeBox = false;
                    modifyUser.ShowDialog(this);
                }
            }
            finally
            {
                this.Show();
                this.Activate();
                _usersDialogOpen = false;
                _suppressUsersRelaunchUntil = DateTime.UtcNow.AddMilliseconds(500);
            }
        }

        // Open discounts form
        private void btnDiscounts_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                using (var discounts = new frmDiscounts())
                {
                    discounts.StartPosition = FormStartPosition.CenterParent;
                    discounts.ShowDialog(this);
                }
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        // Open sales reports form
        private void btnSalesReports_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                using (var reports = new frmReports())
                {
                    reports.StartPosition = FormStartPosition.CenterParent;
                    reports.ShowDialog(this);
                }
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        // Select a customer for POS
        private void button1_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmSelectCustomer())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedPersonID.HasValue)
                {
                    _selectedCustomerId = dlg.SelectedPersonID.Value;

                    string fullName = "(unknown)";
                    string phone = "";
                    try
                    {
                        var row = clsSQL.GetCustomerById(_selectedCustomerId.Value);
                        if (row != null)
                        {
                            string first = Convert.ToString(row["NameFirst"] ?? "").Trim();
                            string last = Convert.ToString(row["NameLast"] ?? "").Trim();
                            fullName = ((first + " " + last).Trim().Length > 0) ? (first + " " + last).Trim() : "(no name on file)";
                            phone = Convert.ToString(row["PhonePrimary"] ?? "").Trim();
                        }
                    }
                    catch { }

                    var lbl = this.Controls.Find("lblSelectedCustomer", true);
                    string summary = $"Customer: {fullName}  (ID {_selectedCustomerId.Value})" + (string.IsNullOrEmpty(phone) ? "" : $"  •  {phone}");
                    if (lbl != null && lbl.Length > 0) lbl[0].Text = summary;
                    else this.Text = "Point of Sale — " + summary;

                    UpdateSelectionUI();

                    var result = MessageBox.Show(
                        $"Customer selected:\n\n{fullName}\nID: {_selectedCustomerId.Value}" +
                        (string.IsNullOrEmpty(phone) ? "" : $"\nPhone: {phone}") +
                        "\n\nDo you want to proceed to the shopping page now?",
                        "Start Shopping?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        StartShoppingForSelectedCustomer();
                    else
                        MessageBox.Show("You can now click 'View History' to review past orders or 'Start Shopping' when ready.",
                            "Next steps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Start shopping for selected customer
        private void StartShoppingForSelectedCustomer()
        {
            if (!_selectedCustomerId.HasValue)
            {
                MessageBox.Show("Please select a customer first.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string personIdString = _selectedCustomerId.Value.ToString();
            this.Hide();
            try
            {
                using (var shop = new frmShopping(personIdString))
                {
                    shop.IsPOSSale = true;
                    shop.ManagerID = GetManagerDisplayNameSafe();
                    shop.Tag = ManagerEmployeeID;
                    shop.StartPosition = FormStartPosition.CenterParent;
                    shop.ShowDialog(this);
                }
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Welcome to General Management.\n\n" +
                "Inventory actions:\n" +
                "1. **Inventory Table** – Large grid shows current products.\n" +
                "2. **Restock Selected Item** – Enter a quantity and click **Re-stock** to add to the selected item's stock.\n" +
                "3. **Remove stock of selected Item** – Enter a quantity and click **Remove** to deduct from stock.\n" +
                "4. **Add New Item!** – Click **Add** to open the Add Item form.\n" +
                "5. **Discontinue an Item** – Select a product and click **Remove** to mark it unavailable.\n\n" +
                "Other management:\n" +
                "• **Discounts** – Open discount management.\n" +
                "• **Edit (User Management)** – Manage user accounts (add, update, disable, remove).\n" +
                "• **Sales Reports** – Open reports for inventory and sales.\n" +
                "• **Select Customer / Start Shopping!** – Choose a customer and launch the POS.\n" +
                "• **View Sales History** – Review past orders.\n" +
                "• **My Profile** – View or edit your profile.\n" +
                "• **Help / Exit** – Get help or leave the screen.\n\n" +
                "Tips:\n" +
                "- Always select the correct row before restocking or removing stock.\n" +
                "- Use **Add New Item** for new products rather than editing in-place.\n" +
                "- Discontinued items won't appear as available for sale.",
                "General Management – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Are you sure you want to exit Manager Mode?\nAny unsaved changes will be lost.",
                "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes) this.Close();
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (!_selectedCustomerId.HasValue)
            {
                MessageBox.Show("Please select a customer first.", "No Customer",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                clsHTML.ShowCustomerHistoryHtml(_selectedCustomerId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load history:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSelectionUI()
        {
            bool hasCustomer = _selectedCustomerId.HasValue;

            var btnView = this.Controls.Find("btnViewHistory", true).FirstOrDefault() as Button;
            var btnStart = this.Controls.Find("btnStartShopping", true).FirstOrDefault() as Button;
            var btnShop = this.Controls.Find("btnShopForTheCustomer", true).FirstOrDefault() as Button;

            if (btnView != null) { btnView.Visible = hasCustomer; btnView.Enabled = hasCustomer; }
            if (btnStart != null) { btnStart.Visible = hasCustomer; btnStart.Enabled = hasCustomer; }
            if (btnShop != null) { btnShop.Visible = hasCustomer; btnShop.Enabled = hasCustomer; }
        }

        private string GetManagerDisplayNameSafe()
        {
            if (!string.IsNullOrWhiteSpace(ManagerDisplayName)) return ManagerDisplayName.Trim();
            var lbl = this.Controls.Find("lblManagerName", true).FirstOrDefault() as Label;
            if (lbl != null && !string.IsNullOrWhiteSpace(lbl.Text)) return lbl.Text.Trim();
            return "";
        }

        private void btnMyProfile_Click(object sender, EventArgs e)
        {
            if (_usersDialogOpen) return;

            if (!ManagerEmployeeID.HasValue)
            {
                MessageBox.Show("Your Employee/Person ID is missing. Please re-login or contact admin.",
                    "My Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _usersDialogOpen = true;
            this.Hide();
            try
            {
                using (var f = new frmUsers { PreselectEmployeeID = ManagerEmployeeID, SelfEditOnly = true })
                {
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.ShowDialog(this);
                }
            }
            finally
            {
                this.Show();
                this.Activate();
                _usersDialogOpen = false;
                _suppressUsersRelaunchUntil = DateTime.UtcNow.AddMilliseconds(500);
            }
        }
    }
}