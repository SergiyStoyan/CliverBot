//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliver.Bot
{
    public static partial class Cache
    {
        /// <summary>
        /// Saves downloaded binary file to current download dir
        /// </summary>
        /// <param name="content_is_text"></param>
        /// <param name="binary">bytes to be saved</param>
        /// <param name="page_count">number of page within current bot cycle</param>
        /// <param name="cycle_identifier">identifier of current bot cycle</param>
        /// <returns>full name of the saved file</returns>
        internal static string SaveDownloadedFile(bool content_is_text, byte[] binary, int page_count, string cycle_identifier)
        {
            if (!Settings.Log.LogDownloadedFiles)
                return null;

            if (binary == null)
                return null;

            string file = null;
            try
            {
                cycle_identifier = clear_path.Replace(cycle_identifier, "_");
                file = Log.DownloadDir + "/" + Log.Id.ToString() + "_" + page_count.ToString() + "_" + cycle_identifier;

                if (content_is_text)
                    file += ".html";
                else
                    file += ".binary";

                BinaryWriter bw = new BinaryWriter(File.Create(file));
                bw.Write(binary);
                bw.Close();
            }
            catch (Exception e)
            {
                Log.Main.Exit(e);
            }
            return file;
        }

        /// <summary>
        /// Cache downloaded file to current download dir
        /// </summary>
        /// <param name="content_is_text"></param>
        /// <param name="url"></param>
        /// <param name="response_url"></param>
        /// <param name="page">string to be saved</param>
        /// <param name="page_count">number of page within current bot cycle</param>
        /// <param name="cycle_identifier">identifier of current bot cycle</param>
        internal static string CacheDownloadedFile(bool content_is_text, string url, string post_parameters, string response_url, byte[] binary, int page_count, string cycle_identifier, WebRoutineStatus state)
        {
            string file = SaveDownloadedFile(content_is_text, binary, page_count, cycle_identifier);
            if (!Settings.Log.LogPostRequestParameters)
                post_parameters = null;
            Cache.AddFile2CacheMap(url, post_parameters, file, response_url, state, false);
            return file;
        }

        /// <summary>
        /// Used by IE routines
        /// </summary>
        /// <param name="text"></param>
        /// <param name="page_count"></param>
        /// <param name="cycle_identifier"></param>
        /// <returns></returns>
        internal static string SaveDownloadedFile(string text, int page_count, string cycle_identifier)
        {
            if (!Settings.Log.LogDownloadedFiles)
                return null;

            if (text == null)
                return null;

            string file = null;
            try
            {
                cycle_identifier = clear_path.Replace(cycle_identifier, "_");
                file = Log.DownloadDir + "/" + Log.Id.ToString() + "_" + page_count.ToString() + "_" + cycle_identifier + ".html";
                File.WriteAllText(file, text);
            }
            catch (Exception e)
            {
                Log.Main.Exit(e);
            }
            return file;
        }

        internal static string CacheDownloadedFile(string url, string post_parameters, string response_url, string text, int page_count, string cycle_identifier, WebRoutineStatus state)
        {
            string file = SaveDownloadedFile(text, page_count, cycle_identifier);
            if (!Settings.Log.LogPostRequestParameters)
                post_parameters = null;
            Cache.AddFile2CacheMap(url, post_parameters, file, response_url, state, true);
            return file;
        }

        public static string SaveFile(string text, string name)
        {
            if (text == null)
                return null;

            string file = null;
            try
            {
                file = Log.DownloadDir + "/" + Log.Id.ToString() + "_" + name + "_" + DateTime.Now.ToString("yyMMddHHmmss") + ".html";
                File.WriteAllText(file, text);
            }
            catch (Exception e)
            {
                Log.Main.Exit(e);
            }
            return file;
        }

        public static string CacheFile(string url, string post_parameters, string response_url, string text, string name, WebRoutineStatus state)
        {
            string file = SaveFile(text, name);
            Cache.AddFile2CacheMap(url, post_parameters, file, response_url, state, true);
            return file;
        }

        static Regex clear_path = new Regex(@"[\:\""\'\|\<\>\?\/\\\*\s]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }
}
