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

namespace Cliver.Bot
{
    /// <summary>
    /// Web link scraped from html page
    /// </summary>
    public class WebLink
    {
        public string Url;
        public string Text;
        public string Title;
        public WebLinkType WebLinkType;
        public int IndexInPreparedPage;

        internal WebLink(string url, string text, string title, WebLinkType link_type, int index)
        {
            if (url != null)
                Url = HttpUtility.HtmlDecode(url).Trim();
            Text = text;
            Title = title;
            WebLinkType = link_type;
            IndexInPreparedPage = index;
        }
    }

    /// <summary>
    /// Web link types can be contained by a html
    /// </summary>
    public enum WebLinkType
    {
        MetaTag = 1,
        Anchor = 2,
        Frame = 4,
        Javascript = 8,
        Image = 16,
        Area = 32,
        Form = 64
    }

    public partial class Spider
    {
        /// <summary>
        /// Find 'base' tag within page and construct absolute uri.
        /// </summary>
        /// <param name="parent_page">html page where to search</param>
        /// <returns>absolute uri for the page</returns>
        public static Uri GetBaseUri(string parent_url, string parent_page)
        {
            parent_page = PreparePage(parent_page);
            return get_base_uri( parent_url,  parent_page);
        }

        static Uri get_base_uri(string parent_url, string parent_page)
        {
            Uri abs_uri = null;
            Match m = Regex.Match(parent_page, @"<base\s+(?:[^>]+\s+)?href=([\'\""])?(.*?)(?:\1|>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (m.Success)
                abs_uri = new Uri(m.Result("$2"), UriKind.Absolute);
            else
                abs_uri = new Uri(parent_url, UriKind.Absolute);
            return abs_uri;
        }
        
        /// <summary>
        /// Purify web page before parsing
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string PreparePage(string page)
        {
            page = Regex.Replace(page, "<!--.*?-->|[\r\n]+", "", RegexOptions.Compiled | RegexOptions.Singleline);
            return page;
        }

        /// <summary>
        /// Build absolute url. If possible it is better to use GetAbsoluteUrls as it can find base url within page.
        /// </summary>
        /// <param name="link">relative url</param>
        /// <param name="parent_url">base url</param>
        /// <returns>absolute url</returns>
        public static string GetAbsoluteUrl(string link, string parent_url)
        {
            try
            {
                if (link == null)
                    return null;
                link = HttpUtility.HtmlDecode(link);
                Uri u = new Uri(parent_url);
                Uri ulink = new Uri(u, link);
                return ulink.ToString();
            }
            //catch (Exception e)
            catch
            {
                //Log.Error(e.Message + "\n" + e.StackTrace + "\nlink=" + link + "\nparent_url=" + parent_url);
                return null;
            }
        }

        /// <summary>
        /// Build array of absolute urls. It will base url if it is found within page.
        /// </summary>
        /// <param name="link">relative url</param>
        /// <param name="parent_url">base url</param>
        /// <param name="page">page where link was found</param>
        /// <returns>array of absolute urls</returns>
        public static string[] GetAbsoluteUrls(string[] links, string parent_url, string page)
        {
            page = PreparePage(page);

            Uri ubase = get_base_uri(parent_url, page);

            List<string> abs_links = new List<string>();
            foreach (string link in links)
            {
                if (link == null)
                    continue;
                string l = HttpUtility.HtmlDecode(link).Trim();
                if (l.StartsWith("?"))
                    l = ubase.GetLeftPart(UriPartial.Path) + l;
                Uri ulink = new Uri(ubase, l);
                abs_links.Add(ulink.AbsoluteUri);
            }

            string[] ls = new string[abs_links.Count];
            abs_links.CopyTo(ls, 0);
            return ls;
        }

        static public List<WebLink> GetSpiderableLinks(Uri parent_uri, List<WebLink> web_links, out List<WebLink> beyond_domain_web_links)
        {
            List<WebLink> web_links2 = new List<WebLink>();
            beyond_domain_web_links = new List<WebLink>();

            string[] ubase_parts = new string[0];
            if (Properties.Spider.Default.UnchangableDomainPartNumber > 0)
            {
                string domain = parent_uri.Host;
                ubase_parts = domain.Split('.');
            }

            foreach (WebLink wl in web_links)
            {
                Uri u = new Uri(parent_uri, wl.Url);

                if (u.Scheme != "http" && u.Scheme != "https")
                    continue;
                wl.Url = Regex.Replace(wl.Url, @"(\#.*)", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (wl.Url.Contains("@"))
                    continue;

                bool beyond_domain = false;
                string[] u_parts = u.Host.Split('.');
                int length = Properties.Spider.Default.UnchangableDomainPartNumber;
                if (Properties.Spider.Default.UnchangableDomainPartNumber > ubase_parts.Length)
                    length = ubase_parts.Length;
                if (ubase_parts.Length > u_parts.Length)
                    length = u_parts.Length;
                for (int i = 1; i <= length; i++)
                {
                    if (ubase_parts[ubase_parts.Length - i] != u_parts[u_parts.Length - i])
                    {
                        beyond_domain = true;
                        break;
                    }
                }

                if(beyond_domain)
                    beyond_domain_web_links.Add(wl);
                else
                    web_links2.Add(wl);
            }
            return web_links2;
        }

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
            Match m = Regex.Match(parent_uri.Host, @"([^\.]+\.?){" + Properties.Spider.Default.UnchangableDomainPartNumber.ToString() + "}$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
            if (m.Success)
                return m.Groups[0].Value;
            return url;
        }
    }
}
