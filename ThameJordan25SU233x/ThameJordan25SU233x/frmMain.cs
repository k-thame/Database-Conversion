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
    public partial class frmMain : Form
    {
        // Form constructor
        public frmMain(string positionTitle)
        {
            InitializeComponent();
        }

        // Form load event
        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        // Transfer user to shopping form 
        private void tsbShop_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //frmShopping letsGoShopping = new frmShopping();
            //letsGoShopping.ShowDialog();
            //this.Close();
        }
    }
}