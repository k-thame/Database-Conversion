using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ThameJordan25SU233x.clsSQL;
using ACS_JThameM7;

namespace ThameJordan25SU233x
{
    public partial class frmCheckout : Form
    {
        private readonly string _personID;
        private readonly frmShopping _shoppingForm;

        public List<clsCartItem> CartItems { get; set; } = new List<clsCartItem>();
        public clsCart currentCart { get; set; } = new clsCart(0, new List<clsCartItem>());

        public bool IsPOSSale { get; set; } = false;
        public string ManagerID { get; set; } = "";

        public frmCheckout(string personID, List<clsCartItem> cartItems, frmShopping shoppingForm)
        {
            InitializeComponent();
            _personID = personID;
            _shoppingForm = shoppingForm;
            CartItems = cartItems;
            currentCart.CartItems = cartItems;
        }

        private void frmCheckout_Load(object sender, EventArgs e)
        {
            // Input restrictions
            tbxCardNumber.KeyPress += tbxCardNumber_KeyPress;
            tbxCVV.KeyPress += tbxCVV_KeyPress;
            tbxExpiration.KeyPress += tbxExpiration_KeyPress;

            DisplayCartSummary();
            currentCart.CartItems = CartItems;
            currentCart.Recalculate();

            lblSubtotal.Text = currentCart.Subtotal.ToString("C");
            lblTax.Text = currentCart.TaxAmount.ToString("C");
            lblTotalDue.Text = currentCart.TotalDue.ToString("C");

            if (IsPOSSale)
            {
                try { this.Text = (this.Text ?? "Checkout") + " — POS Mode"; } catch { }

                var soldBy = this.Controls.Find("lblSoldBy", true);
                if (soldBy != null && soldBy.Length > 0)
                {
                    soldBy[0].Text = string.IsNullOrWhiteSpace(ManagerID)
                        ? "Sold by: (manager)"
                        : "Sold by: " + ManagerID;
                }

                if (lblActiveDiscounts != null) lblActiveDiscounts.Visible = true;
                if (dgvActiveDiscounts != null)
                {
                    dgvActiveDiscounts.Visible = true;
                    LoadAvailableDiscountsForPOS();
                }
            }
            else
            {
                if (lblActiveDiscounts != null) lblActiveDiscounts.Visible = false;
                if (dgvActiveDiscounts != null) dgvActiveDiscounts.Visible = false;
            }
        }

