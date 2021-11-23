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

namespace Cliver.Bot
{
    public partial class Settings
    {
        public static readonly AppSettings App;

        public class AppSettings : Cliver.UserSettings
        {
            public int FiddlerPort = 8877;
            public int MaxTime2WaitForSessionStopInSecs = 60;
            public bool SingleProcessOnly = false;
            public string RegistryGeneralSubkey = @"SOFTWARE\CliverSoft\";
            public string HelpUri = "http://cliversoft.com/articles/manual_to_cliverbot_based_applications.php";
            public bool RegistryHiveIsUserDependent = false;
            public string RegistryAppSubkeyNameRegexForBaseDirectory = @"([^\\\/]*\#[^\\\/]*)(?:[\\\/]|$)";
        }
    }
}