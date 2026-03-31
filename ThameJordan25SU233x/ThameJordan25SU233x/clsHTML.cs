using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACS_JThameM7;

namespace ThameJordan25SU233x
{
    internal class clsHTML
    {
        public static void PrintReport(clsCart cart, string customerPersonID, bool isPOSSale = false, string managerFullName = "")
        {
            try
            {
                var person = clsSQL.GetPersonDetails(customerPersonID);
                if (person != null && person.Rows.Count > 0)
                {
                    cart.CustomerName = $"{Convert.ToString(person.Rows[0]["NameFirst"])} {Convert.ToString(person.Rows[0]["NameLast"])}".Trim();
                    cart.PhoneNumber = Convert.ToString(person.Rows[0]["PhonePrimary"]);
                }
            }
            catch 
            { 

            }

            string html = BuildReceiptHtml(cart, customerPersonID, isPOSSale, managerFullName);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"SmolTech");
            Directory.CreateDirectory(path);
            string file = Path.Combine(path, $"Receipt_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            File.WriteAllText(file, html, Encoding.UTF8);
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }

        private static string BuildReceiptHtml(clsCart cart, string customerPersonID, bool isPOSSale, string managerFullName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'>");
            sb.AppendLine("<title>Receipt / Invoice</title>");
            sb.AppendLine(@"<style>
                body{font-family:Segoe UI,Tahoma,Arial,sans-serif;background:#f4f4f4;margin:0;padding:24px;}
                .wrap{max-width:860px;margin:0 auto;background:#fff;border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,.08);padding:20px;}
                h1{margin:0 0 8px 0;font-size:22px;color:#222;}
                .meta,.muted{color:#555;margin:0 0 10px 0;}
                .muted{font-size:12px}
                .grid{display:grid;grid-template-columns:1fr 1fr;gap:16px;margin:10px 0 16px 0;}
                table{border-collapse:collapse;width:100%;}
                th,td{border:1px solid #ddd;padding:8px;font-size:13px;text-align:left;}
                th{background:#f5f5f5;}
                .right{text-align:right;}
                .print{margin:14px 0 18px 0;}
                .print button{padding:6px 10px;cursor:pointer;}
                .totals{width:360px;margin-top:12px;margin-left:auto;}
                .totals th,.totals td{font-size:14px}
            </style>");
            sb.AppendLine("</head><body>");
            sb.AppendLine("<div class='wrap'>");

            sb.AppendLine("<h1>Receipt / Invoice</h1>");
            sb.AppendLine($"<div class='muted'>Date: {DateTime.Now:yyyy-MM-dd HH:mm}</div>");

            sb.AppendLine("<div class='grid'>");


            sb.AppendLine("<div>");
            sb.AppendLine("<div class='meta'><strong>Customer</strong></div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Name</th><th>Phone</th><th>Customer ID</th></tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(cart.CustomerName ?? "")}</td>");
            sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(cart.PhoneNumber ?? "")}</td>");
            sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(customerPersonID)}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div>");
            if (isPOSSale)
            {
                var display = string.IsNullOrWhiteSpace(managerFullName) ? "(manager)" : managerFullName.Trim();
                sb.AppendLine("<div class='meta'><strong>Point of Sale</strong></div>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Order conducted by</th></tr>");
                sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(display)}</td></tr>");
                sb.AppendLine("</table>");
            }
            else
            {
                sb.AppendLine("<div class='meta'><strong>Point of Sale</strong></div>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Order conducted by</th></tr>");
                sb.AppendLine("<tr><td>Customer self-checkout</td></tr>");
                sb.AppendLine("</table>");
            }
            sb.AppendLine("</div>");

            sb.AppendLine("</div>");

            sb.AppendLine("<div class='print'><button onclick='window.print()'>Print</button></div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<thead><tr><th>Item</th><th class='right'>Item Price</th><th class='right'>Quantity</th><th class='right'>Line Total</th></tr></thead>");
            sb.AppendLine("<tbody>");
            foreach (var it in cart.CartItems ?? new List<clsCartItem>())
            {
                var line = it.Price * it.Quantity;
                sb.AppendLine("<tr>"
                    + $"<td>{System.Net.WebUtility.HtmlEncode(it.ItemName)}</td>"
                    + $"<td class='right'>{it.Price:C}</td>"
                    + $"<td class='right'>{it.Quantity}</td>"
                    + $"<td class='right'>{line:C}</td>"
                    + "</tr>");
            }
            sb.AppendLine("</tbody></table>");


            if (!string.IsNullOrWhiteSpace(cart.DiscountCode) && (cart.ItemDiscounts?.Any() == true || cart.DiscountAmount > 0))
            {
                sb.AppendLine("<h2 style='margin-top:18px;font-size:18px'>Discount Summary</h2>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Code</th><th>Item</th><th>Type</th><th class='right'>Amount Saved</th></tr>");

                decimal totalSaved = 0m;

                if (cart.ItemDiscounts != null && cart.ItemDiscounts.Count > 0)
                {
                    foreach (var item in cart.CartItems ?? new List<clsCartItem>())
                    {
                        var d = cart.ItemDiscounts.FirstOrDefault(x => x.ItemID == item.ItemID);
                        if (d == null) continue;

                        decimal saved = 0m;
                        string type = "";

                        if (d.DiscountPercentage > 0)
                        {
                            saved = Math.Round(item.TotalPrice * d.DiscountPercentage, 2);
                            type = $"{d.DiscountPercentage:P0} Off";
                        }
                        else if (d.DiscountDollarAmount > 0)
                        {
                            saved = d.DiscountDollarAmount * item.Quantity;
                            type = $"${d.DiscountDollarAmount:F2} Off Each";
                        }

                        totalSaved += saved;
                        sb.AppendLine("<tr>"
                            + $"<td>{System.Net.WebUtility.HtmlEncode(cart.DiscountCode)}</td>"
                            + $"<td>{System.Net.WebUtility.HtmlEncode(item.ItemName)}</td>"
                            + $"<td>{System.Net.WebUtility.HtmlEncode(type)}</td>"
                            + $"<td class='right'>{saved:C}</td>"
                            + "</tr>");
                    }
                }

                decimal itemLevelTotal = cart.ItemDiscounts?.Sum(d =>
                {
                    var item = (cart.CartItems ?? new List<clsCartItem>()).FirstOrDefault(i => i.ItemID == d.ItemID);
                    if (item == null) return 0m;
                    return d.DiscountPercentage > 0
                        ? Math.Round(item.TotalPrice * d.DiscountPercentage, 2)
                        : d.DiscountDollarAmount * item.Quantity;
                }) ?? 0m;

                decimal cartOnly = cart.DiscountAmount - itemLevelTotal;
                if (cartOnly > 0)
                {
                    totalSaved += cartOnly;
                    sb.AppendLine("<tr>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(cart.DiscountCode)}</td>"
                        + "<td>Entire Cart</td><td>Cart-Level Discount</td>"
                        + $"<td class='right'>{cartOnly:C}</td>"
                        + "</tr>");
                }

                sb.AppendLine("<tr><td colspan='3' class='right'><strong>Total Saved:</strong></td>"
                    + $"<td class='right'><strong>{totalSaved:C}</strong></td></tr>");
                sb.AppendLine("</table>");
            }

            sb.AppendLine("<table class='totals'>");
            var discountedSubtotal = cart.DiscountedSubtotal != 0 ? cart.DiscountedSubtotal : cart.Subtotal;
            sb.AppendLine($"<tr><td>Subtotal</td><td class='right'>{discountedSubtotal:C}</td></tr>");
            if (!string.IsNullOrWhiteSpace(cart.DiscountCode) && cart.DiscountAmount > 0)
                sb.AppendLine($"<tr><td>Discount ({System.Net.WebUtility.HtmlEncode(cart.DiscountCode)})</td><td class='right' style='color:green'>-{cart.DiscountAmount:C}</td></tr>");
            sb.AppendLine($"<tr><td>Tax</td><td class='right'>{cart.TaxAmount:C}</td></tr>");
            sb.AppendLine($"<tr><th>Total Due</th><th class='right'>{cart.TotalDue:C}</th></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<p class='muted' style='margin-top:24px'>Thank you for your business!</p>");
            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        public static void ShowCustomerHistoryHtml(int personId)
        {
            var cust = clsSQL.GetCustomerById(personId);
            string first = cust != null ? Convert.ToString(cust["NameFirst"] ?? "").Trim() : "";
            string last = cust != null ? Convert.ToString(cust["NameLast"] ?? "").Trim() : "";
            string phone = cust != null ? Convert.ToString(cust["PhonePrimary"] ?? "").Trim() : "";
            string customerName = ((first + " " + last).Trim().Length > 0) ? (first + " " + last).Trim()
                                     : $"PersonID {personId}";
            var orders = clsSQL.GetOrdersForCustomer(personId);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
            sb.AppendLine("<title>Customer History</title>");
            sb.AppendLine(@"<style>
                body{font-family:Segoe UI,Tahoma,Arial,sans-serif;margin:24px;}
                h1{font-size:20px;margin:0 0 6px 0;}
                .sub{color:#555;margin:0 0 18px 0;}
                table{border-collapse:collapse;width:100%;}
                th,td{border:1px solid #ddd;padding:8px;text-align:left;}
                th{background:#f5f5f5;}
                tr:nth-child(even){background:#fafafa;}
                .empty{color:#777;font-style:italic;margin-top:10px;}
                .print{margin:12px 0 18px 0;}
                .print button{padding:6px 10px;cursor:pointer;}
                .right{text-align:right;}
            </style></head><body>");

            sb.AppendLine("<h1>Customer History</h1>");
            sb.AppendLine($"<div class='sub'>{System.Net.WebUtility.HtmlEncode(customerName)}"
                         + (string.IsNullOrEmpty(phone) ? "" : $" &nbsp;•&nbsp; {System.Net.WebUtility.HtmlEncode(phone)}")
                         + $" &nbsp;•&nbsp; ID {personId}</div>");

            sb.AppendLine("<div class='print'><button onclick='window.print()'>Print</button></div>");

            if (orders == null || orders.Rows.Count == 0)
            {
                sb.AppendLine("<div class='empty'>No past orders found for this customer.</div>");
            }
            else
            {
                sb.AppendLine("<table><thead><tr>"
                    + "<th>Order #</th>"
                    + "<th>Order Date</th>"
                    + "<th class='right'>Total</th>"
                    + "<th>CC Number</th>"
                    + "<th>CCV</th>"
                    + "<th>Exp Date</th>"
                    + "</tr></thead><tbody>");

                foreach (DataRow r in orders.Rows)
                {
                    string oid = Convert.ToString(r["OrderID"]);
                    string dt = "";
                    if (orders.Columns.Contains("OrderDate") && r["OrderDate"] != DBNull.Value)
                    {
                        if (DateTime.TryParse(Convert.ToString(r["OrderDate"]), out DateTime parsed))
                            dt = parsed.ToString("yyyy-MM-dd HH:mm");
                        else
                            dt = Convert.ToString(r["OrderDate"]);
                    }

                    string total = "";
                    if (orders.Columns.Contains("TotalDue") && r["TotalDue"] != DBNull.Value)
                        total = Convert.ToDecimal(r["TotalDue"]).ToString("C");

                    string cc = orders.Columns.Contains("CC_Number") ? Convert.ToString(r["CC_Number"] ?? "") : "";
                    string ccv = orders.Columns.Contains("CCV") ? Convert.ToString(r["CCV"] ?? "") : "";
                    string exp = orders.Columns.Contains("ExpDate") ? Convert.ToString(r["ExpDate"] ?? "") : "";

                    sb.AppendLine("<tr>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(oid)}</td>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(dt)}</td>"
                        + $"<td class='right'>{System.Net.WebUtility.HtmlEncode(total)}</td>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(cc)}</td>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(ccv)}</td>"
                        + $"<td>{System.Net.WebUtility.HtmlEncode(exp)}</td>"
                        + "</tr>");
                }
                sb.AppendLine("</tbody></table>");
            }

            sb.AppendLine("</body></html>");

            string file = Path.Combine(Path.GetTempPath(), $"CustomerHistory_{personId}_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(file, sb.ToString(), Encoding.UTF8);
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }

        public static void ShowSalesTotalsHtml(DateTime start, DateTime end, string title)
        {
            var dt = clsSQL.GetSalesTotals(start, end);

            decimal orders = 0, items = 0, gross = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                if (r["OrdersCount"] != DBNull.Value) orders = Convert.ToDecimal(r["OrdersCount"]);
                if (r["ItemsCount"] != DBNull.Value) items = Convert.ToDecimal(r["ItemsCount"]);
                if (r["GrossTotal"] != DBNull.Value) gross = Convert.ToDecimal(r["GrossTotal"]);
            }

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
            sb.AppendLine($"<title>{title}</title>");
            sb.AppendLine(@"<style>
                body{font-family:Segoe UI,Tahoma,Arial,sans-serif;margin:24px;}
                h1{font-size:22px;margin:0 0 8px 0;}
                .range{color:#555;margin:0 0 18px 0;}
                table{border-collapse:collapse;width:420px;}
                th,td{border:1px solid #ddd;padding:8px;text-align:left;}
                th{background:#f5f5f5;}
                .print{margin:12px 0 18px 0;}
                .print button{padding:6px 10px;cursor:pointer;}
            </style></head><body>");
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(title)}</h1>");
            sb.AppendLine($"<div class='range'>{start:yyyy-MM-dd} to {end:yyyy-MM-dd}</div>");
            sb.AppendLine("<div class='print'><button onclick='window.print()'>Print</button></div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Metric</th><th>Value</th></tr>");
            sb.AppendLine($"<tr><td>Total Orders</td><td>{orders:0}</td></tr>");
            sb.AppendLine($"<tr><td>Total Items Sold</td><td>{items:0}</td></tr>");
            sb.AppendLine($"<tr><td>Gross Sales</td><td>{gross:C}</td></tr>");
            sb.AppendLine("</table></body></html>");

            string file = Path.Combine(Path.GetTempPath(), $"SalesTotals_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(file, sb.ToString(), Encoding.UTF8);
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }

        public static void ShowInventoryHtml(DataGridView grid, string title)
        {
            var dt = grid?.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("There is no inventory data to print.", "Nothing to Print",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowInventoryHtml(dt, title);
        }

        public static void ShowInventoryHtml(DataTable dt, string title)
        {
            string[] preferred =
            {
                "InventoryID","ItemName","CategoryID","RetailPrice","Quantity","RestockThreshold","Discontinued"
            };

            var cols = preferred.Where(c => dt.Columns.Contains(c)).ToList();
            if (cols.Count == 0) cols = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
            sb.AppendLine($"<title>{System.Net.WebUtility.HtmlEncode(title)}</title>");
            sb.AppendLine(@"<style>
                body{font-family:Segoe UI,Tahoma,Arial,sans-serif;margin:24px;}
                h1{font-size:22px;margin:0 0 8px 0;}
                .sub{color:#555;margin:0 0 18px 0;}
                table{border-collapse:collapse;width:100%;}
                th,td{border:1px solid #ddd;padding:8px;text-align:left;font-size:12px;}
                th{background:#f5f5f5;}
                tr:nth-child(even){background:#fafafa;}
                .print{margin:12px 0 18px 0;}
                .print button{padding:6px 10px;cursor:pointer;}
            </style></head><body>");

            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(title)}</h1>");
            sb.AppendLine($"<div class='sub'>Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</div>");
            sb.AppendLine("<div class='print'><button onclick='window.print()'>Print</button></div>");

            sb.AppendLine("<table><thead><tr>");
            foreach (var c in cols) sb.AppendLine($"<th>{System.Net.WebUtility.HtmlEncode(c)}</th>");
            sb.AppendLine("</tr></thead><tbody>");

            foreach (DataRow r in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (var c in cols)
                {
                    object v = r[c];
                    string cell;
                    if (string.Equals(c, "RetailPrice", StringComparison.OrdinalIgnoreCase) && v != DBNull.Value)
                        cell = Convert.ToString(v);
                    else if (v == DBNull.Value) cell = "";
                    else cell = Convert.ToString(v);
                    sb.Append($"<td>{System.Net.WebUtility.HtmlEncode(cell)}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table></body></html>");

            string file = Path.Combine(Path.GetTempPath(), $"Inventory_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(file, sb.ToString(), Encoding.UTF8);
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }
    }
}