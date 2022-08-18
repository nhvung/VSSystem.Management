using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VSSystem.Management.Models;
using VSSystem.Management.Extensions;
using System.Net;
using System.Net.NetworkInformation;

namespace VSSystem.Management.Ubuntu
{
    public partial class ServerProcess : AServerProcess
    {
        public ServerProcess() : base()
        {
        }

        Task<List<CPUInfo>> _GetCPUInfo()
        {
            List<CPUInfo> result = new List<CPUInfo>();
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo("/proc/cpuinfo");
                if (file.Exists)
                {
                    using (var sr = file.OpenText())
                    {
                        string cpuName = "";
                        int cpuCores = 0, cpuThreads = 0;
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var temp = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp.Length > 1)
                            {
                                var key = temp[0].Trim(new char[] { '\t', ' ' });
                                var value = temp[1].Trim(new char[] { '\t', ' ' });

                                if (key.Equals("model name", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    cpuName = value;
                                }
                                else if (key.Equals("cpu cores", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int.TryParse(value, out cpuCores);
                                }
                                else if (key.Equals("siblings", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int.TryParse(value, out cpuThreads);
                                }
                            }

                        }
                        var cpuObj = new CPUInfo
                        {
                            Name = cpuName,
                            NumberOfCores = cpuCores,
                            NumberOfThreads = cpuThreads
                        };
                        result.Add(cpuObj);
                        sr.Close();
                        sr.Dispose();
                    }
                }
            }
            catch //(Exception ex)
            {
            }
            return Task.FromResult(result);
        }

