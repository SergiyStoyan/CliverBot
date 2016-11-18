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
using System.Linq;

namespace Cliver.BotWeb
{
    public class AgileSpider
    {
        public AgileSpider(string url, string page)
        {
            this.parent_url = url;

            parent_document = new HtmlAgilityPack.HtmlDocument();
            parent_document.LoadHtml(page);
            if (parent_document.ParseErrors != null)
                foreach (HtmlAgilityPack.HtmlParseError e in parent_document.ParseErrors)
                    Log.Error("Html parser error: " + e.Reason);
        }
        HtmlAgilityPack.HtmlDocument parent_document;
        string parent_url;

        /// <summary>
        /// Find 'base' tag within page and construct absolute uri.
        /// </summary>
        /// <returns>absolute uri for the page</returns>
        public Uri BaseUri
        {
            get
            {
                lock (parent_document)
                {
                    if (base_uri == null)
                    {
                        try
                        {
                            base_uri = new Uri(parent_document.DocumentNode.SelectSingleNode("//base").Attributes["href"].Value, UriKind.Absolute);
                        }
                        catch { }
                        if (base_uri == null)
                            base_uri = new Uri(parent_url, UriKind.Absolute);
                    }
                    return base_uri;
                }
            }
        }
        Uri base_uri = null;

        public string GetAbsoluteUrl(string link)
        {
            try
            {
                if (link == null)
                    return null;
                link = HttpUtility.HtmlDecode(link);
                Uri ulink = new Uri(BaseUri, link);
                return ulink.ToString();
            }
            catch(Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
                
        /// <summary>
        /// Finds web links in the page
        /// </summary>
        /// <returns>absolute links</returns>      
        public List<WebLink> GetWebLinks(WebLinkType link_type)
        {
            lock (parent_document)
            {
                List<WebLink> links = new List<WebLink>();

                //anchors
                if ((link_type & WebLinkType.Anchor) == WebLinkType.Anchor)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//a");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if (hn.Attributes["href"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["href"].Value), hn.InnerText, hn.Attributes["title"] != null ? hn.Attributes["title"].Value : null, WebLinkType.Anchor, hn.StreamPosition));
                    }
                }

                //areas
                if ((link_type & WebLinkType.Area) == WebLinkType.Area)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//area");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if (hn.Attributes["src"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["src"].Value), hn.InnerText, hn.Attributes["title"] != null ? hn.Attributes["title"].Value : null, WebLinkType.Area, hn.StreamPosition));
                    }
                }

                //meta tag url
                if ((link_type & WebLinkType.MetaTag) == WebLinkType.MetaTag)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//meta");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if (hn.Attributes["url"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["url"].Value), hn.InnerText, null, WebLinkType.MetaTag, hn.StreamPosition));
                    }
                }

                //images
                if ((link_type & WebLinkType.Image) == WebLinkType.Image)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//img");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if (hn.Attributes["src"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["src"].Value), hn.InnerText, hn.Attributes["title"] != null ? hn.Attributes["title"].Value : null, WebLinkType.Image, hn.StreamPosition));
                    }
                }

                //frames
                if ((link_type & WebLinkType.Frame) == WebLinkType.Frame)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//frame");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if (hn.Attributes["src"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["src"].Value), hn.InnerText, hn.Attributes["title"] != null ? hn.Attributes["title"].Value : null, WebLinkType.Frame, hn.StreamPosition));
                    }
                }

                //forms
                if ((link_type & WebLinkType.Form) == WebLinkType.Form)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//form");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                            if(hn.Attributes["action"] != null)
                                links.Add(new WebLink(GetAbsoluteUrl(hn.Attributes["action"].Value), hn.InnerText, hn.Attributes["title"] != null ? hn.Attributes["title"].Value : null, WebLinkType.Form, hn.StreamPosition));
                    }
                }

                //javascript links
                if ((link_type & WebLinkType.Javascript) == WebLinkType.Javascript)
                {
                    HtmlAgilityPack.HtmlNodeCollection hnc = parent_document.DocumentNode.SelectNodes("//script");
                    if (hnc != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode hn in (from x in hnc select x))
                        {
                            foreach (Match mm in Regex.Matches(hn.InnerText, @"(?:location.href|window.open)\((['""])(?'Url'[^\>\;]+?)\1\)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                links.Add(new WebLink(GetAbsoluteUrl(mm.Groups["Url"].Value), null, null, WebLinkType.Javascript, hn.StreamPosition + mm.Index));
                        }
                    }
                }

                return links;
            }
        }
    }
}
