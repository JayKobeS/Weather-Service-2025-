using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Pogodynka
{

    public partial class MainWindow : Window
    {
        private readonly WeatherService _weatherService;
        public MainWindow()
        {
            InitializeComponent();

            _weatherService = new WeatherService("e5b58509f4e48f9c2d803cb5a586003d");

            LoadEuropeanCapitals();

        }

        private void LoadEuropeanCapitals()
        {
            List<string> capitals = new List<string>
            {
                "Warsaw", "London"," Vienna", "Paris", "Lisboa","Riga","Oslo","Gliwice","Budapest", "Katowice","Amsterdam","Rome","Barcelona", "Madrid",
                "Berlin", "Kapstadt", "Prague", "Tokio","Beijing"
            };
            cityComboBox.ItemsSource = capitals;
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = string.IsNullOrEmpty(cityBoxName.Text) ? cityComboBox.SelectedItem?.ToString() : cityBoxName.Text;

                if (string.IsNullOrEmpty(cityName))
                {
                    MessageBox.Show("Proszę wprowadzić nazwę miasta lub wybrać z listy.");
                    return;
                }

                var weatherData = await _weatherService.GetWeatherDataAsync(cityName);

                DisplayWeatherData(weatherData);
                cityComboBox.SelectedItem = null;
                cityBoxName.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
            finally
            {
            }
        }
        private void DisplayWeatherData(WeatherData data)
        {
            weatherInfo.Children.Clear();

            var panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock
            {
                Text = data.CityName,
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var weatherIcon = new Image
            {
                Source = GetWeatherIconFromCode(data.WeatherIconCode),
                Width = 100,
                Height = 100,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            panel.Children.Add(weatherIcon);

            var weatherDetails = new Grid();
            weatherDetails.Margin = new Thickness(0, 10, 0, 0);

            weatherDetails.ColumnDefinitions.Add(new ColumnDefinition());
            weatherDetails.ColumnDefinitions.Add(new ColumnDefinition());
            weatherDetails.ColumnDefinitions.Add(new ColumnDefinition());

            var tempPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            tempPanel.Children.Add(new TextBlock
            {
                Text = $"{data.Temperature}°C",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            tempPanel.Children.Add(new TextBlock
            {
                Text = "Temperatura",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(tempPanel, 0);
            weatherDetails.Children.Add(tempPanel);

            var pressurePanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            pressurePanel.Children.Add(new TextBlock
            {
                Text = $"{data.Pressure} hPa",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            pressurePanel.Children.Add(new TextBlock
            {
                Text = "Ciśnienie",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(pressurePanel, 1);
            weatherDetails.Children.Add(pressurePanel);

            var humidityPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            humidityPanel.Children.Add(new TextBlock
            {
                Text = $"{data.Humidity}%",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            humidityPanel.Children.Add(new TextBlock
            {
                Text = "Wilgotność",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(humidityPanel, 2);
            weatherDetails.Children.Add(humidityPanel);

            panel.Children.Add(weatherDetails);

            weatherInfo.Children.Add(panel);
        }

        private System.Windows.Media.ImageSource GetWeatherIconFromCode(string iconCode)
        {
            string iconUrl = $"https://openweathermap.org/img/wn/{iconCode}@2x.png";


            var image = new System.Windows.Media.Imaging.BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(iconUrl);
            image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            image.EndInit();

            return image;
        }
    }
}