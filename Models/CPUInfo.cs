using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class CPUInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        int _NumberOfThreads;
        public int NumberOfThreads { get { return _NumberOfThreads; } set { _NumberOfThreads = value; } }

        int _NumberOfCores;
        public int NumberOfCores { get { return _NumberOfCores; } set { _NumberOfCores = value; } }
        public CPUInfo(string name = default, int numberOfCores = 0, int numberOfThreads = 0)
        {
            _Name = name;
            _NumberOfCores = numberOfCores;
            _NumberOfThreads = numberOfThreads;
        }

        public override string ToString()
        {
            return $"{_Name} ({_NumberOfCores}/{_NumberOfThreads})";
        }
    }
}
