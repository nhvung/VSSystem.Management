using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Management.Models;

namespace VSSystem.Management
{
    public interface IServerProcess
    {
        Task<List<PingInfo>> Ping(string hostNameOrAddress, int round = 1);
        Task<ServerInfo> GetServerInfo(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default);
    }
}
