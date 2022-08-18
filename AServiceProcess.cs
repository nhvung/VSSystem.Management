using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Management.Models.Service;

namespace VSSystem.Management
{
    public abstract class AServiceProcess : IServiceProcess
    {
        public List<ServiceInfo> GetServices(List<string> names = null, bool isPrefixName = false, bool includeWorkingSet = false)
        {
            return _GetServices(names, isPrefixName, includeWorkingSet);
        }
        protected virtual List<ServiceInfo> _GetServices(List<string> names, bool isPreName, bool includeWorkingSet)
        {
            return new List<ServiceInfo>();
        }

        public bool ActionService(string name, EServiceAction action = EServiceAction.DoNotThing) { return _ActionService(name, action); }
        protected virtual bool _ActionService(string name, EServiceAction action)
        {
            return false;
        }

        protected virtual ServiceProcessInfo _GetServiceProcessInfo(string name) { return null; }
    }
}
