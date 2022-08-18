using System;
using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Management.Models;

namespace VSSystem.Management
{
    public abstract class AServerProcess : IServerProcess
    {
        protected AServerProcess()
        {
        }
        async Task<PingInfo> _Ping(string hostNameOrAddress)
        {
            try
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync(hostNameOrAddress);
                return new PingInfo(pingReply);
            }
            catch { }
            return default;
        }

        public async Task<List<PingInfo>> Ping(string hostNameOrAddress, int round = 1)
        {
            List<PingInfo> result = new List<PingInfo>();
            try
            {
                for (int i = 0; i < round; i++)
                {
                    var pingInfo = await _Ping(hostNameOrAddress);
                    result.Add(pingInfo);
                }
            }
            catch { }
            return result;
        }

        public abstract Task<ServerInfo> GetServerInfo(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default);
    }
}
