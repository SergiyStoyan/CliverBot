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
using Cliver;
using System.Windows.Forms;
using Cliver.Bot;
using Cliver.BotGui;
using Cliver.BotWeb;

/// <summary>
/// Link checker: crawls listed sites and checks broken links
/// </summary>
namespace Cliver.BotCustomization
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Cliver.Config.Initialize(new string[] { "Engine", "Input", "Output", "Web", "Spider", "Log" });
            Cliver.BotGui.BotGui.ConfigControlSections = new string[] { "Engine", "Input", "Output", "Web", "Spider", "Log", };
            Cliver.BotGui.BotGui.BotThreadControlType = typeof(WebRoutineBotThreadControl);

            //Cliver.Bot.Program.Run();//It is the entry when the app runs as a console app.
            Cliver.BotGui.Program.Run();//It is the entry when the app uses the default GUI.
        }
    }

    /// <summary>
    /// Most important interface that defines certain routines of CliverBot customization.
    /// This implementation demos use of GetInstanceForThisThread routings.
    /// </summary>
    public class CustomBot : Cliver.Bot.Bot
    {
        new static public string GetAbout()
        {
            return @"WEB LINK CHECKER3
Created: " + Cliver.Bot.Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        /// <summary>
        /// Invoked when the session is creatig.
        /// </summary>
        new static public void SessionCreating()
        {
            Cliver.BotGui.Program.BindProgressBar2InputItemQueue<Site>();
            FileWriter.This.WriteHeader("Parent Page", "Broken Link");
            domain2page_count = Session.GetSingleValueWorkItemDictionary<PageCounter, int>();
        }

        public class PageCounter : SingleValueWorkItem<int> { }
        static SingleValueWorkItemDictionary<PageCounter, int> domain2page_count;

        /// <summary>
        /// Invoked when the session is closing.
        /// </summary>
        new static public void SessionClosing()
        {
        }

        new static public void FatalError(string message)
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread as it has been started.
        /// </summary>
        public override void CycleStarting()
        {
            ((WebRoutineBotThreadControl)BotThreadControl.GetInstanceForThisThread()).WR = hr;
        }

        HttpRoutine hr = new BotWeb.HttpRoutine();

        /// <summary>
        /// Invoked by BotCycle thread when it is exiting.
        /// </summary>
        public override void CycleExiting()
        {
        }
        
        /// <summary>
        /// Custom InputItem types are defined as classes based on InputItem
        /// </summary>
        public class Site : InputItem
        {
            readonly public string Url;

            override public void PROCESSOR(BotCycle bc)
            {
                CustomBot cb = (CustomBot)bc.Bot;
                if (!cb.hr.GetPage(Url))
                    throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get site: " + Url);
                cb.get_links(1);
            }
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

            override public void PROCESSOR(BotCycle bc)
            {
                CustomBot cb = (CustomBot)bc.Bot;
                int _MaxDownloadedFileLength = Cliver.BotWeb.Settings.Web.MaxDownloadedFileLength;
                if (!Download)
                    Cliver.BotWeb.Settings.Web.MaxDownloadedFileLength = 0;
                bool rc = cb.hr.GetPage(Url);
                Cliver.BotWeb.Settings.Web.MaxDownloadedFileLength = _MaxDownloadedFileLength;
                if (!rc)
                {
                    if (cb.hr.Status == WebRoutineStatus.UNACCEPTABLE_CONTENT_TYPE)
                        return;
                    if (cb.hr.HWResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                        FileWriter.This.WriteLine(ParentLink.Url, Url);
                    //site2boken_urls[item.Site.Url] = site2boken_urls[item.Site.Url] + "\n" + item.Url;
                    else
                        throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get: " + Url);
                    return;
                }
                if (Download)
                    cb.get_links(Depth + 1);
            }
        }

        public void get_links(int depth2)
        {
            if (depth2 > Cliver.BotWeb.Settings.Spider.MaxDownloadLinkDepth)
                return;

            string domain = Spider.GetDomain(hr.ResponseUrl);
            int page_count = domain2page_count[domain];
            if (Cliver.BotWeb.Settings.Spider.MaxPageCountPerSite > -1 && page_count >= Cliver.BotWeb.Settings.Spider.MaxPageCountPerSite)
                return;

            AgileSpider ags = new AgileSpider(hr.ResponseUrl, hr.HtmlResult);
            List<WebLink> wls = ags.GetWebLinks(WebLinkType.Anchor | WebLinkType.Area | WebLinkType.Form | WebLinkType.MetaTag | WebLinkType.Frame | WebLinkType.Image | WebLinkType.Javascript);
            List<WebLink> beyond_domain_web_links;
            wls = Spider.GetSpiderableLinks(ags.BaseUri, wls, out beyond_domain_web_links);
            bool download = true;
            if (depth2 >= Cliver.BotWeb.Settings.Spider.MaxDownloadLinkDepth)
                download = false;
            foreach (WebLink wl in wls)
            {
                BotCycle.Add(new Link(url: wl.Url, depth: depth2, download: download));
                page_count++;
                if (Cliver.BotWeb.Settings.Spider.MaxPageCountPerSite > -1 && Cliver.BotWeb.Settings.Spider.MaxPageCountPerSite <= page_count)
                {
                    Log.Warning(domain + " reached MaxPageCountPerSite: " + Cliver.BotWeb.Settings.Spider.MaxPageCountPerSite.ToString());
                    break;
                }
            }
            domain2page_count[domain] = page_count;
            foreach (WebLink wl in beyond_domain_web_links)
                BotCycle.Add(new Link(url: wl.Url, depth: depth2, download: false));
        }
    }
}
