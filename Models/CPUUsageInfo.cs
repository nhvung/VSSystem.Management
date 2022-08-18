using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class CPUUsageInfo
    {
        double _UsedPercentage;
        public double UsedPercentage { get { return _UsedPercentage; } set { _UsedPercentage = value; } }
        double _IdlePercentage;
        public double IdlePercentage { get { return _IdlePercentage; } set { _IdlePercentage = value; } }
        public CPUUsageInfo(string name = default, int numberOfCores = 0, int numberOfThreads = 0)
        {
            _UsedPercentage = 0;
            _IdlePercentage = 0;
        }

        public override string ToString()
        {
            return $"{_UsedPercentage}";
        }
    }
}
