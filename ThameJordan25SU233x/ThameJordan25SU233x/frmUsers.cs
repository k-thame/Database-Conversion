using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmUsers : Form
    {
        //
        private readonly clsSQL db = new clsSQL();

        // for self-edit
        private bool _suppressSelectionChanged = false;
        private bool _isClosing = false;
        private int? _selfRowIndex = null; 

        // Constructor
        public frmUsers()
        {
            InitializeComponent();
            dgvUsers.DataBindingComplete += (s, e) => UsersDGV();
        }

        // 
        public int? PreselectEmployeeID { get; set; } = null;   
        public bool SelfEditOnly { get; set; } = false;         


        private string FindIdColumnName()
        {
            if (dgvUsers.Columns.Contains("EmployeeID")) return "EmployeeID";
            if (dgvUsers.Columns.Contains("PersonID")) return "PersonID";
            foreach (DataGridViewColumn c in dgvUsers.Columns)
                if (c.ValueType == typeof(int) || c.ValueType == typeof(int?)) return c.Name;
            return null;
        }

        private void EnsureSelfRowSelected()
        {
            if (!_selfRowIndex.HasValue) return;
            int want = _selfRowIndex.Value;
            if (want < 0 || want >= dgvUsers.Rows.Count) return;

            try
            {
                _suppressSelectionChanged = true;

                // Keep selection model simple
                dgvUsers.MultiSelect = false;
                dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dgvUsers.ClearSelection();
                var row = dgvUsers.Rows[want];
                row.Selected = true;

                // Move focus/current cell onto a visible cell of that row
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Visible)
                    {
                        dgvUsers.CurrentCell = cell;
                        break;
                    }
                }
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        private void LockToSelfRow()
        {
            if (!SelfEditOnly || !PreselectEmployeeID.HasValue || dgvUsers.Rows.Count == 0) return;

            string idCol = FindIdColumnName();
            if (string.IsNullOrEmpty(idCol)) return;

            // Find row with matching ID
            _selfRowIndex = null;
            for (int i = 0; i < dgvUsers.Rows.Count; i++)
            {
                var v = dgvUsers.Rows[i].Cells[idCol].Value;
                if (v != null && int.TryParse(v.ToString(), out int id) && id == PreselectEmployeeID.Value)
                {
                    _selfRowIndex = i;
                    break;
                }
            }

            // Enforce selection and lock 
            EnsureSelfRowSelected();

            // Make all non-self rows read-only and visually disabled
            foreach (DataGridViewRow r in dgvUsers.Rows)
            {
                bool isSelf = _selfRowIndex.HasValue && r.Index == _selfRowIndex.Value;
                r.ReadOnly = !isSelf;
                r.DefaultCellStyle.BackColor = isSelf ? SystemColors.Window : Color.Gainsboro;
                r.DefaultCellStyle.ForeColor = isSelf ? SystemColors.ControlText : Color.DimGray;
                r.DefaultCellStyle.SelectionBackColor = isSelf ? SystemColors.Highlight : Color.Gainsboro;
                r.DefaultCellStyle.SelectionForeColor = isSelf ? SystemColors.HighlightText : Color.DimGray;
            }

            // disable Add/Delete buttons if they exist
            var addBtn = this.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
            if (addBtn != null) addBtn.Enabled = false;
            var delBtn = this.Controls.Find("btnDelete", true).FirstOrDefault() as Button;
            if (delBtn != null) delBtn.Enabled = false;
        }

        //
        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (_isClosing || _suppressSelectionChanged) return;
            if (!SelfEditOnly || !_selfRowIndex.HasValue) return;

            int want = _selfRowIndex.Value;
            if (want < 0 || want >= dgvUsers.Rows.Count) return;
            if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.Index == want) return;

            EnsureSelfRowSelected();
        }

        // Keep user from clicking other rows in self edit mode 
        private void dgvUsers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!SelfEditOnly || !_selfRowIndex.HasValue) return;
            if (e.RowIndex >= 0 && e.RowIndex != _selfRowIndex.Value)
            {
                BeginInvoke(new Action(EnsureSelfRowSelected));
            }
        }

        // Block keyboard navigation away from self row
        private void dgvUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (SelfEditOnly && _selfRowIndex.HasValue)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                    e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown ||
                    e.KeyCode == Keys.Home || e.KeyCode == Keys.End)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    EnsureSelfRowSelected();
                }
            }
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            // Populate ComboBox from DB plus "All"
            cbxPositionTitle.Items.Clear();
            cbxPositionTitle.Items.Add("All");
            List<string> roles = clsSQL.GetAllPositionTitles();
            foreach (string role in roles) cbxPositionTitle.Items.Add(role);
            cbxPositionTitle.SelectedIndex = 0;

            cbxPositionTitle.SelectedIndexChanged += cbxPositionTitle_SelectedIndexChanged;

            // Load all users initially
            clsSQL.DisplayUsers(dgvUsers);
            UsersDGV();

            // Wire interaction
            dgvUsers.SelectionChanged -= dgvUsers_SelectionChanged;
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
            dgvUsers.CellMouseDown -= dgvUsers_CellMouseDown;
            dgvUsers.CellMouseDown += dgvUsers_CellMouseDown;
            dgvUsers.KeyDown -= dgvUsers_KeyDown;
            dgvUsers.KeyDown += dgvUsers_KeyDown;

            // Apply self-edit lock 
            LockToSelfRow();
        }

        private void UsersDGV()
        {
            // Minimize columns for compact display
            if (dgvUsers.Columns.Contains("Zipcode"))
                dgvUsers.Columns["Zipcode"].Width = 55;
            if (dgvUsers.Columns.Contains("State"))
                dgvUsers.Columns["State"].Width = 40;
            if (dgvUsers.Columns.Contains("Title"))
                dgvUsers.Columns["Title"].Width = 40;
            if (dgvUsers.Columns.Contains("Suffix"))
                dgvUsers.Columns["Suffix"].Width = 40;

            if (dgvUsers.Columns.Contains("NameFirst"))
                dgvUsers.Columns["NameFirst"].Width = 100;
            if (dgvUsers.Columns.Contains("NameMiddle"))
                dgvUsers.Columns["NameMiddle"].Width = 60;
            if (dgvUsers.Columns.Contains("NameLast"))
                dgvUsers.Columns["NameLast"].Width = 100;

            if (dgvUsers.Columns.Contains("Address1"))
                dgvUsers.Columns["Address1"].Width = 120;
            if (dgvUsers.Columns.Contains("Address2"))
                dgvUsers.Columns["Address2"].Width = 80;
            if (dgvUsers.Columns.Contains("Address3"))
                dgvUsers.Columns["Address3"].Width = 80;
            if (dgvUsers.Columns.Contains("City"))
                dgvUsers.Columns["City"].Width = 80;

            if (dgvUsers.Columns.Contains("Email"))
            {
                dgvUsers.Columns["Email"].Width = 150;
                dgvUsers.Columns["Email"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            if (dgvUsers.Columns.Contains("PhonePrimary"))
                dgvUsers.Columns["PhonePrimary"].Width = 110;
            if (dgvUsers.Columns.Contains("PhoneSecondary"))
                dgvUsers.Columns["PhoneSecondary"].Width = 110;

            if (dgvUsers.Columns.Contains("PositionTitle"))
                dgvUsers.Columns["PositionTitle"].Width = 90;
            if (dgvUsers.Columns.Contains("LogonName"))
                dgvUsers.Columns["LogonName"].Width = 110;
            if (dgvUsers.Columns.Contains("AccountDisabled"))
                dgvUsers.Columns["AccountDisabled"].Width = 60;
            if (dgvUsers.Columns.Contains("AccountDeleted"))
                dgvUsers.Columns["AccountDeleted"].Width = 60;

            // Wrapping and autosizing
            dgvUsers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUsers.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10);
        
            //
            dgvUsers.ReadOnly = false;
            dgvUsers.MultiSelect = false;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadUsers(string role = null)
        {
            if (_isClosing) return;
            clsSQL.DisplayUsers(dgvUsers, role);
 
            if (SelfEditOnly) LockToSelfRow();
        }

        private int GetSelectedPersonID()
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                return Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["PersonID"].Value);
            }
            return -1;
        }

        private void cbxPositionTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRole = cbxPositionTitle.SelectedItem?.ToString();
            LoadUsers(selectedRole);
            UsersDGV();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            var createUser = new frmNewAccount();
            createUser.IsUserManager = true;
            if (createUser.ShowDialog() == DialogResult.OK)
            {
                LoadUsers(cbxPositionTitle.SelectedItem?.ToString());
            }
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            int personID = GetSelectedPersonID();
            if (personID > 0)
            {
                frmUpdateUser updateForm = new frmUpdateUser(personID);
                updateForm.ShowDialog();
                LoadUsers(cbxPositionTitle.SelectedItem?.ToString());
            }
        }

        private void btnDisableUser_Click(object sender, EventArgs e)
        {
            int personID = GetSelectedPersonID();
            if (personID > 0)
            {
                DialogResult confirm = MessageBox.Show("Are you sure you want to disable this account?",
                    "Confirm Disable", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    bool success = db.DisableUser(personID);
                    if (success)
                    {
                        MessageBox.Show("User account disabled.");
                        LoadUsers(cbxPositionTitle.SelectedItem?.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Failed to disable user.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No user selected.");
            }
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            int personID = GetSelectedPersonID();
            if (personID > 0)
            {
                DialogResult confirm = MessageBox.Show("Are you sure you want to permanently remove this account?",
                    "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    bool success = db.DeleteUser(personID);
                    if (success)
                    {
                        MessageBox.Show("User account removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers(cbxPositionTitle.SelectedItem?.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No user selected.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Help button (MODIFY)
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Manage customer and manager accounts here.\n\n" +
                "How to use:\n" +
                "1. **All Users Grid** – Browse and select a user to manage.\n" +
                "2. **Filter by Position Title** – Narrow the list to Managers, Customers, etc.\n" +
                "3. **Actions**:\n" +
                "   • **Add** – Create a new user account.\n" +
                "   • **Update** – Edit the selected user’s details.\n" +
                "   • **Disable** – Temporarily prevent login without deleting the account.\n" +
                "   • **Remove** – Permanently delete the account.\n\n" +
                "Other options:\n" +
                "• **Help** – You’re here now! 😊\n" +
                "• **Exit** – Close user management.\n\n" +
                "Tips:\n" +
                "- Prefer **Disable** over **Remove** if you may need the account later.\n" +
                "- Double-check you’ve selected the correct user before updating or removing.\n" +
                "- Use the filter to avoid editing the wrong role.",
                "User Management – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Exit button (MODIFY)
        private void btnExit_Click(object sender, EventArgs e)
        {
            _isClosing = true;
            this.Close();
        }
    }
}
