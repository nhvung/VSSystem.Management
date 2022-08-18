using System.Collections.Generic;
using System.Diagnostics;
using System;
namespace VSSystem.Management.Extensions
{
    public class CmdExtension : CLIExtension
    {
        public CmdExtension() : base(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe")
        {

        }
    }
}