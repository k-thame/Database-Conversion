using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace ThameJordan25SU233x
{
    internal static class DatabaseInitializer
    {
        // Creates SmolTech.db on first launch; does nothing if it already exists
        public static void EnsureCreated()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmolTech.db");
            string sqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CreateDatabase_SQLite.sql");

            // Database already exists, nothing to do
            if (File.Exists(dbPath)) return;

            // SQL script must be in the same folder as the .exe
            if (!File.Exists(sqlPath))
            {
                MessageBox.Show("Database schema file not found:\n" + sqlPath + "\n\nPlease ensure CreateDatabase_SQLite.sql is in the application folder.", "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string script = File.ReadAllText(sqlPath);

                using (var cn = new SqliteConnection($"Data Source={dbPath};"))
                {
                    cn.Open();

                    // Turn on foreign key support
                    using (var pragma = cn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA foreign_keys = ON;";
                        pragma.ExecuteNonQuery();
                    }

                    // Run each statement from the script one at a time
                    foreach (string raw in script.Split(';'))
                    {
                        string statement = raw.Trim();
                        if (string.IsNullOrWhiteSpace(statement) || statement.StartsWith("--")) continue;
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