        private void LoadAvailableDiscountsForPOS()
        {
            try
            {
                if (dgvActiveDiscounts == null) return;

                DataTable dt = clsSQL.GetActiveDiscounts(); 
                if (dt == null || dt.Rows.Count == 0)
                {
                    dgvActiveDiscounts.DataSource = null;
                    return;
                }

                dgvActiveDiscounts.DataSource = dt;

                // Grid cosmetics
                dgvActiveDiscounts.ReadOnly = true;
                dgvActiveDiscounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvActiveDiscounts.MultiSelect = false;
                dgvActiveDiscounts.RowHeadersVisible = false;
                dgvActiveDiscounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvActiveDiscounts.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                foreach (DataGridViewColumn c in dgvActiveDiscounts.Columns) c.Visible = false;

                if (dgvActiveDiscounts.Columns.Contains("DiscountName"))
                {
                    var col = dgvActiveDiscounts.Columns["DiscountName"];
                    col.HeaderText = "Discount";
                    col.Visible = true;
                    col.FillWeight = 40;
                }
                if (dgvActiveDiscounts.Columns.Contains("DiscountDescription"))
                {
                    var col = dgvActiveDiscounts.Columns["DiscountDescription"];
                    col.HeaderText = "Description";
                    col.Visible = true;
                    col.FillWeight = 60;
                    col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load discounts:\n" + ex.Message, "Discounts",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayCartSummary()
        {
            lbxCartSummary.Items.Clear();
            foreach (var item in CartItems)
            {
                lbxCartSummary.Items.Add($"{item.ItemName} - ${item.Price:F2} x{item.Quantity}");
            }
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxPaymentTypes.SelectedItem == null)
                {
                    MessageBox.Show("Please select a payment type.", "Missing Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string cardNumber = tbxCardNumber.Text;
                if (!clsValidation.IsValidCardNumber(cardNumber, out string cardError))
                {
                    MessageBox.Show(cardError, "Card Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxCardNumber.Focus();
                    return;
                }

                string expDate = tbxExpiration.Text;
                if (!clsValidation.IsValidExpiration(expDate, out string expError))
                {
                    MessageBox.Show(expError, "Expiration Date Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxExpiration.Focus();
                    return;
                }

                string cvv = tbxCVV.Text;
                if (!clsValidation.IsValidCVV(cvv, out string cvvError))
                {
                    MessageBox.Show(cvvError, "CVV Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxCVV.Focus();
                    return;
                }

                if (IsPOSSale)
                {
                    var confirm = MessageBox.Show(
                        "This transaction will be recorded as a manager-assisted POS sale."
                        + (string.IsNullOrWhiteSpace(ManagerID) ? "" : $"\nManager: {ManagerID}")
                        + "\n\nProceed with purchase?",
                        "Confirm POS Purchase", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes) return;
                }


                int? employeeId = null;
                if (this.Owner is frmShopping shop && shop.Owner is frmManaging mgr && mgr.ManagerEmployeeID.HasValue)
                    employeeId = mgr.ManagerEmployeeID.Value;

                int orderID;
                try
                {
                    orderID = clsSQL.InsertOrder(_personID, cardNumber, expDate, cvv, employeeId);
                }
                catch (MissingMethodException)
                {
                    orderID = clsSQL.InsertOrder(_personID, cardNumber, expDate, cvv);
                }

                if (orderID > 0)
                {
                    clsSQL.InsertOrderDetails(orderID, currentCart.CartItems, currentCart.ItemDiscounts, currentCart.DiscountCode);
                    clsHTML.PrintReport(currentCart, _personID, this.IsPOSSale, this.ManagerID);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to insert order. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to complete your purchase. Please try again later.\n\n" + ex.Message,
                    "Checkout Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbxCardNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || tbxCardNumber.Text.Length >= 16;
        }

        private void tbxExpiration_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '/') || tbxExpiration.Text.Length >= 7;
        }

        private void tbxCVV_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || tbxCVV.Text.Length >= 3;
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("If you go back, your payment info will be cleared. Continue?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Need help checking out?\n\n" +
                "Steps to complete your order:\n\n" +
                "1. **Review Cart** – The left list shows items in your cart.\n" +
                "   • **Subtotal / Tax / Discount / Total Due** are shown below.\n" +
                "2. **Active Discounts** – The panel on the right lists any discounts currently applied.\n" +
                "3. **Payment Info** – Provide:\n" +
                "   • **Payment Type** – Choose your payment method.\n" +
                "   • **First/Last Name on Card** – As printed on the card.\n" +
                "   • **Credit Card Number**, **Expiration (MM/YY)**, **CVV** – Required for card payments.\n" +
                "4. **Promo Code** – Enter a code and click **Apply** to validate and add the discount.\n" +
                "5. **Place Order** – Click **PLACE ORDER** to process payment and finalize.\n\n" +
                "Other options:\n" +
                "• **Continue Shopping** – Return to the store without placing the order.\n" +
                "• **Help** – You’re here now! 😊\n\n" +
                "Tips:\n" +
                "- Only one promo code is applied at a time.\n" +
                "- Make sure billing details match the card to avoid declines.\n" +
                "- The order isn’t created until you click **PLACE ORDER**.",
                "Checkout – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnApplyDiscount_Click(object sender, EventArgs e)
        {
            string code = tbxDiscount.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter a discount code.");
                return;
            }

            var rawItemDiscounts = clsSQL.GetItemDiscounts(code);
            bool isCartLevel = clsSQL.IsCartLevelDiscount(code);

            if (rawItemDiscounts.Count == 0 && !isCartLevel)
            {
                MessageBox.Show("Invalid or expired discount code.");
                return;
            }

            if (!isCartLevel)
            {
                bool matchesCart = currentCart.CartItems.Any(cartItem => rawItemDiscounts.Any(d => d.ItemID == cartItem.ItemID));
                if (!matchesCart)
                {
                    MessageBox.Show("This discount does not match any items in your cart.");
                    return;
                }
            }

            currentCart.DiscountCode = code;

            List<clsItemDiscount> convertedItemDiscounts = rawItemDiscounts.Select(d => new clsItemDiscount
            {
                ItemID = d.ItemID,
                DiscountID = d.DiscountID,
                DiscountPercentage = d.DiscountPercentage,
                DiscountDollarAmount = d.DiscountDollarAmount
            }).ToList();

            currentCart.ItemDiscounts = convertedItemDiscounts;
            currentCart.Recalculate();

            lblSubtotal.Text = currentCart.DiscountedSubtotal.ToString("C");
            lblDiscount.Text = "-" + currentCart.DiscountAmount.ToString("C");
            lblTax.Text = currentCart.TaxAmount.ToString("C");
            lblTotalDue.Text = currentCart.TotalDue.ToString("C");

            MessageBox.Show($"Discount applied! You saved {currentCart.DiscountAmount:C}.");
            tbxDiscount.Enabled = false;
        }
    }
}