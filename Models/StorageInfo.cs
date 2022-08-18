using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class StorageInfo
    {
        double _Value;
        public double Value { get { return _Value; } set { _Value = value; } }
        EUnit _Unit;
        public EUnit Unit { get { return _Unit; } set { _Unit = value; } }
        public StorageInfo(double value = 0, EUnit unit = EUnit.Byte)
        {
            _Value = value;
            _Unit = unit;
        }
        public static StorageInfo operator +(StorageInfo s1, StorageInfo s2)
        {

            try
            {
                double val1 = s1.Value, val2 = s2.Value;
                EUnit unit1 = s1.Unit, unit2 = s2.Unit;
                double baseVal = 1024;
                if (Environment.OSVersion?.VersionString?.IndexOf("Unix ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    baseVal = 1000;
                }
                while (unit1 > unit2)
                {
                    val1 *= baseVal;
                    unit1++;
                }
                while (unit2 > unit1)
                {
                    val2 *= baseVal;
                    unit2++;
                }
                double value = val1 + val2;
                return new StorageInfo(value, unit1);
            }
            catch { }
            return default;
        }
        public override string ToString()
        {
            double value = _Value;
            EUnit unit = _Unit;
            double baseVal = 1024;
            while (value > baseVal)
            {
                value /= baseVal;
                unit++;
            }
            return $"{value.ToString("#.##")} {unit}";
        }
        public string ToString(string osVersion)
        {
            double value = _Value;
            EUnit unit = _Unit;
            double baseVal = 1024;
            if (osVersion?.IndexOf("Linux ", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                baseVal = 1000;
            }
            while (value > baseVal)
            {
                value /= baseVal;
                unit++;
            }
            return $"{value.ToString("#.##")} {unit}";
        }
    }
}
