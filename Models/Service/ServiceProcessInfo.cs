using System.Collections.Generic;
namespace VSSystem.Management.Models.Service
{
    public class ServiceProcessInfo
    {
        int _MainProcessID;
        public int MainProcessID { get { return _MainProcessID; } set { _MainProcessID = value; } }
        List<int> _ProcessIDs;
        public List<int> ProcessIDs { get { return _ProcessIDs; } set { _ProcessIDs = value; } }
        int _NumberOfTasks;
        public int NumberOfTasks { get { return _NumberOfTasks; } set { _NumberOfTasks = value; } }
        double _WorkingSet;
        public double WorkingSet { get { return _WorkingSet; } set { _WorkingSet = value; } }
    }
}