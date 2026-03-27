using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ACS_JThameM7;
using System.IO;
using System.Drawing;


namespace ThameJordan25SU233x
{
    public partial class frmReports : Form
    {
        //
        private bool _isInit = false;

        public frmReports()
        {
            InitializeComponent();
            this.Shown += frmReports_Shown;


            //
            this.Load += frmReports_Load;

            btnInvAll.Click += btnInvAll_Click;
            btnInvAvailable.Click += btnInvAvailable_Click;
            btnInvNeedsRestock.Click += btnInvNeedsRestock_Click;


            btnSalesDaily.Click += btnSalesDaily_Click;
            btnSalesWeekly.Click += btnSalesWeekly_Click;
            btnSalesMonthly.Click += btnSalesMonthly_Click;
            btnSalesCustomRange.Click += btnSalesCustomRange_Click;  

            btnSalesViewHTML.Click += btnSalesViewHTML_Click;

            

            // 
            dtpStart.ValueChanged += (s, e) =>
            {
                if (dtpEnd.Value.Date < dtpStart.Value.Date)
                    dtpEnd.Value = dtpStart.Value.Date;
            };

            //
            dtpStart.ValueChanged += (s, e) =>
            {
                if (!_isInit) return;      
                if (dtpEnd.Value.Date < dtpStart.Value.Date)
                    dtpEnd.Value = dtpStart.Value.Date;
            };
        }

        private void frmReports_Load(object sender, EventArgs e)
        {
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;

            if (btnSalesViewHTML != null) btnSalesViewHTML.Visible = false;

            //TryBindInventory(() => clsSQL.ManagerViewInventory(dgvInventory));
            //btnInvAvailable_Click(null, EventArgs.Empty);
        }

        private void frmReports_Shown(object sender, EventArgs e)
        {
            // Force fresh/current dates on open
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;

            //
            if (dtpEnd.Value.Date < dtpStart.Value.Date)
                dtpEnd.Value = dtpStart.Value.Date;

            _isInit = true;
        }

        private (DateTime start, DateTime end) GetRangeFromPickers()
        {
            // Centralize reads so all buttons use the same source
            return (dtpStart.Value.Date, dtpEnd.Value.Date);
        }

        // ----------------------- Inventory buttons -----------------------

