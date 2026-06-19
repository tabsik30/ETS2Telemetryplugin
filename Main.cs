using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;

namespace tabsik12.Ets2TelemetryPlugin
{
    public class Main : MacroDeckPlugin
    {
        private static readonly string TelemetryUrl = "http://localhost:25555/api/ets2/telemetry";
        private readonly HttpClient _httpClient = new HttpClient();
        private Timer _updateTimer;

        // Ścieżka do naszego własnego pliku konfiguracyjnego
        private string ConfigFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Macro Deck", "plugins", "Ets2TelemetryPlugin", "Ets2Telemetry.config.json");

        public bool UseMph { get; private set; }

        public override bool CanConfigure => true;

        public override void Enable()
        {
            LoadConfiguration();

            Actions = new List<PluginAction>
            {
                new SpeedAction(),
                new SpeedLimitAction(),
                new FuelAction(),
                new GearAction()
            };

            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            _updateTimer = new Timer(async _ => await UpdateTelemetry(), null, 0, 200);
        }

        public override void OpenConfigurator()
        {
            using (var settings = new SettingsForm(this))
            {
                settings.ShowDialog();
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    var cfg = JsonConvert.DeserializeObject<PluginConfig>(json);
                    UseMph = cfg?.UseMph ?? false;
                }
                else
                {
                    UseMph = false; // domyślnie km/h
                }
            }
            catch
            {
                UseMph = false;
            }
        }

