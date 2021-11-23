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
        public static readonly SpiderClass Spider;

        public class SpiderClass : Cliver.UserSettings
        {
            public bool ComplyRobotProtocol = false;
            public int MaxDownloadLinkDepth = 3;
            public int MaxPageCountPerSite = 1000;
            public int UnchangableDomainPartNumber = 2;

            override protected void Loaded()
            {
                if (UnchangableDomainPartNumber <= 1)
                    if (!Win.LogMessage.AskYesNo("UnchangableDomainPartNumber is < 2. That will make the bot crawl through external links of sites. This may consume all RAM on your comp and hang the app. Are you sure to proceed?", false))
                        Cliver.Log.Exit("Exit since UnchangableDomainPartNumber < 2");

                if (MaxPageCountPerSite < 0)
                    if (!Win.LogMessage.AskYesNo("MaxPageCountPerSite is < 0. That will make the bot crawl through all links of the site independently on their quantity. This may consume all RAM on your comp and hang the app. Are you sure to proceed?", false))
                        Cliver.Log.Exit("MaxPageCountPerSite is < 0");
            }
        }
    }
}