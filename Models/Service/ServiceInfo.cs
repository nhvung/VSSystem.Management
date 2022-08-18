using System.Collections.Generic;
namespace VSSystem.Management.Models.Service
{
    public class ServiceInfo
    {
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }
        EServiceStatus _Status;
        public EServiceStatus Status { get { return _Status; } set { _Status = value; } }
        string _Description;
        public string Description { get { return _Description; } set { _Description = value; } }
        int _ProcessID;
        public int ProcessID { get { return _ProcessID; } set { _ProcessID = value; } }
        List<int> _SubProcessIDs;
        public List<int> SubProcessIDs { get { return _SubProcessIDs; } set { _SubProcessIDs = value; } }
        int _NumberOfTasks;
        public int NumberOfTasks { get { return _NumberOfTasks; } set { _NumberOfTasks = value; } }
        double _WorkingSet;
        public double WorkingSet { get { return _WorkingSet; } set { _WorkingSet = value; } }
    }
}