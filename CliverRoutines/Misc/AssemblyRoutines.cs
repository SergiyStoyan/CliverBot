//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;
using System.Reflection;

namespace Cliver
{
    public static class AssemblyRoutines
    {
        public static DateTime GetAssemblyCompiledTime(Assembly assembly)
        {
            byte[] bs = new byte[2048];
            System.IO.Stream s = new System.IO.FileStream(assembly.Location, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            s.Read(bs, 0, bs.Length);
            s.Close();
            int i = System.BitConverter.ToInt32(bs, 60);
            int secs_since_1970 = System.BitConverter.ToInt32(bs, i + 8);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secs_since_1970);
            return dt.ToLocalTime();
        }

        public static string GetAppVersion()
        {
            DateTime dt = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetEntryAssembly());
            DateTime dt2 = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetCallingAssembly());
            dt = dt > dt2 ? dt : dt2;
            return dt.ToString("yy-MM-dd-HH-mm-ss");
        }

        public static System.Drawing.Icon GetAppIcon()
        {
            return System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
        }

        public static System.Windows.Media.ImageSource GetAppIconImageSource()
        {
            return GetAppIcon().ToImageSource();
        }
    }
}