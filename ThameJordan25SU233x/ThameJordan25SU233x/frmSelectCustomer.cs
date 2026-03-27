using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmSelectCustomer : Form
    {
        //
        public int? SelectedPersonID { get; private set; }

        // Local data
        private DataTable _customers;

        // Controls for minimal UI
        private Label lblSearch;
        private TextBox txtSearch;
        private DataGridView dgvCustomers;
        private Button btnOK;
        private Button btnCancel;

        public frmSelectCustomer()
        {
            InitializeComponent();

            // Wire events
            this.Load += frmSelectCustomer_Load;
            txtSearch.TextChanged += txtSearch_TextChanged;
            dgvCustomers.CellDoubleClick += dgvCustomers_CellDoubleClick;
            btnOK.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void frmSelectCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                // Initial load
                _customers = clsSQL.GetCustomerLookupForPicker(null);
                BindGrid(_customers);
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load customers:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _customers = clsSQL.GetCustomerLookupForPicker(txtSearch.Text);
                BindGrid(_customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) CommitSelection();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CommitSelection();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CommitSelection()
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("Please select a customer.", "Select",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!dgvCustomers.Columns.Contains("PersonID"))
            {
                MessageBox.Show("PersonID column not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var val = dgvCustomers.CurrentRow.Cells["PersonID"].Value;
            if (val == null || !int.TryParse(val.ToString(), out int pid))
            {
                MessageBox.Show("Invalid PersonID.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedPersonID = pid;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BindGrid(DataTable dt)
        {
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.DataSource = dt;

            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;

            // Headers
            SetHeader("PersonID", "ID", 70);
            SetHeader("NameFirst", "First Name", 140);
            SetHeader("NameLast", "Last Name", 140);
            SetHeader("Email", "Email", 220);
            SetHeader("PhonePrimary", "Phone (Primary)", 140);
            SetHeader("PhoneSecondary", "Phone (Alt)", 140);
            SetHeader("MemberID", "Member ID", 110);
        }

        private void SetHeader(string colName, string header, int width = -1)
        {
            if (!dgvCustomers.Columns.Contains(colName)) return;
            var c = dgvCustomers.Columns[colName];
            c.HeaderText = header;
            if (width > 0) c.Width = width;
        }

        // minimal ui (MODIFY)
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectCustomer));
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(166, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search (ID, Name, Email, Phone):";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(15, 35);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(620, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // dgvCustomers
            // 
            this.dgvCustomers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCustomers.Location = new System.Drawing.Point(15, 70);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.Size = new System.Drawing.Size(790, 360);
            this.dgvCustomers.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(620, 440);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 28);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(720, 440);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            // 
            // frmSelectCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 480);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "frmSelectCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Customer";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
