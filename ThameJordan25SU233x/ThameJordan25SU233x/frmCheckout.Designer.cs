namespace ThameJordan25SU233x
{
    partial class frmCheckout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCheckout));
            this.tbxCardNumber = new System.Windows.Forms.TextBox();
            this.lblCreditNumber = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.tbxFirstName = new System.Windows.Forms.TextBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.tbxLastName = new System.Windows.Forms.TextBox();
            this.tbxExpiration = new System.Windows.Forms.TextBox();
            this.tbxCVV = new System.Windows.Forms.TextBox();
            this.lblExpiration = new System.Windows.Forms.Label();
            this.lblCVV = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnReturn = new System.Windows.Forms.Button();
            this.btnPurchase = new System.Windows.Forms.Button();
            this.cbxPaymentTypes = new System.Windows.Forms.ComboBox();
            this.lblPaymentType = new System.Windows.Forms.Label();
            this.tbxDiscount = new System.Windows.Forms.TextBox();
            this.lbxCartSummary = new System.Windows.Forms.ListBox();
            this.btnApplyDiscount = new System.Windows.Forms.Button();
            this.lblSubtotalLabel = new System.Windows.Forms.Label();
            this.lblTaxLabel = new System.Windows.Forms.Label();
            this.lblTotalDueLabel = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblTax = new System.Windows.Forms.Label();
            this.lblTotalDue = new System.Windows.Forms.Label();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.lblDiscountLabel = new System.Windows.Forms.Label();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.dgvActiveDiscounts = new System.Windows.Forms.DataGridView();
            this.lblActiveDiscounts = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActiveDiscounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxCardNumber
            // 
            this.tbxCardNumber.Location = new System.Drawing.Point(370, 324);
            this.tbxCardNumber.Name = "tbxCardNumber";
            this.tbxCardNumber.Size = new System.Drawing.Size(181, 20);
            this.tbxCardNumber.TabIndex = 0;
            // 
            // lblCreditNumber
            // 
            this.lblCreditNumber.AutoSize = true;
            this.lblCreditNumber.Location = new System.Drawing.Point(367, 308);
            this.lblCreditNumber.Name = "lblCreditNumber";
            this.lblCreditNumber.Size = new System.Drawing.Size(99, 13);
            this.lblCreditNumber.TabIndex = 1;
            this.lblCreditNumber.Text = "Credit Card Number";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(475, 257);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(97, 13);
            this.lblFirstName.TabIndex = 3;
            this.lblFirstName.Text = "First Name on Card";
            // 
            // tbxFirstName
            // 
            this.tbxFirstName.Location = new System.Drawing.Point(478, 273);
            this.tbxFirstName.Name = "tbxFirstName";
            this.tbxFirstName.Size = new System.Drawing.Size(140, 20);
            this.tbxFirstName.TabIndex = 2;
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(621, 257);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(98, 13);
            this.lblLastName.TabIndex = 5;
            this.lblLastName.Text = "Last Name on Card";
            // 
            // tbxLastName
            // 
            this.tbxLastName.Location = new System.Drawing.Point(624, 273);
            this.tbxLastName.Name = "tbxLastName";
            this.tbxLastName.Size = new System.Drawing.Size(140, 20);
            this.tbxLastName.TabIndex = 4;
            // 
            // tbxExpiration
            // 
            this.tbxExpiration.Location = new System.Drawing.Point(560, 324);
            this.tbxExpiration.Name = "tbxExpiration";
            this.tbxExpiration.Size = new System.Drawing.Size(122, 20);
            this.tbxExpiration.TabIndex = 6;
            this.tbxExpiration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxExpiration_KeyPress);
            // 
            // tbxCVV
            // 
            this.tbxCVV.Location = new System.Drawing.Point(695, 324);
            this.tbxCVV.Name = "tbxCVV";
            this.tbxCVV.Size = new System.Drawing.Size(48, 20);
            this.tbxCVV.TabIndex = 7;
            this.tbxCVV.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxCVV_KeyPress);
            // 
            // lblExpiration
            // 
            this.lblExpiration.AutoSize = true;
            this.lblExpiration.Location = new System.Drawing.Point(557, 308);
            this.lblExpiration.Name = "lblExpiration";
            this.lblExpiration.Size = new System.Drawing.Size(125, 13);
            this.lblExpiration.TabIndex = 8;
            this.lblExpiration.Text = "Expiration Date (MM/YY)";
            // 
            // lblCVV
            // 
            this.lblCVV.AutoSize = true;
            this.lblCVV.Location = new System.Drawing.Point(692, 308);
            this.lblCVV.Name = "lblCVV";
            this.lblCVV.Size = new System.Drawing.Size(28, 13);
            this.lblCVV.TabIndex = 9;
            this.lblCVV.Text = "CVV";
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(289, 415);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "Help?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnReturn
            // 
            this.btnReturn.Location = new System.Drawing.Point(382, 415);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(124, 23);
            this.btnReturn.TabIndex = 13;
            this.btnReturn.Text = "Continue Shopping";
            this.btnReturn.UseVisualStyleBackColor = true;
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // btnPurchase
            // 
            this.btnPurchase.Location = new System.Drawing.Point(625, 364);
            this.btnPurchase.Name = "btnPurchase";
            this.btnPurchase.Size = new System.Drawing.Size(118, 23);
            this.btnPurchase.TabIndex = 14;
            this.btnPurchase.Text = "PLACE ORDER";
            this.btnPurchase.UseVisualStyleBackColor = true;
            this.btnPurchase.Click += new System.EventHandler(this.btnPurchase_Click);
            // 
            // cbxPaymentTypes
            // 
            this.cbxPaymentTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPaymentTypes.FormattingEnabled = true;
            this.cbxPaymentTypes.Items.AddRange(new object[] {
            "Mastercard",
            "Visa",
            "American Express",
            "Discover"});
            this.cbxPaymentTypes.Location = new System.Drawing.Point(370, 272);
            this.cbxPaymentTypes.Name = "cbxPaymentTypes";
            this.cbxPaymentTypes.Size = new System.Drawing.Size(102, 21);
            this.cbxPaymentTypes.TabIndex = 15;
            // 
            // lblPaymentType
            // 
            this.lblPaymentType.AutoSize = true;
            this.lblPaymentType.Location = new System.Drawing.Point(367, 256);
            this.lblPaymentType.Name = "lblPaymentType";
            this.lblPaymentType.Size = new System.Drawing.Size(75, 13);
            this.lblPaymentType.TabIndex = 16;
            this.lblPaymentType.Text = "Payment Type";
            // 
            // tbxDiscount
            // 
            this.tbxDiscount.Location = new System.Drawing.Point(441, 365);
            this.tbxDiscount.Name = "tbxDiscount";
            this.tbxDiscount.Size = new System.Drawing.Size(73, 20);
            this.tbxDiscount.TabIndex = 17;
            // 
            // lbxCartSummary
            // 
            this.lbxCartSummary.FormattingEnabled = true;
            this.lbxCartSummary.Location = new System.Drawing.Point(12, 12);
            this.lbxCartSummary.Name = "lbxCartSummary";
            this.lbxCartSummary.Size = new System.Drawing.Size(321, 238);
            this.lbxCartSummary.TabIndex = 19;
            // 
            // btnApplyDiscount
            // 
            this.btnApplyDiscount.Location = new System.Drawing.Point(520, 363);
            this.btnApplyDiscount.Name = "btnApplyDiscount";
            this.btnApplyDiscount.Size = new System.Drawing.Size(69, 25);
            this.btnApplyDiscount.TabIndex = 20;
            this.btnApplyDiscount.Text = "Apply";
            this.btnApplyDiscount.UseVisualStyleBackColor = true;
            this.btnApplyDiscount.Click += new System.EventHandler(this.btnApplyDiscount_Click);
            // 
            // lblSubtotalLabel
            // 
            this.lblSubtotalLabel.AutoSize = true;
            this.lblSubtotalLabel.Location = new System.Drawing.Point(12, 256);
            this.lblSubtotalLabel.Name = "lblSubtotalLabel";
            this.lblSubtotalLabel.Size = new System.Drawing.Size(49, 13);
            this.lblSubtotalLabel.TabIndex = 21;
            this.lblSubtotalLabel.Text = "Subtotal:";
            // 
            // lblTaxLabel
            // 
            this.lblTaxLabel.AutoSize = true;
            this.lblTaxLabel.Location = new System.Drawing.Point(33, 280);
            this.lblTaxLabel.Name = "lblTaxLabel";
            this.lblTaxLabel.Size = new System.Drawing.Size(28, 13);
            this.lblTaxLabel.TabIndex = 22;
            this.lblTaxLabel.Text = "Tax:";
            // 
            // lblTotalDueLabel
            // 
            this.lblTotalDueLabel.AutoSize = true;
            this.lblTotalDueLabel.Location = new System.Drawing.Point(4, 327);
            this.lblTotalDueLabel.Name = "lblTotalDueLabel";
            this.lblTotalDueLabel.Size = new System.Drawing.Size(57, 13);
            this.lblTotalDueLabel.TabIndex = 23;
            this.lblTotalDueLabel.Text = "Total Due:";
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Location = new System.Drawing.Point(67, 256);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(0, 13);
            this.lblSubtotal.TabIndex = 24;
            // 
            // lblTax
            // 
            this.lblTax.AutoSize = true;
            this.lblTax.Location = new System.Drawing.Point(67, 280);
            this.lblTax.Name = "lblTax";
            this.lblTax.Size = new System.Drawing.Size(0, 13);
            this.lblTax.TabIndex = 25;
            // 
            // lblTotalDue
            // 
            this.lblTotalDue.AutoSize = true;
            this.lblTotalDue.Location = new System.Drawing.Point(67, 327);
            this.lblTotalDue.Name = "lblTotalDue";
            this.lblTotalDue.Size = new System.Drawing.Size(0, 13);
            this.lblTotalDue.TabIndex = 26;
            // 
            // lblDiscount
            // 
            this.lblDiscount.AutoSize = true;
            this.lblDiscount.Location = new System.Drawing.Point(67, 304);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(0, 13);
            this.lblDiscount.TabIndex = 28;
            // 
            // lblDiscountLabel
            // 
            this.lblDiscountLabel.AutoSize = true;
            this.lblDiscountLabel.Location = new System.Drawing.Point(9, 304);
            this.lblDiscountLabel.Name = "lblDiscountLabel";
            this.lblDiscountLabel.Size = new System.Drawing.Size(52, 13);
            this.lblDiscountLabel.TabIndex = 27;
            this.lblDiscountLabel.Text = "Discount:";
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(365, 369);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(70, 13);
            this.lblPrompt.TabIndex = 29;
            this.lblPrompt.Text = "Promo code?";
            // 
            // dgvActiveDiscounts
            // 
            this.dgvActiveDiscounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActiveDiscounts.Location = new System.Drawing.Point(368, 29);
            this.dgvActiveDiscounts.Name = "dgvActiveDiscounts";
            this.dgvActiveDiscounts.Size = new System.Drawing.Size(396, 220);
            this.dgvActiveDiscounts.TabIndex = 30;
            // 
            // lblActiveDiscounts
            // 
            this.lblActiveDiscounts.AutoSize = true;
            this.lblActiveDiscounts.Location = new System.Drawing.Point(367, 12);
            this.lblActiveDiscounts.Name = "lblActiveDiscounts";
            this.lblActiveDiscounts.Size = new System.Drawing.Size(87, 13);
            this.lblActiveDiscounts.TabIndex = 31;
            this.lblActiveDiscounts.Text = "Active Discounts";
            // 
            // frmCheckout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblActiveDiscounts);
            this.Controls.Add(this.dgvActiveDiscounts);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.lblDiscount);
            this.Controls.Add(this.lblDiscountLabel);
            this.Controls.Add(this.lblTotalDue);
            this.Controls.Add(this.lblTax);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblTotalDueLabel);
            this.Controls.Add(this.lblTaxLabel);
            this.Controls.Add(this.lblSubtotalLabel);
            this.Controls.Add(this.btnApplyDiscount);
            this.Controls.Add(this.lbxCartSummary);
            this.Controls.Add(this.tbxDiscount);
            this.Controls.Add(this.lblPaymentType);
            this.Controls.Add(this.cbxPaymentTypes);
            this.Controls.Add(this.btnPurchase);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblCVV);
            this.Controls.Add(this.lblExpiration);
            this.Controls.Add(this.tbxCVV);
            this.Controls.Add(this.tbxExpiration);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.tbxLastName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.tbxFirstName);
            this.Controls.Add(this.lblCreditNumber);
            this.Controls.Add(this.tbxCardNumber);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCheckout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Checkout";
            this.Load += new System.EventHandler(this.frmCheckout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvActiveDiscounts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxCardNumber;
        private System.Windows.Forms.Label lblCreditNumber;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.TextBox tbxFirstName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.TextBox tbxLastName;
        private System.Windows.Forms.TextBox tbxExpiration;
        private System.Windows.Forms.TextBox tbxCVV;
        private System.Windows.Forms.Label lblExpiration;
        private System.Windows.Forms.Label lblCVV;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Button btnPurchase;
        private System.Windows.Forms.ComboBox cbxPaymentTypes;
        private System.Windows.Forms.Label lblPaymentType;
        private System.Windows.Forms.TextBox tbxDiscount;
        private System.Windows.Forms.ListBox lbxCartSummary;
        private System.Windows.Forms.Button btnApplyDiscount;
        private System.Windows.Forms.Label lblSubtotalLabel;
        private System.Windows.Forms.Label lblTaxLabel;
        private System.Windows.Forms.Label lblTotalDueLabel;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblTax;
        private System.Windows.Forms.Label lblTotalDue;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.Label lblDiscountLabel;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.DataGridView dgvActiveDiscounts;
        private System.Windows.Forms.Label lblActiveDiscounts;
    }
}