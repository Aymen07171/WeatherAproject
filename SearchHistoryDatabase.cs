using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;

namespace WeatherApp
{
    public class SearchHistoryDatabase
    {
        private readonly string _connectionString;

        public SearchHistoryDatabase()
        {
            // Create database file in the application directory
            string dbPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "weather_search_history.db"
            );
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        /// Create Database (CREATE TABLE)
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SearchHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        City TEXT NOT NULL,
                        SearchDateTime TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }
        }

        /// Write Data to Database (INSERT)
        public void SaveSearch(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO SearchHistory (City, SearchDateTime)
                    VALUES (@city, @dateTime)";
                command.Parameters.AddWithValue("@city", city.Trim());
                command.Parameters.AddWithValue("@dateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
        }
        /// Read Data from Database (SELECT)
        public List<SearchHistoryItem> GetRecentSearches(int limit = 10)
        {
            var searches = new List<SearchHistoryItem>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT City, SearchDateTime
                    FROM SearchHistory
                    ORDER BY SearchDateTime DESC
                    LIMIT @limit";
                command.Parameters.AddWithValue("@limit", limit);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        searches.Add(new SearchHistoryItem
                        {
                            City = reader.GetString("City"),
                            SearchDateTime = DateTime.Parse(reader.GetString("SearchDateTime"))
                        });
                    }
                }
            }

            return searches;
        }
    }

    public class SearchHistoryItem
    {
        public string City { get; set; } = string.Empty;
        public DateTime SearchDateTime { get; set; }
    }
}

