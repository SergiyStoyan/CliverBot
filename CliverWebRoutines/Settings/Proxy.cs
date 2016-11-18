using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace Cliver.BotWeb
{
    public partial class Settings
    {
        public static readonly ProxyClass Proxy;

        public class ProxyClass : Cliver.Settings
        {
            public int MaxAttemptCountWithNewProxy = 3;
            public string ProxiesFileUri;
            public string ProxyLogin;
            public string ProxyPassword;
            public ProxyType ProxyType = ProxyType.HTTP;
            public int ReloadProxyFileInSeconds = 1000;
        }
    }
}