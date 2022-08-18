namespace VSSystem.Management.Windows.IIS
{
    public class WebApplicationInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _Path;
        public string Path { get { return _Path; } set { _Path = value; } }

        string _PhysicalPath;
        public string PhysicalPath { get { return _PhysicalPath; } set { _PhysicalPath = value; } }

        string _SiteName;
        public string SiteName { get { return _SiteName; } set { _SiteName = value; } }

        string _ApplicationPoolName;
        public string ApplicationPoolName { get { return _ApplicationPoolName; } set { _ApplicationPoolName = value; } }

    }
}
