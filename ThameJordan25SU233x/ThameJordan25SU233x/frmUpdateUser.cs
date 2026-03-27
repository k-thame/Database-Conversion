using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    public partial class frmUpdateUser : Form
    {
        // Variable(s) for personID
        private readonly int _personID;

        // Variable(s) for original data
        private string originalTitle, originalNameFirst, originalNameMiddle, originalNameLast, originalSuffix, originalAddress1, originalAddress2, originalAddress3, originalCity, originalZipcode, originalState, originalEmail, originalPhonePrimary, originalPhoneSecondary, originalPositionTitle;

        // Constructor
        public frmUpdateUser(int personID)
        {
            InitializeComponent();
            _personID = personID;
        }

        // Form load
        private void frmUpdateUser_Load(object sender, EventArgs e)
        {
            DataRow user = clsSQL.GetUserDetails(_personID);
            if (user != null)
            {
                // populate controls from DB
                cbxTitles.Text = user["Title"].ToString();
                tbxFirstName.Text = user["NameFirst"].ToString();
                tbxMiddleName.Text = user["NameMiddle"].ToString();
                tbxLastName.Text = user["NameLast"].ToString();
                cbxSuffix.Text = user["Suffix"].ToString();
                tbxStreetAddressOne.Text = user["Address1"].ToString();
                tbxStreetAddressTwo.Text = user["Address2"].ToString();
                tbxStreetAddressThree.Text = user["Address3"].ToString();
                tbxCity.Text = user["City"].ToString();
                tbxZipCode.Text = user["Zipcode"].ToString();
                cbxStates.Text = user["State"].ToString();
                tbxEmailAddress.Text = user["Email"].ToString();
                tbxPhoneNumberOne.Text = user["PhonePrimary"].ToString();
                tbxPhoneNumberTwo.Text = user["PhoneSecondary"].ToString();
                cbxPositionTitle.SelectedItem = user["PositionTitle"].ToString();

                // store originals for use if fields are left blank
                originalTitle = cbxTitles.Text;
                originalNameFirst = tbxFirstName.Text;
                originalNameMiddle = tbxMiddleName.Text;
                originalNameLast = tbxLastName.Text;
                originalSuffix = cbxSuffix.Text;
                originalAddress1 = tbxStreetAddressOne.Text;
                originalAddress2 = tbxStreetAddressTwo.Text;
                originalAddress3 = tbxStreetAddressThree.Text;
                originalCity = tbxCity.Text;
                originalZipcode = tbxZipCode.Text;
                originalState = cbxStates.Text;
                originalEmail = tbxEmailAddress.Text;
                originalPhonePrimary = tbxPhoneNumberOne.Text;
                originalPhoneSecondary = tbxPhoneNumberTwo.Text;
                originalPositionTitle = cbxPositionTitle.SelectedItem?.ToString();
            }
        }

        // Update account information
        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            // Validate First Name if changed
            string firstNameInput = tbxFirstName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(firstNameInput) &&
                !clsValidation.IsValidFirstName(firstNameInput, out string firstNameError))
            {
                MessageBox.Show(firstNameError, "Invalid First Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxFirstName.Focus(); return;
            }

            // Validate Middle Name if changed
            string middleNameInput = tbxMiddleName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(middleNameInput) &&
                !clsValidation.IsValidMiddleName(middleNameInput, out string middleNameError))
            {
                MessageBox.Show(middleNameError, "Invalid Middle Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxMiddleName.Focus(); return;
            }

            // Validate Last Name if changed
            string lastNameInput = tbxLastName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(lastNameInput) &&
                !clsValidation.IsValidLastName(lastNameInput, out string lastNameError))
            {
                MessageBox.Show(lastNameError, "Invalid Last Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxLastName.Focus(); return;
            }

            // Validate Email if changed
            string emailInput = tbxEmailAddress.Text.Trim();
            if (!string.IsNullOrWhiteSpace(emailInput) &&
                !clsValidation.IsValidEmail(emailInput, out string emailAddressError))
            {
                MessageBox.Show(emailAddressError, "Invalid Email Address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxEmailAddress.Focus(); return;
            }

            // Validate Zip if changed
            string zipInput = tbxZipCode.Text.Trim();
            if (!string.IsNullOrWhiteSpace(zipInput) &&
                !clsValidation.IsValidZipCode(zipInput, out string zipError))
            {
                MessageBox.Show(zipError, "Invalid ZIP Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxZipCode.Focus(); return;
            }

            // Validate Primary Phone if changed
            string phonePrimaryInput = tbxPhoneNumberOne.Text.Trim();
            if (!string.IsNullOrWhiteSpace(phonePrimaryInput) &&
                !clsValidation.IsValidPrimaryPhoneNumber(phonePrimaryInput, out string phonePrimaryError))
            {
                MessageBox.Show(phonePrimaryError, "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxPhoneNumberOne.Focus(); return;
            }

            // Validate Secondary Phone if changed
            string phoneSecondaryInput = tbxPhoneNumberTwo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(phoneSecondaryInput) &&
                !clsValidation.IsValidSecondaryPhoneNumber(phoneSecondaryInput, out string phoneSecondaryError))
            {
                MessageBox.Show(phoneSecondaryError, "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxPhoneNumberTwo.Focus(); return;
            }

            //
            var data = new Dictionary<string, object>
            {
                ["Title"] = string.IsNullOrWhiteSpace(cbxTitles.Text) ? originalTitle : cbxTitles.Text.Trim(),
                ["NameFirst"] = string.IsNullOrWhiteSpace(tbxFirstName.Text) ? originalNameFirst : tbxFirstName.Text.Trim(),
                ["NameMiddle"] = string.IsNullOrWhiteSpace(tbxMiddleName.Text) ? originalNameMiddle : tbxMiddleName.Text.Trim(),
                ["NameLast"] = string.IsNullOrWhiteSpace(tbxLastName.Text) ? originalNameLast : tbxLastName.Text.Trim(),
                ["Suffix"] = string.IsNullOrWhiteSpace(cbxSuffix.Text) ? originalSuffix : cbxSuffix.Text.Trim(),
                ["Address1"] = string.IsNullOrWhiteSpace(tbxStreetAddressOne.Text) ? originalAddress1 : tbxStreetAddressOne.Text.Trim(),
                ["Address2"] = string.IsNullOrWhiteSpace(tbxStreetAddressTwo.Text) ? originalAddress2 : tbxStreetAddressTwo.Text.Trim(),
                ["Address3"] = string.IsNullOrWhiteSpace(tbxStreetAddressThree.Text) ? originalAddress3 : tbxStreetAddressThree.Text.Trim(),
                ["City"] = string.IsNullOrWhiteSpace(tbxCity.Text) ? originalCity : tbxCity.Text.Trim(),
                ["Zipcode"] = string.IsNullOrWhiteSpace(tbxZipCode.Text) ? originalZipcode : tbxZipCode.Text.Trim(),
                ["State"] = string.IsNullOrWhiteSpace(cbxStates.Text) ? originalState : cbxStates.Text.Trim(),
                ["Email"] = string.IsNullOrWhiteSpace(tbxEmailAddress.Text) ? originalEmail : tbxEmailAddress.Text.Trim(),
                ["PhonePrimary"] = string.IsNullOrWhiteSpace(tbxPhoneNumberOne.Text) ? originalPhonePrimary : tbxPhoneNumberOne.Text.Trim(),
                ["PhoneSecondary"] = string.IsNullOrWhiteSpace(tbxPhoneNumberTwo.Text) ? originalPhoneSecondary : tbxPhoneNumberTwo.Text.Trim(),
                ["PositionTitle"] = cbxPositionTitle.SelectedItem?.ToString() ?? originalPositionTitle
            };

            bool success = clsSQL.UpdateUserDetails(_personID, data);
            if (success)
            {
                MessageBox.Show("User updated successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update the user.");
            }
        }

        // Help button (MODIFY)
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Update account information here.\n\n" +
                "What to fill in:\n" +
                "1. **Title / Name** – Select Title/Suffix if used and update First, Middle, Last names.\n" +
                "2. **Email & Phones** – Primary and optional Secondary phone numbers.\n" +
                "3. **Addresses** – Street Address 1–3, City, State, Zip Code.\n" +
                "4. **Position Title** – For managers: set the user’s role/position.\n\n" +
                "Finish:\n" +
                "• Click **Update account!** to save changes.\n\n" +
                "Other options:\n" +
                "• **Help** – You’re here now! 😊\n" +
                "• **Exit** – Close without saving.\n\n" +
                "Tips:\n" +
                "- Verify phone and email formats.\n" +
                "- Keep address lines consistent for shipping and records.\n" +
                "- Position Title may affect permissions.",
                "Update User – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Exit button (MODIFY)
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}