using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ThameJordan25SU233x
{
    public partial class frmNewAccount : Form
    {
        // Detect if user holds the manager position title
        public bool IsUserManager { get; set; } = false;

        // Form constructor
        public frmNewAccount()
        {
            InitializeComponent();
        }

        // Form load event
        private void frmNewAccount_Load(object sender, EventArgs e)
        {
            tbxPhoneNumberOne.KeyPress += tbxPhoneNumber_KeyPress;

            loadSecurityQuestions();

            // Show/hide manager-only fields
            cbxPositionTitles.Visible = IsUserManager;
            lblPositionTitle.Visible = IsUserManager;

            if (IsUserManager)
            {
                // Populate Position Titles for managers
                cbxPositionTitles.Items.Clear();
                List<string> positions = clsSQL.GetAllPositionTitles();
                foreach (var pos in positions)
                    cbxPositionTitles.Items.Add(pos);
                if (cbxPositionTitles.Items.Count > 0)
                    cbxPositionTitles.SelectedIndex = 0;
            }
        }

        // tbxPhoneNumber input filter
        private void tbxPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Disable user from entering any non-numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '(' && e.KeyChar != ')' && e.KeyChar != ' ' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        // Load Security Questions
        private void loadSecurityQuestions()
        {
            // Call method
            var secQuesSets = clsSQL.GetSecurityQuestionSets();

            // Load SetID 1 questions
            cbxSecQuestion1.DisplayMember = "QuestionPrompt";
            cbxSecQuestion1.ValueMember = "QuestionID";
            cbxSecQuestion1.DataSource = secQuesSets.ContainsKey(1) ? secQuesSets[1] : null;

            // Load SetID 2 questions
            cbxSecQuestion2.DisplayMember = "QuestionPrompt";
            cbxSecQuestion2.ValueMember = "QuestionID";
            cbxSecQuestion2.DataSource = secQuesSets.ContainsKey(2) ? secQuesSets[2] : null;

            // Load SetID 3 questions
            cbxSecQuestion3.DisplayMember = "QuestionPrompt";
            cbxSecQuestion3.ValueMember = "QuestionID";
            cbxSecQuestion3.DataSource = secQuesSets.ContainsKey(3) ? secQuesSets[3] : null;
        }

        // Gather new user data
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                // Username
                string newUsernameInput = tbxNewUsername.Text.Trim();
                if (!clsValidation.IsValidUsername(newUsernameInput, out string newUsernameError))
                {
                    MessageBox.Show(newUsernameError, "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxNewUsername.Focus(); return;
                }
                if (clsSQL.CheckUsernameExists(newUsernameInput))
                {
                    MessageBox.Show("That username is already taken. Please try another.", "Username Taken", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxNewUsername.Focus(); return;
                }

                // Password
                string newPasswordInput = tbxNewPassword.Text.Trim();
                if (!clsValidation.IsValidPassword(newPasswordInput, out string newPasswordError))
                {
                    MessageBox.Show(newPasswordError, "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxNewPassword.Focus(); return;
                }

                // Email
                string userEmailAddress = tbxEmailAddress.Text.Trim();
                if (!clsValidation.IsValidEmail(userEmailAddress, out string emailAddressError))
                {
                    MessageBox.Show(emailAddressError, "Invalid Email Address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbxEmailAddress.Focus(); return;
                }

                // Gather all user input
                string userTitle = cbxTitles.Text.Trim();
                string userFirstName = tbxFirstName.Text.Trim();
                string userMiddleName = tbxMiddleName.Text.Trim();
                string userLastName = tbxLastName.Text.Trim();
                string userSuffix = cbxSuffix.Text.Trim();
                string userAddressOne = tbxStreetAddressOne.Text.Trim();
                string userAddressTwo = tbxStreetAddressTwo.Text.Trim();
                string userAddressThree = tbxStreetAddressThree.Text.Trim();
                string userCity = tbxCity.Text.Trim();
                string userState = cbxStates.Text.Trim();
                string userZipCode = tbxZipCode.Text.Trim();
                string userPrimaryPhoneNumber = tbxPhoneNumberOne.Text.Trim();
                string userSecondaryPhoneNumber = tbxPhoneNumberTwo.Text.Trim();

                // Security Questions 
                int FirstChallengeQuestion = cbxSecQuestion1.SelectedValue != null ? Convert.ToInt32(cbxSecQuestion1.SelectedValue) : 0;
                int SecondChallengeQuestion = cbxSecQuestion2.SelectedValue != null ? Convert.ToInt32(cbxSecQuestion2.SelectedValue) : 0;
                int ThirdChallengeQuestion = cbxSecQuestion3.SelectedValue != null ? Convert.ToInt32(cbxSecQuestion3.SelectedValue) : 0;
                string FirstChallengeAnswer = tbxSecQuestAns1.Text.Trim();
                string SecondChallengeAnswer = tbxSecQuestAns2.Text.Trim();
                string ThirdChallengeAnswer = tbxSecQuestAns3.Text.Trim();

                // Validate security questions 
                if (FirstChallengeQuestion == 0 || string.IsNullOrWhiteSpace(FirstChallengeAnswer))
                {
                    MessageBox.Show("Please select and answer Security Question 1.", "Invalid Security Prompt 1", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxSecQuestion1.Focus(); return;
                }
                if (SecondChallengeQuestion == 0 || string.IsNullOrWhiteSpace(SecondChallengeAnswer))
                {
                    MessageBox.Show("Please select and answer Security Question 2.", "Invalid Security Prompt 2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxSecQuestion2.Focus(); return;
                }
                if (ThirdChallengeQuestion == 0 || string.IsNullOrWhiteSpace(ThirdChallengeAnswer))
                {
                    MessageBox.Show("Please select and answer Security Question 3.", "Invalid Security Prompt 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxSecQuestion3.Focus(); return;
                }

                // Position Title assignment 
                string userPosition = "Customer";
                if (IsUserManager && cbxPositionTitles.SelectedItem != null)
                {
                    userPosition = cbxPositionTitles.SelectedItem.ToString();
                }

                // Bundle all values for SQL 
                var userData = new Dictionary<string, object>
                {
                    ["Title"] = cbxTitles.Text.Trim(),
                    ["FirstName"] = tbxFirstName.Text.Trim(),
                    ["MiddleName"] = tbxMiddleName.Text.Trim(),
                    ["LastName"] = tbxLastName.Text.Trim(),
                    ["Suffix"] = cbxSuffix.Text.Trim(),
                    ["Address1"] = tbxStreetAddressOne.Text.Trim(),
                    ["Address2"] = tbxStreetAddressTwo.Text.Trim(),
                    ["Address3"] = tbxStreetAddressThree.Text.Trim(),
                    ["City"] = tbxCity.Text.Trim(),
                    ["Zipcode"] = tbxZipCode.Text.Trim(),
                    ["State"] = cbxStates.Text.Trim(),
                    ["Email"] = tbxEmailAddress.Text.Trim(),
                    ["PhonePrimary"] = tbxPhoneNumberOne.Text.Trim(),
                    ["PhoneSecondary"] = tbxPhoneNumberTwo.Text.Trim(),
                    ["PositionTitle"] = userPosition,
                    ["Username"] = tbxNewUsername.Text.Trim(),
                    ["Password"] = tbxNewPassword.Text.Trim(),
                    ["FirstChallengeQuestion"] = FirstChallengeQuestion,
                    ["FirstChallengeAnswer"] = FirstChallengeAnswer,
                    ["SecondChallengeQuestion"] = SecondChallengeQuestion,
                    ["SecondChallengeAnswer"] = SecondChallengeAnswer,
                    ["ThirdChallengeQuestion"] = ThirdChallengeQuestion,
                    ["ThirdChallengeAnswer"] = ThirdChallengeAnswer,
                };

                // Call create method
                if (clsSQL.CreateNewAccount(userData, out string errorMessage))
                {
                    MessageBox.Show("Account created successfully!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error creating account: " + errorMessage, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch  
            { 
            
            }
        }

        // Help button
        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Help guide for user
            MessageBox.Show(
                "Need help creating your account?\n\n" +
                "Follow these steps:\n\n" +
                "1. **Fill in Personal Information** – Enter your name, contact info, and address details.\n" +
                "2. **Security Questions** – Select and answer 3 security questions. These help you recover your account later.\n" +
                "3. **Set Credentials** – Enter your desired username and password.\n" +
                "   - Passwords should be strong and secure.\n" +
                "4. **Create Account** – Click 'Create account!' to register.\n\n" +
                "Other options:\n" +
                "• **Return to Login** – Already have an account? Click this to go back to the login screen.\n" +
                "• **Help** – You’re here now! 😊\n\n" +
                "Tips:\n" +
                "- All required fields must be completed.\n" +
                "- Avoid using spaces in the username.\n" +
                "- Passwords should be at least 8 characters, with a mix of letters and numbers.\n\n" +
                "Still stuck? Contact support at: support@example.com",
                "Account Creation Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Return to Login form
        private void btnReturn_Click(object sender, EventArgs e)
        {
            // Transfer user back to login page
            this.Close();
        }
    }
}