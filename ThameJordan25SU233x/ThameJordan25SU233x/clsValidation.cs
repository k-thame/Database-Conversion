using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


// User input Validation
namespace ThameJordan25SU233x
{
    internal class clsValidation
    {
        // Username validation
        public static bool IsValidUsername(string username, out string errorMessage)
        {
            // Error prompt for username
            errorMessage = string.Empty;

            if(string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username cannot be empty or contain empty spaces";
                return false;
            }
            
            if (char.IsDigit(username[0]))
            {
                errorMessage = "Username cannot begin with a digit";
                return false;
            }
                
            if(!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
            {
                errorMessage = "Username can only contain letters and numbers. No special characters or spaces allowed.";
                return false;
            }

            if (username.Length < 8 || username.Length > 20)
            {
                errorMessage = "Username must be between 8 and 20 characters";
                return false;
            }

            // Create if for checking existing username

            return true;
        }

        // Password validation
        public static bool IsValidPassword(string password, out string errorMessage) 
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password)) 
            {
                errorMessage = "Password cannot be empty or contain empty spaces.";
                return false;
            }

            if (password.Length < 8 || password.Length > 20)
            {
                errorMessage = "Password must be between 8 and 20 characters.";
                return false;
            }

            if (password.StartsWith(" ") || password.EndsWith(" "))
            {
                errorMessage = "Password cannot start or end with a space.";
                return false;
            }

            if (password.Contains(" "))
            {
                errorMessage = "Password cannot contain spaces.";
                return false;
            }

            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9()!@#$%^&*]+$"))
            {
                errorMessage = "Password can only contain letters, numbers, and the following special characters: ()!@#$%^&*.";
                return false;
            }

            int complexityCount = 0;
            if (Regex.IsMatch(password, @"[A-Z]")) complexityCount++;           // Uppercase
            if (Regex.IsMatch(password, @"[a-z]")) complexityCount++;           // Lowercase
            if (Regex.IsMatch(password, @"[0-9]")) complexityCount++;           // Number
            if (Regex.IsMatch(password, @"[()!@#$%^&*]")) complexityCount++;    // Special characters

            if (complexityCount < 3)
            {
                errorMessage = "Password must include at least 3 of the following: uppercase letters, lowercase letters, numbers, or special characters ()!@#$%^&*.";
                return false;
            }

            // Create if for checking existing username

