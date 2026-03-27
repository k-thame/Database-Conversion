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
        private bool _isInit = false;

        public frmReports()
        {
            InitializeComponent();
            this.Shown += frmReports_Shown;
            this.Load += frmReports_Load;

            btnInvAll.Click += btnInvAll_Click;
            btnInvAvailable.Click += btnInvAvailable_Click;
            btnInvNeedsRestock.Click += btnInvNeedsRestock_Click;

            btnSalesDaily.Click += btnSalesDaily_Click;
            btnSalesWeekly.Click += btnSalesWeekly_Click;
            btnSalesMonthly.Click += btnSalesMonthly_Click;
            btnSalesCustomRange.Click += btnSalesCustomRange_Click;
            btnSalesViewHTML.Click += btnSalesViewHTML_Click;

            // Keep end date from going before start date
            dtpStart.ValueChanged += (s, e) =>
            {
                if (dtpEnd.Value.Date < dtpStart.Value.Date)
                    dtpEnd.Value = dtpStart.Value.Date;
            };

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
        }

        private void frmReports_Shown(object sender, EventArgs e)
        {
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            if (dtpEnd.Value.Date < dtpStart.Value.Date)
                dtpEnd.Value = dtpStart.Value.Date;
            _isInit = true;
        }

        // Returns the selected date range from the pickers
        private (DateTime start, DateTime end) GetRangeFromPickers()
        {
            return (dtpStart.Value.Date, dtpEnd.Value.Date);
        }

        // Formats money columns in a grid as USD currency
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

        // Filters and displays inventory in the grid then prints HTML
        private void BindInventoryFiltered(DataTable full, string rowFilter, string printTitle)
        {
            if (full == null) return;

            // Apply row filter
            var view = full.DefaultView;
            view.RowFilter = rowFilter ?? string.Empty;
            DataTable gridData = view.ToTable();

            // Rebuild grid columns
            dgvInventory.Columns.Clear();
            dgvInventory.Columns.Add(new DataGridViewImageColumn
            {
                Name = "ProductImage",
                HeaderText = "Product Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            });

            dgvInventory.DataSource = gridData;

            // Load images separately by InventoryID
            using (var cn = clsSQL.GetOpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT InventoryID, ItemImage FROM Inventory WHERE ItemImage IS NOT NULL";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int invID = r.GetInt32(0);
                        byte[] bytes = (byte[])r[1];
                        if (bytes == null || bytes.Length == 0) continue;

                        foreach (DataGridViewRow row in dgvInventory.Rows)
                        {
                            if (row.Cells["InventoryID"] != null &&
                                Convert.ToInt32(row.Cells["InventoryID"].Value) == invID)
                            {
                                using (var ms = new MemoryStream(bytes))
                                    row.Cells["ProductImage"].Value = Image.FromStream(ms);
                                break;
                            }
                        }
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

            if (dgvInventory.Columns.Contains("InventoryID"))
                dgvInventory.Columns["InventoryID"].Visible = false;

            if (dgvInventory.Columns.Contains("RetailPrice"))
                dgvInventory.Columns["RetailPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvInventory.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInventory.RowTemplate.Height = 65;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventory.ClearSelection();

            clsHTML.ShowInventoryHtml(gridData, printTitle);
        }

        // Load all inventory
        private void btnInvAll_Click(object sender, EventArgs e)
        {
            try
            {
                BindInventoryDirect(clsSQL.GetInventoryAll(), "All Inventory");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print All Inventory:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load available inventory only
        private void btnInvAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                BindInventoryDirect(clsSQL.GetInventoryAvailable(), "Available Inventory");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print Available Inventory:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load items needing restock
        private void btnInvNeedsRestock_Click(object sender, EventArgs e)
        {
            try
            {
                BindInventoryDirect(clsSQL.GetInventoryNeedingRestock(), "Needs Restock");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load/print Needs Restock:\n\n" + ex.Message,
                    "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Binds pre-filtered inventory data directly without row filtering
        private void BindInventoryDirect(DataTable dt, string printTitle)
        {
            if (dt == null) return;

            dgvInventory.Columns.Clear();
            dgvInventory.Columns.Add(new DataGridViewImageColumn
            {
                Name = "ProductImage",
                HeaderText = "Product Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            });

            dgvInventory.DataSource = dt;

            // Load images into grid
            if (dt.Columns.Contains("ItemImage"))
            {
                foreach (DataGridViewRow row in dgvInventory.Rows)
                {
                    int invID = Convert.ToInt32(row.Cells["InventoryID"].Value);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToInt32(dr["InventoryID"]) == invID && dr["ItemImage"] != DBNull.Value)
                        {
                            byte[] bytes = dr["ItemImage"] as byte[];
                            if (bytes != null)
                                using (var ms = new MemoryStream(bytes))
                                    row.Cells["ProductImage"].Value = Image.FromStream(ms);
                            break;
                        }
                    }
                }
                dgvInventory.Columns["ItemImage"].Visible = false;
            }

            if (dgvInventory.Columns.Contains("InventoryID")) dgvInventory.Columns["InventoryID"].Visible = false;

            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInventory.RowTemplate.Height = 65;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventory.ClearSelection();

            clsHTML.ShowInventoryHtml(dt, printTitle);
        }

        private void btnInvNeedsRestock_Click_1(object sender, EventArgs e)
        {
            btnInvNeedsRestock_Click(sender, e);
        }

        private void btnInvViewHTML_Click(object sender, EventArgs e)
        {
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
            clsHTML.ShowInventoryHtml(dgvInventory, "Inventory Report");
        }

        // Small helper to catch and notify on inventory load errors
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

        // Daily sales button
        private void btnSalesDaily_Click(object sender, EventArgs e)
        {
            var (s, _) = GetRangeFromPickers();
            BindSalesAndPreview(s, s, "Daily Sales Totals");
        }

        // Weekly sales button
        private void btnSalesWeekly_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            BindSalesAndPreview(today.AddDays(-7), today.AddTicks(-1), "Weekly Sales Totals");
        }

        // Monthly sales button
        private void btnSalesMonthly_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            DateTime firstOfCurrent = new DateTime(today.Year, today.Month, 1);
            BindSalesAndPreview(firstOfCurrent.AddMonths(-1), firstOfCurrent.AddTicks(-1), "Monthly Sales Totals");
        }

        // Custom range sales button
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

        // Loads sales data into the grid and opens HTML preview
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
                    FormatMoneyColumnsUSD(dgvSales);

                    if (dgvSales.Columns.Contains("ItemsSold"))
                    {
                        dgvSales.Columns["ItemsSold"].DefaultCellStyle.Format = "N0";
                        dgvSales.Columns["ItemsSold"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                clsHTML.ShowSalesTotalsHtml(start, end, title);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load sales totals:\n\n" + ex.Message, "Sales Reports",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInViewHMTL_Click(object sender, EventArgs e)
        {
            try
            {
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