        //
        private void FormatMoneyColumnsUSD(DataGridView grid)
        {
            if (grid?.Columns == null) return;

            var usd = CultureInfo.GetCultureInfo("en-US");
            string[] moneyCols = { "GrossTotal", "Total", "TotalDue", "Subtotal", "DiscountAmount", "TaxAmount", "RetailPrice" };

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (moneyCols.Contains(col.Name))
                {
                    col.DefaultCellStyle.Format = "C2";
                    col.DefaultCellStyle.FormatProvider = usd;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        private void BindInventoryFiltered(DataTable fullWithImages, string rowFilter, string printTitle)
        {
            if (fullWithImages == null) return;

            // Filter while the ItemImage is still present
            var view = fullWithImages.DefaultView;
            view.RowFilter = rowFilter ?? string.Empty;

            // Copy with images 
            DataTable filteredWithImages = view.ToTable();

            // Create a version for the DataSource that does not contain raw bytes
            DataTable gridData = filteredWithImages.Copy();
            if (gridData.Columns.Contains("ItemImage"))
                gridData.Columns.Remove("ItemImage");

            // Rebuild columns
            dgvInventory.Columns.Clear();
            var imgCol = new DataGridViewImageColumn
            {
                Name = "ProductImage",
                HeaderText = "Product Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dgvInventory.Columns.Add(imgCol);

            // Bind data rows
            dgvInventory.DataSource = gridData;

            //
            for (int i = 0; i < dgvInventory.Rows.Count; i++)
            {
                var bytesObj = filteredWithImages.Rows[i]["ItemImage"];
                if (bytesObj != DBNull.Value && bytesObj is byte[] bytes && bytes.Length > 0)
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        dgvInventory.Rows[i].Cells["ProductImage"].Value = Image.FromStream(ms);
                    }
                }
            }

            void SetHeader(string col, string header, int? width = null)
            {
                if (!dgvInventory.Columns.Contains(col)) return;
                dgvInventory.Columns[col].HeaderText = header;
                if (width.HasValue) dgvInventory.Columns[col].Width = width.Value;
            }

            SetHeader("ItemName", "Product Name", 140);
            SetHeader("ItemDescription", "Description", 260);
            SetHeader("RetailPrice", "Price", 80);
            SetHeader("Quantity", "Stock Left", 80);
            SetHeader("RestockThreshold", "Restock Threshold", 110);
            SetHeader("Discontinued", "Discontinued", 90);

            // Hide tech columns if they exist
            if (dgvInventory.Columns.Contains("InventoryID")) dgvInventory.Columns["InventoryID"].Visible = false;

            // Formatting
            dgvInventory.Columns["RetailPrice"].DefaultCellStyle.Format = "C2";
            dgvInventory.Columns["RetailPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvInventory.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInventory.RowTemplate.Height = 65;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvInventory.ClearSelection();

            // Print exactly what is shown 
            clsHTML.ShowInventoryHtml(gridData, printTitle);
        }


        // All inventory (including discontinued)
        private void btnInvAll_Click(object sender, EventArgs e)
        {
            try
            {
                // 
                DataTable full = clsSQL.ManagerViewInventory(dgvInventory);
                if (full == null) return;

                // No filter for "All"
                BindInventoryFiltered(full, null, "All Inventory");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print All Inventory:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Available inventory
        private void btnInvAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable full = clsSQL.ManagerViewInventory(dgvInventory);
                if (full == null) return;

                BindInventoryFiltered(full, "Discontinued = False AND Quantity > 0", "Available Inventory");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print Available Inventory:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnInvNeedsRestock_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable full = clsSQL.ManagerViewInventory(dgvInventory);
                if (full == null) return;

                BindInventoryFiltered(full, "Discontinued = False AND Quantity <= RestockThreshold", "Needs Restock");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print Needs Restock:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btnInvViewHTML_Click(object sender, EventArgs e)
        {
            //
            var dt = dgvInventory.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                clsSQL.ManagerViewInventory(dgvInventory);
                dt = dgvInventory.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("There is no inventory data to print.", "Nothing to Print",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // Calls the HTML helper 
            clsHTML.ShowInventoryHtml(dgvInventory, "Inventory Report");
        }

        // Small helper t0 catch/notify 
        private void TryBindInventory(Func<DataTable> loader)
        {
            try
            {
                var _ = loader?.Invoke();
                dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load inventory:\n\n" + ex.Message, "Inventory",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ----------------------- Sales buttons -----------------------

        private void btnSalesDaily_Click(object sender, EventArgs e)
        {
            var (s, _) = GetRangeFromPickers();
            BindSalesAndPreview(s, s, "Daily Sales Totals");
        }

        private void btnSalesWeekly_Click(object sender, EventArgs e)
        {
            // Last 7 full days
            DateTime today = DateTime.Today;
            DateTime start = today.AddDays(-7);    
            DateTime end = today.AddTicks(-1);   

            BindSalesAndPreview(start, end, "Weekly Sales Totals");
        }

        private void btnSalesMonthly_Click(object sender, EventArgs e)
        {
            // Previous full calendar month
            DateTime today = DateTime.Today;
            DateTime firstOfCurrent = new DateTime(today.Year, today.Month, 1);
            DateTime start = firstOfCurrent.AddMonths(-1);
            DateTime end = firstOfCurrent.AddTicks(-1); 

            BindSalesAndPreview(start, end, "Monthly Sales Totals");
        }


        private void btnSalesCustomRange_Click(object sender, EventArgs e)
        {
            var (start, end) = GetRangeFromPickers();
            if (end < start)
            {
                MessageBox.Show("End date cannot be before start date.", "Invalid Range",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BindSalesAndPreview(start, end, "Sales Totals (Custom Range)");
        }


        private void btnSalesViewHTML_Click(object sender, EventArgs e)
        {
            btnSalesCustomRange_Click(sender, e);
        }


        private void BindSalesAndPreview(DateTime start, DateTime end, string title)
        {
            try
            {
                DataTable totals = clsSQL.GetSalesTotals(start, end);
                if (totals == null || totals.Rows.Count == 0)
                {
                    dgvSales.DataSource = null;
                    MessageBox.Show("No sales were found for the selected range.", "No Results",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    dgvSales.DataSource = totals;
                    dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Format currency
                    FormatMoneyColumnsUSD(dgvSales);

                    // Format ItemsSold as whole numbers 
                    if (dgvSales.Columns.Contains("ItemsSold"))
                    {
                        dgvSales.Columns["ItemsSold"].DefaultCellStyle.Format = "N0"; 
                        dgvSales.Columns["ItemsSold"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                // Open printable HTML
                clsHTML.ShowSalesTotalsHtml(start, end, title);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load sales totals:\n\n" + ex.Message, "Sales Reports",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Help button
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "View inventory and sales reports here.\n\n" +
                "Inventory Reports (top):\n" +
                "• **Available Inventory** – Show items with stock > 0 (excludes discontinued).\n" +
                "• **Restock Soon** – Show items at or below their Restock Threshold (not discontinued).\n" +
                "• **All Inventory** – Show everything, including discontinued.\n\n" +
                "Sales Reports (bottom):\n" +
                "• **Daily Sales** – Sets the last 7 full days before today.\n" +
                "• **Weekly Sales** – Sets the last 7 full days before today.\n" +
                "• **Monthly Sales** – Sets the previous full calendar month.\n" +
                "• **Custom Range** – Choose **Start** and **End** dates, then click **Custom Range**.\n" +
                "• **View Sales Report** – Generate the sales report for the selected range.\n\n" +
                "Tips:\n" +
                "- Adjust the date pickers if you need a different period.\n" +
                "- Large result sets may take a moment to load.\n" +
                "- Use inventory buttons to print or preview inventory lists when available.",
                "Sales & Inventory Reports – Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Needs Restock 
        private void btnInvNeedsRestock_Click_1(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = clsSQL.ManagerViewInventory(dgvInventory);
                if (dt == null) return;

                var view = dt.DefaultView;
                view.RowFilter = "Quantity < RestockThreshold AND (Discontinued = False OR Discontinued IS NULL)";
                dgvInventory.DataSource = view.ToTable();

                dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load 'Needs Restock' view:\n\n" + ex.Message,
                                "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Print whatever inventory view is currently shown
        private void btnInViewHMTL_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure there’s data to print
                var dt = dgvInventory.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = clsSQL.ManagerViewInventory(dgvInventory);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        MessageBox.Show("There is no inventory data to print.", "Nothing to Print",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                clsHTML.ShowInventoryHtml(dgvInventory, "Inventory Report");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to generate inventory report:\n\n" + ex.Message,
                                "Inventory HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}