            return true;
        }

        // Email validation
        public static bool IsValidEmail(string emailAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            const int MaxEmailLength = 254;

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                errorMessage = "Email address cannot be empty or contain empty spaces.";
                return false;
            }

            emailAddress = emailAddress.Trim();
            if (emailAddress.Length > MaxEmailLength)
            {
                errorMessage = $"Email address cannot be longer than {MaxEmailLength} characters.";
                return false;
            }

            Regex emailPattern = new Regex(@"^(?![.])([a-zA-Z0-9]+([._%+-][a-zA-Z0-9]+)*)@(?!(?:[-.])|.*[-.]{2,})([a-zA-Z0-9]+(-[a-zA-Z0-9]+)*\.)+[a-zA-Z]{2,}$");

            if (!emailPattern.IsMatch(emailAddress))
            {
                errorMessage = "Email address is not in a valid format (example: user@example.com).";
                return false;
            }

            // // Create if for checking existing username

            return true;
        }

        // First Name validation
        public static bool IsValidFirstName(string firstName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(firstName))
            {
                errorMessage = "Please enter your first name.";
                return false;
            }
            if (firstName.Length < 3 || firstName.Length > 26)
            {
                errorMessage = "Your first name must be greater than 2 characters and less than 26 characters.";
                return false;
            }

            Regex validNamePattern = new Regex(@"^[a-zA-Z]+([ \.\'-]?[a-zA-Z]+)*$");
            if (!validNamePattern.IsMatch(firstName))
            {
                errorMessage = "Your first name can only contain letters, spaces, periods, apostrophes, or hyphens.";
                return false;
            }
            return true;
        }

        // Middle Name validation
        public static bool IsValidMiddleName(string middleName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(middleName))
                return true;

            Regex validNamePattern = new Regex(@"^[a-zA-Z]+(\.?[ \'\-]?[a-zA-Z]+)*\.?$");
            if (!validNamePattern.IsMatch(middleName))
            {
                errorMessage = "Your middle name can only contain letters, spaces, periods, apostrophes, or hyphens.";
                return false;
            }
            return true;
        }

        // Last Name validation
        public static bool IsValidLastName(string lastName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errorMessage = "Please enter your last name.";
                return false;
            }
            if (lastName.Length < 3 || lastName.Length > 32)
            {
                errorMessage = "Your last name must be greater than 2 characters and less than 32 characters.";
                return false;
            }

            Regex validNamePattern = new Regex(@"^[a-zA-Z]+([ \.\'-]?[a-zA-Z]+)*$");
            if (!validNamePattern.IsMatch(lastName))
            {
                errorMessage = "Your last name can only contain letters, spaces, periods, apostrophes, or hyphens.";
                return false;
            }
            return true;
        }

        // City validation
        public static bool IsValidCity(string city, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(city))
            {
                errorMessage = "Please enter your city.";
                return false;
            }
            if (city.Length < 3 || city.Length > 50)
            {
                errorMessage = "Your City name must be greater than 2 characters and less than 50 characters.";
                return false;
            }

            Regex validNamePattern = new Regex("^[a-zA-Z' ]+$");
            if (!validNamePattern.IsMatch(city))
            {
                errorMessage = "No numbers, hyphens, or special characters allowed.";
                return false;
            }

            return true;
        }

        // Street Address validation
        public static bool IsValidStreetAddressOne(string streetAddress, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(streetAddress))
            {
                errorMessage = "Please enter your street address.";
                return false;
            }
            if(streetAddress.Length < 3 || streetAddress.Length > 50)
            {
                errorMessage = "Your street address must be greater than 2 characters and less than 50 characters.";
                return false;
            }
            return true;
        }

        // Address 2 validation                                     
        public static bool IsValidStreetAddressTwo(string addressTwo,  out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(addressTwo))
            {
                return true;
            }
            if (addressTwo.Length < 3 || addressTwo.Length > 50)
            {
                errorMessage = "Your address must be greater than 2 characters and less than 50 characters.";
                return false;
            }
            return true;
        }

        // Address 3 validation                                     
        public static bool IsValidStreetAddressThree(string addressThree, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(addressThree))
            {
                return true;
            }
            if (addressThree.Length < 3 || addressThree.Length > 50)
            {
                errorMessage = "Your address must be greater than 2 characters and less than 50 characters.";
                return false;
            }
            return true;
        }

        // ZipCode validation
        public static bool IsValidZipCode(string zipCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(zipCode))
            {
                errorMessage = "Please enter your ZIP Code.";
                return false;
            }

            zipCode = zipCode.Trim();
            Regex zipCodePattern = new Regex(@"^\d{5}(-\d{4})?$");

            if (!zipCodePattern.IsMatch(zipCode))
            {
                errorMessage = "ZIP Code must be a 5-digit number or in the ZIP+4 format (e.g., 12345 or 12345-1234).";
                return false;
            }
            return true;
        }

        // Primary Phone Number validation
        public static bool IsValidPrimaryPhoneNumber(string primaryPhoneNumber, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(primaryPhoneNumber))
            {
                errorMessage = "Please enter your primary phone number."; 
                return false;
            }

            Regex phonePattern = new Regex(@"^\(\d{3}\) \d{3}-\d{4}$");
            if (!phonePattern.IsMatch(primaryPhoneNumber))
            {
                errorMessage = "Phone number must be in the format (000) 000-0000.";
                return false;
            }
            return true;
        }

        // Secondary Phone Number validation
        public static bool IsValidSecondaryPhoneNumber(string secondaryPhoneNumber, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(secondaryPhoneNumber))
            {
                return true;
            }
            Regex phonePattern = new Regex(@"^\(\d{3}\) \d{3}-\d{4}$");
            if (!phonePattern.IsMatch(secondaryPhoneNumber))
            {
                errorMessage = "Phone number must be in the format (000) 000-0000.";
                return false;
            }
            return true;
        }

        // Security Question 1 validation
        public static bool IsValidSecurityQuestionOne(string selectedQuestionOne, string securityQuestionAnswerOne, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(selectedQuestionOne))
            {
                errorMessage = "Please select your first security question.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(securityQuestionAnswerOne))
            {
                errorMessage = "Please provide an answer for your first security question.";
                return false;
            }
            return true;
        }

        // Security Question 2 validation
        public static bool IsValidSecurityQuestionTwo(string selectedQuestionTwo, string securityQuestionAnswerTwo, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(selectedQuestionTwo))
            {
                errorMessage = "Please select your second security question.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(securityQuestionAnswerTwo))
            {
                errorMessage = "Please provide an answer for your second security question.";
                return false;
            }
            return true;
        }

        // Security Question 3 validation
        public static bool IsValidSecurityQuestionThree(string selectedQuestionThree, string securityQuestionAnswerThree, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(selectedQuestionThree))
            {
                errorMessage = "Please select your third security question.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(securityQuestionAnswerThree))
            {
                errorMessage = "Please provide an answer for your third security question.";
                return false;
            }
            return true;
        }

        // Card number validation
        public static bool IsValidCardNumber(string cardNumber, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                errorMessage = "Your card number is required.";
                return false;
            }
            if (!Regex.IsMatch(cardNumber, @"^\d{16}$"))
            {
                errorMessage = "Card number must be exactly 16 digits.";
                return false;
            }

            return true;
        }

        // Card Expiration validation
        public static bool IsValidExpiration(string expirationDate, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(expirationDate))
            {
                errorMessage = "Please enter your card's expiration date.";
                return false;
            }

            // Match MM/YY or MM/YYYY
            var match = Regex.Match(expirationDate.Trim(), @"^(\d{2})/(\d{2}|\d{4})$");
            if (!match.Success)
            {
                errorMessage = "Expiration date must be in MM/YY or MM/YYYY format.";
                return false;
            }

            int month = int.Parse(match.Groups[1].Value);
            int year = int.Parse(match.Groups[2].Value);

            // Normalize 2-digit year 
            if (year < 100)
            {
                int currentYear = DateTime.Now.Year;
                int century = currentYear / 100 * 100;
                year += century;
            }

            // Check if month is between 1–12 
            if (month < 1 || month > 12)
            {
                errorMessage = "Invalid month. Must be between 01 and 12.";
                return false;
            }

            // Check if date is in the past
            DateTime expiration = new DateTime(year, month, 1).AddMonths(1).AddDays(-1); // end of month
            if (expiration < DateTime.Today)
            {
                errorMessage = "Card is expired.";
                return false;
            }

            // Check if year is more than 5 years in the future
            if (year > DateTime.Now.Year + 5)
            {
                errorMessage = "Expiration year cannot be more than 5 years from now.";
                return false;
            }

            return true;
        }

        // Card CVV validation
        public static bool IsValidCVV(string cvv, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(cvv))
            {
                errorMessage = "Your CVV is required.";
                return false;
            }

            if (!Regex.IsMatch(cvv, @"^\d{3}$"))
            {
                errorMessage = "CVV must be exactly 3 digits.";
                return false;
            }

            return true;
        }

        // Discount entry/edit validation
        public static bool IsValidDiscount(int discountLevel, int discountType, decimal discountPercentage, decimal discountDollarAmount, string inventoryID, DateTime startDate, DateTime expirationDate, out string errorMessage)
        {
            errorMessage = "";

            bool percentSet = discountPercentage > 0;
            bool dollarSet = discountDollarAmount > 0;
            if (percentSet && dollarSet)
            {
                errorMessage = "Only one of discount percentage or dollar amount can be entered.";
                return false;
            }
            if (!percentSet && !dollarSet)
            {
                errorMessage = "Enter either a discount percentage or a discount dollar amount.";
                return false;
            }

            // Validate date range
            if (startDate.Date < DateTime.Now.Date)
            {
                errorMessage = "The start date cannot be in the past.";
                return false;
            }
            if (expirationDate.Date < startDate.Date)
            {
                errorMessage = "The expiration date must be after the start date.";
                return false;
            }

            if (discountLevel == 0 && (discountType != 0 && discountType != 1))
            {
                errorMessage = "Cart-level discounts must use either 'cart level percentage' or 'cart level dollar amount' types.";
                return false;
            }
            if (discountLevel == 1 && (discountType != 2 && discountType != 3))
            {
                errorMessage = "Item-level discounts must use either 'item level percentage' or 'item level dollar amount' types.";
                return false;
            }
            if (discountLevel == 1)
            {
                if (string.IsNullOrWhiteSpace(inventoryID) || !int.TryParse(inventoryID, out int invID) || invID <= 0)
                {
                    errorMessage = "For item-level discounts, you must enter a valid InventoryID.";
                    return false;
                }
            }

            if (discountLevel == 0)
            {
                if (!string.IsNullOrWhiteSpace(inventoryID))
                {
                    errorMessage = "InventoryID should not be entered for cart-level discounts.";
                    return false;
                }
            }

            return true;
        }

        public static bool TryValidateExistingPersonId(string input, out int personId, out string errorMessage)
        {
            personId = 0;
            errorMessage = "";

            string trimmed = (input ?? "").Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                errorMessage = "Please enter a PersonID.";
                return false;
            }

            for (int i = 0; i < trimmed.Length; i++)
            {
                if (!char.IsDigit(trimmed[i]))
                {
                    errorMessage = "PersonID can only contain digits (0-9).";
                    return false;
                }
            }

            int id;
            if (!int.TryParse(trimmed, out id) || id <= 0)
            {
                errorMessage = "PersonID must be a positive whole number.";
                return false;
            }

            try
            {
                var row = clsSQL.GetCustomerById(id);
                if (row == null)
                {
                    errorMessage = "No customer was found with that PersonID.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Unable to validate the customer: " + ex.Message;
                return false;
            }

            personId = id;
            return true;
        }

        //
        public static bool RequiredText(string value, string fieldName, out string error)
        {
            if (!string.IsNullOrWhiteSpace(value)) { error = null; return true; }
            error = $"{fieldName} is required."; return false;
        }

        public static bool RequiredSelection(ComboBox cb, string fieldName, out string error)
        {
            if (cb != null && cb.SelectedIndex >= 0) { error = null; return true; }
            error = $"Please select a {fieldName}."; return false;
        }

        public static bool RequireOneOf(decimal a, decimal b, string nameA, string nameB, out string error)
        {
            bool hasA = a > 0m, hasB = b > 0m;
            if (hasA ^ hasB) { error = null; return true; }
            error = hasA && hasB
                ? $"Enter either {nameA} or {nameB}, not both."
                : $"You must enter either {nameA} or {nameB}.";
            return false;
        }

        public static bool IsValidPercentFraction(decimal pct, out string error)
        {
            if (pct > 0m && pct < 1m) { error = null; return true; }
            error = "Percentage must be between 0 and 100 (or 0.00–0.99 as a fraction).";
            return false;
        }

        public static bool IsPositiveMoney(decimal amt, out string error)
        {
            if (amt > 0m) { error = null; return true; }
            error = "Amount must be greater than 0.00."; return false;
        }

        public static bool ValidDateRange(DateTime start, DateTime end, string nameStart, string nameEnd, out string error)
        {
            if (end >= start) { error = null; return true; }
            error = $"{nameEnd} cannot be earlier than {nameStart}."; return false;
        }

        public static bool IsValidItemName(string name, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Please enter the item name.";
                return false;
            }
            name = name.Trim();
            if (name.Length < 3 || name.Length > 80)
            {
                errorMessage = "Item name must be between 3 and 80 characters.";
                return false;
            }
            // letters, digits, spaces 
            if (!Regex.IsMatch(name, @"^[A-Za-z0-9 ]+$"))
            {
                errorMessage = "Item name can only contain letters, numbers, and spaces (no special characters).";
                return false;
            }
            return true;
        }

        //
        public static bool IsValidItemDescription(string desc, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(desc))
            {
                errorMessage = "Please enter the item description.";
                return false;
            }

            desc = desc.Trim();
            if (desc.Length < 5)
            {
                errorMessage = "Item description must be at least 5 characters.";
                return false;
            }

            return true;
        }

        public static bool IsValidImagePath(string path, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = "Please select an image.";
                return false;
            }

            try
            {
                if (!File.Exists(path))
                {
                    errorMessage = "Selected image file was not found.";
                    return false;
                }
                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (!(ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif"))
                {
                    errorMessage = "Image must be PNG, JPG, JPEG, BMP, or GIF.";
                    return false;
                }
                return true;
            }
            catch
            {
                errorMessage = "Unable to read the selected image.";
                return false;
            }
        }

        public static bool IsValidMoney(decimal value, string fieldName, bool allowZero, out string errorMessage)
        {
            errorMessage = string.Empty;
            if ((!allowZero && value <= 0m) || (allowZero && value < 0m))
            {
                errorMessage = $"{fieldName} must be {(allowZero ? "zero or greater" : "greater than 0.00")}.";
                return false;
            }
            if (value > 1_000_000m)
            {
                errorMessage = $"{fieldName} is too large.";
                return false;
            }
            return true;
        }

        public static bool IsValidQuantity(decimal value, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (value <= 0m)
            {
                errorMessage = "Quantity must be greater than zero.";
                return false;
            }
            if (value != Math.Truncate(value))
            {
                errorMessage = "Quantity must be a whole number.";
                return false;
            }
            if (value > 1_000_000m)
            {
                errorMessage = "Quantity is too large.";
                return false;
            }
            return true;
        }

        public static bool IsValidRestockThreshold(decimal value, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (value <= 0m)
            {
                errorMessage = "Restock Threshold must be greater than zero.";
                return false;
            }
            if (value != Math.Truncate(value))
            {
                errorMessage = "Restock Threshold must be a whole number.";
                return false;
            }
            if (value > 1_000_000m)
            {
                errorMessage = "Restock Threshold is too large.";
                return false;
            }
            return true;
        }

        public static bool IsValidCategory(ComboBox cb, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (cb == null || cb.SelectedIndex < 0)
            {
                errorMessage = "Please choose a category.";
                return false;
            }
            return true;
        }
    }
}