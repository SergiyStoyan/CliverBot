//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Web;

namespace Cliver.Bot
{
    /// <summary>
    /// Miscellaneous useful methods for file path/name construction
    /// </summary>
    public static class PathRoutines
    {
        /// <summary>
        /// Clear file name from entities
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNormalizedFileName(string path)
        {
            path = HttpUtility.HtmlDecode(path);
            path = HttpUtility.UrlDecode(path);
            char[] cs = new char[2] { '/', '\\' };
            return Regex.Replace(path.Substring(path.LastIndexOfAny(cs) + 1), @"[^\w]", "-", RegexOptions.Compiled);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetFileName()
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameFromPath(string path)
        {
            return Regex.Replace(path, @".*\\", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetFileName()
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileExtensionFromPath(string path)
        {
            return Regex.Replace(path, @".*\.", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetDir()
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>        
        public static string GetDirFromPath(string path)
        {
            return Regex.Replace(path, @"[^\\]*$", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }        
    }
}

