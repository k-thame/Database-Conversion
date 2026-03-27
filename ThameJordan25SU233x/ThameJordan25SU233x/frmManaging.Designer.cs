namespace ThameJordan25SU233x
{
    partial class frmManaging
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManaging));
            this.lblRestock = new System.Windows.Forms.Label();
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.nudRestock = new System.Windows.Forms.NumericUpDown();
            this.btnRestock = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.lblAdd = new System.Windows.Forms.Label();
            this.nudRemoveFromStock = new System.Windows.Forms.NumericUpDown();
            this.btnDecreaseQty = new System.Windows.Forms.Button();
            this.lblRemoveFromStock = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblRemoveItem = new System.Windows.Forms.Label();
            this.lblModifyUser = new System.Windows.Forms.Label();
            this.btnModifyUser = new System.Windows.Forms.Button();
            this.btnDiscounts = new System.Windows.Forms.Button();
            this.lblDiscounts = new System.Windows.Forms.Label();
            this.btnSalesReports = new System.Windows.Forms.Button();
            this.lblSalesReports = new System.Windows.Forms.Label();
            this.lblInventoryTable = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.btnShopForTheCustomer = new System.Windows.Forms.Button();
            this.btnMyProfile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRestock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemoveFromStock)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRestock
            // 
            this.lblRestock.AutoSize = true;
            this.lblRestock.Location = new System.Drawing.Point(1152, 48);
            this.lblRestock.Name = "lblRestock";
            this.lblRestock.Size = new System.Drawing.Size(115, 13);
            this.lblRestock.TabIndex = 0;
            this.lblRestock.Text = "Restock Selected Item";
            // 
            // dgvInventory
            // 
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Location = new System.Drawing.Point(12, 48);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.Size = new System.Drawing.Size(1111, 657);
            this.dgvInventory.TabIndex = 2;
            // 
            // nudRestock
            // 
            this.nudRestock.Location = new System.Drawing.Point(1137, 64);
            this.nudRestock.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudRestock.Name = "nudRestock";
            this.nudRestock.Size = new System.Drawing.Size(62, 20);
            this.nudRestock.TabIndex = 3;
            // 
            // btnRestock
            // 
            this.btnRestock.Location = new System.Drawing.Point(1205, 64);
            this.btnRestock.Name = "btnRestock";
            this.btnRestock.Size = new System.Drawing.Size(75, 20);
            this.btnRestock.TabIndex = 1;
            this.btnRestock.Text = "Re-stock";
            this.btnRestock.UseVisualStyleBackColor = true;
            this.btnRestock.Click += new System.EventHandler(this.btnRestock_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1226, 682);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(1145, 682);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 5;
            this.btnHelp.Text = "Help?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Location = new System.Drawing.Point(1157, 177);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(104, 23);
            this.btnAddItem.TabIndex = 6;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // lblAdd
            // 
            this.lblAdd.AutoSize = true;
            this.lblAdd.Location = new System.Drawing.Point(1168, 161);
            this.lblAdd.Name = "lblAdd";
            this.lblAdd.Size = new System.Drawing.Size(77, 13);
            this.lblAdd.TabIndex = 7;
            this.lblAdd.Text = "Add New Item!";
            // 
            // nudRemoveFromStock
            // 
            this.nudRemoveFromStock.Location = new System.Drawing.Point(1137, 114);
            this.nudRemoveFromStock.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudRemoveFromStock.Name = "nudRemoveFromStock";
            this.nudRemoveFromStock.Size = new System.Drawing.Size(62, 20);
            this.nudRemoveFromStock.TabIndex = 10;
            // 
            // btnDecreaseQty
            // 
            this.btnDecreaseQty.Location = new System.Drawing.Point(1205, 114);
            this.btnDecreaseQty.Name = "btnDecreaseQty";
            this.btnDecreaseQty.Size = new System.Drawing.Size(75, 20);
            this.btnDecreaseQty.TabIndex = 9;
            this.btnDecreaseQty.Text = "Remove";
            this.btnDecreaseQty.UseVisualStyleBackColor = true;
            this.btnDecreaseQty.Click += new System.EventHandler(this.btnDecreaseQty_Click);
            // 
            // lblRemoveFromStock
            // 
            this.lblRemoveFromStock.AutoSize = true;
            this.lblRemoveFromStock.Location = new System.Drawing.Point(1131, 98);
            this.lblRemoveFromStock.Name = "lblRemoveFromStock";
            this.lblRemoveFromStock.Size = new System.Drawing.Size(154, 13);
            this.lblRemoveFromStock.TabIndex = 8;
            this.lblRemoveFromStock.Text = "Remove stock of selected Item";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(1158, 221);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(103, 23);
            this.btnRemove.TabIndex = 11;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lblRemoveItem
            // 
            this.lblRemoveItem.AutoSize = true;
            this.lblRemoveItem.Location = new System.Drawing.Point(1160, 205);
            this.lblRemoveItem.Name = "lblRemoveItem";
            this.lblRemoveItem.Size = new System.Drawing.Size(101, 13);
            this.lblRemoveItem.TabIndex = 12;
            this.lblRemoveItem.Text = "Discontinue an Item";
            // 
            // lblModifyUser
            // 
            this.lblModifyUser.AutoSize = true;
            this.lblModifyUser.Location = new System.Drawing.Point(1159, 301);
            this.lblModifyUser.Name = "lblModifyUser";
            this.lblModifyUser.Size = new System.Drawing.Size(94, 13);
            this.lblModifyUser.TabIndex = 13;
            this.lblModifyUser.Text = "User Management";
            // 
            // btnModifyUser
            // 
            this.btnModifyUser.Location = new System.Drawing.Point(1157, 317);
            this.btnModifyUser.Name = "btnModifyUser";
            this.btnModifyUser.Size = new System.Drawing.Size(104, 23);
            this.btnModifyUser.TabIndex = 14;
            this.btnModifyUser.Text = "Edit";
            this.btnModifyUser.UseVisualStyleBackColor = true;
            this.btnModifyUser.Click += new System.EventHandler(this.btnModifyUser_Click);
            // 
            // btnDiscounts
            // 
            this.btnDiscounts.Location = new System.Drawing.Point(1157, 268);
            this.btnDiscounts.Name = "btnDiscounts";
            this.btnDiscounts.Size = new System.Drawing.Size(103, 23);
            this.btnDiscounts.TabIndex = 15;
            this.btnDiscounts.Text = "Discounts";
            this.btnDiscounts.UseVisualStyleBackColor = true;
            this.btnDiscounts.Click += new System.EventHandler(this.btnDiscounts_Click);
            // 
            // lblDiscounts
            // 
            this.lblDiscounts.AutoSize = true;
            this.lblDiscounts.Location = new System.Drawing.Point(1152, 252);
            this.lblDiscounts.Name = "lblDiscounts";
            this.lblDiscounts.Size = new System.Drawing.Size(114, 13);
            this.lblDiscounts.TabIndex = 16;
            this.lblDiscounts.Text = "Discount Management";
            // 
            // btnSalesReports
            // 
            this.btnSalesReports.Location = new System.Drawing.Point(1157, 369);
            this.btnSalesReports.Name = "btnSalesReports";
            this.btnSalesReports.Size = new System.Drawing.Size(104, 23);
            this.btnSalesReports.TabIndex = 17;
            this.btnSalesReports.Text = "Sales Reports";
            this.btnSalesReports.UseVisualStyleBackColor = true;
            this.btnSalesReports.Click += new System.EventHandler(this.btnSalesReports_Click);
            // 
            // lblSalesReports
            // 
            this.lblSalesReports.AutoSize = true;
            this.lblSalesReports.Location = new System.Drawing.Point(1159, 353);
            this.lblSalesReports.Name = "lblSalesReports";
            this.lblSalesReports.Size = new System.Drawing.Size(99, 13);
            this.lblSalesReports.TabIndex = 18;
            this.lblSalesReports.Text = "View Sales Reports";
            // 
            // lblInventoryTable
            // 
            this.lblInventoryTable.AutoSize = true;
            this.lblInventoryTable.Location = new System.Drawing.Point(12, 23);
            this.lblInventoryTable.Name = "lblInventoryTable";
            this.lblInventoryTable.Size = new System.Drawing.Size(81, 13);
            this.lblInventoryTable.TabIndex = 19;
            this.lblInventoryTable.Text = "Inventory Table";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1157, 489);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Select Customer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.Location = new System.Drawing.Point(1158, 547);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(103, 23);
            this.btnViewHistory.TabIndex = 21;
            this.btnViewHistory.Text = "View Sales History";
            this.btnViewHistory.UseVisualStyleBackColor = true;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);
            // 
            // btnShopForTheCustomer
            // 
            this.btnShopForTheCustomer.Location = new System.Drawing.Point(1158, 518);
            this.btnShopForTheCustomer.Name = "btnShopForTheCustomer";
            this.btnShopForTheCustomer.Size = new System.Drawing.Size(103, 23);
            this.btnShopForTheCustomer.TabIndex = 22;
            this.btnShopForTheCustomer.Text = "Start Shopping!";
            this.btnShopForTheCustomer.UseVisualStyleBackColor = true;
            this.btnShopForTheCustomer.Click += new System.EventHandler(this.btnShopForTheCustomer_Click);
            // 
            // btnMyProfile
            // 
            this.btnMyProfile.Location = new System.Drawing.Point(1158, 594);
            this.btnMyProfile.Name = "btnMyProfile";
            this.btnMyProfile.Size = new System.Drawing.Size(102, 23);
            this.btnMyProfile.TabIndex = 23;
            this.btnMyProfile.Text = "My Profile";
            this.btnMyProfile.UseVisualStyleBackColor = true;
            this.btnMyProfile.Click += new System.EventHandler(this.btnMyProfile_Click);
            // 
            // frmManaging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 717);
            this.Controls.Add(this.btnMyProfile);
            this.Controls.Add(this.btnShopForTheCustomer);
            this.Controls.Add(this.btnViewHistory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblInventoryTable);
            this.Controls.Add(this.lblSalesReports);
            this.Controls.Add(this.btnSalesReports);
            this.Controls.Add(this.lblDiscounts);
            this.Controls.Add(this.btnDiscounts);
            this.Controls.Add(this.btnModifyUser);
            this.Controls.Add(this.lblModifyUser);
            this.Controls.Add(this.lblRemoveItem);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.nudRemoveFromStock);
            this.Controls.Add(this.btnDecreaseQty);
            this.Controls.Add(this.lblRemoveFromStock);
            this.Controls.Add(this.lblAdd);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.nudRestock);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.btnRestock);
            this.Controls.Add(this.lblRestock);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmManaging";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General Management";
            this.Load += new System.EventHandler(this.frmManaging_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRestock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemoveFromStock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRestock;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.NumericUpDown nudRestock;
        private System.Windows.Forms.Button btnRestock;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Label lblAdd;
        private System.Windows.Forms.NumericUpDown nudRemoveFromStock;
        private System.Windows.Forms.Button btnDecreaseQty;
        private System.Windows.Forms.Label lblRemoveFromStock;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblRemoveItem;
        private System.Windows.Forms.Label lblModifyUser;
        private System.Windows.Forms.Button btnModifyUser;
        private System.Windows.Forms.Button btnDiscounts;
        private System.Windows.Forms.Label lblDiscounts;
        private System.Windows.Forms.Button btnSalesReports;
        private System.Windows.Forms.Label lblSalesReports;
        private System.Windows.Forms.Label lblInventoryTable;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.Button btnShopForTheCustomer;
        private System.Windows.Forms.Button btnMyProfile;
    }
}