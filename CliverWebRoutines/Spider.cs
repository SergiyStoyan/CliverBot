//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using System.Collections.Generic;

namespace Cliver.BotWeb
{
	/// <summary>
	/// It is a spider. Finds web links within web page. 
	/// </summary>
    public partial class Spider
    {
        /// <summary>
        ///defines that links with these extentions will not be spidered  
        /// </summary>
        public string NonSpideredExtensions = "msi|exe|gif|jpg|jpeg|bmp|rtf|ps|arj|zip|z|gzip|rar|gz|tar|tgz|bz|bz2|wav|mp3|au|aiff|bin|mpg|mpeg|mov|qt|tif|tiff|avi|ram|ra|arc|hqx|sit|sea|uu|png|css|ico|cl|jar";

        // <summary>
        //determines if spider will follow the rules of the robot.txt file. 
        // Also determines if the spider will follow the spider meta tags.
        // </summary>
        //public bool ComplyRobotProtocol = true;

        // <summary>
        // defines how many path parts in links can be other than input link.
        // E.g. = 0 means that spider can not go from www.google.com/help/index/ to www.google.com/help/
        // </summary>
        //public int UnchangableLinkPartNumber = 0;

        /// <summary>
        /// defines whether drop email links
        /// </summary>
        //bool DoNotReturnEmailLinks = true;
        
        /// <summary>
        /// Put new links to InputItems queues so that the bot crawl site by site and go to more deep links. 
        /// Each input item in START_QUEUE should have the following keys: "Url".
        /// The rest input items should have the following keys: "Url", "Site", "Depth".
        /// </summary>
        /// <param name="parent_item">input item where the links where extracted</param>
        /// <param name="links">links to be added</param>
        /// <param name="itput_items">Input Items</param>
        //void add_links2InputItems(InputItem parent_item, List<WebLink> links, InputItems itput_items)
        //{
        //    try
        //    {
        //        if (links.Count > 0)
        //        {
        //            Uri parent_uri = new Uri(parent_item["Url"]);
        //            string domain = parent_uri.Host;
        //            int page_count = 0;
        //            Match m = base_domain_regex.Match(domain);
        //            if (m.Success)
        //                domain = m.Groups[0].Value;
        //            page_counts.TryGetValue(domain, out page_count);

        //            if (Properties.Spider.Default.MaxPageCountPerSite > -1 && Properties.Spider.Default.MaxPageCountPerSite <= page_count)
        //                return;

        //            int depth = 0;
        //            if (parent_item.Queue != InputItems.START_QUEUE)
        //                depth = int.Parse(parent_item["Depth"]);

        //            string next_depth = ((int)(depth + 1)).ToString();

        //            string queue = domain + "," + next_depth;

        //            //find order for the queue
        //            int last_queue_index = 0;
        //            for (int i = 0; i < itput_items.QueueOrder.Count; i++)
        //            {
        //                string[] qps = itput_items.QueueOrder[i].Split(',');
        //                if (qps[0] == domain)
        //                {
        //                    last_queue_index = i;
        //                    break;
        //                }
        //            }
        //            itput_items.QueueOrder.Insert(last_queue_index + 1, queue);
        //            itput_items.QueueOrder.Add(InputItems.START_QUEUE);

        //            foreach (WebLink link in links)
        //            {
        //                if (!is_link_valid(parent_uri, link.Url))
        //                    continue;

        //                itput_items.Add(new InputItem(queue, "Url", link.Url, "Depth", next_depth, "Domain", domain));
        //                page_count++;
        //                if (Properties.Spider.Default.MaxPageCountPerSite > -1 && Properties.Spider.Default.MaxPageCountPerSite <= page_count)
        //                {
        //                    Log.Warning(domain + " reached MaxPageCountPerSite: " + Properties.Spider.Default.MaxPageCountPerSite.ToString());
        //                    break;
        //                }
        //            }
        //            page_counts[domain] = page_count;
        //        }
        //    }
        //    catch (ThreadAbortException)
        //    {
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //    }
        //}
        Dictionary<string, int> page_counts = new Dictionary<string, int>();
        Regex base_domain_regex = new Regex(@"([^\.]+\.?){"+ Properties.Spider.Default.UnchangableDomainPartNumber.ToString() +"}$", RegexOptions.Compiled| RegexOptions.IgnoreCase| RegexOptions.Singleline| RegexOptions.RightToLeft);

