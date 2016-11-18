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
        public static readonly BrowserClass Browser;

        public class BrowserClass : Cliver.Settings
        {
            public int PageCompletedTimeoutInSeconds = 60;
            public bool CloseWebBrowserDialogsAutomatically = true;
            public bool SuppressScriptErrors = true;
        }
    }
}