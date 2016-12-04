//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        16 October 2007
//Copyright: (C) 2006-2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Data; 
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Cliver.BotWeb
{
    /// <summary>
    /// Interface used to customize Cache. May be not implemented.
    /// </summary>
    public interface ICustomCache
    {
        /// <summary>
        /// Create normalized url key. Used to eliminate sessionID from url etc.
        /// </summary>
        /// <param name="url">url to be searched within cache</param>
        /// <returns>url key</returns>       
        string CreateUrlKey(string url);
    }
    
    internal class CacheInfo
    {
        readonly internal string response_url;
        readonly internal string path;

        internal CacheInfo(string response_url, string path)
        {
            this.response_url = response_url;
            this.path = path;
        }

        internal CacheInfo(string path)
        {
            this.response_url = null;
            this.path = path;
        }
    }

	/// <summary>
	/// Used to write/read files to/from cache
	/// </summary>
    public static partial class Cache
    {
        static Cache()
        {
            Cliver.Bot.Session.Closing += clear_session;
            
            if (ProgramRoutines.IsParameterSet(Cliver.Bot.CommandLineParameters.PRODUCTION))
                Settings.Web.LogDownloadedFiles = false;
        }

        static object static_lock_variable = new object();
        const string CACHE_MAP_FILE_NAME = "cache_map.txt";
        static TextWriter cache_map_writer = null;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        //internal enum FileMark
        //{
        //    OK,
        //    TRUNCATED
        //}

        static string get_url_with_post(string url, string post_parameters)
        {
            url = url.Trim();
            if(!string.IsNullOrEmpty(post_parameters))
                url += "\r\n" + post_parameters.Trim();
            return url;
        }
        
        public static string CacheDir
        {
            get
            {
                if (cache_dir == null)
                    cache_dir = PathRoutines.CreateDirectory(Bot.Session.This.Dir + "\\" + CACHE_DIR_NAME);
                return cache_dir;
            }
        }
        static string cache_dir = null;
        const string CACHE_DIR_NAME = "cache";

        internal static void AddFile2CacheMap(string url, string post_parameters, string path, string response_url, WebRoutineStatus file_mark, bool text)
        {
            if (path == null)
                return;

            try
            {
                lock (static_lock_variable)
                {
                    if (cache_map_writer == null)
                    {
                        cache_map_writer = new StreamWriter(CacheDir + "\\" + CACHE_MAP_FILE_NAME, true, Encoding.UTF8);
                        if (cache_map == null)
                            cache_map = new Dictionary<string, CacheInfo>();
                    }
                }
                lock (cache_map_writer)
                {
                    Dictionary<string, object> d = new Dictionary<string, object>() ;
                    d["url"] = url;
                    if (url != response_url
                        && string.IsNullOrEmpty(post_parameters)//POST request response does not used!
                        )
                        d.Add("response_url", response_url);
                    d.Add("path", Path.GetFileName(path));
                    if (file_mark != WebRoutineStatus.OK)
                        d.Add("error", file_mark);
                    d.Add("text", text);
                    cache_map_writer.WriteLine(serializer.Serialize(d));
                    cache_map_writer.Flush();
                }
                add2cache_map(url, path, response_url);
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }

        static void add2cache_map(string url, string path, string response_url)
        {
            lock (cache_map)
            {
                if (url != response_url)
                    cache_map[url] = new CacheInfo(response_url, path);
                else
                    cache_map[url] = new CacheInfo(path);
                if (url != response_url && !string.IsNullOrEmpty(response_url))
                {//POST responses are not added!
                    cache_map[response_url] = new CacheInfo(response_url, path);
                }
                if (custom_cache != null)
                {
                    string url_key = custom_cache.CreateUrlKey(url);
                    if (!cache_map.ContainsKey(url_key))
                        cache_map[url_key] = new CacheInfo(response_url, path);

                    if (url != response_url && response_url != null)
                    {
                        url_key = custom_cache.CreateUrlKey(response_url);
                        if (!cache_map.ContainsKey(url_key))
                            cache_map[url_key] = new CacheInfo(response_url, path);
                    }
                }
            }
        }

        static public ICustomCache CustomCache
        {
            set
            {
                if (cache_map != null)
                    throw new Cliver.Bot.Session.FatalException("CustomCache should be set before cache_map is created.");
                custom_cache = value;
            }
        }

        static ICustomCache custom_cache = null;
        static Dictionary<string, CacheInfo> cache_map = null;
        //internal static CacheInfo GetCacheInfo(string url)
        //{
        //    lock (static_lock_variable)
        //    {
        //        if (url == null)
        //            return null;
        //        return (CacheInfo)cache_map[url];
        //    }
        //}        

        /// <summary>
        /// 
        /// </summary>
        /// <returns>false if cache was not restored due to any reason</returns>
        static bool restore_cache()
        {
            lock (static_lock_variable)
            {
                try
                {
                    cache_map = new Dictionary<string, CacheInfo>();
                    //custom_cache = (ICustomCache)CustomizationApi.CreateCustomCache();
                    DirectoryInfo wdi = new DirectoryInfo(Log.WorkDir);
                    DirectoryInfo[] session_dis = wdi.GetDirectories("*", SearchOption.TopDirectoryOnly);
                    Array.Sort(session_dis, new CompareDirectoryInfo());
                    for (int i = session_dis.Length - 1; i >= 0; i--)
                    {
                        DirectoryInfo sdi = session_dis[i];
                        if (!Directory.Exists(sdi.FullName + "\\" + CACHE_DIR_NAME))
                            continue;
                        if (sdi.FullName == Log.SessionDir)
                            continue;
                        string cm_file = sdi.FullName + "\\" + CACHE_DIR_NAME + "\\" + CACHE_MAP_FILE_NAME;
                        if (!File.Exists(cm_file))
                        {
                            Log.Main.Error("No cache map found: " + cm_file);
                            continue;
                        }
                        try
                        {
                            using (TextReader sr = new StreamReader(cm_file))
                            {
                                string session_name = Regex.Replace(sdi.FullName, @".*[\/\\](?=.+)", "", RegexOptions.Compiled | RegexOptions.Singleline);
                                for (string l = sr.ReadLine(); l != null; l = sr.ReadLine())
                                {
                                    Dictionary<string, object> d = serializer.Deserialize<Dictionary<string, object>>(l);
                                    string url = get<string>(d, "url");
                                    string response_url = get<string>(d, "response_url");
                                    string path = get<string>(d, "path");
                                    if (!path.Contains(session_name))
                                        path = "\\" + session_name + "\\" + CACHE_DIR_NAME + "\\" + path;
                                    bool text = get<bool>(d, "text", true);
                                    if (DoNotRestoreErrorFiles)
                                    {
                                        string error = get<string>(d, "error");
                                        if (error != null)
                                            continue;
                                    }
                                    add2cache_map(url, path, response_url);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Main.Error(e);
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                return false;
            }
        }
        class CompareDirectoryInfo : IComparer<DirectoryInfo>
        {
            public int Compare(DirectoryInfo d1, DirectoryInfo d2)
            {
                return  d1.LastWriteTime.CompareTo(d2.LastWriteTime);
            }
        }
       static T get<T>(Dictionary<string, object> d, string key, T default_value = default(T)) 
        {
            object o;
            if (d.TryGetValue(key, out o))
                return (T)o;
            return default_value;
        }

        public static bool DoNotRestoreErrorFiles = true;

        internal static bool GetCachedFile(string url, string post_parameters, out byte[] binary, out string response_url, out string cached_file)
        {
            //if (!Settings.General.flagUseFilesFromCache || url == null)
            //{
            //    binary = null;
            //    response_url = null;
            //    return false;
            //}
            lock (static_lock_variable)
            {
                if (cache_map == null)
                {
                    if (!restore_cache() && !LogMessage.AskYesNo("Could not restore cache! Continue with no cache?", false))
                        Log.Exit("Could not restore cache.");
                }
            }
            lock (cache_map)
            {
                try
                {
                    string url_post = get_url_with_post(url, post_parameters);

                    CacheInfo ci = null;
                    if (custom_cache != null)
                    {
                        string url_key = custom_cache.CreateUrlKey(url_post);
                        cache_map.TryGetValue(url_key, out ci);
                    }
                    if (ci == null)
                        cache_map.TryGetValue(url_post, out ci);
                    if (ci == null)
                    {
                        binary = null;
                        response_url = null;
                        cached_file = null;
                        return false;
                    }
                    cached_file = Log.WorkDir + "\\" + ci.path;
                    binary = File.ReadAllBytes(cached_file);
                    if (ci.response_url != null)
                        response_url = ci.response_url;
                    else
                        response_url = url;
                    return true;
                }
                catch (Exception e)
                {
                    Log.Main.Error(e);
                    binary = null;
                    response_url = null;
                    cached_file = null;
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Used to close cache in order to start a new session
        /// </summary>
        static void clear_session()
        {
            lock (static_lock_variable)
            {
                if (cache_map_writer != null)
                {
                    cache_map_writer.Close();
                    cache_map_writer.Dispose();
                    cache_map_writer = null;
                }
                cache_dir = null;
                custom_cache = null;
                cache_map = null;
            }
        }

        /// <summary>
        /// Remove url from cache
        /// </summary>
        public static void Remove(string url, string post_parameters)
        {
            lock (cache_map)
            {
                cache_map.Remove(get_url_with_post(url, post_parameters));
            }
        }
                
        /// <summary>
        /// Calculates volume of bytes in download directory
        /// </summary>
        /// <param name="download_dir"></param>
        /// <returns></returns>
        static long get_downloaded_bytes(string download_dir)
        {
            lock (static_lock_variable)
            {
                long downloaded_bytes = 0;
                try
                {
                    DirectoryInfo di = new DirectoryInfo(download_dir);
                    if (!di.Exists)
                    {
                        //					LogMessage.Write2Log("WARNING: Could not find download dir " + download_dir + " to calculate downloaded bytes!");
                        return 0;
                    }
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        downloaded_bytes += fi.Length;
                    }
                    foreach (DirectoryInfo di2 in di.GetDirectories())
                    {
                        downloaded_bytes += get_downloaded_bytes(di2.FullName);
                    }
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                return downloaded_bytes;
            }
        }
	}
}
