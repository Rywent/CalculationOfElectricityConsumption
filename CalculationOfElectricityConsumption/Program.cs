using CalculationOfElectricityConsumption;
using System;
using System.IO;
using System.Text.Json;
using System.Timers;

class Program
{
    static double totalPowerConsumption; // Watt-hours
    static double costPerKwh; // cost per kWh

    static double totalHours;
    static double totalCost;
    static DateTime lastSavedDate;


    static System.Timers.Timer measurementTimer;

    static ComputerMonitoring _monitoring = new ComputerMonitoring();

    static void Main(string[] args)
    {
        LoadSettings();
        LoadLogForToday();

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

        double intervalHours = measurementTimer.Interval / (1000.0 * 60.0 * 60.0); // milliseconds to hours

        double energyUsed = currentTotalPower * intervalHours; // watt per hour for the interval

        // checking the date change
        if (DateTime.Now.Date != lastSavedDate.Date)
        {
            // saving the accumulated data from the previous day
            SaveData();

            // saving settings after logging
            SaveSettings();

            // resetting the accumulations for a new day
            totalPowerConsumption = 0;
            totalHours = 0;
            totalCost = 0;

            // updating the last saved date
            lastSavedDate = DateTime.Now.Date;
        }


        // accumulation
        totalPowerConsumption += energyUsed;
        totalHours += intervalHours;
        totalCost = (totalPowerConsumption / 1000.0) * costPerKwh;

        Console.WriteLine($"Accumulated: Time: {totalHours:F2} ч, Energy: {totalPowerConsumption:F2} Вт·ч, Cost: {totalCost:F2}");
    }

    static void SaveData()
    {
        if (totalHours == 0) return;

        string dateStr = lastSavedDate.ToString("yyyy-MM-dd");
        string newRecord = $"{dateStr} Activity: {totalHours:F2} ч, Consumed: {totalPowerConsumption:F2} Вт·ч, Cost: {totalCost:F2} руб.";

        Console.WriteLine("Saving data: " + newRecord);

        string logFile = "energy_log.txt";
        string tempFileName = "energy_log.tmp";
        string errorFile = "error_log.txt";

        try
        {
            string[] lines = File.Exists(logFile) ? File.ReadAllLines(logFile) : Array.Empty<string>();
            bool foundToday = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(dateStr))
                {
                    lines[i] = newRecord;
                    foundToday = true;
                    break;
                }
            }
            if (!foundToday)
            {
                // Adding a new entry
                var tempList = new List<string>(lines) { newRecord };
                lines = tempList.ToArray();
            }

            // we write the entire file through a temporary file
            File.WriteAllLines(tempFileName, lines);

            if (File.Exists(logFile))
                File.Delete(logFile);

            File.Move(tempFileName, logFile);

            SaveSettings();
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
                    totalHours = settings.TotalHours;
                    totalCost = settings.TotalCost;   
                    lastSavedDate = settings.LastSavedDate; 
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
            totalHours = 0;
            totalCost = 0;
            lastSavedDate = DateTime.Today;
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
                TotalHours = totalHours,
                TotalCost = totalCost,
                LastSavedDate = lastSavedDate
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

    static void LoadLogForToday()
    {
        string logFile = "energy_log.txt";
        if (!File.Exists(logFile))
        {
            return;
        }

        string todayStr = DateTime.Today.ToString("yyyy-MM-dd");
        string[] lines = File.ReadAllLines(logFile);

        foreach (var line in lines)
        {
            if (line.StartsWith(todayStr))
            {
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    try
                    {
                        // parsing Activity
                        var actPart = parts[0].Split(new[] { "Activity:" }, StringSplitOptions.None)[1].Trim().Split(' ')[0];
                        totalHours = double.Parse(actPart);

                        // parsing Consumed
                        var consPart = parts[1].Split(new[] { "Consumed:" }, StringSplitOptions.None)[1].Trim().Split(' ')[0];
                        totalPowerConsumption = double.Parse(consPart);

                        // parsing Cost
                        var costPart = parts[2].Split(new[] { "Cost:" }, StringSplitOptions.None)[1].Trim().Split(' ')[0];
                        totalCost = double.Parse(costPart);
                    }
                    catch
                    {
                        // on parsing error - reset
                        totalHours = 0;
                        totalPowerConsumption = 0;
                        totalCost = 0;
                    }
                }
                return; // found and uploaded
            }
        }
        // if there are no records for today
        totalHours = 0;
        totalPowerConsumption = 0;
        totalCost = 0;
    }

}