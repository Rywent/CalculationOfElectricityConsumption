using CalculationOfElectricityConsumption;
using System;
using System.IO;
using System.Text.Json;
using System.Timers;

class Program
{
    static int measurementsCount = 0;
    static double totalPowerConsumption; // Watt-hours
    static double costPerKwh; // cost per kWh

    static System.Timers.Timer measurementTimer;
    static DateTime dayStart = DateTime.Today;

    static ComputerMonitoring _monitoring = new ComputerMonitoring();

    static void Main(string[] args)
    {
        LoadSettings();

        string output = _monitoring.ToString();
        Console.WriteLine(output);

        SetTimer();

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();

        // Stop the timer and save the data upon exit
        measurementTimer.Stop();
        SaveData();
        SaveSettings();
    }

    static void SetTimer()
    {
        // 3 minutes interval
        measurementTimer = new System.Timers.Timer(180000);

        measurementTimer.Elapsed += MeasurementTimer_Elapsed;
        measurementTimer.AutoReset = true;
        measurementTimer.Enabled = true;
    }

    static void MeasurementTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _monitoring.GetInfoPC();
        float currentTotalPower = _monitoring.GetTotalPower();

        Console.WriteLine($"{DateTime.Now}: {_monitoring.ToString()}");

        double intervalHours = measurementTimer.Interval / (1000.0 * 60.0 * 60.0); // convert milliseconds to hours

        double energyUsed = currentTotalPower * intervalHours; // watt-hours for the interval
        totalPowerConsumption += energyUsed;
        measurementsCount++;

        if (DateTime.Now.Date != dayStart)
        {
            SaveData();
            dayStart = DateTime.Now.Date;
            totalPowerConsumption = 0;
            measurementsCount = 0;
        }
    }

    static void SaveData()
    {
        if (measurementsCount == 0) return;

        double totalHours = measurementsCount * measurementTimer.Interval / (1000.0 * 60 * 60);
        double cost = totalPowerConsumption / 1000.0 * costPerKwh;

        string record = $"{dayStart:yyyy-MM-dd} Activity: {totalHours:F2} ч, " +
                        $"Consumed: {totalPowerConsumption:F2} Вт·ч, Cost: {cost:F2}$." + $" Total measurements: {measurementsCount}    ";

        Console.WriteLine("Saving data: " + record);

        string tempFileName = "energy_log.tmp";
        string mainFileName = "energy_log.txt";
        string errorFile = "error_log.txt";

        try
        {
            File.WriteAllText(tempFileName, record + Environment.NewLine);

            if (File.Exists(mainFileName))
                File.Delete(mainFileName);

            File.Move(tempFileName, mainFileName);
        }
        catch (Exception ex)
        {
            File.WriteAllText(errorFile, "Error saving the file: " + ex.Message);
        }
    }

    static void LoadSettings()
    {
        string fileName = "settings.json";
        if (File.Exists(fileName))
        {
            try
            {
                string json = File.ReadAllText(fileName);
                var settings = JsonSerializer.Deserialize<Settings>(json);

                if (settings != null)
                {
                    costPerKwh = settings.CostPerKwh;
                    totalPowerConsumption = settings.TotalPowerConsumption;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Configuration file not found, default values will be used");
        }
    }

    static void SaveSettings()
    {
        string fileName = "settings.json";
        try
        {
            var settings = new Settings()
            {
                CostPerKwh = costPerKwh,
                TotalPowerConsumption = totalPowerConsumption,
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(fileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
}