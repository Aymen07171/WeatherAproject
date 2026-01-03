# Weather App with SQLite Search History

A Windows Forms weather application that uses ADO.NET with SQLite to store and display search history.

## Features

- 🌤️ **Weather Search**: Search for weather information by city name
- 🌍 **Multi-language Support**: Supports multiple languages (en, ru, fr, es, ja, kr, zh_cn)
- 💾 **Search History**: Automatically saves all city searches with date/time using SQLite database
- 📊 **History Table**: View search history in a DataGridView table
- 🔄 **Quick Re-search**: Double-click any history entry to search that city again

## Technology Stack

- **.NET 6.0** - Windows Forms Application
- **ADO.NET** - Database access technology
- **SQLite** - Local file-based database (Microsoft.Data.Sqlite)
- **WeatherAPI.com** - Weather data provider

## Database

The application uses **SQLite** with **ADO.NET** to store search history locally. The database file (`weather_search_history.db`) is automatically created in the application directory.

### Database Schema

```sql
CREATE TABLE SearchHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    City TEXT NOT NULL,
    SearchDateTime TEXT NOT NULL
)
```

## ADO.NET Implementation

This project demonstrates ADO.NET patterns:

- **Connection Management**: Using `SqliteConnection` with `using` statements
- **Parameterized Queries**: Preventing SQL injection with `Parameters.AddWithValue()`
- **ExecuteNonQuery**: For INSERT operations
- **ExecuteReader**: For SELECT operations with `DataReader`
- **Type-Safe Data Access**: Using `GetString()` and other typed methods

## Project Structure

```
WeatherApp/
├── Form1.cs                    # Main form with UI logic
├── Form1.Designer.cs           # Form designer code
├── SearchHistoryDatabase.cs    # ADO.NET database operations
├── Program.cs                  # Application entry point
└── WeatherApp.csproj          # Project file with dependencies
```

## Dependencies

- **Newtonsoft.Json** (v13.0.3) - JSON parsing
- **Microsoft.Data.Sqlite** (v8.0.0) - ADO.NET provider for SQLite

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build and run the application

## Usage

1. Enter a city name in the text box
2. Select a language (optional)
3. Click "Get-Weather" button
4. View weather information
5. Check the "Search History" table at the bottom for all previous searches
6. Double-click any history entry to search that city again

## License

This project is for educational purposes.