        /// <summary>
        /// Retrieve links from the page and add them to the InputItems
        /// </summary>
        /// <param name="item"></param>
        /// <param name="WR"></param>
        /// <param name="IIS"></param>
        /// <returns>if page was downloaded then True</returns>
       /* public bool ProcessItem(InputItem item, WebRoutine WR, InputItems IIS)
        {
            lock (this)
            {
                try
                {
                    string url = item["Url"];

                    if (!Net.GetPage(url))
                    {
                        if (Status == WebRoutineStatus.UNACCEPTABLE_CONTENT_TYPE)
                            return true;
                        else
                            return false;
                    }

                    url = ResponseUrl;

                    int depth = 0;
                    if (item.Queue != InputItems.START_QUEUE)
                        depth = int.Parse(item["Depth"]);
                    if (depth >= Properties.Spider.Default.MaxDownloadedPageDepth)
                        return true;
                                                           
                    Uri parent_uri = new Uri(url);
                    string domain = parent_uri.Host;
                    int page_count = 0;
                    Match m = base_domain_regex.Match(domain);
                    if (m.Success)
                        domain = m.Groups[0].Value;
                    page_counts.TryGetValue(domain, out page_count);

                    if (Properties.Spider.Default.MaxPageCountPerSite > -1 && Properties.Spider.Default.MaxPageCountPerSite <= page_count)
                        return true;
                    
                    List<WebLink> links = Spider.GetWebLinks(url, HtmlResult, WebLinkType.Anchor | WebLinkType.Area | WebLinkType.Form | WebLinkType.Frame | WebLinkType.Javascript);
                    
                    string next_depth = ((int)(depth + 1)).ToString();

                    string queue = domain + "," + next_depth;

                    //find order for the queue
                    int last_queue_index = 0;
                    for (int i = 0; i < IIS.QueueOrder.Count; i++)
                    {
                        string[] qps = IIS.QueueOrder[i].Split(',');
                        if (qps[0] == domain)
                        {
                            last_queue_index = i;
                            break;
                        }
                    }
                    IIS.QueueOrder.Insert(last_queue_index + 1, queue);
                    IIS.QueueOrder.Add(InputItems.START_QUEUE);

                    foreach (WebLink link in links)
                    {
                        if (!is_link_valid(parent_uri, link.Url))
                            continue;

                        IIS.Add(new InputItem(queue, "Url", link.Url, "Depth", next_depth, "Domain", domain));
                        page_count++;
                        if (Properties.Spider.Default.MaxPageCountPerSite > -1 && Properties.Spider.Default.MaxPageCountPerSite <= page_count)
                        {
                            Log.Warning(domain + " reached MaxPageCountPerSite: " + Properties.Spider.Default.MaxPageCountPerSite.ToString());
                            break;
                        }
                    }
                    page_counts[domain] = page_count;                    

                    return true;
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                return false;
            }
        }*/
        
