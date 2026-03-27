namespace ThameJordan25SU233x
{
    partial class frmReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReports));
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnSalesDaily = new System.Windows.Forms.Button();
            this.btnSalesWeekly = new System.Windows.Forms.Button();
            this.btnSalesMonthly = new System.Windows.Forms.Button();
            this.btnSalesViewHTML = new System.Windows.Forms.Button();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.btnInvAvailable = new System.Windows.Forms.Button();
            this.btnInvNeedsRestock = new System.Windows.Forms.Button();
            this.btnInvAll = new System.Windows.Forms.Button();
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.lblCustomRange = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSalesCustomRange = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(13, 713);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(200, 20);
            this.dtpStart.TabIndex = 0;
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(219, 713);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(200, 20);
            this.dtpEnd.TabIndex = 1;
            // 
            // btnSalesDaily
            // 
            this.btnSalesDaily.Location = new System.Drawing.Point(456, 714);
            this.btnSalesDaily.Name = "btnSalesDaily";
            this.btnSalesDaily.Size = new System.Drawing.Size(112, 23);
            this.btnSalesDaily.TabIndex = 2;
            this.btnSalesDaily.Text = "Daily Sales";
            this.btnSalesDaily.UseVisualStyleBackColor = true;
            this.btnSalesDaily.Click += new System.EventHandler(this.btnSalesDaily_Click);
            // 
            // btnSalesWeekly
            // 
            this.btnSalesWeekly.Location = new System.Drawing.Point(574, 714);
            this.btnSalesWeekly.Name = "btnSalesWeekly";
            this.btnSalesWeekly.Size = new System.Drawing.Size(112, 23);
            this.btnSalesWeekly.TabIndex = 3;
            this.btnSalesWeekly.Text = "Weekly Sales";
            this.btnSalesWeekly.UseVisualStyleBackColor = true;
            // 
            // btnSalesMonthly
            // 
            this.btnSalesMonthly.Location = new System.Drawing.Point(513, 743);
            this.btnSalesMonthly.Name = "btnSalesMonthly";
            this.btnSalesMonthly.Size = new System.Drawing.Size(112, 23);
            this.btnSalesMonthly.TabIndex = 4;
            this.btnSalesMonthly.Text = "Monthly Sales";
            this.btnSalesMonthly.UseVisualStyleBackColor = true;
            // 
            // btnSalesViewHTML
            // 
            this.btnSalesViewHTML.Location = new System.Drawing.Point(161, 772);
            this.btnSalesViewHTML.Name = "btnSalesViewHTML";
            this.btnSalesViewHTML.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSalesViewHTML.Size = new System.Drawing.Size(112, 23);
            this.btnSalesViewHTML.TabIndex = 6;
            this.btnSalesViewHTML.Text = "View Sales Report";
            this.btnSalesViewHTML.UseVisualStyleBackColor = true;
            // 
            // dgvSales
            // 
            this.dgvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSales.Location = new System.Drawing.Point(12, 562);
            this.dgvSales.Name = "dgvSales";
            this.dgvSales.Size = new System.Drawing.Size(733, 116);
            this.dgvSales.TabIndex = 7;
            // 
            // btnInvAvailable
            // 
            this.btnInvAvailable.Location = new System.Drawing.Point(812, 552);
            this.btnInvAvailable.Name = "btnInvAvailable";
            this.btnInvAvailable.Size = new System.Drawing.Size(122, 23);
            this.btnInvAvailable.TabIndex = 8;
            this.btnInvAvailable.Text = "Available Inventory";
            this.btnInvAvailable.UseVisualStyleBackColor = true;
            // 
            // btnInvNeedsRestock
            // 
            this.btnInvNeedsRestock.Location = new System.Drawing.Point(969, 552);
            this.btnInvNeedsRestock.Name = "btnInvNeedsRestock";
            this.btnInvNeedsRestock.Size = new System.Drawing.Size(122, 23);
            this.btnInvNeedsRestock.TabIndex = 9;
            this.btnInvNeedsRestock.Text = "Restock Soon";
            this.btnInvNeedsRestock.UseVisualStyleBackColor = true;
            this.btnInvNeedsRestock.Click += new System.EventHandler(this.btnInvNeedsRestock_Click_1);
            // 
            // btnInvAll
            // 
            this.btnInvAll.Location = new System.Drawing.Point(1123, 552);
            this.btnInvAll.Name = "btnInvAll";
            this.btnInvAll.Size = new System.Drawing.Size(122, 23);
            this.btnInvAll.TabIndex = 10;
            this.btnInvAll.Text = "All Inventory";
            this.btnInvAll.UseVisualStyleBackColor = true;
            // 
            // dgvInventory
            // 
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Location = new System.Drawing.Point(12, 23);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.Size = new System.Drawing.Size(1233, 491);
            this.dgvInventory.TabIndex = 12;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1616, 770);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(1535, 770);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 15;
            this.btnHelp.Text = "Help?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // lblCustomRange
            // 
            this.lblCustomRange.AutoSize = true;
            this.lblCustomRange.Location = new System.Drawing.Point(176, 697);
            this.lblCustomRange.Name = "lblCustomRange";
            this.lblCustomRange.Size = new System.Drawing.Size(77, 13);
            this.lblCustomRange.TabIndex = 16;
            this.lblCustomRange.Text = "Custom Range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 531);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Sales Reports";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Inventory Reports";
            // 
            // btnSalesCustomRange
            // 
            this.btnSalesCustomRange.Location = new System.Drawing.Point(144, 743);
            this.btnSalesCustomRange.Name = "btnSalesCustomRange";
            this.btnSalesCustomRange.Size = new System.Drawing.Size(146, 23);
            this.btnSalesCustomRange.TabIndex = 19;
            this.btnSalesCustomRange.Text = "Custom Range";
            this.btnSalesCustomRange.UseVisualStyleBackColor = true;
            // 
            // frmReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1703, 805);
            this.Controls.Add(this.btnSalesCustomRange);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCustomRange);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.btnInvAll);
            this.Controls.Add(this.btnInvNeedsRestock);
            this.Controls.Add(this.btnInvAvailable);
            this.Controls.Add(this.dgvSales);
            this.Controls.Add(this.btnSalesViewHTML);
            this.Controls.Add(this.btnSalesMonthly);
            this.Controls.Add(this.btnSalesWeekly);
            this.Controls.Add(this.btnSalesDaily);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.dtpStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmReports";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales and Inventory Reports";
            this.Load += new System.EventHandler(this.frmReports_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Button btnSalesDaily;
        private System.Windows.Forms.Button btnSalesWeekly;
        private System.Windows.Forms.Button btnSalesMonthly;
        private System.Windows.Forms.Button btnSalesViewHTML;
        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.Button btnInvAvailable;
        private System.Windows.Forms.Button btnInvNeedsRestock;
        private System.Windows.Forms.Button btnInvAll;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label lblCustomRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSalesCustomRange;
    }
}