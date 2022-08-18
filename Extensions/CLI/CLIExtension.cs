using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;

namespace VSSystem.Management.Extensions
{
    public class CLIExtension
    {
        protected string _FileName;
        public string FileName { get { return _FileName; } set { _FileName = value; } }
        protected string _prefixCommand;
        public CLIExtension(string fileName)
        {
            _FileName = fileName;
            _prefixCommand = string.Empty;
        }
        public string ExecuteCommand(List<string> args, bool includeQuot = true)
        {
            return _Execute(args, includeQuot);
        }
        protected virtual string _Execute(List<string> args, bool includeQuot = true)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(_FileName))
                {
                    return result;
                }
                if (args?.Count > 0)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = _FileName;
                    string arguments = string.Empty;
                    if (!string.IsNullOrWhiteSpace(_prefixCommand))
                    {
                        arguments += _prefixCommand;
                    }
                    if (includeQuot)
                    {
                        arguments += string.Format(" \"{0}\"", string.Join(" ", args));
                    }
                    else
                    {
                        arguments += string.Format(" {0}", string.Join(" ", args));
                    }
                    psi.Arguments = arguments;
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    using (var p = new System.Diagnostics.Process())
                    {
                        p.StartInfo = psi;
                        p.Start();
                        result = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();
                        p.Close();
                    }
                }
            }
            catch //(Exception ex)
            {

            }
            return result;
        }

        #region Static Methods
        static CLIExtension _CLI = null;
        static void _Init()
        {
            try
            {
                if (Environment.OSVersion?.VersionString?.IndexOf("Microsoft Windows", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    _CLI = new CmdExtension();
                }
                else
                {
                    _CLI = new BashExtension();
                }
            }
            catch { }
        }
        static public string Execute(List<string> args, bool includeQuot = true)
        {
            try
            {
                if (_CLI == null)
                {
                    _Init();
                }
                if (_CLI != null)
                {
                    return _CLI._Execute(args, includeQuot);
                }
            }
            catch { }
            return string.Empty;
        }
        static public string Execute(string fileName, List<string> args, bool includeQuot = true)
        {
            try
            {
                return new CLIExtension(fileName)._Execute(args, includeQuot);
            }
            catch { }
            return string.Empty;
        }
        #endregion

    }
}