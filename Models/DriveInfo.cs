using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class DriveInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _Label;
        public string Label { get { return _Label; } set { _Label = value; } }

        StorageInfo _Total;
        public StorageInfo Total { get { return _Total; } set { _Total = value; } }

        StorageInfo _Free;
        public StorageInfo Free { get { return _Free; } set { _Free = value; } }

        public DriveInfo(string name, string label = "")
        {
            _Name = name;
            _Label = label;
            _Total = new StorageInfo();
            _Free = new StorageInfo();
        }
        public override string ToString()
        {
            return $"{_Name} [{_Label}] {_Free}/{_Total}";
        }
    }
}