        Task<CPUUsageInfo> _GetCPUUsageInfo()
        {
            CPUUsageInfo result = new CPUUsageInfo();
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo("/proc/stat");
                if (file.Exists)
                {
                    using (var sr = file.OpenText())
                    {
                        string line = null;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("cpu", StringComparison.InvariantCultureIgnoreCase))
                            {
                                long userVal = 0, niceVal = 0, systemVal = 0, idleVal = 0, iowaitVal = 0, irqVal = 0, softirqVal = 0, stealVal = 0, guestVal = 0, guestNiceVal = 0, total = 0;
                                var temp = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (temp.Length > 10)
                                {
                                    long.TryParse(temp[1], out userVal); total += userVal;
                                    long.TryParse(temp[2], out niceVal); total += niceVal;
                                    long.TryParse(temp[3], out systemVal); total += systemVal;
                                    long.TryParse(temp[4], out idleVal); total += idleVal;
                                    long.TryParse(temp[5], out iowaitVal); total += iowaitVal;
                                    long.TryParse(temp[6], out irqVal); total += irqVal;
                                    long.TryParse(temp[7], out softirqVal); total += softirqVal;
                                    long.TryParse(temp[8], out stealVal); total += stealVal;
                                    long.TryParse(temp[9], out guestVal); total += guestVal;
                                    long.TryParse(temp[10], out guestNiceVal); total += guestNiceVal;
                                }

                                if (total > 0)
                                {
                                    result.IdlePercentage = idleVal * 100.0 / total;
                                    result.UsedPercentage = 100 - result.IdlePercentage;

                                }
                            }
                        }
                        sr.Close();
                        sr.Dispose();
                    }
                }
            }
            catch //(Exception ex)
            {
            }
            return Task.FromResult(result);
        }

        Task<RAMInfo> _GetRAMInfo()
        {
            RAMInfo result = new RAMInfo();
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo("/proc/meminfo");
                if (file.Exists)
                {
                    using (var sr = file.OpenText())
                    {
                        double total = 0, free = 0, buffer = 0, cached = 0, available = 0;
                        EUnit unit = EUnit.Byte;
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var temp = line.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp.Length > 2)
                            {
                                var key = temp[0].Trim(new char[] { '\t', ' ' });
                                var value = temp[1].Trim(new char[] { '\t', ' ' });
                                var sUnit = temp[2].Trim(new char[] { '\t', ' ' });
                                Enum.TryParse(sUnit, true, out unit);

                                if (key.Equals("MemTotal", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    double.TryParse(value, out total);
                                }
                                else if (key.Equals("MemFree", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    double.TryParse(value, out free);
                                }
                                else if (key.Equals("Buffers", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    double.TryParse(value, out buffer);
                                }
                                else if (key.Equals("Cached", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    double.TryParse(value, out cached);
                                }
                                else if (key.Equals("MemAvailable", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    double.TryParse(value, out available);
                                }
                            }
                        }
                        result = new RAMInfo
                        {
                            Free = new StorageInfo(free + cached + buffer, unit),
                            Total = new StorageInfo(total, unit)
                        };
                        sr.Close();
                        sr.Dispose();

                    }
                }
            }
            catch { }
            return Task.FromResult(result);
        }

        public override Task<ServerInfo> GetServerInfo(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            ServerInfo result = new ServerInfo();
            result.HostName = System.Net.Dns.GetHostName();
            result.OS = "Linux";

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

            DateTime bTime = DateTime.Now;
            try
            {
                var cpuTask = _GetCPUInfo();
                var cpuUsageTask = _GetCPUUsageInfo();

                var ramTask = _GetRAMInfo();

                var driveTask = _GetDrivesInfo();

                Task.WaitAll(cpuTask, ramTask, driveTask, cpuUsageTask);

                result.CPUs = cpuTask.Result;
                result.RAM = ramTask.Result;
                result.Drives = driveTask.Result;
                result.CPUUsage = cpuUsageTask.Result;
            }
            catch //(Exception ex)
            {

            }

            var ts = DateTime.Now - bTime;
            return Task.FromResult(result);
        }

        Task<List<VSSystem.Management.Models.DriveInfo>> _GetDrivesInfo()
        {
            List<VSSystem.Management.Models.DriveInfo> result = new List<VSSystem.Management.Models.DriveInfo>();
            try
            {

                List<string> args = new List<string>() { "df -h | grep ^/dev/sd" };

                string cmdResult = CLIExtension.Execute(args);

                if (!string.IsNullOrEmpty(cmdResult))
                {
                    var lines = cmdResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        string name = "", label = "";
                        foreach (var line in lines.Distinct(StringComparer.InvariantCultureIgnoreCase))
                        {
                            var temp = line.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            name = temp[0];
                            label = temp[5];
                            if (label == "/")
                            {
                                label = string.Empty;
                            }

                            VSSystem.Management.Models.DriveInfo driveInfoObj = new VSSystem.Management.Models.DriveInfo(name, label);

                            driveInfoObj.Total = CalculateExtension.ConvertToValue(temp[1]); // total
                            driveInfoObj.Free = CalculateExtension.ConvertToValue(temp[3]); // free

                            result.Add(driveInfoObj);

                        }
                    }
                }

            }
            catch //(Exception ex)
            {
            }
            return Task.FromResult(result);
        }

        public virtual void UpdateNetworkShare(string sharePath, string shareUsername, string sharePassword, string localPath, string localUsername, string localGroup
        , Action<string> debugLog = default, Action<Exception> errorLog = default)
        {
            try
            {
                System.IO.DirectoryInfo localFolder = new System.IO.DirectoryInfo(localPath);
                if (!localFolder.Exists)
                {
                    localFolder.Create();
                    debugLog?.Invoke("Create new folder " + localFolder.FullName);
                }


                debugLog?.Invoke("Updating fstab...");
                System.IO.FileInfo fstabFile = new System.IO.FileInfo("/etc/fstab");


                List<string> lines = System.IO.File.ReadAllLines(fstabFile.FullName)?.ToList();

                string updateLine = string.Format("{0} {1} cifs user={2},pass={3},uid={4},gid={5} 0 0", sharePath.Replace("\\", "/"), localPath.Replace("\\", "/"), shareUsername, sharePassword, localUsername, localGroup);
                if (!lines.Contains(updateLine, StringComparer.InvariantCultureIgnoreCase))
                {
                    lines.Add(updateLine);

                    System.IO.File.WriteAllLines(fstabFile.FullName, lines, Encoding.UTF8);

                    using (var p = Process.Start("/bin/bash", "-c \"mount -a\""))
                    {
                        p.WaitForExit();
                        p.Close();
                        p.Dispose();
                    }

                }

                debugLog?.Invoke("Update fstab done.");
            }
            catch (Exception ex)
            {
                errorLog?.Invoke(ex);
            }
        }
    }
}
