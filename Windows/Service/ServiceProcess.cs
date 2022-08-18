using System;
using System.Collections.Generic;
using VSSystem.Management.Models.Service;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using VSSystem.Management.Extensions;
using System.Management;

namespace VSSystem.Management.Windows
{
    public class ServiceProcess : AServiceProcess
    {
        protected override bool _ActionService(string name, EServiceAction action)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && action != EServiceAction.DoNotThing)
                {
                    CLIExtension.Execute("net.exe", new List<string>() { $"{action.ToString().ToLower()} {name}" }, false);
                    if (action == EServiceAction.Stop)
                    {
                        var processInfoObj = _GetServiceProcessInfo(name);
                        if (processInfoObj != null)
                        {
                            if (processInfoObj.ProcessIDs?.Count > 0)
                            {
                                foreach (var pid in processInfoObj.ProcessIDs.Distinct())
                                {
                                    CLIExtension.Execute("taskkill.exe", new List<string>() { $"/f /pid {pid}" }, false);
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
                List<string> args = new List<string>();
                args.Add("/fi \"SERVICES eq " + name + "*\"");
                args.Add("/fo list");

                var cmdResult = CLIExtension.Execute("tasklist.exe", args, false);
                if (!string.IsNullOrWhiteSpace(cmdResult))
                {
                    var lines = cmdResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        result = new ServiceProcessInfo();
                        result.ProcessIDs = new List<int>();
                        foreach (var line in lines)
                        {
                            if (line.Trim(new char[] { ' ' }).StartsWith("pid:", StringComparison.InvariantCultureIgnoreCase))
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
                            else if (line.Trim(new char[] { ' ' }).StartsWith("mem usage:", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var tempVals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (tempVals?.Length > 2)
                                {
                                    var storageObj = CalculateExtension.ConvertToValue(tempVals[2] + "K", Models.EUnit.KB);
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
            catch { }
            return result;
        }

        protected override List<ServiceInfo> _GetServices(List<string> names, bool isPrefixName, bool includeWorkingSet)
        {
            List<ServiceInfo> result = new List<ServiceInfo>();
            try
            {

                var scObjs = System.ServiceProcess.ServiceController.GetServices();
                if (scObjs?.Length > 0)
                {
                    foreach (var scObj in scObjs)
                    {
                        string nameVal = scObj.ServiceName;
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
                            serviceObj.Description = scObj.DisplayName;

                            string state = scObj.Status.ToString();
                            if (state.Equals("running", StringComparison.InvariantCultureIgnoreCase))
                            {
                                serviceObj.Status = EServiceStatus.Running;
                            }
                            else if (state.Equals("starting", StringComparison.InvariantCultureIgnoreCase))
                            {
                                serviceObj.Status = EServiceStatus.Starting;
                            }
                            else if (state.Equals("stopping", StringComparison.InvariantCultureIgnoreCase))
                            {
                                serviceObj.Status = EServiceStatus.Stopping;
                            }
                            else
                            {
                                serviceObj.Status = EServiceStatus.Stopped;
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
            catch { }
            return result;
        }


    }
}