        /// <summary>
        /// Crawl completely site defined by the url.
        /// </summary>
        /// <param name="url">initial url</param>
        /// <param name="WR">WebRoutine</param>
        /// <param name="process_page">process page delegate</param>
        /// <returns>true if site crawled successfully (more 50% pages were downloaded without error)</returns>
       /* public bool CrawlSite(string url, WebRoutine WR, ProcessPage process_page)
        {
            lock (this)
            {
                try
                {
                    Uri ulink = new Uri(url);
                    string domain = ulink.Host;
                    Hashtable links = new Hashtable();
                    Queue[] links_by_depth = new Queue[Properties.Spider.Default.MaxDownloadedPageDepth + 1];
                    for (int i = 0; i < links_by_depth.Length; i++)
                        links_by_depth[i] = new Queue();
                    int depth = 0;
                    links_by_depth[depth].Enqueue(url);
                    links[url] = 0;

                    int page_count = 0;
                    int bad_page_count = 0;
                    bool site_crawled_successfully = false;
                    while (depth <= Properties.Spider.Default.MaxDownloadedPageDepth)
                    {
                        if (Properties.Spider.Default.MaxPageCountPerSite > -1 && ++page_count >= Properties.Spider.Default.MaxPageCountPerSite)
                        {
                            Log.Warning(url + " reached MaxPageCountPerSite: " + Properties.Spider.Default.MaxPageCountPerSite.ToString());
                            return site_crawled_successfully;
                        }
                        while (links_by_depth[depth].Count < 1)
                        {
                            depth++;
                            if (depth > Properties.Spider.Default.MaxDownloadedPageDepth)
                                return site_crawled_successfully;
                        }
                        url = (string)links_by_depth[depth].Dequeue();

                        if (!Net.GetPage(url))
                        {
                            bad_page_count++;
                            continue;
                        }
                        Match m = Regex.Match(HtmlResult, @"<META\s[^>]*?HTTP-EQUIV=([""'])refresh\1[^>]*?CONTENT=([""']).+?(\s*;URL=.*?)?(?:\2|>)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (m.Success && m.Result("$3").Length == 0)
                        {//this page is to be reloaded
                            Cache.Remove(url, null);
                            Cache.Remove(ResponseUrl, null);
                            if (!Net.GetPage(url))
                            {
                                bad_page_count++;
                                continue;
                            }
                        }

                        site_crawled_successfully = 0.5 < (((float)(page_count - bad_page_count)) / page_count);

                        links[url] = 0;
                        links[ResponseUrl] = 0;
                        if (!process_page(HtmlResult, url, ResponseUrl))
                            return site_crawled_successfully;

                        if (depth < Properties.Spider.Default.MaxDownloadedPageDepth)
                        {
                            List<WebLink> ls = Spider.GetWebLinks(url, HtmlResult, WebLinkType.Anchor | WebLinkType.Area | WebLinkType.Form | WebLinkType.Frame | WebLinkType.Javascript);
                            int next_depth = depth + 1;
                            foreach (WebLink l in ls)
                            {
                                if (links.ContainsKey(l.Url))
                                    continue;
                                links_by_depth[next_depth].Enqueue(l.Url);
                                links[l.Url] = 0;
                            }
                        }
                    }
                    return site_crawled_successfully;
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                return false;
            }
        }*/

        /// <summary>
        /// Process downloaded page.
        /// </summary>
        /// <param name="page">page</param>
        /// <returns>false if stop crawling the site</returns>
        public delegate bool ProcessPage(string page, string url, string response_url);

        //string get_absolute_url(string current_url, string url)
        //{
        //    try
        //    {
        //        url = url.Trim();

        //        url = Regex.Replace(url, @"(.*?)\#", "$1", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //        Match m = Regex.Match(url, @"\:\/\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //        if (m.Success)
        //        {
        //            //url is absolute
        //        }
        //        else
        //        {//url is relative
        //            m = Regex.Match(url, @"^\/", RegexOptions.Compiled);
        //            if (m.Success)
        //            {
        //                string host_url = current_url;
        //                m = Regex.Match(current_url, @"(.+\:\/\/[^\/]+)|([^\/]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //                if (m.Success)
        //                    host_url = m.Result("$1") + "/";

        //                url = host_url + url;
        //            }
        //            else
        //            {
        //                m = Regex.Match(url, @"^\?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //                if (m.Success)
        //                {
        //                    url = current_url + url;
        //                }
        //                else
        //                {
        //                    string current_path = current_url + "/";
        //                    //m = Regex.Match(current_url, @"(.+)\/");
        //                    m = Regex.Match(current_url, @"(.+?\:\/\/.+)[\/$]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //                    if (m.Success)
        //                        current_path = m.Result("$1") + "/";

        //                    url = current_path + url;
        //                }
        //            }
        //        }

        //        //remove dubbed slashes
        //        url = Regex.Replace(url, @"([^\:\/]\/)\/+", "$1", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //        //remove point between slashes
        //        url = Regex.Replace(url, @"([^\:\/]\/)\.\/", "$1", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //        //move to lower level if 2 points in path
        //        url = Regex.Replace(url, @"([^\:\/]\/)[^\/]+?\/\.\.\/", "$1", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //        //validate
        //        m = Regex.Match(url, @"[^\w\d\:\/\.]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //        if (m.Success)
        //            return null;
        //    }
        //    catch (ThreadAbortException)
        //    {
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //    }

        //    return url;
        //}
	}
}
