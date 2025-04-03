using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace weather;

[McpServerToolType]
public static class WeatherTools
{
    [McpServerTool, Description("Get weather alerts for a state")]
    public static async Task<string> GetAlerts(
        HttpClient client,
        [Description("Two-letter state code (e.g. CA, NY)")] string state)
    {
        try 
        {
            // The correct URL format for state alerts
            var requestUrl = $"/alerts/active/area/{state.ToUpper()}";
            Console.Error.WriteLine($"DEBUG: Requesting alerts from URL: {client.BaseAddress}{requestUrl}");

            var response = await client.GetAsync(requestUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"DEBUG: Response Status: {response.StatusCode}");
            Console.Error.WriteLine($"DEBUG: Response Content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {responseContent}";
            }

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var features = jsonElement.GetProperty("features").EnumerateArray();

            if (!features.Any())
            {
                return "No active alerts for this state.";
            }

            return string.Join("\n--\n", features.Select(alert =>
            {
                var properties = alert.GetProperty("properties");
                return $"""
                        Event: {properties.GetProperty("event").GetString()}
                        Area: {properties.GetProperty("areaDesc").GetString()}
                        Severity: {properties.GetProperty("severity").GetString()}
                        Description: {properties.GetProperty("description").GetString()}
                        Instructions: {(properties.TryGetProperty("instruction", out var instruction) ? instruction.GetString() : "No specific instructions provided")}
                        """;
            }));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"DEBUG: Exception details: {ex}");
            return $"Error fetching alerts: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get weather forecast for a location")]
    public static async Task<string> GetForecast(
        HttpClient client,
        [Description("Latitude of the location")] double latitude,
        [Description("Longitude of the location")] double longitude)
    {
        try
        {
            // First get the forecast URL from the points endpoint
            var pointsResponse = await client.GetFromJsonAsync<JsonElement>($"/points/{latitude},{longitude}");
            var forecastUrl = pointsResponse
                .GetProperty("properties")
                .GetProperty("forecast")
                .GetString();

            if (string.IsNullOrEmpty(forecastUrl))
            {
                return "Could not get forecast URL for the specified location.";
            }

            // Now get the actual forecast
            var forecastResponse = await client.GetFromJsonAsync<JsonElement>(new Uri(forecastUrl));
            var periods = forecastResponse
                .GetProperty("properties")
                .GetProperty("periods")
                .EnumerateArray();

            return string.Join("\n---\n", periods.Select(period => $"""
                    {period.GetProperty("name").GetString()}
                    Temperature: {period.GetProperty("temperature").GetInt32()}°F
                    Wind: {period.GetProperty("windSpeed").GetString()} {period.GetProperty("windDirection").GetString()}
                    Forecast: {period.GetProperty("detailedForecast").GetString()}
                    """));
        }
        catch (Exception ex)
        {
            return $"Error fetching forecast: {ex.Message}";
        }
    }
}