using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Web.Administration;

namespace VSSystem.Management.Windows.IIS
{
    public class IISProcess
    {
        public static List<WebSiteInfo> GetIISWebSites(string[] names = null, bool isPreName = false, bool includeApplication = true)
        {
            try
            {
                List<WebSiteInfo> webSites = new List<WebSiteInfo>();
                string systemDrive = Environment.GetEnvironmentVariable("systemdrive");
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    foreach (Site site in iis.Sites)
                    {
                        WebSiteInfo wSite = new WebSiteInfo();
                        wSite.Name = site.Name;
                        wSite.PhysicalPath = site.Applications["/"].VirtualDirectories["/"].PhysicalPath;
                        if (wSite.PhysicalPath.StartsWith("%systemdrive%", StringComparison.InvariantCultureIgnoreCase))
                            wSite.PhysicalPath = systemDrive + wSite.PhysicalPath.Substring("%systemdrive%".Length);


                        if (names == null || names.Length == 0 || (!isPreName && names.Contains(wSite.Name, StringComparer.InvariantCultureIgnoreCase)) || (isPreName && names.Any(name => wSite.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            if (includeApplication)
                            {
                                List<WebApplicationInfo> webApplications = new List<WebApplicationInfo>();
                                foreach (Application app in site.Applications)
                                {

                                    try
                                    {
                                        WebApplicationInfo wApp = new WebApplicationInfo();
                                        wApp.Path = app.Path;
                                        wApp.SiteName = site.Name;
                                        wApp.ApplicationPoolName = app.ApplicationPoolName;
                                        wApp.PhysicalPath = app.VirtualDirectories["/"].PhysicalPath;

                                        if (wApp.PhysicalPath.StartsWith("%systemdrive%", StringComparison.InvariantCultureIgnoreCase))
                                            wApp.PhysicalPath = systemDrive + wApp.PhysicalPath.Substring("%systemdrive%".Length);

                                        if (app.Path == "/")
                                        {
                                            wApp.Name = site.Name;
                                            wSite.MainApplication = wApp;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                DirectoryInfo workingFolder = new DirectoryInfo(wApp.PhysicalPath);
                                                wApp.Name = workingFolder.Name;
                                            }
                                            catch { wApp.Name = Path.GetFileNameWithoutExtension(wApp.Path); }
                                            if (names == null || names.Length == 0 || (!isPreName && names.Contains(wApp.Name, StringComparer.InvariantCultureIgnoreCase)) || (isPreName && names.Any(name => wApp.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))))
                                                webApplications.Add(wApp);
                                        }
                                    }
                                    catch { }
                                }
                                wSite.Applications = webApplications.OrderBy(ite => ite.Name).ToList();
                            }
                            webSites.Add(wSite);
                        }
                    }
                }
                return webSites;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<WebApplicationInfo> GetIISWebApplications(string[] names = null, bool isPreName = false)
        {
            try
            {
                List<WebApplicationInfo> webApplications = new List<WebApplicationInfo>();
                string systemDrive = Environment.GetEnvironmentVariable("systemdrive");
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    foreach (Site site in iis.Sites)
                    {
                        foreach (Application app in site.Applications)
                        {
                            WebApplicationInfo wApp = new WebApplicationInfo();
                            wApp.Path = app.Path;
                            wApp.SiteName = site.Name;
                            wApp.ApplicationPoolName = app.ApplicationPoolName;
                            wApp.PhysicalPath = app.VirtualDirectories["/"].PhysicalPath;

                            if (wApp.PhysicalPath.StartsWith("%systemdrive%", StringComparison.InvariantCultureIgnoreCase))
                                wApp.PhysicalPath = systemDrive + wApp.PhysicalPath.Substring("%systemdrive%".Length);

                            if (app.Path == "/")
                                wApp.Name = site.Name;
                            else
                            {
                                try
                                {
                                    DirectoryInfo workingFolder = new DirectoryInfo(wApp.PhysicalPath);
                                    wApp.Name = workingFolder.Name;
                                }
                                catch { wApp.Name = Path.GetFileNameWithoutExtension(wApp.Path); }
                            }

                            if (names == null || (!isPreName && names.Contains(wApp.Name, StringComparer.InvariantCultureIgnoreCase)) || (isPreName && names.Any(name => wApp.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))))
                                webApplications.Add(wApp);
                        }
                    }
                }
                webApplications = webApplications.OrderBy(ite => ite.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
                iis.Dispose();
                return webApplications;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void StopIISWebSites(params string[] siteNames)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    foreach (string siteName in siteNames)
                    {
                        Site site = iis.Sites[siteName];
                        if (site != null)
                        {
                            site.Stop();
                        }
                    }
                }
                iis.CommitChanges();
                iis.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void StartIISWebSites(params string[] siteNames)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    foreach (string siteName in siteNames)
                    {
                        Site site = iis.Sites[siteName];
                        if (site != null)
                        {
                            site.Start();
                        }
                    }
                }
                iis.CommitChanges();
                iis.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void AddIISWebApplication(WebApplicationInfo webApplication)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    Site site = iis.Sites[webApplication.SiteName];
                    if (site != null)
                    {
                        Application app = site.Applications[webApplication.Path];
                        if (app == null)
                        {
                            app = site.Applications.Add(webApplication.Path, webApplication.PhysicalPath);
                        }
                        app.VirtualDirectories["/"].PhysicalPath = webApplication.PhysicalPath;
                        app.ApplicationPoolName = webApplication.ApplicationPoolName;
                    }
                }
                iis.CommitChanges();
                iis.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<WebApplicationPoolInfo> GetIISApplicationPools()
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                List<WebApplicationPoolInfo> applicationPools = new List<WebApplicationPoolInfo>();

