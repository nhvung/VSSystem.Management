using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class RAMInfo
    {

        StorageInfo _Total;
        public StorageInfo Total { get { return _Total; } set { _Total = value; } }

        StorageInfo _Free;
        public StorageInfo Free { get { return _Free; } set { _Free = value; } }
        public RAMInfo()
        {

        }
        public override string ToString()
        {
            return $"Free: {_Free}, Total: {_Total}";
        }
    }
}
