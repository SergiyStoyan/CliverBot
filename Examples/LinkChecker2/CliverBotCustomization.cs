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

namespace Cliver.BotCustomization
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //Bot.Properties.General.Default.RestoreBrokenSession = true;
            //Bot.Properties.General.Default.WriteSessionRestoringLog = true;
            //Bot.Properties.General.Default.Save();

            Config.Save();            
            Cliver.BotGui.Program.Run();
        }
    }

    public class CustomBotGui : Cliver.BotGui.BotGui
    {
        override public string[] GetConfigControlNames()
        {
            return new string[] { "General", "Input", "Output", "Web", "Spider", "Log" };
        }

        override public Type GetBotThreadControlType()
        {
            return typeof(WebRoutineBotThreadControl);
        }

        override public Cliver.BaseForm GetToolsForm()
        {
            return null;
        }
    }

    /// <summary>
    /// Most important interface that defines certain routines of CliverBot customization.
    /// This implementation demos use of PROCESSOR's defined in CustomBot class.
    /// </summary>
    public class CustomBot : Cliver.Bot.Bot
    {
        new static public string GetAbout()
        {
            return @"WEB LINK CHECKER2
Created: " + Cliver.Bot.Program.CustomizationModificationTime.ToString() + @"
Developed by: www.cliversoft.com";
        }

        new static public void SessionCreating()
        {
            FileWriter.This.WriteHeader("Parent Page", "Broken Link");
            domain2page_count = Session.GetSingleValueWorkItemDictionary<PageCounter, int>();
        }

        public class PageCounter : SingleValueWorkItem<int> { }
        static SingleValueWorkItemDictionary<PageCounter, int> domain2page_count;

        HttpRoutine hr = new HttpRoutine();

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
            readonly public Site Site;
            readonly public Link ParentLink;
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
            int _MaxDownloadedFileLength = Bot.Properties.Web.Default.MaxDownloadedFileLength;
            if (!link.Download)
                Bot.Properties.Web.Default.MaxDownloadedFileLength = 0;
            bool rc = hr.GetPage(link.Url);
            Bot.Properties.Web.Default.MaxDownloadedFileLength = _MaxDownloadedFileLength;
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
            if (depth2 > Bot.Properties.Spider.Default.MaxDownloadLinkDepth)
                return;

            string domain = Spider.GetDomain(hr.ResponseUrl);
            int page_count = domain2page_count[domain];
            if (Bot.Properties.Spider.Default.MaxPageCountPerSite > -1 && page_count >= Bot.Properties.Spider.Default.MaxPageCountPerSite)
                return;
            string queue2 = domain + "-" + depth2.ToString();
            if (depth2 > 1)
                Session.SetInputItemQueuePositionAfterQueue(queue2, queue);//by default queue name is item type name but it can be different if needed

            AgileSpider ags = new AgileSpider(hr.ResponseUrl, hr.HtmlResult);
            List<WebLink> wls = ags.GetWebLinks(WebLinkType.Anchor | WebLinkType.Area | WebLinkType.Form | WebLinkType.MetaTag | WebLinkType.Frame | WebLinkType.Image | WebLinkType.Javascript);
            List<WebLink> beyond_domain_web_links;
            wls = Spider.GetSpiderableLinks(ags.BaseUri, wls, out beyond_domain_web_links);
            bool download = true;
            if (depth2 >= Bot.Properties.Spider.Default.MaxDownloadLinkDepth)
                download = false;
            foreach (WebLink wl in wls)
            {
                BotCycle.Add(queue2, new Link(url: wl.Url, depth: depth2, download: download));
                page_count++;
                if (Bot.Properties.Spider.Default.MaxPageCountPerSite > -1 && Bot.Properties.Spider.Default.MaxPageCountPerSite <= page_count)
                {
                    Log.Warning(domain + " reached MaxPageCountPerSite: " + Bot.Properties.Spider.Default.MaxPageCountPerSite.ToString());
                    break;
                }
            }
            domain2page_count[domain] = page_count;
            foreach (WebLink wl in beyond_domain_web_links)
                BotCycle.Add(queue2, new Link(url: wl.Url, depth: depth2, download: false));//by default queue name is item type name but it can be deifferent if needed
        }
    }
}
