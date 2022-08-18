namespace VSSystem.Management.Windows.IIS
{
    public class WebApplicationPoolInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _Username;
        public string Username { get { return _Username; } set { _Username = value; } }

        string _Password;
        public string Password { get { return _Password; } set { _Password = value; } }

        int _ApplicationCount;
        public int ApplicationCount { get { return _ApplicationCount; } set { _ApplicationCount = value; } }

        string _RuntimeVersion;
        /// <summary>
        /// v2.0, v4.0, No
        /// </summary>
        public string RuntimeVersion { get { return _RuntimeVersion; } set { _RuntimeVersion = value; } }

        WebManagedPipelineMode _PipelineMode;
        public WebManagedPipelineMode PipelineMode { get { return _PipelineMode; } set { _PipelineMode = value; } }

        WebApplicationPoolState _State;
        public WebApplicationPoolState State { get { return _State; } set { _State = value; } }


        IdentityType _Identity;
        public IdentityType Identity { get { return _Identity; } set { _Identity = value; } }
        float _RAM;
        public float RAM { get { return _RAM; } set { _RAM = value; } }
        float _CPU;
        public float CPU { get { return _CPU; } set { _CPU = value; } }
        float _PrivateMemorySize;
        public float PrivateMemorySize { get { return _PrivateMemorySize; } set { _PrivateMemorySize = value; } }

        float _VirtualMemorySize;
        public float VirtualMemorySize { get { return _VirtualMemorySize; } set { _VirtualMemorySize = value; } }
        public override string ToString()
        {
            return string.Format("Name: {0}, Username: {1}, Application: {2}, Runtime: {3}, {4}, {5}, {6}", _Name, _Username, _ApplicationCount, _RuntimeVersion, _PipelineMode, _State, _Identity);
        }

        public WebApplicationPoolInfo()
        {
            _Name = string.Empty;
            _Username = string.Empty;
            _Password = string.Empty;
            _RuntimeVersion = "v4.0";
            _PipelineMode = WebManagedPipelineMode.Integrated;
            _Identity = IdentityType.ApplicationPoolIdentity;

        }
    }
}