using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmDiscounts : Form
    {
        public frmDiscounts()
        {
            InitializeComponent();
        }

        private void frmDiscounts_Load(object sender, EventArgs e)
        {
            // Data
            DisplayDiscountsInGrid();
            LoadInventoryToDGV();

            cbxDiscountLevel.SelectedIndexChanged += cbxDiscountLevel_SelectedIndexChanged;
            cbxDiscountType.SelectedIndexChanged += cbxDiscountType_SelectedIndexChanged;

            if (cbxDiscountLevel.Items.Count == 0)
            {
                cbxDiscountLevel.Items.Add("Item Level");
                cbxDiscountLevel.Items.Add("Cart Level");
            }
            cbxDiscountLevel.SelectedIndex = 0;  
            cbxDiscountLevel_SelectedIndexChanged(cbxDiscountLevel, EventArgs.Empty);
        }

        private int GetSelectedDiscountTypeCode()
        {
            return cbxDiscountType.SelectedIndex == 0 ? 0 : 1;
        }

        private void cbxDiscountLevel_SelectedIndexChanged(object sender, EventArgs e)
        {

            cbxDiscountType.Items.Clear();
            cbxDiscountType.Items.Add("Percentage");
            cbxDiscountType.Items.Add("Dollar Amount");
            cbxDiscountType.SelectedIndex = 0;

            bool itemLevel = cbxDiscountLevel.SelectedIndex == 0;
            tbxInventoryID.Enabled = itemLevel;
            if (!itemLevel) tbxInventoryID.Text = "";

            cbxDiscountType_SelectedIndexChanged(cbxDiscountType, EventArgs.Empty);
        }

        private void cbxDiscountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isPct = (cbxDiscountType.SelectedIndex == 0);
            nudDiscountPercentage.Enabled = isPct;
            nudDiscountDollarAmount.Enabled = !isPct;

            if (isPct)
            {
                nudDiscountDollarAmount.Value = 0;
            }
            else
            {
                nudDiscountPercentage.Value = 0;
            }
        }

        private void DisplayDiscountsInGrid()
        {
            var dt = clsSQL.DisplayDiscounts(); 
            dgvDiscounts.DataSource = dt;
            dgvDiscounts.ReadOnly = true;
            dgvDiscounts.AllowUserToAddRows = false;
            dgvDiscounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dgvDiscounts.Columns.Contains("DiscountID")) dgvDiscounts.Columns["DiscountID"].Visible = false;
            if (dgvDiscounts.Columns.Contains("ItemImage")) dgvDiscounts.Columns["ItemImage"].Visible = false;

            SetHeader("DiscountCode", "Code");
            SetHeader("Description", "Description");
            SetHeader("DiscountLevel", "Level (0=Item,1=Cart)");
            SetHeader("DiscountType", "Type (0=% , 1=$)");
            SetHeader("DiscountPercentage", "Percent Off");
            SetHeader("DiscountDollarAmount", "Amount Off");
            SetHeader("StartDate", "Starts");
            SetHeader("ExpirationDate", "Expires");
            SetHeader("InventoryID", "Inventory ID");
            SetHeader("ItemName", "Item Name");

            dgvDiscounts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvDiscounts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10);
        }

        private void SetHeader(string colName, string header)
        {
            if (dgvDiscounts.Columns.Contains(colName))
                dgvDiscounts.Columns[colName].HeaderText = header;
        }

        private void LoadInventoryToDGV()
        {
            DataTable dt = clsSQL.GetAllInventoryItems();
            dgvInventory.DataSource = dt;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dgvInventory.Columns.Contains("InventoryID"))
            {
                dgvInventory.Columns["InventoryID"].HeaderText = "ID";
                dgvInventory.Columns["InventoryID"].Width = 60;
            }
            if (dgvInventory.Columns.Contains("ItemName"))
            {
                dgvInventory.Columns["ItemName"].HeaderText = "Item Name";
                dgvInventory.Columns["ItemName"].Width = 200;
            }
        }

        private void ClearAddDiscountFields()
        {
            tbxDiscountCode.Clear();
            tbxDescription.Clear();
            cbxDiscountLevel.SelectedIndex = 0;
            cbxDiscountType.SelectedIndex = 0;
            nudDiscountPercentage.Value = 0;
            nudDiscountDollarAmount.Value = 0;
            dtpStartDate.Value = DateTime.Today;
            dtpExpDate.Value = DateTime.Today;
            tbxInventoryID.Clear();
        }

        // -------------------- Add Discount --------------------

        private void btnAddDiscount_Click(object sender, EventArgs e)
        {
            string code = (tbxDiscountCode.Text ?? "").Trim();
            if (code.Length < 5)
            {
                MessageBox.Show("Discount Code must be at least 5 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxDiscountCode.Focus();
                return;
            }
            if (!Regex.IsMatch(code, @"^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("Discount Code can only contain letters and numbers (no spaces or special characters).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxDiscountCode.Focus();
                return;
            }

            string desc = (tbxDescription.Text ?? "").Trim();
            if (desc.Length < 5)
            {
                MessageBox.Show("Description must be at least 5 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxDescription.Focus();
                return;
            }

            int discountLevel = cbxDiscountLevel.SelectedIndex; 
            int discountType = GetSelectedDiscountTypeCode();  

            int? inventoryId = null;
            if (discountLevel == 0)
            {
                if (!int.TryParse(tbxInventoryID.Text.Trim(), out int invId) || invId <= 0)
                {
                    MessageBox.Show("Item-level discounts require a valid InventoryID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxInventoryID.Focus();
                    return;
                }
                if (!clsSQL.InventoryExists(invId))
                {
                    MessageBox.Show("The specified InventoryID does not exist.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxInventoryID.Focus();
                    return;
                }
                inventoryId = invId;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(tbxInventoryID.Text))
                {
                    MessageBox.Show("Cart-level discounts cannot specify an InventoryID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxInventoryID.Clear();
                    return;
                }
            }

            decimal pct = nudDiscountPercentage.Value;   
            decimal amt = nudDiscountDollarAmount.Value;  

            if ((pct <= 0 && amt <= 0) || (pct > 0 && amt > 0))
            {
                MessageBox.Show("Enter either a percentage OR a dollar amount (not both).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? pctNorm = null;
            decimal? amtNorm = null;

            if (discountType == 0)
            {
                pctNorm = (pct > 1m) ? (pct / 100m) : pct;
                if (pctNorm < 0m || pctNorm > 0.99m)
                {
                    MessageBox.Show("Percentage must be between 0 and 100 (or 0.00–0.99 as a fraction).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                amtNorm = null;
                amt = 0;
            }
            else
            {
                amtNorm = Math.Round(amt, 2);
                pctNorm = null;
                pct = 0;
            }

            DateTime start = dtpStartDate.Value.Date;
            DateTime end = dtpExpDate.Value.Date;
            if (end < start)
            {
                MessageBox.Show("Expiration date cannot be earlier than the start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clsSQL.DiscountCodeExists(code))
            {
                MessageBox.Show("A discount with this code already exists. Please choose a unique code.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (clsSQL.DiscountCombinationExists(discountLevel, inventoryId, discountType, pctNorm, amtNorm, start, end))
            {
                MessageBox.Show("An identical discount already exists (same level/type/target, value, and date range).", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmation summary
            string summary =
                "Please confirm the discount details:\n\n" +
                $"Code: {code}\n" +
                $"Description: {desc}\n" +
                $"Level: {(discountLevel == 0 ? "Item" : "Cart")}\n" +
                $"Type: {(discountType == 0 ? "Percentage" : "Dollar Amount")}\n" +
                $"Percentage: {(pctNorm.HasValue ? (pctNorm.Value * 100m).ToString("0.##") + "%" : "N/A")}\n" +
                $"Dollar Amount: {(amtNorm.HasValue ? "$" + amtNorm.Value.ToString("0.00") : "N/A")}\n" +
                $"InventoryID: {(inventoryId.HasValue ? inventoryId.Value.ToString() : "N/A")}\n" +
                $"Start Date: {start:yyyy-MM-dd}\n" +
                $"Expiration Date: {end:yyyy-MM-dd}";

            if (MessageBox.Show(summary, "Confirm Discount", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            bool ok = clsSQL.AddDiscount(code, desc, discountLevel, inventoryId, discountType,
                                         pctNorm ?? 0m, amtNorm ?? 0m, start, end);

            if (ok)
            {
                MessageBox.Show("Discount added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayDiscountsInGrid();
                ClearAddDiscountFields();
            }
            else
            {
                MessageBox.Show("Failed to add discount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRemoveDiscount_Click(object sender, EventArgs e)
        {
            if (dgvDiscounts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a discount to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int discountID = Convert.ToInt32(dgvDiscounts.SelectedRows[0].Cells["DiscountID"].Value);

            if (MessageBox.Show("Are you sure you want to delete this discount?", "Confirm Deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            bool removed = clsSQL.RemoveDiscount(discountID);
            if (removed)
            {
                MessageBox.Show("Discount removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayDiscountsInGrid();
            }
            else
            {
                MessageBox.Show("Failed to remove discount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Manage discounts for items and the cart here.\n\n" +
                "How to add or update a discount:\n\n" +
                "1. **Discounts Table** – The top table lists all existing discounts.\n" +
                "2. **Item Reference** – The lower-left grid shows items to help you find Inventory IDs.\n" +
                "3. **Fields** – Fill in the editor on the right:\n" +
                "   • **Discount Code** – What customers enter at checkout.\n" +
                "   • **Inventory ID** – Leave blank for cart-level discounts; set for item-level.\n" +
                "   • **Discount Description** – Short explanation of the offer.\n" +
                "   • **Discount Level** – Choose **Cart** or **Item**.\n" +
                "   • **Discount Type** – Choose **Percent Off** or **Amount Off**.\n" +
                "   • **Discount % / Amount Off** – Use only one type. Set the other to 0.\n" +
                "   • **Start Date / Expiration Date** – Valid timeframe for the discount.\n" +
                "4. **Add Discount** – Saves the discount to the system.\n" +
                "5. **Remove** – Select a discount in the table and click **Remove** to delete it.\n\n" +
                "Other options:\n" +
                "• **Help** – You’re here now! 😊\n" +
                "• **Exit** – Close the window.\n\n" +
                "Tips:\n" +
                "- Cart-level codes apply to the whole order; item-level codes apply only to the specified item.\n" +
                "- Avoid overlapping codes with the same effect.\n" +
                "- Set realistic expiration dates and keep descriptions clear for reports.",
                "Discount Management – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnExit_Click(object sender, EventArgs e) 
        { 
            this.Close(); 
        }
    }
}