using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Management.Models.Service;

namespace VSSystem.Management
{
    public interface IServiceProcess
    {
        List<ServiceInfo> GetServices(List<string> names = null, bool isPreName = false, bool includeWorkingSet = false);
        bool ActionService(string name, EServiceAction action = EServiceAction.DoNotThing);
    }
}
