using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    public partial class MainForm : Form
    {
        private const string ApiKey = "97f7c6e0a26e4c3ca4c134509253012";
        private const string ApiBaseUrl = "https://api.weatherapi.com/v1/current.json";

        //Connect Database to Your Form
        private readonly SearchHistoryDatabase _searchHistoryDb;

        public MainForm()
        {
            InitializeComponent();

            // Initialize database
            _searchHistoryDb = new SearchHistoryDatabase();

            // language combo box components
            cboLanguage.Items.AddRange(new string[] { "en", "ru", "fr", "es", "ja", "kr", "zh_cn" });
            cboLanguage.SelectedIndex = 0; // default

            // Initialize DataGridView columns
            InitializeSearchHistoryTable();

            // Load recent searches
            LoadRecentSearches();
        }

        private void InitializeSearchHistoryTable()
        {
            // Clear existing columns
            dgvSearchHistory.Columns.Clear();

            // Add City column
            DataGridViewTextBoxColumn cityColumn = new DataGridViewTextBoxColumn();
            cityColumn.Name = "City";
            cityColumn.HeaderText = "City";
            cityColumn.ReadOnly = true;
            cityColumn.Width = 200;
            dgvSearchHistory.Columns.Add(cityColumn);

            // Add Date/Time column
            DataGridViewTextBoxColumn dateTimeColumn = new DataGridViewTextBoxColumn();
            dateTimeColumn.Name = "DateTime";
            dateTimeColumn.HeaderText = "Date & Time";
            dateTimeColumn.ReadOnly = true;
            dateTimeColumn.Width = 140;
            dgvSearchHistory.Columns.Add(dateTimeColumn);
        }

        private async void btnGetWeather_Click(object sender, EventArgs e)
        {
            string city = txtCity.Text.Trim();
            string language = cboLanguage.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(city))
            {
                // Build the correct URL with WeatherAPI parameters
                string apiUrl = $"{ApiBaseUrl}?key={ApiKey}&q={city}&lang={language}&aqi=no";

                try
                {
                    string jsonResponse = await FetchDataAsync(apiUrl);
                    DisplayWeather(jsonResponse);
                    
                    // Save search to database
                    _searchHistoryDb.SaveSearch(city);
                    
                    // Refresh recent searches list
                    LoadRecentSearches();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a city name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task<string> FetchDataAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"Failed to fetch data. Status Code: {response.StatusCode}");
                }
            }
        }

        private void DisplayWeather(string jsonResponse)
        {
            JObject data = JObject.Parse(jsonResponse);

            // WeatherAPI.com JSON structure:
            // location -> name, country
            // current -> temp_c, condition -> text, humidity, pressure_mb
            string cityName = data["location"]["name"].ToString();
            string country = data["location"]["country"].ToString();

            string temperature = data["current"]["temp_c"].ToString();
            string description = data["current"]["condition"]["text"].ToString();

            string humidity = data["current"]["humidity"].ToString();
            string pressure = data["current"]["pressure_mb"].ToString();

            lblCity.Text = $"City: {cityName}";
            lblCountry.Text = $"Country: {country}";
            lblTemperature.Text = $"Temperature: {temperature} �C";
            lblDescription.Text = $"Description: {description}";
            lblHumadity.Text = $"Humidity: {humidity}%";
            lblPressure.Text = $"Pressure: {pressure} mb";
        }

        private void LoadRecentSearches()
        {
            dgvSearchHistory.Rows.Clear();
            var recentSearches = _searchHistoryDb.GetRecentSearches(50); // Show more in table format
            
            foreach (var search in recentSearches)
            {
                int rowIndex = dgvSearchHistory.Rows.Add();
                dgvSearchHistory.Rows[rowIndex].Cells["City"].Value = search.City;
                dgvSearchHistory.Rows[rowIndex].Cells["DateTime"].Value = search.SearchDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private async void dgvSearchHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvSearchHistory.Rows.Count)
            {
                DataGridViewRow selectedRow = dgvSearchHistory.Rows[e.RowIndex];
                string city = selectedRow.Cells["City"].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(city))
                {
                    txtCity.Text = city;
                    
                    // Trigger weather search
                    string language = cboLanguage.SelectedItem?.ToString() ?? "en";
                    string apiUrl = $"{ApiBaseUrl}?key={ApiKey}&q={city}&lang={language}&aqi=no";

                    try
                    {
                        string jsonResponse = await FetchDataAsync(apiUrl);
                        DisplayWeather(jsonResponse);
                        
                        // Save search to database
                        _searchHistoryDb.SaveSearch(city);
                        
                        // Refresh search history table
                        LoadRecentSearches();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
