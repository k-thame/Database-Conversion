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
    public partial class frmPasswordReset : Form
    {
        // Form constructor
        public frmPasswordReset()
        {
            InitializeComponent();
        }

        // Form load event
        private void frmPasswordReset_Load(object sender, EventArgs e)
        {

        }

        // Validate users answers
        private bool ValidateSecurityAnswers(string username, string ans1, string ans2, string ans3, out string errorMessage)
        {
            return clsSQL.ValidateSecurityAnswers(username, ans1, ans2, ans3, out errorMessage);
        }

        // Load users security questions
        private void loadSecurityQuestions(string username)
        {
            try
            {
                //
                var prompts = clsSQL.GetSecurityQuestionPrompts(username);

                //
                if (prompts != null && prompts.Count == 3)
                {
                    tbxSecQues1.Text = prompts[0];
                    tbxSecQues2.Text = prompts[1];
                    tbxSecQues3.Text = prompts[2];
                }
                else
                {
                    // Error message
                    MessageBox.Show("No security questions found for that username.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error loading security questions:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reset password button
        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            // Variables containing user input
            string usernameInput = tbxUsername.Text.Trim();
            string passwordResetInput = tbxPasswordReset.Text.Trim();

            // If user does not enter any username values
            if (string.IsNullOrWhiteSpace(usernameInput))
            {
                // Error message
                MessageBox.Show("Please enter your username before resetting your password.", "Username Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxUsername.Focus();
                return;
            }

            // If user does not enter any password values
            if (string.IsNullOrWhiteSpace(passwordResetInput))
            {
                // Error message
                MessageBox.Show("Please enter your password.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxPasswordReset.Focus();
                return;
            }

            // If user entered username does not meet requirements
            if (!clsValidation.IsValidUsername(usernameInput, out string usernameError))
            {
                // Error message
                MessageBox.Show(usernameError, "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxUsername.Focus();
                return;
            }

            // If user entered password does not meet requirements
            if (!clsValidation.IsValidPassword(passwordResetInput, out string passwordError))
            {
                // Error message
                MessageBox.Show(passwordError, "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxPasswordReset.Focus();
                return;
            }

            // Variables to hold user entered security question answers
            string answer1 = tbxSecQuestAns1.Text.Trim();
            string answer2 = tbxSecQuestAns2.Text.Trim();
            string answer3 = tbxSecQuestAns3.Text.Trim();

            try
            {
                // Check if user entered security question answers are incorrect
                if (!ValidateSecurityAnswers(usernameInput, answer1, answer2, answer3, out string validationError))
                {
                    // Error message
                    MessageBox.Show(validationError, "Answer Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // If criteria is met, prompt to user that password has been reset
                if (clsSQL.ResetUserPassword(usernameInput, passwordResetInput))
                {
                    // Successful password reset message
                    MessageBox.Show("Password reset successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    // Error message
                    MessageBox.Show("Password reset failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Submit username button
        private void btnSubmitUsername_Click(object sender, EventArgs e)
        {
            // Variable to hold users username
            string username = tbxUsername.Text.Trim();

            // If user does not enter any username
            if (string.IsNullOrEmpty(username))
            {
                // Error message
                MessageBox.Show("Please enter a username.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxUsername.Focus();
                return;
            }
            
            // If users enterd username does not meet requirements
            if (!clsValidation.IsValidUsername(username, out string error))
            {
                // Error message
                MessageBox.Show(error, "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxUsername.Focus();
                return;
            }

            try
            {
                // Check if entered username exist, then call method. If not, alert user
                if (clsSQL.UsernameExists(username))
                {
                    loadSecurityQuestions(username);
                }
                else
                {
                    // Error message
                    MessageBox.Show("Username not found in the database.", "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                // Error message
                MessageBox.Show("Error checking username: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Help button
        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Help guide for user
            MessageBox.Show(
                "Need help resetting your password?\n\n" +
                "Here's how to do it:\n\n" +
                "1. **Enter Username** – Type your username in the textbox provided.\n" +
                "2. **Submit** – Click the 'Submit' button to search for your account and display your security questions.\n" +
                "3. **Answer Questions** – Answer the security questions correctly to proceed.\n" +
                "4. **New Password** – Enter your new password in the 'New Password' textbox.\n" +
                "5. **Reset Password** – Click the 'Reset Password' button to save your new password.\n\n" +
                "Other options:\n" +
                "• **Help** – You’re here now! 😊\n" +
                "• **Exit** – Click 'Exit' to close the reset window without making changes.\n\n" +
                "Tips:\n" +
                "- Make sure your username is correct.\n" +
                "- Passwords must meet any required criteria (length, complexity).\n" +
                "- If your security answers don’t match, double-check your spelling or try again.\n\n" +
                "Still having trouble? Contact support at: support@example.com",
                "Password Reset Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Close the form
            this.Close();
        }
    }
}
