using System;
using System.Collections.Generic;

namespace O2DESNet
{
    public class UtilizationRowData
    {
        public string ResName { get; set; }
        public List<double> HourlyUtilization { get; set; }
        public List<double> DailyUtilization { get; set; }
        public List<double> MonthlyUtilization { get; set; }
        public double TotalQtt { get; set; }
        public List<DateTime> ChangeTime { get; set; }
    }
}
