using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace VSSystem.Management.Models
{
    public class PingInfo
    {

        string _IPAddress;
        public string IPAddress { get { return _IPAddress; } set { _IPAddress = value; } }

        int _Bytes;
        public int Bytes { get { return _Bytes; } set { _Bytes = value; } }

        long _RoundtripTime;
        public long RoundtripTime { get { return _RoundtripTime; } set { _RoundtripTime = value; } }

        int _TTL;
        public int TTL { get { return _TTL; } set { _TTL = value; } }

        string _Status;
        public string Status { get { return _Status; } set { _Status = value; } }

        public PingInfo(PingReply pingReply = default)
        {
            if(pingReply != null)
            {
                _IPAddress = pingReply.Address.ToString();
                _Bytes = pingReply.Buffer.Length;
                _RoundtripTime = pingReply.RoundtripTime;
                _TTL = pingReply.Options?.Ttl ?? 0;
                _Status = pingReply.Status.ToString();
            }
        }

        public override string ToString()
        {
            return $"Reply from {_IPAddress}: bytes={_Bytes} time={_RoundtripTime}ms TTL={_TTL}";
        }
    }
}