        public void SaveConfiguration(bool useMph)
        {
            UseMph = useMph;

            var cfg = new PluginConfig
            {
                UseMph = useMph
            };

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath)!);
                var json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Error(this, $"Failed to save ETS2 telemetry config: {ex.Message}");
            }
        }

        private async Task UpdateTelemetry()
        {
            try
            {
                var response = await _httpClient.GetAsync(TelemetryUrl);
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var telemetry = JObject.Parse(json);

                var truck = telemetry["truck"];
                var navigation = telemetry["navigation"];

                // 1. Prędkość – wartość bezwzględna, km/h lub mph
                double speedRaw = truck?["speed"]?.ToObject<double?>() ?? 0.0;
                double speedAbsKm = Math.Abs(speedRaw); // zakładamy km/h z telemetrii

                double speedDisplay = speedAbsKm;
                if (UseMph)
                {
                    speedDisplay = speedDisplay * 0.621371;
                }

                int speedRounded = (int)Math.Round(speedDisplay);
                string speedText = speedRounded.ToString();

                // 2. Ograniczenie prędkości – te same jednostki co prędkość
                double speedLimitValKm = navigation?["speedLimit"]?.ToObject<double?>() ?? 0.0;
                double speedLimitDisplay = speedLimitValKm;
                if (UseMph)
                {
                    speedLimitDisplay = speedLimitDisplay * 0.621371;
                }
                int speedLimitRounded = (int)Math.Round(speedLimitDisplay);
                string speedLimitText = speedLimitRounded.ToString();

                // 2a. Tempomat – stan i ustawiona prędkość (w tych samych jednostkach co prędkość)
                bool cruiseOnRaw = truck?["cruiseControlOn"]?.ToObject<bool?>() ?? false;
                double cruiseSpeedValKm = truck?["cruiseControlSpeed"]?.ToObject<double?>() ?? 0.0; // km/h z telemetrii

                if (!cruiseOnRaw || cruiseSpeedValKm <= 0.1)
                {
                    cruiseSpeedValKm = 0.0;
                }

                double cruiseSpeedDisplay = cruiseSpeedValKm;
                if (UseMph)
                {
                    cruiseSpeedDisplay = cruiseSpeedDisplay * 0.621371;
                }

                int cruiseSpeedRounded = (int)Math.Round(cruiseSpeedDisplay);
                string cruiseSpeedText = cruiseSpeedRounded > 0 ? cruiseSpeedRounded.ToString() : string.Empty;

                // 3. Paliwo – fuelPercentage lub fuel/fuelCapacity
                double fuelPct = 0.0;

                if (truck?["fuelPercentage"] != null)
                {
                    var rawFuel = truck["fuelPercentage"].ToObject<double?>();
                    if (rawFuel.HasValue)
                    {
                        fuelPct = rawFuel.Value > 1.5 ? rawFuel.Value : rawFuel.Value * 100.0;
                    }
                }
                else
                {
                    double fuel = truck?["fuel"]?.ToObject<double?>() ?? 0.0;
                    double fuelCapacity = truck?["fuelCapacity"]?.ToObject<double?>() ?? 0.0;
                    if (fuelCapacity > 0.0)
                    {
                        fuelPct = (fuel / fuelCapacity) * 100.0;
                    }
                }

                int fuelRounded = (int)Math.Round(fuelPct);
                string fuelText = fuelRounded.ToString();

                // 4. Bieg – <0 = R, 0 = N, >0 = 1,2,3...
                int gearRaw = truck?["gear"]?.ToObject<int?>() ?? 0;
                string gearText;

                if (gearRaw < 0)
                {
                    gearText = "R";
                }
                else if (gearRaw == 0)
                {
                    gearText = "N";
                }
                else
                {
                    gearText = gearRaw.ToString();
                }

                // 5. Światła
                bool parkingOn = truck?["lightsParkingOn"]?.ToObject<bool?>() ?? false;
                bool lowOn     = truck?["lightsBeamLowOn"]?.ToObject<bool?>() ?? false;
                bool highOn    = truck?["lightsBeamHighOn"]?.ToObject<bool?>() ?? false;

                string lightsText;
                if (highOn)
                {
                    lightsText = "HIGH";
                }
                else if (lowOn)
                {
                    lightsText = "LOW";
                }
                else if (parkingOn)
                {
                    lightsText = "PARK";
                }
                else
                {
                    lightsText = "OFF";
                }

                bool lightsParkingVar = parkingOn;
                bool lightsLowVar     = lowOn;
                bool lightsHighVar    = highOn;

                // 6. Kierunkowskazy
                bool blinkerLeftOn  = truck?["blinkerLeftOn"]?.ToObject<bool?>() ?? false;
                bool blinkerRightOn = truck?["blinkerRightOn"]?.ToObject<bool?>() ?? false;

                string blinkerText;
                if (blinkerLeftOn && blinkerRightOn)
                {
                    blinkerText = "HAZARD";
                }
                else if (blinkerLeftOn)
                {
                    blinkerText = "LEFT";
                }
                else if (blinkerRightOn)
                {
                    blinkerText = "RIGHT";
                }
                else
                {
                    blinkerText = "OFF";
                }

                bool blinkerLeftVar   = blinkerLeftOn && !blinkerRightOn;
                bool blinkerRightVar  = blinkerRightOn && !blinkerLeftOn;
                bool blinkerHazardVar = blinkerLeftOn && blinkerRightOn;

                // 7. Hamulec ręczny
                bool parkBrakeOn = truck?["parkBrakeOn"]?.ToObject<bool?>() ?? false;
                string parkBrakeText = parkBrakeOn ? "ON" : "OFF";

                // Zmienne Macro Deck

                // Prędkość i limit
                VariableManager.SetValue("ets2_speed", speedText, VariableType.String, this, null);
                VariableManager.SetValue("ets2_speed_limit", speedLimitText, VariableType.String, this, null);

                // Tempomat
                VariableManager.SetValue("ets2_cruise_on", cruiseOnRaw, VariableType.Bool, this, null);
                VariableManager.SetValue("ets2_cruise_speed", cruiseSpeedText, VariableType.String, this, null);

                // Paliwo i bieg
                VariableManager.SetValue("ets2_fuel_percentage", fuelText, VariableType.String, this, null);
                VariableManager.SetValue("ets2_gear", gearText, VariableType.String, this, null);

                // Światła
                VariableManager.SetValue("ets2_lights", lightsText, VariableType.String, this, null);
                VariableManager.SetValue("ets2_lights_parking", lightsParkingVar, VariableType.Bool, this, null);
                VariableManager.SetValue("ets2_lights_low",     lightsLowVar,     VariableType.Bool, this, null);
                VariableManager.SetValue("ets2_lights_high",    lightsHighVar,    VariableType.Bool, this, null);

                // Kierunkowskazy
                VariableManager.SetValue("ets2_blinker", blinkerText, VariableType.String, this, null);
                VariableManager.SetValue("ets2_blinker_left",   blinkerLeftVar,   VariableType.Bool, this, null);
                VariableManager.SetValue("ets2_blinker_right",  blinkerRightVar,  VariableType.Bool, this, null);
                VariableManager.SetValue("ets2_blinker_hazard", blinkerHazardVar, VariableType.Bool, this, null);

                // Hamulec ręczny
                VariableManager.SetValue("ets2_park_brake", parkBrakeText, VariableType.String, this, null);
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
            {
                // Telemetry server nie działa – ignorujemy, żeby nie spamować błędami
                // MacroDeckLogger.Trace(this, "ETS2 telemetry server not running (connection refused).");
            }
            catch (SocketException se) when (se.SocketErrorCode == SocketError.ConnectionRefused)
            {
                // Gdyby wyjątkowo przyszedł bez HttpRequestException
                // MacroDeckLogger.Trace(this, "ETS2 telemetry server not running (connection refused).");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Error(this, $"ETS2 telemetry update error: {ex.Message}");
            }
        }
    }

    public class PluginConfig
    {
        public bool UseMph { get; set; }
    }
}