                if (iis.ApplicationPools.Count > 0)
                {
                    foreach (var appPool in iis.ApplicationPools)
                    {
                        WebApplicationPoolInfo applicationPoolInfo = new WebApplicationPoolInfo()
                        {
                            Name = appPool.Name,
                            Identity = (IdentityType)appPool.ProcessModel.IdentityType,
                            Username = appPool.ProcessModel.UserName,
                            Password = appPool.ProcessModel.Password,
                            State = (WebApplicationPoolState)appPool.State,
                            PipelineMode = (WebManagedPipelineMode)appPool.ManagedPipelineMode,
                            RuntimeVersion = appPool.ManagedRuntimeVersion
                        };
                        applicationPools.Add(applicationPoolInfo);
                    }
                }
                iis.Dispose();
                applicationPools = applicationPools.OrderBy(ite => ite.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
                return applicationPools;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<WebApplicationPoolInfo> GetIISApplicationPools(string[] names, bool isPrefixName = false, bool loadWorkingSet = false)
        {
            try
            {
                string systemDrive = Environment.GetEnvironmentVariable("systemdrive");
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                List<WebApplicationPoolInfo> applicationPools = new List<WebApplicationPoolInfo>();

                if (iis.ApplicationPools.Count > 0)
                {
                    foreach (var appPool in iis.ApplicationPools)
                    {
                        WebApplicationPoolInfo applicationPoolInfo = new WebApplicationPoolInfo()
                        {
                            Name = appPool.Name,
                            Identity = (IdentityType)appPool.ProcessModel.IdentityType,
                            Username = appPool.ProcessModel.UserName,
                            Password = appPool.ProcessModel.Password,
                            State = (WebApplicationPoolState)appPool.State,
                            PipelineMode = (WebManagedPipelineMode)appPool.ManagedPipelineMode,
                            RuntimeVersion = appPool.ManagedRuntimeVersion
                        };
                        if (names == null || (!isPrefixName && names.Contains(applicationPoolInfo.Name, StringComparer.InvariantCultureIgnoreCase)) || (isPrefixName && names.Any(name => applicationPoolInfo.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            if (loadWorkingSet)
                            {
                                foreach (var ite in appPool.WorkerProcesses)
                                {
                                    if (ite.ProcessId <= 0) continue;
                                    try
                                    {
                                        using (Process p = Process.GetProcessById(ite.ProcessId))
                                        {
                                            p.Refresh();
                                            applicationPoolInfo.PrivateMemorySize = p.PrivateMemorySize64;
                                            applicationPoolInfo.VirtualMemorySize = p.VirtualMemorySize64;
                                            using (PerformanceCounter pc = new PerformanceCounter("Process", "Working Set - Private", p.ProcessName, true))
                                            {
                                                applicationPoolInfo.RAM += pc.NextValue();
                                                pc.Close();
                                                pc.Dispose();
                                            }
                                            using (PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", p.ProcessName, true))
                                            {
                                                applicationPoolInfo.CPU += pc.NextValue();
                                                pc.Close();
                                                pc.Dispose();
                                            }
                                            p.Dispose();
                                        }
                                    }
                                    catch (Exception ex) { throw ex; }
                                }
                            }

                            applicationPools.Add(applicationPoolInfo);
                        }
                    }

                }
                iis.Dispose();
                applicationPools = applicationPools.OrderBy(ite => ite.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
                return applicationPools;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void StopIISApplicationPools(params string[] applicationPoolNames)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.ApplicationPools.Count > 0)
                {
                    Task[] tasks = new Task[applicationPoolNames.Length];
                    int idx = 0;
                    foreach (string applicationPoolName in applicationPoolNames)
                    {
                        tasks[idx] = new Task(() =>
                        {

                            try
                            {
                                ApplicationPool applicationPool = iis.ApplicationPools[applicationPoolName];
                                if (applicationPool != null)
                                {
                                    try
                                    {
                                        ObjectState state = applicationPool.Stop();
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        });
                        tasks[idx].Start();
                        idx++;
                    }
                    Task.WaitAll(tasks);
                }
                iis.CommitChanges();
                iis.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void StartIISApplicationPools(params string[] applicationPoolNames)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.ApplicationPools.Count > 0)
                {
                    Task[] tasks = new Task[applicationPoolNames.Length];
                    int idx = 0;
                    foreach (string applicationPoolName in applicationPoolNames)
                    {
                        tasks[idx] = new Task(() =>
                        {

                            try
                            {
                                ApplicationPool applicationPool = iis.ApplicationPools[applicationPoolName];
                                if (applicationPool != null)
                                {
                                    try
                                    {
                                        ObjectState state = applicationPool.Start();
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        });
                        tasks[idx].Start();
                        idx++;
                    }
                    Task.WaitAll(tasks);
                }
                iis.CommitChanges();
                iis.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RecycleIISApplicationPools(params string[] applicationPoolNames)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.ApplicationPools.Count > 0)
                {
                    Task[] tasks = new Task[applicationPoolNames.Length];
                    int idx = 0;
                    foreach (string applicationPoolName in applicationPoolNames)
                    {
                        tasks[idx] = new Task(() =>
                        {

                            try
                            {
                                ApplicationPool applicationPool = iis.ApplicationPools[applicationPoolName];
                                if (applicationPool != null)
                                {
                                    try
                                    {
                                        ObjectState state = applicationPool.Recycle();
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        });
                        tasks[idx].Start();
                        idx++;
                    }
                    Task.WaitAll(tasks);

                }
                iis.CommitChanges();
                iis.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void AddIISApplicationPools(WebApplicationPoolInfo applicationPoolInfo)
        {
            try
            {
                FileInfo iisConfigFile = new FileInfo(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iisConfigFile.Exists)
                {
                    ServerManager iis = new ServerManager(iisConfigFile.FullName);
                    if (iis.ApplicationPools.Count > 0)
                    {
                        ApplicationPool applicationPool = iis.ApplicationPools[applicationPoolInfo.Name];

                        if (applicationPool == null)
                        {
                            applicationPool = iis.ApplicationPools.Add(applicationPoolInfo.Name);
                        }
                        applicationPool.ProcessModel.IdentityType = (ProcessModelIdentityType)applicationPoolInfo.Identity;
                        applicationPool.ManagedRuntimeVersion = applicationPoolInfo.RuntimeVersion;
                        applicationPool.ManagedPipelineMode = (ManagedPipelineMode)applicationPoolInfo.PipelineMode;
                        if (!string.IsNullOrEmpty(applicationPoolInfo.Username) && !string.IsNullOrEmpty(applicationPoolInfo.Password))
                        {
                            applicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                            applicationPool.ProcessModel.UserName = applicationPoolInfo.Username;
                            applicationPool.ProcessModel.Password = applicationPoolInfo.Password;
                        }
                    }
                    iis.CommitChanges();
                    iis.Dispose();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ChangeAuthenticationIISApplicationPools(string username, string password, params string[] names)
        {
            try
            {
                if (names?.Length > 0)
                {
                    ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                    if (iis.ApplicationPools.Count > 0)
                    {
                        foreach (string name in names)
                        {
                            ApplicationPool applicationPool = iis.ApplicationPools[name];

                            if (applicationPool == null)
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                            {
                                applicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                                applicationPool.ProcessModel.UserName = username;
                                applicationPool.ProcessModel.Password = password;
                            }
                        }

                    }
                    iis.CommitChanges();
                    iis.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void AddIISWebSite(WebSiteInfo webSiteInfo, int port, int sslPort, string username = null, string password = null)
        {
            try
            {
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                Site site = iis.Sites[webSiteInfo.Name];

                if (site == null)
                {
                    site = iis.Sites.Add(webSiteInfo.Name, webSiteInfo.PhysicalPath, port);
                }

                Dictionary<string, byte[]> certs = GetIISCertificates();
                var cert = certs.FirstOrDefault();

                site.ApplicationDefaults.EnabledProtocols = "http,https";

                ApplicationPool applicationPool = iis.ApplicationPools[webSiteInfo.Name];
                if (applicationPool == null)
                {
                    applicationPool = iis.ApplicationPools.Add(webSiteInfo.Name);
                }
                applicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.LocalSystem;
                applicationPool.ManagedRuntimeVersion = "v4.0";
                applicationPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    applicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                    applicationPool.ProcessModel.UserName = username;
                    applicationPool.ProcessModel.Password = password;
                }

                try
                {
                    site.Bindings.Add("*:" + sslPort + ":", cert.Value, cert.Key);
                }
                catch { site.Bindings.Add("*:" + sslPort + ":", "https"); }

                site.ApplicationDefaults.ApplicationPoolName = webSiteInfo.Name;
                site.ServerAutoStart = true;


                iis.CommitChanges();
                iis.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static Dictionary<string, byte[]> GetIISCertificates()
        {
            try
            {
                Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
                string systemDrive = Environment.GetEnvironmentVariable("systemdrive");
                ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                if (iis.Sites.Count > 0)
                {
                    foreach (var site in iis.Sites)
                    {
                        foreach (var binding in site.Bindings)
                        {
                            if (binding.Protocol.StartsWith("https", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (binding.CertificateHash != null)
                                {
                                    result[binding.CertificateStoreName] = binding.CertificateHash;
                                }

                            }
                        }
                    }
                }
                if (result.Count == 0)
                {
                    var kvCert = CreateSelfSignedCert();
                    result[kvCert.Key] = kvCert.Value;
                }

                iis.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static KeyValuePair<string, byte[]> CreateSelfSignedCert()
        {
            var guid = "e9567a1b-03e4-400a-978e-5a1f50842591";
            var binCert = Convert.FromBase64String("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");
            var x509Cert = new X509Certificate2(binCert, guid);

            X509Store x509 = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
            x509.Add(x509Cert);
            x509.Close();

            return new KeyValuePair<string, byte[]>("My", x509Cert.GetCertHash());
        }
        public static void RemoveWebSites(params string[] siteNames)
        {
            try
            {
                if (siteNames?.Length > 0)
                {
                    string systemDrive = Environment.GetEnvironmentVariable("systemdrive");
                    ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                    foreach (string siteName in siteNames)
                    {
                        Site site = iis.Sites[siteName];
                        if (site != null)
                        {
                            iis.Sites.Remove(site);
                        }
                    }
                    iis.CommitChanges();
                    iis.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RemoveIISApplicationPools(params string[] appPoolNames)
        {
            try
            {
                if (appPoolNames?.Length > 0)
                {
                    ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                    if (iis.ApplicationPools.Count > 0)
                    {
                        foreach (string apName in appPoolNames)
                        {
                            var appPool = iis.ApplicationPools[apName];
                            if (appPool != null) iis.ApplicationPools.Remove(appPool);
                        }
                    }
                    iis.CommitChanges();
                    iis.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RemoveIISWebApplications(string siteName, params string[] webAppNames)
        {
            try
            {
                if (webAppNames?.Length > 0)
                {
                    ServerManager iis = new ServerManager(Environment.SystemDirectory + "/inetsrv/config/applicationhost.config");
                    if (iis.Sites.Count > 0)
                    {
                        var webSite = iis.Sites[siteName];
                        if (webSite?.Applications?.Count > 0)
                        {
                            foreach (string wName in webAppNames)
                            {
                                var webApp = webSite.Applications["/" + wName];
                                if (webApp != null) webSite.Applications.Remove(webApp);
                            }
                        }
                    }
                    iis.CommitChanges();
                    iis.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
