using CalculationOfElectricityConsumption;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Timers;

class Program
{
    static int measurementsCount = 0;
    static double totalPowerConsumption; // Watt-hours
<<<<<<< HEAD
    static double costPerKwh; // cost per kWh
=======
    static double powerRated; // power of the PC in W
    static double costPerKwh; // cost per kWh
    static double baseConsumption;
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33

    static System.Timers.Timer measurementTimer;
    static DateTime dayStart = DateTime.Today;
    
    static ComputerMonitoring _monitoring = new ComputerMonitoring();
    static void Main(string[] args)
    {
        LoadSettings();
<<<<<<< HEAD

        string output = _monitoring.ToString();
        Console.WriteLine(output);
=======
        // create and initialize the CPU counter
        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        cpuCounter.NextValue(); // for warming up
        Thread.Sleep(1000);
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33

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
<<<<<<< HEAD
        // 3 minutes
        measurementTimer = new System.Timers.Timer(180000);
=======
        // 5 minutes
        measurementTimer = new System.Timers.Timer(300000);
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33

        // subscribe to event
        measurementTimer.Elapsed += MeasurementTimer_Elapsed;

        measurementTimer.AutoReset = true;
        measurementTimer.Enabled = true;
    }
    static void MeasurementTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _monitoring.GetInfoPC();
        float currentTotalPower = _monitoring.GetTotalPower();

<<<<<<< HEAD
        Console.WriteLine($"{DateTime.Now}: Total Power: {currentTotalPower:F2} W");

        double intervalHours = measurementTimer.Interval / (1000.0 * 60.0 * 60.0); // translation in hours
=======
        Console.WriteLine($"{DateTime.Now}: CPU Load: {cpuLoad:F2}%");

        // calculating energy consumption over the interval in watt-hours
        double intervalHours = measurementTimer.Interval / (1000.0 * 60.0 * 60.0); // translation to hours
        double loadFraction = cpuLoad / 100.0; // convert % to fractions
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33

        double energyUsed = currentTotalPower * intervalHours; // Вт·ч за интервал
        totalPowerConsumption += energyUsed;
        measurementsCount++;

<<<<<<< HEAD
        if (DateTime.Now.Date != dayStart)
=======
        double energyUsed = totalPower * intervalHours; // calculation using the formula expense = powerPk + consumption shares cp * consumption time
        totalPowerConsumption += energyUsed;// total energy consumption
        measurementsCount++; // how many energy counts were there during the work period

        if (DateTime.Now.Date > dayStart) // transition to the current day
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33
        {
            SaveData();
            dayStart = DateTime.Now.Date;
            totalPowerConsumption = 0;
            measurementsCount = 0;
        }
    }
    static void SaveData()
    {
        if (measurementsCount == 0) return;// if measurements are 0, exit

        double totalHours = measurementsCount * measurementTimer.Interval / (1000.0 * 60 * 60); // how many hours is the PC active
        double cost = totalPowerConsumption / 1000.0 * costPerKwh; // summary for the day

        string record = $"{dayStart:yyyy-MM-dd} Activity: {totalHours:F2} ч, " +
<<<<<<< HEAD
                        $"Consumed: {totalPowerConsumption:F2} Вт·ч, Cost: {cost:F2}$." + $"Total calculations: {measurementsCount}    " +
                        _monitoring.ToString();
=======
                        $"Consumed: {totalPowerConsumption:F2} Вт·ч, Cost: {cost:F2}$." + $"Total calculations: {measurementsCount}";
>>>>>>> 154163f6ec4a7b0052814c948a4d9cd8d6471d33

        Console.WriteLine("Saving data: " + record);

        string tempFileName = "energy_log.tmp";
        string mainFileName = "energy_log.txt";
        string errorFile = "error_log.txt";

        try
        {
            // writing to a temporary file (in case of errors)
            File.WriteAllText(tempFileName, record + Environment.NewLine);

            // successfully, replacing with the final file
            if (File.Exists(mainFileName))
                File.Delete(mainFileName);

            File.Move(tempFileName, mainFileName);
        }
        catch (Exception ex)
        {
            File.WriteAllText(errorFile, "Error saving the file:" + ex.Message);
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
