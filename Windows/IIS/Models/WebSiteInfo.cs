using System.Collections.Generic;
namespace VSSystem.Management.Windows.IIS
{
    public class WebSiteInfo
    {
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _PhysicalPath;
        public string PhysicalPath { get { return _PhysicalPath; } set { _PhysicalPath = value; } }

        List<WebApplicationInfo> _Applications;
        public List<WebApplicationInfo> Applications { get { return _Applications; } set { _Applications = value; } }

        WebApplicationInfo _MainApplication;
        public WebApplicationInfo MainApplication { get { return _MainApplication; } set { _MainApplication = value; } }

        public WebSiteInfo()
        {
            _Name = string.Empty;
            _PhysicalPath = string.Empty;
            _Applications = new List<WebApplicationInfo>();
            _MainApplication = new WebApplicationInfo();
        }
    }
}