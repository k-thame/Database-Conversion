using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmLogon : Form
    {
        // Variable(s)
        private bool isPasswordShown = false;
        public string PersonID { get; private set; }

        // Form constructor
        public frmLogon()
        {
            InitializeComponent();
            tbxPassword.UseSystemPasswordChar = true;
            btnShowPassword.Text = "Peek";
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
        }

        // Form load event
        private void frmLogon_Load(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
        }

        // Disable sign-in button
        private void ValidateLoginFields()
        {
            btnLogin.Enabled = !string.IsNullOrWhiteSpace(tbxUsername.Text) && !string.IsNullOrWhiteSpace(tbxPassword.Text);
        }

        // Username textbox
        private void tbxUsername_TextChanged(object sender, EventArgs e)
        {
            string username = tbxUsername.Text;
            ValidateLoginFields();
        }

        // Password textbox
        private void tbxPassword_TextChanged(object sender, EventArgs e)
        {
            string password = tbxPassword.Text;
            ValidateLoginFields();
        }

        // Hide password input
        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            if (isPasswordShown) 
            {
                tbxPassword.UseSystemPasswordChar = true;
                btnShowPassword.Text = "Peek";
                isPasswordShown = false;
            }
            else 
            {
                tbxPassword.UseSystemPasswordChar = false;
                btnShowPassword.Text = "Hide";
                isPasswordShown = true;
            }
        }

        // Login button
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usernameInput = tbxUsername.Text;
            string passwordInput = tbxPassword.Text;

            string error;
            if (clsSQL.ValidateUserCredentials(usernameInput, passwordInput, out error))
            {
                try
                {
                    string positionTitle = clsLogon.GetUserPositionTitle(usernameInput);

                    if (string.Equals(positionTitle, "Manager", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Login Successful! Redirecting to Manager dashboard...", "Welcome, Manager!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        string managerPersonIDStr = clsSQL.GetUserPersonID(usernameInput);
                        if (string.IsNullOrWhiteSpace(managerPersonIDStr) || !int.TryParse(managerPersonIDStr, out int managerPersonID))
                        {
                            MessageBox.Show("Could not determine your Employee/Person ID. Please verify your account.", "Login Context Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string firstName = "", lastName = "", displayName = usernameInput;
                        try
                        {
                            var row = clsSQL.GetCustomerById(managerPersonID);
                            if (row != null)
                            {
                                firstName = Convert.ToString(row["NameFirst"] ?? "").Trim();
                                lastName = Convert.ToString(row["NameLast"] ?? "").Trim();
                                var full = (firstName + " " + lastName).Trim();
                                if (!string.IsNullOrEmpty(full)) displayName = full;
                            }
                        }
                        catch
                        {

                        }

                        var managerForm = new frmManaging
                        {
                            ManagerEmployeeID = managerPersonID,   
                            ManagerDisplayName = displayName        
                        };

                        this.Hide();
                        managerForm.ShowDialog(this);
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show("Login Successful! Redirecting to Shopping dashboard...", "Welcome!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PersonID = clsSQL.GetUserPersonID(usernameInput); 
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the manager dashboard:\n\n" + ex.Message + "\n\n" + ex.StackTrace, "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                }
            }
            else
            {
                MessageBox.Show(error, "Login failed.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxUsername.Focus();
                tbxPassword.Focus();
            }
        }

        // Help button
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Need help signing in?\n\n" +
                "• Sign In – Enter your username and password, then click this button to access your account.\n\n" +
                "• Forgot Password? – Click here if you can't remember your password. You'll be guided through steps to reset it using your security questions.\n\n" +
                "• Create Account – New here? Click this to register for a new account.\n\n" +
                "• Help – That’s where you are now! 😊\n\n" +
                "Still stuck? Make sure:\n" +
                "- Your username and password are typed correctly (check for caps lock!)\n" +
                "- You’ve already created an account\n\n" +
                "If problems continue, contact support at: support@example.com",
                "Sign In Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Password reset button
        private void btnPasswordReset_Click(object sender, EventArgs e)
        {
            tbxUsername.Clear();
            tbxPassword.Clear();
            frmPasswordReset resetPasswordForm = new frmPasswordReset();
            resetPasswordForm.ShowDialog();
        }

        // Create account button
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            frmNewAccount newAccountForm = new frmNewAccount();
            newAccountForm.ShowDialog();
        }

        // Exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to leave? You haven't bought anything yet...", "Leave App?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }

    public static class clsSession
    {
        public static int PersonID { get; set; }
    }
}