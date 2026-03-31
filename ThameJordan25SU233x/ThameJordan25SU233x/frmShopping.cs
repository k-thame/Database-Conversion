using ACS_JThameM7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmShopping : Form
    {
        private DataTable fullImageData;
        private readonly clsSQL db = new clsSQL();
        private readonly string _personID;

        // POS context
        public bool IsPOSSale { get; set; } = false;
        public string ManagerID { get; set; } = "";

        public frmShopping(string personID)
        {
            InitializeComponent();
            _personID = personID;

            cbxCategories.SelectedIndexChanged += cbxCategories_SelectedIndexChanged;
            tbxSearch.TextChanged += tbxSearch_TextChanged;
            dgvItems.DataBindingComplete += dgvItems_DataBindingComplete;
        }

        private void frmShopping_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCategories();
                FilterInventory();
                dgvItems.SelectionChanged += dgvItems_SelectionChanged;

                if (IsPOSSale)
                {
                    try { this.Text = (this.Text ?? "Shopping") + " — POS Mode"; } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Technical Difficulties! Please try again later.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<clsCartItem> _itemsInCart = new List<clsCartItem>();
        private string _selectedItemName;
        private decimal _selectedItemPrice;

        private void LoadCategories()
        {
            DataTable dt = db.GetAllCategories();

            DataRow allRow = dt.NewRow();
            allRow["CategoryID"] = DBNull.Value;
            allRow["CategoryName"] = "All Categories";
            dt.Rows.InsertAt(allRow, 0);

            cbxCategories.DataSource = dt;
            cbxCategories.DisplayMember = "CategoryName";
            cbxCategories.ValueMember = "CategoryID";
            cbxCategories.SelectedIndex = 0;
        }

        private void FilterInventory()
        {
            string search = tbxSearch.Text.Trim();
            int? categoryId = cbxCategories.SelectedValue == null || cbxCategories.SelectedValue is DBNull ? null : (int?)Convert.ToInt32(cbxCategories.SelectedValue);
            DataTable dt = db.SearchAndFilterInventory(search, categoryId);
            fullImageData = dt;
            dgvItems.Columns.Clear();
            DataGridViewImageColumn imageCol = new DataGridViewImageColumn
            {
                Name = "ProductImage",
                HeaderText = "Product Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dgvItems.Columns.Add(imageCol);
            dgvItems.DataSource = dt;

            // Manually call formatting
            dgvItems_DataBindingComplete(dgvItems, new DataGridViewBindingCompleteEventArgs(ListChangedType.Reset));
        }

        private void tbxSearch_TextChanged(object sender, EventArgs e) => FilterInventory();

        private void cbxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxCategories.Focused)
                FilterInventory();
        }

        private void cmbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxSearch_TextChanged(sender, e);
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvItems.Rows[e.RowIndex];
                    _selectedItemName = row.Cells["ItemName"].Value.ToString();
                    _selectedItemPrice = Convert.ToDecimal(row.Cells["RetailPrice"].Value.ToString().Replace("$", ""));

                    int quantityInStock = Convert.ToInt32(row.Cells["Quantity"].Value);

                    if (quantityInStock >= 1)
                    {
                        nudQuantity.Minimum = 1;
                        nudQuantity.Maximum = quantityInStock;
                        nudQuantity.Value = 1;
                        btnAddToCart.Enabled = true;
                    }
                    else
                    {
                        nudQuantity.Minimum = 0;
                        nudQuantity.Maximum = 0;
                        nudQuantity.Value = 0;
                        btnAddToCart.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Unable to display product details.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.CurrentRow != null && dgvItems.Columns.Contains("Quantity"))
                {
                    var row = dgvItems.CurrentRow;
                    int quantityInStock = Convert.ToInt32(row.Cells["Quantity"].Value);

                    if (quantityInStock >= 1)
                    {
                        nudQuantity.Minimum = 1;
                        nudQuantity.Maximum = quantityInStock;
                        if (nudQuantity.Value < 1 || nudQuantity.Value > quantityInStock)
                            nudQuantity.Value = 1;
                        btnAddToCart.Enabled = true;
                    }
                    else
                    {
                        nudQuantity.Minimum = 0;
                        nudQuantity.Maximum = 0;
                        nudQuantity.Value = 0;
                        btnAddToCart.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update quantity selection: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvItems_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (fullImageData == null) return;

            dgvItems.RowTemplate.Height = 65;
            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dgvItems.Columns.Contains("InventoryID"))
                dgvItems.Columns["InventoryID"].Visible = false;
            if (dgvItems.Columns.Contains("ItemImage"))
                dgvItems.Columns["ItemImage"].Visible = false;

            if (dgvItems.Columns.Contains("ItemName"))
            {
                dgvItems.Columns["ItemName"].HeaderText = "Product Name";
                dgvItems.Columns["ItemName"].FillWeight = 100;
            }
            if (dgvItems.Columns.Contains("Quantity"))
            {
                dgvItems.Columns["Quantity"].HeaderText = "Stock Left";
                dgvItems.Columns["Quantity"].FillWeight = 80;
            }
            if (dgvItems.Columns.Contains("RetailPrice"))
            {
                dgvItems.Columns["RetailPrice"].HeaderText = "Price";
                dgvItems.Columns["RetailPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvItems.Columns["RetailPrice"].FillWeight = 80;
            }
            if (dgvItems.Columns.Contains("ItemDescription"))
            {
                dgvItems.Columns["ItemDescription"].HeaderText = "Description";
                dgvItems.Columns["ItemDescription"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvItems.Columns["ItemDescription"].FillWeight = 400;
            }
            if (dgvItems.Columns.Contains("ProductImage"))
                dgvItems.Columns["ProductImage"].FillWeight = 50;

            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvItems.RowTemplate.Height = 65;
            dgvItems.ReadOnly = true;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.AllowUserToAddRows = false;

            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (i >= fullImageData.Rows.Count) break;
                byte[] imgBytes = fullImageData.Rows[i]["ItemImage"] as byte[];
                if (imgBytes != null && dgvItems.Rows[i].Cells["ProductImage"] is DataGridViewImageCell imgCell)
                {
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        Image fullImg = Image.FromStream(ms);
                        Image thumb = new Bitmap(fullImg, new Size(60, 60));
                        imgCell.Value = thumb;
                    }
                }
            }
        }

        private void btnDecreaseQuantity_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbxCart.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an item in your cart to decrease.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedItemText = lbxCart.SelectedItem.ToString();
                string itemName = selectedItemText.Split('-')[0].Trim();

                var itemInCart = _itemsInCart.FirstOrDefault(x => x.ItemName == itemName);
                if (itemInCart == null)
                {
                    MessageBox.Show("Item not found in cart.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int quantityToDecrease = (int)nudDecrease.Value;

                if (quantityToDecrease <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity to decrease.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (quantityToDecrease >= itemInCart.Quantity)
                {
                    bool restored = clsSQL.AddInventoryQuantity(itemInCart.ItemName, itemInCart.Quantity);
                    if (!restored)
                    {
                        MessageBox.Show("Failed to update inventory.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _itemsInCart.Remove(itemInCart);
                }
                else
                {
                    bool restored = clsSQL.AddInventoryQuantity(itemInCart.ItemName, quantityToDecrease);
                    if (!restored)
                    {
                        MessageBox.Show("Failed to update inventory.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    itemInCart.Quantity -= quantityToDecrease;
                }

                lbxCart.Items.Clear();
                foreach (var item in _itemsInCart)
                    lbxCart.Items.Add($"{item.ItemName} - ${item.Price:F2} x{item.Quantity}");

                clsSQL.PopulateDGV(dgvItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decreasing quantity: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.CurrentRow == null)
                {
                    MessageBox.Show("Please select an item.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow row = dgvItems.CurrentRow;

                int itemID = Convert.ToInt32(row.Cells["InventoryID"].Value);
                string itemName = row.Cells["ItemName"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["RetailPrice"].Value.ToString().Replace("$", ""));
                int stock = Convert.ToInt32(row.Cells["Quantity"].Value);
                int desiredQty = (int)nudQuantity.Value;

                var existingItem = _itemsInCart.FirstOrDefault(x => x.ItemID == itemID);
                int currentQty = existingItem != null ? existingItem.Quantity : 0;

                if (currentQty + desiredQty > stock)
                {
                    MessageBox.Show("Not enough stock available for this item.", "Stock Limit",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (existingItem != null)
                {
                    existingItem.Quantity += desiredQty;
                }
                else
                {
                    _itemsInCart.Add(new clsCartItem(itemID, itemName, price, desiredQty, stock));
                }

                lbxCart.Items.Clear();
                foreach (var cartItem in _itemsInCart)
                    lbxCart.Items.Add($"{cartItem.ItemName} - ${cartItem.Price:F2} x{cartItem.Quantity}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item to cart: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveFromcCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (_itemsInCart.Count == 0)
                {
                    MessageBox.Show("Your cart is currently empty. There is nothing to remove.",
                        "Cart is Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (lbxCart.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an item to remove from your cart.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to remove this item from your cart? This action cannot be undone.",
                    "Remove from Cart", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string selectedItemText = lbxCart.SelectedItem.ToString();
                    string itemName = selectedItemText.Split('-')[0].Trim();

                    var itemToRemove = _itemsInCart.FirstOrDefault(x => x.ItemName == itemName);
                    if (itemToRemove != null)
                    {
                        _itemsInCart.Remove(itemToRemove);
                        lbxCart.Items.Clear();
                        foreach (var item in _itemsInCart)
                            lbxCart.Items.Add($"{item.ItemName} - ${item.Price:F2} x{item.Quantity}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Unable to remove item from the cart. Please try again.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (_itemsInCart.Count == 0)
                {
                    MessageBox.Show("Your cart is already empty.", "No items to clear",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to clear your cart? This action cannot be undone.",
                    "Clear cart?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    _itemsInCart.Clear();
                    lbxCart.Items.Clear();

                    int selectedRowIndex = dgvItems.CurrentCell?.RowIndex ?? -1;
                    FilterInventory();

                    if (selectedRowIndex >= 0 && selectedRowIndex < dgvItems.Rows.Count)
                    {
                        dgvItems.ClearSelection();
                        dgvItems.Rows[selectedRowIndex].Selected = true;

                        foreach (DataGridViewColumn col in dgvItems.Columns)
                        {
                            if (col.Visible)
                            {
                                dgvItems.CurrentCell = dgvItems.Rows[selectedRowIndex].Cells[col.Index];
                                break;
                            }
                        }
                    }

                    MessageBox.Show("Your cart has been cleared.", "Cart cleared",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to clear the cart! Please try again.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Do you wish to checkout now?", "Checkout?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (_itemsInCart.Count == 0)
                    {
                        MessageBox.Show("Your cart is empty. Please add items before checking out.",
                            "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    this.Hide();
                    var checkout = new frmCheckout(_personID, _itemsInCart, this)
                    {
                        IsPOSSale = this.IsPOSSale,
                        ManagerID = this.ManagerID ?? ""
                    };

                    DialogResult checkoutResult = checkout.ShowDialog();
                    this.Show();

                    if (checkoutResult == DialogResult.OK)
                    {
                        _itemsInCart.Clear();
                        lbxCart.Items.Clear();
                        FilterInventory();
                        MessageBox.Show("Thank you for your purchase!", "Order Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Checkout is currently down! Please try again later.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "**SmolTech Shopping Help Guide**\n\n" +
                "1) Filter by category or search\n" +
                "2) Select a product to see details\n" +
                "3) Choose quantity, Add To Cart\n" +
                "4) Manage your cart (decrease/remove/clear)\n" +
                "5) Checkout when ready\n\n" +
                "Tips:\n- Zero stock items can't be added\n- Review your cart before checkout",
                "Shopping Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (IsPOSSale && this.Owner is frmManaging)
            {
                var confirm = MessageBox.Show("Exit shopping and return to Manager screen?",
                    "Exit Shopping", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                    this.Close();

                return;
            }

            var result = MessageBox.Show(
                "If you exit, you will be signed out. Do you wish to continue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
    }
}
