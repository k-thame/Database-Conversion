namespace ThameJordan25SU233x
{
    partial class frmDiscounts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDiscounts));
            this.dgvDiscounts = new System.Windows.Forms.DataGridView();
            this.btnAddDiscount = new System.Windows.Forms.Button();
            this.btnRemoveDiscount = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblDiscountCode = new System.Windows.Forms.Label();
            this.tbxDiscountCode = new System.Windows.Forms.TextBox();
            this.lblDiscountDescription = new System.Windows.Forms.Label();
            this.tbxDescription = new System.Windows.Forms.TextBox();
            this.cbxDiscountLevel = new System.Windows.Forms.ComboBox();
            this.lblDiscountLevel = new System.Windows.Forms.Label();
            this.cbxDiscountType = new System.Windows.Forms.ComboBox();
            this.lblDiscountType = new System.Windows.Forms.Label();
            this.nudDiscountPercentage = new System.Windows.Forms.NumericUpDown();
            this.lblDiscountPercentage = new System.Windows.Forms.Label();
            this.lblDiscountDollarAmount = new System.Windows.Forms.Label();
            this.nudDiscountDollarAmount = new System.Windows.Forms.NumericUpDown();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpExpDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblExpDate = new System.Windows.Forms.Label();
            this.lblInventoryID = new System.Windows.Forms.Label();
            this.tbxInventoryID = new System.Windows.Forms.TextBox();
            this.lblRemoveDiscount = new System.Windows.Forms.Label();
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.lblItemReference = new System.Windows.Forms.Label();
            this.lblDiscountsTable = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiscounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiscountPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiscountDollarAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDiscounts
            // 
            this.dgvDiscounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDiscounts.Location = new System.Drawing.Point(136, 27);
            this.dgvDiscounts.Name = "dgvDiscounts";
            this.dgvDiscounts.Size = new System.Drawing.Size(1042, 305);
            this.dgvDiscounts.TabIndex = 0;
            // 
            // btnAddDiscount
            // 
            this.btnAddDiscount.Location = new System.Drawing.Point(592, 645);
            this.btnAddDiscount.Name = "btnAddDiscount";
            this.btnAddDiscount.Size = new System.Drawing.Size(88, 23);
            this.btnAddDiscount.TabIndex = 2;
            this.btnAddDiscount.Text = "Add Discount";
            this.btnAddDiscount.UseVisualStyleBackColor = true;
            this.btnAddDiscount.Click += new System.EventHandler(this.btnAddDiscount_Click);
            // 
            // btnRemoveDiscount
            // 
            this.btnRemoveDiscount.Location = new System.Drawing.Point(939, 517);
            this.btnRemoveDiscount.Name = "btnRemoveDiscount";
            this.btnRemoveDiscount.Size = new System.Drawing.Size(124, 23);
            this.btnRemoveDiscount.TabIndex = 3;
            this.btnRemoveDiscount.Text = "Remove";
            this.btnRemoveDiscount.UseVisualStyleBackColor = true;
            this.btnRemoveDiscount.Click += new System.EventHandler(this.btnRemoveDiscount_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(1119, 682);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(88, 23);
            this.btnHelp.TabIndex = 5;
            this.btnHelp.Text = "Help?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1213, 682);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(88, 23);
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblDiscountCode
            // 
            this.lblDiscountCode.AutoSize = true;
            this.lblDiscountCode.Location = new System.Drawing.Point(516, 335);
            this.lblDiscountCode.Name = "lblDiscountCode";
            this.lblDiscountCode.Size = new System.Drawing.Size(77, 13);
            this.lblDiscountCode.TabIndex = 7;
            this.lblDiscountCode.Text = "Discount Code";
            // 
            // tbxDiscountCode
            // 
            this.tbxDiscountCode.Location = new System.Drawing.Point(519, 351);
            this.tbxDiscountCode.Name = "tbxDiscountCode";
            this.tbxDiscountCode.Size = new System.Drawing.Size(100, 20);
            this.tbxDiscountCode.TabIndex = 8;
            // 
            // lblDiscountDescription
            // 
            this.lblDiscountDescription.AutoSize = true;
            this.lblDiscountDescription.Location = new System.Drawing.Point(516, 379);
            this.lblDiscountDescription.Name = "lblDiscountDescription";
            this.lblDiscountDescription.Size = new System.Drawing.Size(105, 13);
            this.lblDiscountDescription.TabIndex = 9;
            this.lblDiscountDescription.Text = "Discount Description";
            // 
            // tbxDescription
            // 
            this.tbxDescription.Location = new System.Drawing.Point(519, 395);
            this.tbxDescription.Multiline = true;
            this.tbxDescription.Name = "tbxDescription";
            this.tbxDescription.Size = new System.Drawing.Size(233, 49);
            this.tbxDescription.TabIndex = 10;
            // 
            // cbxDiscountLevel
            // 
            this.cbxDiscountLevel.FormattingEnabled = true;
            this.cbxDiscountLevel.Items.AddRange(new object[] {
            "0 = Item Level",
            "1 = Cart Level"});
            this.cbxDiscountLevel.Location = new System.Drawing.Point(519, 469);
            this.cbxDiscountLevel.Name = "cbxDiscountLevel";
            this.cbxDiscountLevel.Size = new System.Drawing.Size(74, 21);
            this.cbxDiscountLevel.TabIndex = 11;
            // 
            // lblDiscountLevel
            // 
            this.lblDiscountLevel.AutoSize = true;
            this.lblDiscountLevel.Location = new System.Drawing.Point(516, 453);
            this.lblDiscountLevel.Name = "lblDiscountLevel";
            this.lblDiscountLevel.Size = new System.Drawing.Size(78, 13);
            this.lblDiscountLevel.TabIndex = 12;
            this.lblDiscountLevel.Text = "Discount Level";
            // 
            // cbxDiscountType
            // 
            this.cbxDiscountType.FormattingEnabled = true;
            this.cbxDiscountType.Items.AddRange(new object[] {
            "1 = Cart Level Percentage",
            "2 = Cart Level Dollar Amount",
            "3 = Item Level Percentage",
            "4 = Item Level Dollar Amount"});
            this.cbxDiscountType.Location = new System.Drawing.Point(599, 469);
            this.cbxDiscountType.Name = "cbxDiscountType";
            this.cbxDiscountType.Size = new System.Drawing.Size(153, 21);
            this.cbxDiscountType.TabIndex = 13;
            // 
            // lblDiscountType
            // 
            this.lblDiscountType.AutoSize = true;
            this.lblDiscountType.Location = new System.Drawing.Point(600, 453);
            this.lblDiscountType.Name = "lblDiscountType";
            this.lblDiscountType.Size = new System.Drawing.Size(76, 13);
            this.lblDiscountType.TabIndex = 14;
            this.lblDiscountType.Text = "Discount Type";
            // 
            // nudDiscountPercentage
            // 
            this.nudDiscountPercentage.Location = new System.Drawing.Point(519, 517);
            this.nudDiscountPercentage.Name = "nudDiscountPercentage";
            this.nudDiscountPercentage.Size = new System.Drawing.Size(99, 20);
            this.nudDiscountPercentage.TabIndex = 15;
            // 
            // lblDiscountPercentage
            // 
            this.lblDiscountPercentage.AutoSize = true;
            this.lblDiscountPercentage.Location = new System.Drawing.Point(519, 501);
            this.lblDiscountPercentage.Name = "lblDiscountPercentage";
            this.lblDiscountPercentage.Size = new System.Drawing.Size(60, 13);
            this.lblDiscountPercentage.TabIndex = 16;
            this.lblDiscountPercentage.Text = "Discount %";
            // 
            // lblDiscountDollarAmount
            // 
            this.lblDiscountDollarAmount.AutoSize = true;
            this.lblDiscountDollarAmount.Location = new System.Drawing.Point(653, 501);
            this.lblDiscountDollarAmount.Name = "lblDiscountDollarAmount";
            this.lblDiscountDollarAmount.Size = new System.Drawing.Size(60, 13);
            this.lblDiscountDollarAmount.TabIndex = 17;
            this.lblDiscountDollarAmount.Text = "Amount Off";
            // 
            // nudDiscountDollarAmount
            // 
            this.nudDiscountDollarAmount.Location = new System.Drawing.Point(653, 517);
            this.nudDiscountDollarAmount.Name = "nudDiscountDollarAmount";
            this.nudDiscountDollarAmount.Size = new System.Drawing.Size(99, 20);
            this.nudDiscountDollarAmount.TabIndex = 18;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(519, 564);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(233, 20);
            this.dtpStartDate.TabIndex = 19;
            // 
            // dtpExpDate
            // 
            this.dtpExpDate.Location = new System.Drawing.Point(519, 608);
            this.dtpExpDate.Name = "dtpExpDate";
            this.dtpExpDate.Size = new System.Drawing.Size(233, 20);
            this.dtpExpDate.TabIndex = 20;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(518, 547);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(55, 13);
            this.lblStartDate.TabIndex = 21;
            this.lblStartDate.Text = "Start Date";
            // 
            // lblExpDate
            // 
            this.lblExpDate.AutoSize = true;
            this.lblExpDate.Location = new System.Drawing.Point(518, 592);
            this.lblExpDate.Name = "lblExpDate";
            this.lblExpDate.Size = new System.Drawing.Size(79, 13);
            this.lblExpDate.TabIndex = 22;
            this.lblExpDate.Text = "Expiration Date";
            // 
            // lblInventoryID
            // 
            this.lblInventoryID.AutoSize = true;
            this.lblInventoryID.Location = new System.Drawing.Point(649, 335);
            this.lblInventoryID.Name = "lblInventoryID";
            this.lblInventoryID.Size = new System.Drawing.Size(65, 13);
            this.lblInventoryID.TabIndex = 23;
            this.lblInventoryID.Text = "Inventory ID";
            // 
            // tbxInventoryID
            // 
            this.tbxInventoryID.Location = new System.Drawing.Point(652, 351);
            this.tbxInventoryID.Name = "tbxInventoryID";
            this.tbxInventoryID.Size = new System.Drawing.Size(100, 20);
            this.tbxInventoryID.TabIndex = 24;
            // 
            // lblRemoveDiscount
            // 
            this.lblRemoveDiscount.AutoSize = true;
            this.lblRemoveDiscount.Location = new System.Drawing.Point(951, 501);
            this.lblRemoveDiscount.Name = "lblRemoveDiscount";
            this.lblRemoveDiscount.Size = new System.Drawing.Size(98, 13);
            this.lblRemoveDiscount.TabIndex = 25;
            this.lblRemoveDiscount.Text = "Remove Discount?";
            // 
            // dgvInventory
            // 
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Location = new System.Drawing.Point(136, 371);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.Size = new System.Drawing.Size(315, 297);
            this.dgvInventory.TabIndex = 44;
            // 
            // lblItemReference
            // 
            this.lblItemReference.AutoSize = true;
            this.lblItemReference.Location = new System.Drawing.Point(133, 351);
            this.lblItemReference.Name = "lblItemReference";
            this.lblItemReference.Size = new System.Drawing.Size(80, 13);
            this.lblItemReference.TabIndex = 45;
            this.lblItemReference.Text = "Item Reference";
            // 
            // lblDiscountsTable
            // 
            this.lblDiscountsTable.AutoSize = true;
            this.lblDiscountsTable.Location = new System.Drawing.Point(133, 9);
            this.lblDiscountsTable.Name = "lblDiscountsTable";
            this.lblDiscountsTable.Size = new System.Drawing.Size(84, 13);
            this.lblDiscountsTable.TabIndex = 46;
            this.lblDiscountsTable.Text = "Discounts Table";
            // 
            // frmDiscounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 717);
            this.Controls.Add(this.lblDiscountsTable);
            this.Controls.Add(this.lblItemReference);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.lblRemoveDiscount);
            this.Controls.Add(this.tbxInventoryID);
            this.Controls.Add(this.lblInventoryID);
            this.Controls.Add(this.lblExpDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.dtpExpDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.nudDiscountDollarAmount);
            this.Controls.Add(this.lblDiscountDollarAmount);
            this.Controls.Add(this.lblDiscountPercentage);
            this.Controls.Add(this.nudDiscountPercentage);
            this.Controls.Add(this.lblDiscountType);
            this.Controls.Add(this.cbxDiscountType);
            this.Controls.Add(this.lblDiscountLevel);
            this.Controls.Add(this.cbxDiscountLevel);
            this.Controls.Add(this.tbxDescription);
            this.Controls.Add(this.lblDiscountDescription);
            this.Controls.Add(this.tbxDiscountCode);
            this.Controls.Add(this.lblDiscountCode);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnRemoveDiscount);
            this.Controls.Add(this.btnAddDiscount);
            this.Controls.Add(this.dgvDiscounts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDiscounts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Discount Management";
            this.Load += new System.EventHandler(this.frmDiscounts_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiscounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiscountPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiscountDollarAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDiscounts;
        private System.Windows.Forms.Button btnAddDiscount;
        private System.Windows.Forms.Button btnRemoveDiscount;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblDiscountCode;
        private System.Windows.Forms.TextBox tbxDiscountCode;
        private System.Windows.Forms.Label lblDiscountDescription;
        private System.Windows.Forms.TextBox tbxDescription;
        private System.Windows.Forms.ComboBox cbxDiscountLevel;
        private System.Windows.Forms.Label lblDiscountLevel;
        private System.Windows.Forms.ComboBox cbxDiscountType;
        private System.Windows.Forms.Label lblDiscountType;
        private System.Windows.Forms.NumericUpDown nudDiscountPercentage;
        private System.Windows.Forms.Label lblDiscountPercentage;
        private System.Windows.Forms.Label lblDiscountDollarAmount;
        private System.Windows.Forms.NumericUpDown nudDiscountDollarAmount;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpExpDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblExpDate;
        private System.Windows.Forms.Label lblInventoryID;
        private System.Windows.Forms.TextBox tbxInventoryID;
        private System.Windows.Forms.Label lblRemoveDiscount;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.Label lblItemReference;
        private System.Windows.Forms.Label lblDiscountsTable;
    }
}