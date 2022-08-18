using VSSystem.Management.Models;
using System;
namespace VSSystem.Management.Extensions
{
    public class CalculateExtension
    {
        public static StorageInfo ConvertToValue(string s, EUnit destinationUnit = EUnit.Undefined)
        {
            StorageInfo result = new StorageInfo();
            try
            {
                double value;
                double.TryParse(s.Substring(0, s.Length - 1), out value);
                result.Value = value;
                if (s.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Unit = EUnit.Megabyte;
                }
                else if (s.EndsWith("G", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Unit = EUnit.Gigabyte;
                }
                else if (s.EndsWith("T", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Unit = EUnit.Terabyte;
                }
                else if (s.EndsWith("k", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Unit = EUnit.Kylobyte;
                }

                if (destinationUnit != EUnit.Undefined)
                {
                    if (result.Unit > destinationUnit)
                    {
                        while (result.Unit > destinationUnit)
                        {
                            result.Value *= 1024;
                            result.Unit--;
                        }
                    }
                    else if (result.Unit < destinationUnit)
                    {
                        while (result.Unit < destinationUnit)
                        {
                            result.Value /= 1024;
                            result.Unit++;
                        }
                    }
                }
            }
            catch { }
            return result;
        }
    }
}