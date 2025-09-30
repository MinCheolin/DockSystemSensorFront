
using Newtonsoft.Json;
using ShipyardDashboard.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShipyardDashboard.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:8080/api"; // Assuming the same backend port

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ProcessDashboard?> GetProcessDashboardAsync(string processName)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/dashboard/{processName}");
                return JsonConvert.DeserializeObject<ProcessDashboard>(response);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error fetching process dashboard for {processName}: {ex.Message}");
                return new ProcessDashboard 
                {
                    ProcessName = processName,
                    EquipmentGroups = new() { new() { GroupName = "BACKEND CONNECTION FAILED", Equipments = new() { new() { Name = $"Could not connect to {BaseUrl}", Status = "Error" } } } }
                };
            }
        }

        public async Task<GlobalAlerts?> GetGlobalAlertsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/global-alerts");
                return JsonConvert.DeserializeObject<GlobalAlerts>(response);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching global alerts: {ex.Message}");
                return new GlobalAlerts { OverallStatus = "Error", Alerts = new() { new() { Type = "BACKEND NOT RESPONDING", Status = "Error", Value = ex.Message } } };
            }
        }

        public async Task<Overview?> GetOverviewAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/overview");
                return JsonConvert.DeserializeObject<Overview>(response);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching overview data: {ex.Message}");
                return new Overview(); // Return an empty overview or handle error gracefully
            }
        }
    }
}
