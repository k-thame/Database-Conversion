namespace ThameJordan25SU233x
{
    partial class frmAddItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddItem));
            this.tbxItemName = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.tbxDescription = new System.Windows.Forms.TextBox();
            this.lblItemDescription = new System.Windows.Forms.Label();
            this.nudPrice = new System.Windows.Forms.NumericUpDown();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.nudQuantity = new System.Windows.Forms.NumericUpDown();
            this.cbxItemCategory = new System.Windows.Forms.ComboBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblItemCategory = new System.Windows.Forms.Label();
            this.btnItemImage = new System.Windows.Forms.Button();
            this.txtImagePath = new System.Windows.Forms.TextBox();
            this.pbxImagePreview = new System.Windows.Forms.PictureBox();
            this.lblImagePreviewPrompt = new System.Windows.Forms.Label();
            this.tbxAddItem = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.lblCost = new System.Windows.Forms.Label();
            this.nudCost = new System.Windows.Forms.NumericUpDown();
            this.lblRestockThreshold = new System.Windows.Forms.Label();
            this.nudRestockThreshold = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxImagePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRestockThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxItemName
            // 
            this.tbxItemName.Location = new System.Drawing.Point(785, 75);
            this.tbxItemName.Name = "tbxItemName";
            this.tbxItemName.Size = new System.Drawing.Size(181, 20);
            this.tbxItemName.TabIndex = 1;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(721, 78);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(58, 13);
            this.lblItemName.TabIndex = 2;
            this.lblItemName.Text = "Item Name";
            // 
            // tbxDescription
            // 
            this.tbxDescription.Location = new System.Drawing.Point(785, 113);
            this.tbxDescription.Multiline = true;
            this.tbxDescription.Name = "tbxDescription";
            this.tbxDescription.Size = new System.Drawing.Size(181, 148);
            this.tbxDescription.TabIndex = 3;
            // 
            // lblItemDescription
            // 
            this.lblItemDescription.AutoSize = true;
            this.lblItemDescription.Location = new System.Drawing.Point(696, 116);
            this.lblItemDescription.Name = "lblItemDescription";
            this.lblItemDescription.Size = new System.Drawing.Size(83, 13);
            this.lblItemDescription.TabIndex = 4;
            this.lblItemDescription.Text = "Item Description";
            // 
            // nudPrice
            // 
            this.nudPrice.DecimalPlaces = 2;
            this.nudPrice.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudPrice.Location = new System.Drawing.Point(785, 304);
            this.nudPrice.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(121, 20);
            this.nudPrice.TabIndex = 5;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(733, 343);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(46, 13);
            this.lblQuantity.TabIndex = 8;
            this.lblQuantity.Text = "Quantity";
            // 
            // nudQuantity
            // 
            this.nudQuantity.Location = new System.Drawing.Point(785, 341);
            this.nudQuantity.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudQuantity.Name = "nudQuantity";
            this.nudQuantity.Size = new System.Drawing.Size(121, 20);
            this.nudQuantity.TabIndex = 9;
            // 
            // cbxItemCategory
            // 
            this.cbxItemCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxItemCategory.FormattingEnabled = true;
            this.cbxItemCategory.Location = new System.Drawing.Point(785, 267);
            this.cbxItemCategory.Name = "cbxItemCategory";
            this.cbxItemCategory.Size = new System.Drawing.Size(181, 21);
            this.cbxItemCategory.TabIndex = 10;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(718, 306);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(61, 13);
            this.lblPrice.TabIndex = 7;
            this.lblPrice.Text = "Retail Price";
            // 
            // lblItemCategory
            // 
            this.lblItemCategory.AutoSize = true;
            this.lblItemCategory.Location = new System.Drawing.Point(707, 270);
            this.lblItemCategory.Name = "lblItemCategory";
            this.lblItemCategory.Size = new System.Drawing.Size(72, 13);
            this.lblItemCategory.TabIndex = 11;
            this.lblItemCategory.Text = "Item Category";
            // 
            // btnItemImage
            // 
            this.btnItemImage.Location = new System.Drawing.Point(12, 12);
            this.btnItemImage.Name = "btnItemImage";
            this.btnItemImage.Size = new System.Drawing.Size(98, 20);
            this.btnItemImage.TabIndex = 12;
            this.btnItemImage.Text = "Select Image";
            this.btnItemImage.UseVisualStyleBackColor = true;
            this.btnItemImage.Click += new System.EventHandler(this.btnItemImage_Click);
            // 
            // txtImagePath
            // 
            this.txtImagePath.Enabled = false;
            this.txtImagePath.Location = new System.Drawing.Point(116, 12);
            this.txtImagePath.Name = "txtImagePath";
            this.txtImagePath.Size = new System.Drawing.Size(553, 20);
            this.txtImagePath.TabIndex = 13;
            // 
            // pbxImagePreview
            // 
            this.pbxImagePreview.Location = new System.Drawing.Point(12, 69);
            this.pbxImagePreview.Name = "pbxImagePreview";
            this.pbxImagePreview.Size = new System.Drawing.Size(657, 362);
            this.pbxImagePreview.TabIndex = 14;
            this.pbxImagePreview.TabStop = false;
            // 
            // lblImagePreviewPrompt
            // 
            this.lblImagePreviewPrompt.AutoSize = true;
            this.lblImagePreviewPrompt.Location = new System.Drawing.Point(12, 44);
            this.lblImagePreviewPrompt.Name = "lblImagePreviewPrompt";
            this.lblImagePreviewPrompt.Size = new System.Drawing.Size(77, 13);
            this.lblImagePreviewPrompt.TabIndex = 15;
            this.lblImagePreviewPrompt.Text = "Image Preview";
            // 
            // tbxAddItem
            // 
            this.tbxAddItem.Location = new System.Drawing.Point(419, 468);
            this.tbxAddItem.Name = "tbxAddItem";
            this.tbxAddItem.Size = new System.Drawing.Size(141, 21);
            this.tbxAddItem.TabIndex = 18;
            this.tbxAddItem.Text = "Add Item";
            this.tbxAddItem.UseVisualStyleBackColor = true;
            this.tbxAddItem.Click += new System.EventHandler(this.tbxAddItem_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(500, 560);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(407, 560);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 20;
            this.btnHelp.Text = "Help?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.Location = new System.Drawing.Point(751, 378);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(28, 13);
            this.lblCost.TabIndex = 22;
            this.lblCost.Text = "Cost";
            // 
            // nudCost
            // 
            this.nudCost.DecimalPlaces = 2;
            this.nudCost.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudCost.Location = new System.Drawing.Point(785, 376);
            this.nudCost.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.nudCost.Name = "nudCost";
            this.nudCost.Size = new System.Drawing.Size(121, 20);
            this.nudCost.TabIndex = 21;
            // 
            // lblRestockThreshold
            // 
            this.lblRestockThreshold.AutoSize = true;
            this.lblRestockThreshold.Location = new System.Drawing.Point(682, 413);
            this.lblRestockThreshold.Name = "lblRestockThreshold";
            this.lblRestockThreshold.Size = new System.Drawing.Size(97, 13);
            this.lblRestockThreshold.TabIndex = 24;
            this.lblRestockThreshold.Text = "Restock Threshold";
            // 
            // nudRestockThreshold
            // 
            this.nudRestockThreshold.Location = new System.Drawing.Point(785, 411);
            this.nudRestockThreshold.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.nudRestockThreshold.Name = "nudRestockThreshold";
            this.nudRestockThreshold.Size = new System.Drawing.Size(121, 20);
            this.nudRestockThreshold.TabIndex = 23;
            // 
            // frmAddItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 717);
            this.Controls.Add(this.lblRestockThreshold);
            this.Controls.Add(this.nudRestockThreshold);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.nudCost);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tbxAddItem);
            this.Controls.Add(this.lblImagePreviewPrompt);
            this.Controls.Add(this.pbxImagePreview);
            this.Controls.Add(this.txtImagePath);
            this.Controls.Add(this.btnItemImage);
            this.Controls.Add(this.lblItemCategory);
            this.Controls.Add(this.cbxItemCategory);
            this.Controls.Add(this.nudQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.nudPrice);
            this.Controls.Add(this.lblItemDescription);
            this.Controls.Add(this.tbxDescription);
            this.Controls.Add(this.lblItemName);
            this.Controls.Add(this.tbxItemName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAddItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventory Item Addition";
            this.Load += new System.EventHandler(this.frmAddItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxImagePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRestockThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbxItemName;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.TextBox tbxDescription;
        private System.Windows.Forms.Label lblItemDescription;
        private System.Windows.Forms.NumericUpDown nudPrice;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown nudQuantity;
        private System.Windows.Forms.ComboBox cbxItemCategory;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblItemCategory;
        private System.Windows.Forms.Button btnItemImage;
        private System.Windows.Forms.TextBox txtImagePath;
        private System.Windows.Forms.PictureBox pbxImagePreview;
        private System.Windows.Forms.Label lblImagePreviewPrompt;
        private System.Windows.Forms.Button tbxAddItem;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.NumericUpDown nudCost;
        private System.Windows.Forms.Label lblRestockThreshold;
        private System.Windows.Forms.NumericUpDown nudRestockThreshold;
    }
}