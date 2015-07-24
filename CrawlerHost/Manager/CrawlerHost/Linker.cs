using System;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
//using mshtml;
using Cliver;
using System.Configuration;
using System.Windows.Forms;
//using MySql.Data.MySqlClient;
using Cliver.Bot;
using Microsoft.Win32;
using System.Reflection;

namespace Cliver.CrawlerHost
{
    public class Linker
    {
        public static void ResolveAssembly()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            while (!System.IO.Directory.Exists(Properties.Host.Default.CrawlerHostPath))
            {
                LogMessage.Error("Path to Crawler Host libs is not correct: '" + Properties.Host.Default.CrawlerHostPath + "'\nPlease set it in settings.");
                HostSettingsForm f = new HostSettingsForm();
                f.ShowDialog();
            }
        }

        static private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string file_name = Regex.Replace(args.Name, ",.*", "") + ".dll";
            List<string> folders = Directory.GetDirectories(Properties.Host.Default.CrawlerHostPath).ToList();
            folders.Add(Properties.Host.Default.CrawlerHostPath);
            foreach (string f in folders)
            {
                string file = f + @"\" + file_name;
                if (File.Exists(file))
                {
                    Log.Inform("Resolved assembly '" + args.Name + "' to '" + file + "'");
                    return Assembly.LoadFrom(file);
                }
            }
            return null;
        }
    }
}
