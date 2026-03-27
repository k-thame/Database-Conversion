using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThameJordan25SU233x
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the local SQLite database on first run (skipped if it already exists)
            DatabaseInitializer.EnsureCreated();

            // Handle user sign in
            using (frmLogon loginForm = new frmLogon())
            {
                // Only open the shopping form if login was successful
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string personID = loginForm.PersonID;
                    Application.Run(new frmShopping(personID));
                }
            }
        }
    }
}