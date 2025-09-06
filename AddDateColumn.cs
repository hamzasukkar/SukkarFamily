using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static void Main()
    {
        string dbPath = @"SukkarFamily\data\FamilyTree.db";
        
        if (!File.Exists(dbPath))
        {
            Console.WriteLine("Database file not found: " + dbPath);
            return;
        }

        try
        {
            using var connection = new SQLiteConnection($"Data Source={dbPath}");
            connection.Open();
            
            // Check if Date column already exists
            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = "PRAGMA table_info(News)";
            
            bool dateColumnExists = false;
            using (var reader = checkCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == "Date")
                    {
                        dateColumnExists = true;
                        break;
                    }
                }
            }
            
            if (dateColumnExists)
            {
                Console.WriteLine("Date column already exists in News table.");
            }
            else
            {
                // Add the Date column
                var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = "ALTER TABLE News ADD COLUMN Date TEXT";
                alterCmd.ExecuteNonQuery();
                
                Console.WriteLine("Successfully added Date column to News table.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}