using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace CalculationOfElectricityConsumption
{
    internal class ComputerMonitoring
    {
        // lists for storing component loading metrics
        List<float> cpuLoad = new List<float>();
        List<float> gpuLoad = new List<float>();
        List<float> memoryLoad = new List<float>();
        List<float> storageLoad = new List<float>();

        // lists for storing the power consumption indicators of components
        List<float> cpuPower = new List<float>();
        List<float> gpuPower = new List<float>();
        List<float> memoryPower = new List<float>();
        List<float> storagePower = new List<float>();

        // lists for storing temperature readings of components
        List<float> cpuTemperature  = new List<float>();
        List<float> gpuTemperature = new List<float>();
        List<float> memoryTemperature = new List<float>();
        List<float> storageTemperature = new List<float>();

        public void GetInfoPC()
        {
            Reset();

            Computer computer = new Computer() // retrieving components: cpu, gpu, memory, storage
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsStorageEnabled = true,
            };

            computer.Open();
            try
            {
                foreach (var hardwareItem in computer.Hardware) // iterating through components among other components
                {
                    hardwareItem.Update();  // device data update

                    foreach (var sensor in hardwareItem.Sensors) // sensor enumeration
                    {
                        if (!sensor.Value.HasValue)
                            continue;  // skipping empty values

                        if (sensor.SensorType == SensorType.Load)
                        {

                            switch (hardwareItem.HardwareType) // adding types to collections
                            {
                                case HardwareType.Cpu:
                                    cpuLoad.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.GpuNvidia:
                                case HardwareType.GpuAmd:
                                    gpuLoad.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Memory:
                                    memoryLoad.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Storage:
                                    storageLoad.Add(sensor.Value.Value);
                                    break;

                                default:
                                    break;
                            }
                        }
                        else if (sensor.SensorType == SensorType.Power)
                        {

                            switch (hardwareItem.HardwareType)
                            {
                                case HardwareType.Cpu:
                                    cpuPower.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.GpuNvidia:
                                case HardwareType.GpuAmd:
                                    gpuPower.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Memory:
                                    memoryPower.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Storage:
                                    storagePower.Add(sensor.Value.Value);
                                    break;

                                default:
                                    break;
                            }
                        }
                        else if (sensor.SensorType == SensorType.Temperature)
                        {
                            switch (hardwareItem.HardwareType)
                            {
                                case HardwareType.Cpu:
                                    cpuTemperature.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.GpuNvidia:
                                case HardwareType.GpuAmd:
                                    gpuTemperature.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Memory:
                                    memoryTemperature.Add(sensor.Value.Value);
                                    break;

                                case HardwareType.Storage:
                                    storageTemperature.Add(sensor.Value.Value);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                computer.Close();
            }
            
        }
        void Reset() // reset for new values
        {
            cpuLoad.Clear();
            gpuLoad.Clear();
            memoryLoad.Clear();
            storageLoad.Clear();

            cpuPower.Clear();
            gpuPower.Clear();
            memoryPower.Clear();
            storagePower.Clear();

            cpuTemperature.Clear();
            gpuTemperature.Clear();
            memoryTemperature.Clear();
            storageTemperature.Clear();
 
        }
        public float GetCpuLoadAverage() => cpuLoad.Any() ? cpuLoad.Average() : 0f; // getting the average CPU value
        public float GetGpuLoadAverage() => gpuLoad.Any() ? gpuLoad.Average() : 0f; // obtaining the average value of the GPU
        public float GetMemoryLoadAverage() => memoryLoad.Any() ? memoryLoad.Average() : 0f; // getting the average memory value
        public float GetStorageLoadAverage() => storageLoad.Any() ? storageLoad.Average() : 0f; // getting the average value of the storage

        //Temperature 
        public float GetCpuTemperatureAverage() => cpuTemperature.Any() ? cpuTemperature.Average() : 0f;
        public float GetGpuTemperatureAverage() => gpuTemperature.Any() ? gpuTemperature.Average() : 0f;
        public float GetMemoryTemperatureAverage() => memoryTemperature.Any() ? memoryTemperature.Average() : 0f;
        public float GetStorageTemperatureAverage() => storageTemperature.Any() ? storageTemperature.Average() : 0f;

        // total consumption
        public float GetTotalPower() => cpuPower.Sum() + gpuPower.Sum() + memoryPower.Sum() + storagePower.Sum(); 

        public override string ToString() // print 
        {
            GetInfoPC();
            return $"CPU Load: {GetCpuLoadAverage():F2}% | CPU Temp: {GetCpuTemperatureAverage():F2} °C | " +
               $"GPU Load: {GetGpuLoadAverage():F2}% | GPU Temp: {GetGpuTemperatureAverage():F2} °C | " +
               $"Memory Load: {GetMemoryLoadAverage():F2}% | Memory Temp: {GetMemoryTemperatureAverage():F2} °C | " +
               $"Storage Load: {GetStorageLoadAverage():F2}% | Storage Temp: {GetStorageTemperatureAverage():F2} °C | " +
               $"Total Power: {GetTotalPower():F2} W";
        }
    }
}
