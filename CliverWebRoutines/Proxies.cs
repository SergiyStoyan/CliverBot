//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using Cliver.Win;

namespace Cliver.BotWeb
{
    /// <summary>
    /// Creates and manages web proxy list
    /// </summary>
    public class Proxy
    {
        public string Ip = null;
        public string Port = null;
        public string Login = null;
        public string Password = null;
        public ProxyType Type = ProxyType.HTTP;
        
        public WebProxy WebProxy
        {
            get
            {
                if (web_proxy == null)
                {
                    web_proxy = new WebProxy(Ip + ":" + Port);
                    web_proxy.Credentials = new NetworkCredential(Login, Password);
                }
                return web_proxy;
            }
        }
        WebProxy web_proxy = null;
    }

    public enum ProxyType
    {
        HTTP,
        HTTPS,
        SOCKS4,
        SOCKS4A,
        SOCKS5
    }

    public static class Proxies
    {
        static Proxies()
        {
            Cliver.Bot.Session.Closing += clear_session;
        }

        //static Queue proxies = new Queue();
        static List<Proxy> proxies = new List<Proxy>();
        static bool use_proxy = true;

        /// <summary>
        /// Used to clear proxy list in order to start a new session
        /// </summary>
        internal static void clear_session()
        {
            lock (proxies)
            {
                proxies.Clear();
            }
        }

        static DateTime refresh_proxy_time = DateTime.Now.AddYears(1);

        static void load_proxies_from_file()
        {
            lock (proxies)
            {
                if (Settings.Proxy == null || string.IsNullOrEmpty(Settings.Proxy.ProxiesFileUri))
                {
                    use_proxy = false;
                    Log.Main.Inform("Proxies are not used.");
                    return;
                }

                string file_path = null;
                if (Settings.Proxy.ProxiesFileUri.StartsWith("http", 0))
                {
                    WebClient w = new WebClient();
                    file_path = Log.AppDir + "\\" + "proxies.txt";
                    w.DownloadFile(Settings.Proxy.ProxiesFileUri, file_path);
                    Log.Main.Inform("Proxy file was downloaded from: " + Settings.Proxy.ProxiesFileUri + " to: " + file_path);
                }
                else
                    if (Settings.Proxy.ProxiesFileUri.Contains(":"))
                        file_path = Settings.Proxy.ProxiesFileUri;
                    else
                        file_path = Log.AppDir + "\\" + Settings.Proxy.ProxiesFileUri;

                string file_string = File.ReadAllText(file_path);
                Match m = Regex.Match(file_string, @"^\s*(.+?)\s*[,:\s](\d+)\s*$", RegexOptions.Multiline);
                while (m.Success)
                {
                    Proxy p = new Proxy();
                    p.Ip = m.Result("$1");
                    p.Port = m.Result("$2");
                    p.Login = Settings.Proxy.ProxyLogin;
                    p.Password = Settings.Proxy.ProxyPassword;
                    p.Type = Settings.Proxy.ProxyType;
                    Add(p);
                    m = m.NextMatch();
                }

                Log.Main.Inform("Proxy file was read: " + file_path);

                if (Settings.Proxy.ReloadProxyFileInSeconds > 0)
                    refresh_proxy_time = DateTime.Now.AddSeconds(Settings.Proxy.ReloadProxyFileInSeconds);
            }
        }

        static public Proxy Pop()
        {
            if (!use_proxy)
                return null;
            lock (proxies)
            {
                if (proxies.Count < 1
                        || refresh_proxy_time < DateTime.Now
                        )
                {
                    load_proxies_from_file();
                    if (!use_proxy)
                        return null;
                    if (proxies.Count < 1)
                        LogMessage.Exit("Proxy list is empty.");
                }

                //proxy = (Proxy)proxies.Dequeue();
                Proxy proxy = proxy = (Proxy)proxies[0];
                proxies.RemoveAt(0);
                return proxy;
            }
        }

        static public void Add(Proxy proxy)
        {
            if (!use_proxy)
                return;
            lock (proxies)
            {
                proxies.Add(proxy);
            }
        }

        static public Proxy Next()
        {
            if (!use_proxy)
                return null;
            lock (proxies)
            {
                Proxy proxy = (Proxy)Pop();
                Add(proxy);
                return proxy;
            }
        }

        static public void Delete(Proxy proxy)
        {
            if (!use_proxy)
                return;
            lock (proxies)
            {
                proxies.Remove(proxy);
                Log.Main.Write("Proxy deleted:" + proxy.Ip + ":" + proxy.Port);
            }
        }
    }
}
