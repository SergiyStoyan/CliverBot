////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        sergey_stoyan@yahoo.com
////        http://www.cliversoft.com
////        26 September 2006
////Copyright: (C) 2006, Sergey Stoyan
////********************************************************************************************
//using System;
//using System.Windows.Forms;
//using System.Diagnostics;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Threading;
//using System.Configuration;
//using System.Media;
//using System.Web;
//using System.Net.NetworkInformation;
//using System.Runtime.InteropServices;
//using NetFwTypeLib;


//namespace Cliver
//{
//    public static class FirewallRoutines
//    {
//        public sealed class NetFwAuthorizedApplication :
//            INetFwAuthorizedApplication
//        {
//            public string Name { get; set; }
//            public bool Enabled { get; set; }
//            public NET_FW_SCOPE_ Scope { get; set; }
//            public string RemoteAddresses { get; set; }
//            public string ProcessImageFileName { get; set; }
//            public NET_FW_IP_VERSION_ IpVersion { get; set; }

//            public NetFwAuthorizedApplication()
//            {
//                this.Name = "";
//                this.Enabled = false;
//                this.RemoteAddresses = "";
//                this.ProcessImageFileName = "";
//                this.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
//                this.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
//            }

//            public NetFwAuthorizedApplication(string name, bool enabled, string remoteAddresses, NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipVersion, string processImageFileName)
//            {
//                this.Name = name;
//                this.Scope = scope;
//                this.Enabled = enabled;
//                this.IpVersion = ipVersion;
//                this.RemoteAddresses = remoteAddresses;
//                this.ProcessImageFileName = processImageFileName;
//            }

//            public static NetFwAuthorizedApplication FromINetFwAuthorizedApplication(INetFwAuthorizedApplication application)
//            {
//                return (new NetFwAuthorizedApplication(application.Name, application.Enabled, application.RemoteAddresses, application.Scope, application.IpVersion, application.ProcessImageFileName));
//            }
//        }
//    }

//    namespace MySolution.Configurator.Firewall
//    {
//        using System;
//        using System.Collections.Generic;
//        using System.Globalization;
//        using System.Linq;
//        using NetFwTypeLib;

//        public static class FirewallUtilities
//        {
//            public static bool GetApplication(string processImageFileName, out INetFwAuthorizedApplication application, out Exception exception)
//            {
//                var result = false;
//                var comObjects = new Stack<object>();

//                exception = null;
//                application = null;

//                if (processImageFileName == null) { throw (new ArgumentNullException("processImageFileName")); }
//                if (processImageFileName.Trim().Length == 0) { throw (new ArgumentException("The argument [processImageFileName] cannot be empty.", "processImageFileName")); }

//                try
//                {
//                    var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", true);

//                    try
//                    {
//                        var manager = (INetFwMgr)Activator.CreateInstance(type);
//                        comObjects.Push(manager);

//                        try
//                        {
//                            var policy = manager.LocalPolicy;
//                            comObjects.Push(policy);

//                            var profile = policy.CurrentProfile;
//                            comObjects.Push(profile);

//                            var applications = profile.AuthorizedApplications;
//                            comObjects.Push(applications);

//                            foreach (INetFwAuthorizedApplication app in applications)
//                            {
//                                comObjects.Push(app);

//                                if (string.Compare(app.ProcessImageFileName, processImageFileName, true, CultureInfo.InvariantCulture) == 0)
//                                {
//                                    result = true;
//                                    application = NetFwAuthorizedApplication.FromINetFwAuthorizedApplication(app);

//                                    break;
//                                }
//                            }

//                            if (!result) { throw (new Exception("The requested application was not found.")); }
//                        }
//                        catch (Exception e)
//                        {
//                            exception = e;
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        exception = e;
//                    }
//                    finally
//                    {
//                        while (comObjects.Count > 0)
//                        {
//                            ComUtilities.ReleaseComObject(comObjects.Pop());
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    exception = e;
//                }
//                finally
//                {
//                }

//                return (result);
//            }

//            public static bool AddApplication(INetFwAuthorizedApplication application, out Exception exception)
//            {
//                var result = false;
//                var comObjects = new Stack<object>();

//                exception = null;

//                if (application == null) { throw (new ArgumentNullException("application")); }

//                try
//                {
//                    var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", true);

//                    try
//                    {
//                        var manager = (INetFwMgr)Activator.CreateInstance(type);
//                        comObjects.Push(manager);

//                        try
//                        {
//                            var policy = manager.LocalPolicy;
//                            comObjects.Push(policy);

//                            var profile = policy.CurrentProfile;
//                            comObjects.Push(profile);

//                            var applications = profile.AuthorizedApplications;
//                            comObjects.Push(applications);

//                            applications.Add(application);

//                            result = true;
//                        }
//                        catch (Exception e)
//                        {
//                            exception = e;
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        exception = e;
//                    }
//                    finally
//                    {
//                        while (comObjects.Count > 0)
//                        {
//                            ComUtilities.ReleaseComObject(comObjects.Pop());
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    exception = e;
//                }
//                finally
//                {
//                }

//                return (result);
//            }

//            public static bool RemoveApplication(string processImageFileName, out Exception exception)
//            {
//                var result = false;
//                var comObjects = new Stack<object>();

//                exception = null;

//                if (processImageFileName == null) { throw (new ArgumentNullException("processImageFileName")); }
//                if (processImageFileName.Trim().Length == 0) { throw (new ArgumentException("The argument [processImageFileName] cannot be empty.", "processImageFileName")); }

//                try
//                {
//                    var type = Type.GetTypeFromProgID("HNetCfg.FwMgr", true);

//                    try
//                    {
//                        var manager = (INetFwMgr)Activator.CreateInstance(type);
//                        comObjects.Push(manager);

//                        try
//                        {
//                            var policy = manager.LocalPolicy;
//                            comObjects.Push(policy);

//                            var profile = policy.CurrentProfile;
//                            comObjects.Push(profile);

//                            var applications = profile.AuthorizedApplications;
//                            comObjects.Push(applications);

//                            applications.Remove(processImageFileName);

//                            result = true;
//                        }
//                        catch (Exception e)
//                        {
//                            exception = e;
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        exception = e;
//                    }
//                    finally
//                    {
//                        while (comObjects.Count > 0)
//                        {
//                            ComUtilities.ReleaseComObject(comObjects.Pop());
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    exception = e;
//                }
//                finally
//                {
//                }

//                return (result);
//            }
//        }
//    }
//}

