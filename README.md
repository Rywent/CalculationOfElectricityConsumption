# Electricity Consumption Monitor

This project is a simple C# console application that monitors your PC's CPU load, GPU usage, memory, and storage activity to estimate electricity consumption and cost based on configurable parameters, providing more accurate results.

---

## Features

- Measures CPU usage periodically (every 3 minutes).
- Measures GPU usage, memory load, and storage activity for more comprehensive monitoring.
- Calculates estimated power consumption based on combined load metrics and a base power consumption value.
- Saves the daily summary of electricity consumption and cost to a log file.
- Configurable parameters are read from a JSON settings file.
- Safe file saving using temporary files to prevent data corruption.

---

## Requirements

- .NET Core 3.1 or later / .NET 5 or later
- Windows OS (due to usage of LibreHardwareMonitor)
- Basic knowledge of running console applications

---

## Setup and Usage

### 1. Clone the repository

git clone https://github.com/your_username/your_repository.git
cd your_repository

### 2. Create and configure `settings.json`

Before running the program, create a file called `settings.json` in the root folder of the project. This file contains your configuration parameters. Example content:

{
"costPerKwh": 7.5,
"totalPowerConsumption": 0.0
}


- **costPerKwh** — electricity cost per kilowatt-hour in your local currency.
- **totalPowerConsumption** — value will be updated automatically, set to 0.0 on first run.

Adjust these values according to your PC specifications and electricity tariff.

### 3. Build and run

Using the .NET CLI, you can build and run the project as follows:

dotnet build
dotnet run


The console will output CPU, GPU, memory, and storage usage and update the consumption data every 3 minutes. Press **Enter** to stop the program safely, which will save the final data.

---

## Log Output

- The daily summary log file `energy_log.txt` is saved in the project root.
- Each day's data includes total active hours, energy consumed (in watt-hours), calculated cost, and number of measurements.

---

## Notes

- The program usesLibreHardwareMonitor, so it works on Windows OS only.
- Make sure to run the program with appropriate permissions to access performance counters.
- For accurate power ratings, consider measuring your PC power consumption with a wattmeter.
- Now the application calculates estimated consumption based not only on CPU load but also includes GPU usage, memory load, and storage activity, providing more precise and realistic power consumption estimates.

---

## Contributing

Feel free to fork the project and submit pull requests with improvements or bug fixes.

---

## Contact

For questions or support, please open an issue on GitHub.

