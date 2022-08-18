using System;
using System.Collections.Generic;
using VSSystem.Management.Models.Service;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using VSSystem.Management.Extensions;

namespace VSSystem.Management.Ubuntu
{
    public class ServiceProcess : AServiceProcess
    {
        protected override bool _ActionService(string name, EServiceAction action = EServiceAction.DoNotThing)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && action != EServiceAction.DoNotThing)
                {
                    List<string> args = new List<string>();
                    args.Add("systemctl");
                    args.Add($"{action.ToString().ToLower()}");
                    args.Add($"{name}.service");

                    string cmdResult = CLIExtension.Execute(args);
                    if (action == EServiceAction.Stop)
                    {
                        var processInfoObj = _GetServiceProcessInfo(name);
                        if (processInfoObj != null)
                        {
                            if (processInfoObj.ProcessIDs?.Count > 0)
                            {
                                foreach (var pid in processInfoObj.ProcessIDs.Distinct())
                                {
                                    CLIExtension.Execute(new List<string>() { $"kill -9 {pid}" });
                                }
                            }
                        }
                    }
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }

        protected override ServiceProcessInfo _GetServiceProcessInfo(string name)
        {
            ServiceProcessInfo result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    List<string> args = new List<string>();
                    args.Add("systemctl");
                    args.Add("status");
                    args.Add($"{name}.service");

                    string cmdResult = CLIExtension.Execute(args);

                    if (!string.IsNullOrEmpty(cmdResult))
                    {
                        var lines = cmdResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lines.Length > 0)
                        {
                            result = new ServiceProcessInfo();
                            result.ProcessIDs = new List<int>();
                            foreach (var line in lines)
                            {
                                if (line.Trim(new char[] { ' ' }).StartsWith("main pid:", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var tempVals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    int pid = 0;
                                    foreach (var tempVal in tempVals)
                                    {
                                        if (int.TryParse(tempVal, out pid))
                                        {
                                            break;
                                        }
                                    }
                                    if (pid > 0)
                                    {
                                        result.MainProcessID = pid;
                                        result.ProcessIDs.Add(pid);
                                    }
                                }
                                else if (line.Trim(new char[] { ' ' }).StartsWith("├─", StringComparison.InvariantCultureIgnoreCase)
                                || line.Trim(new char[] { ' ' }).StartsWith("└─", StringComparison.InvariantCultureIgnoreCase)) //
                                {
                                    var tempVals = line.Split(new char[] { ' ', '└', '├', '─' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tempVals?.Length > 1)
                                    {

                                        int pid = 0;
                                        foreach (var tempVal in tempVals)
                                        {
                                            if (int.TryParse(tempVal, out pid))
                                            {
                                                break;
                                            }
                                        }
                                        if (pid > 0)
                                        {
                                            result.ProcessIDs.Add(pid);
                                        }
                                    }
                                }
                                else if (line.Trim(new char[] { ' ' }).StartsWith("tasks:", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var tempVals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    int nTasks = 0;
                                    foreach (var tempVal in tempVals)
                                    {
                                        if (int.TryParse(tempVal, out nTasks))
                                        {
                                            break;
                                        }
                                    }
                                    if (nTasks > 0)
                                    {
                                        result.NumberOfTasks = nTasks;
                                    }
                                }
                                else if (line.Trim(new char[] { ' ' }).StartsWith("memory:", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var tempVals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tempVals?.Length > 1)
                                    {
                                        var storageObj = CalculateExtension.ConvertToValue(tempVals[1], Models.EUnit.KB);
                                        if (storageObj != null)
                                        {
                                            result.WorkingSet = storageObj.Value;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //throw(ex);
            }
            return result;
        }

        protected override List<ServiceInfo> _GetServices(List<string> names = null, bool isPrefixName = false, bool includeWorkingSet = false)
        {
            List<ServiceInfo> result = new List<ServiceInfo>();
            try
            {
                List<string> args = new List<string>();
                args.Add("systemctl");
                args.Add("list-units -a");
                args.Add("--type=service");
                args.Add("--state=loaded");

                string cmdResult = CLIExtension.Execute(args);

                if (!string.IsNullOrEmpty(cmdResult))
                {
                    var lines = cmdResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        int unitIdx = 0, loadIdx = 0, activeIdx = 0, subIdx = 0, descriptionIdx = 0;
                        foreach (var line in lines.Distinct(StringComparer.InvariantCultureIgnoreCase))
                        {
                            if (line.IndexOf("unit", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                unitIdx = line.IndexOf("unit ", StringComparison.InvariantCultureIgnoreCase);
                                loadIdx = line.IndexOf("load ", StringComparison.InvariantCultureIgnoreCase);
                                activeIdx = line.IndexOf("active ", StringComparison.InvariantCultureIgnoreCase);
                                subIdx = line.IndexOf("sub ", StringComparison.InvariantCultureIgnoreCase);
                                descriptionIdx = line.IndexOf("description ", StringComparison.InvariantCultureIgnoreCase);

                            }
                            else
                            {
                                if (unitIdx > 0 && loadIdx > unitIdx)
                                {
                                    var nameVal = line.Substring(unitIdx, loadIdx - unitIdx - 1).Trim(' ');
                                    int extIdx = nameVal.IndexOf(".service", StringComparison.InvariantCultureIgnoreCase);
                                    if (extIdx > 0)
                                    {
                                        nameVal = nameVal.Substring(0, extIdx);
                                    }

                                    bool isValidName = false;
                                    if (names?.Count > 0)
                                    {
                                        if (isPrefixName)
                                        {
                                            foreach (var prefixName in names)
                                            {
                                                if (nameVal.StartsWith(prefixName, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    isValidName = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (names.Contains(nameVal, StringComparer.InvariantCultureIgnoreCase))
                                            {
                                                isValidName = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isValidName = true;
                                    }

                                    if (isValidName)
                                    {
                                        ServiceInfo serviceObj = new ServiceInfo();
                                        serviceObj.Name = nameVal;

                                        if (activeIdx > loadIdx)
                                        {
                                            var loadVal = line.Substring(loadIdx, activeIdx - loadIdx - 1).Trim(' ');

                                            if (subIdx > activeIdx)
                                            {
                                                var activeVal = line.Substring(activeIdx, subIdx - activeIdx - 1).Trim(' ');

                                                if (descriptionIdx > subIdx)
                                                {
                                                    var subVal = line.Substring(subIdx, descriptionIdx - subIdx - 1).Trim(' ');
                                                    if (subVal.Equals("running", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        serviceObj.Status = EServiceStatus.Running;
                                                    }
                                                    else if (subVal.Equals("starting", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        serviceObj.Status = EServiceStatus.Starting;
                                                    }
                                                    else if (subVal.Equals("stopping", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        serviceObj.Status = EServiceStatus.Stopping;
                                                    }
                                                    else
                                                    {
                                                        serviceObj.Status = EServiceStatus.Stopped;
                                                    }
                                                    serviceObj.Description = line.Substring(descriptionIdx).Trim(' ');
                                                }
                                            }
                                        }
                                        if (includeWorkingSet)
                                        {
                                            if (serviceObj.Status == EServiceStatus.Running)
                                            {
                                                var processInfoObj = _GetServiceProcessInfo(serviceObj.Name);
                                                if (processInfoObj != null)
                                                {
                                                    serviceObj.ProcessID = processInfoObj.MainProcessID;
                                                    serviceObj.SubProcessIDs = processInfoObj.ProcessIDs;
                                                    serviceObj.WorkingSet = processInfoObj.WorkingSet;
                                                    serviceObj.NumberOfTasks = processInfoObj.NumberOfTasks;
                                                }
                                            }

                                        }
                                        result.Add(serviceObj);
                                    }


                                }

                            }
                        }
                    }
                }
            }
            catch //(System.Exception ex) 
            {

            }
            return result;
        }
    }
}