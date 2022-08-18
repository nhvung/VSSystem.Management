using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Management.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using System.Diagnostics;

namespace VSSystem.Management.Windows
{
    public class ServerProcess : AServerProcess
    {
        public ServerProcess() : base()
        {
        }

        public override Task<ServerInfo> GetServerInfo(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            ServerInfo result = new ServerInfo();
            result.HostName = System.Net.Dns.GetHostName();
            result.OS = "Windows";
            try
            {
                var ipv4Addresses = new List<string>();

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    var ipProps = ni.GetIPProperties();

                    var gwObj = ipProps?.GatewayAddresses?.FirstOrDefault();
                    if (gwObj != null && !gwObj.Address.ToString().Equals("0.0.0.0"))
                    {
                        if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                            || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            foreach (UnicastIPAddressInformation ipObj in ipProps.UnicastAddresses)
                            {
                                if (ipObj.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    ipv4Addresses.Add(ipObj.Address.ToString());
                                }
                            }
                        }
                    }
                }

                if (ipv4Addresses?.Count > 0)
                {
                    result.IPAddress = string.Join("|", ipv4Addresses);
                }
            }
            catch { }

            try
            {
                var cpuTask = _GetCPUInfo();
                var cpuUsageTask = _GetCPUUsageInfo();
                var ramTask = _GetRAMInfo();
                var driveTask = _GetDrivesInfo();

                Task.WaitAll(cpuTask, cpuUsageTask, ramTask, driveTask);

                result.CPUs = cpuTask.Result;
                result.RAM = ramTask.Result;
                result.Drives = driveTask.Result;
                result.CPUUsage = cpuUsageTask.Result;
            }
            catch (Exception ex)
            {
                errorLogAction?.Invoke(ex);
            }
            return Task.FromResult(result);
        }
        Task<List<CPUInfo>> _GetCPUInfo()
        {

            List<CPUInfo> result = new List<CPUInfo>();
            try
            {
                string query = "select Name, NumberOfCores, NumberOfLogicalProcessors from Win32_Processor";
                using (ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(new ObjectQuery(query)))
                {
                    using (ManagementObjectCollection mObjs = moSearcher.Get())
                    {
                        foreach (ManagementObject mObj in mObjs)
                        {
                            var name = mObj["Name"]?.ToString();
                            var nCores = Convert.ToInt32(mObj["NumberOfCores"] ?? 0);
                            var nProcessors = Convert.ToInt32(mObj["NumberOfLogicalProcessors"] ?? 0);

                            var cpuObj = new CPUInfo
                            {
                                Name = name,
                                NumberOfCores = nCores,
                                NumberOfThreads = nProcessors
                            };
                            result.Add(cpuObj);
                            mObj.Dispose();
                        }
                    }
                }

            }
            catch //(Exception e
            {
            }
            return Task.FromResult(result);
        }
        Task<RAMInfo> _GetRAMInfo()
        {

            RAMInfo result = new RAMInfo();
            try
            {
                string query = "select FreePhysicalMemory, TotalVisibleMemorySize from Win32_OperatingSystem";
                using (ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(new ObjectQuery(query)))
                {
                    using (ManagementObjectCollection mObjs = moSearcher.Get())
                    {
                        foreach (ManagementObject mObj in mObjs)
                        {
                            var freeRam = Convert.ToInt64(mObj["FreePhysicalMemory"] ?? 0);
                            var totalRam = Convert.ToInt64(mObj["TotalVisibleMemorySize"] ?? 0);

                            result = new RAMInfo
                            {
                                Free = new StorageInfo(freeRam, EUnit.KB),
                                Total = new StorageInfo(totalRam, EUnit.KB)
                            };
                            mObj.Dispose();
                            break;
                        }
                    }
                }
            }
            catch //(Exception e
            {
            }
            return Task.FromResult(result);
        }
        Task<List<DriveInfo>> _GetDrivesInfo()
        {
            List<DriveInfo> result = new List<DriveInfo>();
            try
            {
                var driveObjs = System.IO.DriveInfo.GetDrives();
                if (driveObjs?.Length > 0)
                {
                    foreach (var driveObj in driveObjs)
                    {
                        if (driveObj.DriveType != System.IO.DriveType.Fixed)
                        {
                            continue;
                        }
                        DriveInfo driveInfo = new DriveInfo(driveObj.Name.Replace("\\", ""), driveObj.VolumeLabel);
                        driveInfo.Total = new StorageInfo(driveObj.TotalSize, EUnit.Byte);
                        driveInfo.Free = new StorageInfo(driveObj.TotalFreeSpace, EUnit.Byte);
                        result.Add(driveInfo);
                    }
                }
            }
            catch //(Exception e
            {
            }
            return Task.FromResult(result);
        }

        Task<CPUUsageInfo> _GetCPUUsageInfo()
        {
            CPUUsageInfo result = new CPUUsageInfo();
            try
            {
                var cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
                //new PerformanceCounter("Processor", "% Processor Time", "_Total", Environment.MachineName);
                //new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(2000); //This avoid that answer always 0
                result.UsedPercentage = cpuCounter.NextValue();
                result.IdlePercentage = 100 - result.UsedPercentage;
            }
            catch //(Exception ex)
            {
            }
            return Task.FromResult(result);
        }

    }
}
