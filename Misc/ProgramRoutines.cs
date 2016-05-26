﻿//********************************************************************************************
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


namespace Cliver.Bot
{
    public static class ProgramRoutines
    {
        public class CommandLineParameters
        {
            public static readonly CommandLineParameters NOT_SET = new CommandLineParameters(null);

            public override string ToString()
            {
                return Value;
            }

            protected CommandLineParameters(string value)
            {
                this.Value = value;
            }

            public string Value { get; private set; }
        }

        static public bool IsParameterSet<T>(T parameter) where T : CommandLineParameters
        {
            return Regex.IsMatch(Environment.CommandLine, @"\s" + parameter.Value, RegexOptions.IgnoreCase);
        }

        public static bool IsWebContext
        {
            get
            {
                return HttpRuntime.AppDomainAppId != null;
            }
        }

        static public string GetAppDirectory()
        {
            string p;
            if (ProgramRoutines.IsWebContext)
                p = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).CodeBase;
            else
                p = System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase;
            return Path.GetFullPath(p);
        }
    }
}

