using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationOfElectricityConsumption
{
    public class Settings
    {
        public double CostPerKwh { get; set; }         
        public double TotalPowerConsumption { get; set; } 
        public double TotalHours { get; set; }   
        public double TotalCost { get; set; }            
        public DateTime LastSavedDate { get; set; } = DateTime.Today; 
    }

}
