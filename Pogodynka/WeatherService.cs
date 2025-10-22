using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pogodynka
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<WeatherData> GetWeatherDataAsync(string cityName)
        {
            string url = $"{BaseUrl}?q={cityName}&appid={_apiKey}&units=metric";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return ParseWeatherData(json);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Miasto nie zostało znalezione.");
                }
                else
                {
                    throw new Exception($"Błąd API: {response.StatusCode}");
                }
            }
        }

        private WeatherData ParseWeatherData(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;

                return new WeatherData
                {
                    CityName = root.GetProperty("name").GetString(),
                    Temperature = Math.Round(root.GetProperty("main").GetProperty("temp").GetDouble(), 1),
                    Pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    WeatherCondition = root.GetProperty("weather")[0].GetProperty("main").GetString(),
                    WeatherIconCode = root.GetProperty("weather")[0].GetProperty("icon").GetString()
                };
            }
        }
    }

    public class WeatherData
    {
        public string CityName { get; set; }
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public string WeatherCondition { get; set; }
        public string WeatherIconCode { get; set; }
    }
}
