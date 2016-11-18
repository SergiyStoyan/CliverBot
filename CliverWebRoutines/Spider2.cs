//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        30 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace Cliver.BotWeb
{
    public class Spider2
    {
        /// <summary>
        /// Return anchor/area links found within the passed page string. Links are filtered by their text with name_filter_regexes.
        /// </summary>
        /// <param name="name_filter_pattern"></param>
        /// <param name="parent_url"></param>
        /// <param name="parent_page"></param>
        /// <returns></returns>
        public static string[] GetAbsoluteAnchorLinksFilteredByNames(string name_filter_pattern, string url, string page)
        {
            List<string> abs_links = new List<string>();
            Uri parent_uri = new Uri(url);

            foreach (WebLink wl in GetWebLinks(url, page, WebLinkType.Anchor))
            {
                if (!Regex.IsMatch(wl.Text, name_filter_pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
                    continue;

                //if (Spider.IsLinkValid(parent_uri, wl.Url))
                //    abs_links.Add(wl.Url);
            }

            string[] ls = new string[abs_links.Count];
            abs_links.CopyTo(ls, 0);
            return ls;
        }

        /// <summary>
        /// Return anchor/area/frame/iframe links found within the passed page string. Links are filtered by regexes.
        /// </summary>
        /// <param name="name_filter_regexes"></param>
        /// <param name="parent_url"></param>
        /// <param name="parent_page"></param>
        /// <returns></returns>
        public static string[] GetAbsoluteLinksFilteredByRegexes(string[] name_filter_regexes, string parent_url, string parent_page)
        {
            //links = []
            //if not isinstance(filter_regexes, (type([]), type(()))):
            //    filter_regexes = [filter_regexes]
            //ls = get_page_links(base_url, page)
            //for l in ls:
            //    for r in filter_regexes:
            //        if r.search(l):
            //            links.append(l)
            //            break
            //return links
            return null;
        }
        
        /// <summary>
        /// Finds web links in the page
        /// </summary>
        /// <param name="url">absolute url of parsed page</param>
        /// <param name="page">string to be parsed for web links</param>
        /// <returns>absolute links</returns>      
        public static List<WebLink> GetWebLinks(string url, string page, WebLinkType link_type)
        {
            page = Spider.PreparePage(page);
            url = Spider.GetBaseUri(url, page).ToString();

            List<WebLink> links = new List<WebLink>();
            
            //anchors
            if ((link_type & WebLinkType.Anchor) == WebLinkType.Anchor)
                find_links(links, WebLinkType.Anchor, HtmlAnchors, url, page);

            //areas
            if ((link_type & WebLinkType.Area) == WebLinkType.Area)
                find_links(links, WebLinkType.Area, HtmlAreas, url, page);

            //meta tag url
            if ((link_type & WebLinkType.MetaTag) == WebLinkType.MetaTag)
            {
                foreach (Match mm in Regex.Matches(page, @"<META [^>]*CONTENT\s*=\s*([""']).+?;\s*URL\s*=\s*(?'Url'.*?)(?:\1|>)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                    links.Add(new WebLink(mm.Groups["Url"].Value, null, null, WebLinkType.MetaTag, mm.Index));
            }

            ////images
            //if ((link_type & WebLinkType.Image) == WebLinkType.Image)
            //{
            //    m = Regex.Match(page, @"<(?'Body'(?'Tag'img)\s(?:[^>]*?\s)?src\s*=\s*(?'quotation'[\'\""])?(?'Url'.*?)(?:\k'quotation'[^>]*?)?/\s*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //    while (m.Success)
            //    {

            //        string title = null;
            //        Match mm = Regex.Match(m.Groups["Body"].Value, @"\salt\s*=\s*(?'quotation'[\'\""])?(?'Title'.*?)(?:\k'quotation'|$)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //        if (mm.Success)
            //            title = mm.Groups["Title"].Value;

            //        links[m.Groups["Url"].Value] = new WebLink(GetAbsoluteUrl(m.Groups["Url"].Value, url), null, title, WebLinkType.Image);
            //        m = m.NextMatch();
            //    }
            //}

            //frames
            if ((link_type & WebLinkType.Frame) == WebLinkType.Frame)
                find_links(links, WebLinkType.Frame, HtmlFrames, url, page);

            //forms
            if ((link_type & WebLinkType.Form) == WebLinkType.Form)
                find_links(links, WebLinkType.Form, HtmlForms, url, page);

            //javascript links
            if ((link_type & WebLinkType.Javascript) == WebLinkType.Javascript)
            {
               foreach(Match mm in Regex.Matches(page, @"(?:location.href|window.open)\((['""])(?'Url'[^\>\;]+?)\1\)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                    links.Add(new WebLink(mm.Groups["Url"].Value, null, null, WebLinkType.Javascript, mm.Index));
            }

            //WebLink[] wls = new WebLink[links.Count];
            //links.CopyTo(wls, 0);
            //return wls;
            return links;
        }

        static object static_lock_object = new object();

        static void find_links(List<WebLink> links, WebLinkType link_type, Cliver.DataSifter.Parser parser, string url, string page)
        {
            lock (static_lock_object)
            {
                Cliver.DataSifter.Capture gc = parser.Parse(page);
                foreach (Cliver.DataSifter.Capture tag in gc["Tag"])
                {
                    Cliver.DataSifter.Capture html = tag.FirstOf("Html");
                    if (html == null)
                        continue;
                    string u = html.ValueOf("Url");
                    if (u == null)
                        continue;
                    u = Spider.GetAbsoluteUrl(u, url);
                    if (u == null)
                        continue;
                    links.Add(new WebLink(u, tag.ValueOf("Content"), html.ValueOf("Title"), link_type, tag.Index));
                }
            }
        }

        static Cliver.DataSifter.Parser HtmlAnchors = new DataSifter.Parser("HtmlAnchors.rgx");
        static Cliver.DataSifter.Parser HtmlAreas = new DataSifter.Parser("HtmlAreas.rgx");
        static Cliver.DataSifter.Parser HtmlFrames = new DataSifter.Parser("HtmlFrames.rgx");
        static Cliver.DataSifter.Parser HtmlForms = new DataSifter.Parser("HtmlForms.rgx");

        /// <summary>
        /// Checks whether url remains within host
        /// </summary>
        /// <param name="url">url to check</param>
        /// <param name="host_url">host url. Can be any url to the host</param>
        /// <returns></returns>
        public static bool UrlIsWithinHost(string url, string host_url)
        {
            return (url.StartsWith(host_url));
        }

        /// <summary>
        /// Extracts domain of specified part number from url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomain(string url)
        {
            Uri parent_uri = new Uri(url);
            Match m = Regex.Match(parent_uri.Host, @"([^\.]+\.?){" + Settings.Spider.UnchangableDomainPartNumber.ToString() + "}$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
            if (m.Success)
                return m.Groups[0].Value;
            return url;
        }
    }
}
