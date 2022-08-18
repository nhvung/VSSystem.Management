using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Management.Models
{
    public class AuthenticationInfo
    {

        string _HostNameOrAddress;
        public string HostNameOrAddress { get { return _HostNameOrAddress; } set { _HostNameOrAddress = value; } }

        string _Username;
        public string Username { get { return _Username; } set { _Username = value; } }

        string _Password;
        public string Password { get { return _Password; } set { _Password = value; } }

        int _Port;
        public int Port { get { return _Port; } set { _Port = value; } }


        int _Timeout;
        public int Timeout { get { return _Timeout; } set { _Timeout = value; } }

        public AuthenticationInfo(string hostNameOrAddress = "", string username = "", string password = "", int port = 0, int defaultTimeout = 60)
        {
            _HostNameOrAddress = hostNameOrAddress;
            _Username = username;
            _Password = password;
            _Port = port;
            _Timeout = defaultTimeout;
        }
    }
}
