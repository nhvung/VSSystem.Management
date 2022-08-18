using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class ServerInfo
    {
        string _HostName;
        public string HostName { get { return _HostName; } set { _HostName = value; } }

        string _IPAddress;
        public string IPAddress { get { return _IPAddress; } set { _IPAddress = value; } }
        string _OS;
        public string OS { get { return _OS; } set { _OS = value; } }

        RAMInfo _RAM;
        public RAMInfo RAM { get { return _RAM; } set { _RAM = value; } }

        List<CPUInfo> _CPUs;
        public List<CPUInfo> CPUs { get { return _CPUs; } set { _CPUs = value; } }
        CPUUsageInfo _CPUUsage;
        public CPUUsageInfo CPUUsage { get { return _CPUUsage; } set { _CPUUsage = value; } }

        List<DriveInfo> _Drives;
        public List<DriveInfo> Drives { get { return _Drives; } set { _Drives = value; } }
        public ServerInfo()
        {
            _HostName = string.Empty;
            _IPAddress = string.Empty;
            _RAM = new RAMInfo();
            _CPUs = new List<CPUInfo>();
            _Drives = new List<DriveInfo>();
            _OS = string.Empty;
            _CPUUsage = new CPUUsageInfo();
        }
    }
}
