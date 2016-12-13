//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Cliver.Bot;
using Cliver.BotGui;
using Cliver.BotWeb;

/// <summary>
/// Link checker: crawls listed sites and checks broken links
/// </summary>
namespace Cliver.BotCustomization
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Cliver.Config.Initialize(new string[] { "Engine", "Input", "Output", "Web", "Spider", "Log" });

                //Cliver.Bot.Program.Run();//It is the entry when the app runs as a console app.
                Cliver.BotGui.Program.Run();//It is the entry when the app uses the default GUI.
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
    }

    public class CustomMainForm : MainForm
    {
        override public List<ButtonAction> GetButtonActions()
        {
            return base.GetButtonActions();
        }
    }

    public class CustomBotThreadManagerForm : BotThreadManagerForm
    {
        override public Type GetBotThreadControlType()
        {
            return typeof(WebRoutineBotThreadControl);
        }
    }

    public class CustomConfigForm : ConfigForm
    {
        override public List<string> GetConfigControlSections()
        {
            return new List<string> { "Engine", "Input", "Output", "Web", "Spider", "Log", };
        }
    }

    public class AboutFormForm : AboutForm
    {
        override public string GetAbout()
        {
            return @"WEB LINK CHECKER
Compiled: " + Cliver.Bot.Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }
    }

    public class CustomSession : Session
    {
        /// <summary>
        /// Invoked when a fatal error happened and session is aborting.
        /// </summary>
        public override void FATAL_ERROR(string message)
        {
        }

        public override void CREATING()
        {
            FileWriter.This.WriteHeader("Parent Page", "Broken Link");
            domain2page_count = Session.GetSingleValueWorkItemDictionary<PageCounter, int>();
        }

        public class PageCounter : SingleValueWorkItem<int> { }
        SingleValueWorkItemDictionary<PageCounter, int> domain2page_count;

        public class CustomBotCycle : BotCycle
        {
            /// <summary>
            /// Invoked by BotCycle thread as it has been started.
            /// </summary>
            public override void STARTING()
            {
                ((WebRoutineBotThreadControl)BotThreadControl.GetInstanceForThisThread()).WR = hr;
            }
            HttpRoutine hr = new HttpRoutine();

            /// <summary>
            /// Invoked by BotCycle thread when it is exiting.
            /// </summary>
            public override void EXITING()
            {
            }

            /// <summary>
            /// Custom InputItem types are defined as classes based on InputItem
            /// </summary>
            public class Site : InputItem
            {
                readonly public string Url;
            }

            public void PROCESSOR(Site link)
            {
                if (!hr.GetPage(link.Url))
                    throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get site: " + link.Url);
                get_links(1, link.__Queue.Name);
            }

            public class Link : InputItem
            {
                public Link ParentLink { get { return (Link)__ParentItem; } }
                [KeyField]
                readonly public string Url;
                readonly public int Depth;
                readonly public bool Download;

                public Link(string url, int depth, bool download)
                {
                    Url = url;
                    Depth = depth;
                    Download = download;
                }
            }

            public void PROCESSOR(Link link)
            {
                int _MaxDownloadedFileLength = BotWeb.Settings.Web.MaxDownloadedFileLength;
                if (!link.Download)
                    BotWeb.Settings.Web.MaxDownloadedFileLength = 0;
                bool rc = hr.GetPage(link.Url);
                BotWeb.Settings.Web.MaxDownloadedFileLength = _MaxDownloadedFileLength;
                if (!rc)
                {
                    if (hr.Status == WebRoutineStatus.UNACCEPTABLE_CONTENT_TYPE)
                        return;
                    if (hr.HWResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                        FileWriter.This.WriteLine(link.ParentLink.Url, link.Url);
                    //site2boken_urls[item.Site.Url] = site2boken_urls[item.Site.Url] + "\n" + item.Url;
                    else
                        throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get: " + link.Url);
                    return;
                }
                if (link.Download)
                    get_links(link.Depth + 1, link.__Queue.Name);
            }

            public void get_links(int depth2, string queue)
            {
                if (depth2 > BotWeb.Settings.Spider.MaxDownloadLinkDepth)
                    return;

                string domain = Spider.GetDomain(hr.ResponseUrl);
                int page_count = ((CustomSession)Session).domain2page_count[domain];
                if (BotWeb.Settings.Spider.MaxPageCountPerSite > -1 && page_count >= BotWeb.Settings.Spider.MaxPageCountPerSite)
                    return;
                string queue2 = domain + "-" + depth2.ToString();
                if (depth2 > 1)
                    Session.SetInputItemQueuePositionAfterQueue(queue2, queue);//by default queue name is item type name but it can be different if needed

                AgileSpider ags = new AgileSpider(hr.ResponseUrl, hr.HtmlResult);
                List<WebLink> wls = ags.GetWebLinks(WebLinkType.Anchor | WebLinkType.Area | WebLinkType.Form | WebLinkType.MetaTag | WebLinkType.Frame | WebLinkType.Image | WebLinkType.Javascript);
                List<WebLink> beyond_domain_web_links;
                wls = Spider.GetSpiderableLinks(ags.BaseUri, wls, out beyond_domain_web_links);
                bool download = true;
                if (depth2 >= BotWeb.Settings.Spider.MaxDownloadLinkDepth)
                    download = false;
                foreach (WebLink wl in wls)
                {
                    Add(queue2, new Link(url: wl.Url, depth: depth2, download: download));
                    page_count++;
                    if (BotWeb.Settings.Spider.MaxPageCountPerSite > -1 && BotWeb.Settings.Spider.MaxPageCountPerSite <= page_count)
                    {
                        Log.Warning(domain + " reached MaxPageCountPerSite: " + BotWeb.Settings.Spider.MaxPageCountPerSite.ToString());
                        break;
                    }
                }
                ((CustomSession)Session).domain2page_count[domain] = page_count;
                foreach (WebLink wl in beyond_domain_web_links)
                    Add(queue2, new Link(url: wl.Url, depth: depth2, download: false));//by default queue name is item type name but it can be deifferent if needed
            }
        }
    }
}
