//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using Cliver.Bot;
using System.Configuration;

namespace Cliver.CrawlerHost
{
    public class Api
    {
        static public string GetCrawlerHostDirectory()
        {
            string d = GetAppDirectory();
            Match m = Regex.Match(d, @"^(.*\#[^\\\/]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            if(!m.Success)
                throw new Exception("Could not detect CrawlerHost directory path as it does not contain '#':\r\n" + d);
            return m.Groups[1].Value;
        }

        static public string GetCrawlerHostConfigFile()
        {
            return GetCrawlerHostDirectory() + @"\" + CrawlerHost_CONGIG_FILE_NAME;
        }
        public const string CrawlerHost_CONGIG_FILE_NAME = "Host.config";

        static public System.Configuration.Configuration GetCrawlerHostConfiguration()
        {
            ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
            ecfm.ExeConfigFilename = Cliver.CrawlerHost.Api.GetCrawlerHostConfigFile();
            return ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
        }

        static public string GetConnectionString(string name)
        {
            System.Configuration.Configuration c = Cliver.CrawlerHost.Api.GetCrawlerHostConfiguration();
            return c.ConnectionStrings.ConnectionStrings[name].ConnectionString;
        }

        static public void SaveConnectionString(string name, string value)
        {
            System.Configuration.Configuration c = Cliver.CrawlerHost.Api.GetCrawlerHostConfiguration();
            c.ConnectionStrings.ConnectionStrings[name].ConnectionString = value;
            c.Save();
        }

        static public string GetAppDirectory()
        {
            string p;
            if (ProgramRoutines.IsWebContext)
                p = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).CodeBase;
            else
                p = System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase;
            p = Regex.Replace(p, @"^\s*file:///", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Regex.Replace(p, @"[\\\/]*[^\\\/]*$", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
    }
}
