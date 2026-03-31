using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace ThameJordan25SU233x
{
    internal static class DatabaseInitializer
    {
        // Creates SmolTech.db on first launch; does nothing if it already exists
        public static void EnsureCreated()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmolTech", "SmolTech.db");
            string sqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CreateDatabase_SQLite.sql");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            // Database already exists, nothing to do
            if (File.Exists(dbPath)) return;

            // SQL script must be in the same folder as the .exe
            if (!File.Exists(sqlPath))
            {
                MessageBox.Show("Database schema file not found:\n" + sqlPath, "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string script = File.ReadAllText(sqlPath);

                // Remove single-line comments so they don't interfere with splitting
                script = Regex.Replace(script, @"--[^\r\n]*", "");

                using (var cn = new SqliteConnection($"Data Source={dbPath};"))
                {
                    cn.Open();

                    // Split on semicolons to get individual statements
                    string[] statements = script.Split(';');
                    foreach (string raw in statements)
                    {
                        string statement = raw.Trim();
                        if (string.IsNullOrWhiteSpace(statement)) continue;

                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = statement;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Failed to create local database:\n\n" + ex.Message, "Database Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
