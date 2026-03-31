using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmAddItem : Form
    {
        //
        private readonly clsSQL db = new clsSQL();

        // Constructor
        public frmAddItem()
        {
            InitializeComponent();
            LoadCategories();
        }

        // Form Load
        private void frmAddItem_Load(object sender, EventArgs e)
        {

        }

        // Load Categories
        private void LoadCategories()
        {
            try
            {
                DataTable dt = db.GetAllCategories();

                cbxItemCategory.DataSource = dt;
                cbxItemCategory.DisplayMember = "CategoryName";
                cbxItemCategory.ValueMember = "CategoryID";
                cbxItemCategory.SelectedIndex = -1; // optional: no pre-selection
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Select an image for the new item
        private void btnItemImage_Click(object sender, EventArgs e)
        {
            try
            {
                //
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = fileDialog.FileName;
                    pbxImagePreview.Image = new Bitmap(fileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show(ex.Message, "Unable to select image! Please try again.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adding a new item to the Inventory table
        private void tbxAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                string itemName = (tbxItemName.Text ?? "").Trim();
                string itemDesc = (tbxDescription.Text ?? "").Trim();
                decimal retailPrice = nudPrice.Value;           
                decimal cost = nudCost.Value;            
                decimal quantity = nudQuantity.Value;        
                decimal restock = nudRestockThreshold.Value;
                bool discontinued = false;

                // Category
                int categoryID = -1;
                if (cbxItemCategory.SelectedValue != null && cbxItemCategory.SelectedValue != DBNull.Value)
                    categoryID = Convert.ToInt32(cbxItemCategory.SelectedValue);

                // Image bytes
                byte[] imageBytes = null;

                // 
                string msg;
                if (!clsValidation.IsValidItemName(itemName, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); tbxItemName.Focus(); return; }
                if (!clsValidation.IsValidItemDescription(itemDesc, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); tbxDescription.Focus(); return; }
                if (!clsValidation.IsValidCategory(cbxItemCategory, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); cbxItemCategory.DroppedDown = true; return; }

                // Prices
                if (!clsValidation.IsValidMoney(retailPrice, "Retail Price", allowZero: false, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); nudPrice.Focus(); return; }
                if (!clsValidation.IsValidMoney(cost, "Cost", allowZero: false, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); nudCost.Focus(); return; }
                if (cost > retailPrice)
                {
                    MessageBox.Show("Cost cannot be greater than the retail price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    nudCost.Focus(); return;
                }

                // Quantities
                if (!clsValidation.IsValidQuantity(quantity, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); nudQuantity.Focus(); return; }
                if (!clsValidation.IsValidRestockThreshold(restock, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); nudRestockThreshold.Focus(); return; }

                // Image 
                if (!clsValidation.IsValidImagePath(txtImagePath.Text, out msg)) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); btnItemImage.Focus(); return; }
                imageBytes = File.ReadAllBytes(txtImagePath.Text);

                //
                string confirm =
                    "Please confirm the new inventory item:\n\n" +
                    $"Item Name: {itemName}\n" +
                    $"Item Description: {itemDesc}\n" +
                    $"Category: {cbxItemCategory.Text}\n" +
                    $"Retail Price: {retailPrice:C2}\n" +
                    $"Cost: {cost:C2}\n" +
                    $"Quantity: {quantity}\n" +
                    $"Restock Threshold: {restock}\n" +
                    $"Image Path: {txtImagePath.Text}\n\n" +
                    "Add this item to inventory?";

                if (MessageBox.Show(confirm, "Confirm Addition", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return;

                // 
                bool success = clsSQL.InsertNewInventoryItem(
                    itemName,
                    itemDesc,
                    categoryID,
                    retailPrice,
                    cost,
                    Convert.ToInt32(quantity),
                    Convert.ToInt32(restock),
                    imageBytes,
                    discontinued
                );

                if (success)
                {
                    MessageBox.Show("Item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add item.", "Insert Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Help button (MODIFY)
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Need help adding a new inventory item?\n\n" +
                "Follow these steps:\n\n" +
                "1. **Select Image** – Click 'Select Image' to choose a product picture. The file path appears in the box and the preview shows on the left.\n" +
                "2. **Item Details** – Enter:\n" +
                "   • **Item Name** – Product title customers will see.\n" +
                "   • **Item Description** – Short summary/specs.\n" +
                "   • **Item Category** – Choose the correct category.\n" +
                "3. **Pricing & Stock** – Fill in:\n" +
                "   • **Retail Price** – Sales price.\n" +
                "   • **Quantity** – Starting stock on hand.\n" +
                "   • **Cost** – Your internal cost (not shown to customers).\n" +
                "   • **Restock Threshold** – Minimum level before restock alerts.\n" +
                "4. **Add Item** – Click 'Add Item' to save the product.\n\n" +
                "Other options:\n" +
                "• **Help** – You’re here now! 😊\n" +
                "• **Exit** – Close without saving.\n\n" +
                "Tips:\n" +
                "- Images should be clear and representative of the product.\n" +
                "- Ensure price and cost are correct; stock and thresholds can be updated later.\n" +
                "- All database saving happens securely through the application.",
                "Add Inventory Item – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Exit button (MODIFY)
        private void btnExit_Click(object sender, EventArgs e)
        {
            //
            DialogResult result = MessageBox.Show("If you did not finalize adding an item to the inventory, your entries will be erased.", "Cancel Addition?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}