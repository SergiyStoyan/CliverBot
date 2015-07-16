//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using System.Diagnostics;
//using System.Reflection;
//using System.Configuration;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Threading;

//namespace Cliver.Bot
//{
//    public class BaseProgram
//    {
//        public class CommandLineParameters
//        {
//            public const string SILENTLY = "-silently";
//        }

//        static List<string> GetParameters<T>() where T:CommandLineParameters
//        {
//           List<string> parameters = new List<string>();
//            foreach(FieldInfo fi in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
//                if (Regex.IsMatch(Environment.CommandLine, (string)fi.GetValue(null), RegexOptions.IgnoreCase))
//                    parameters.Add(fi.Name);
//            return parameters;
//        }

//        static BaseProgram()
//        {
//            AssemblyName ean = Assembly.GetEntryAssembly().GetName();
//            Title = ean.Name;
//            if (ean.Version.Major > 0 || ean.Version.Minor > 0)
//                Title += ean.Version.Major + "." + ean.Version.Minor;
//        }

//        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//            LogMessage.Exit("Unhandled exception: " + e.ExceptionObject.ToString());
//        }

//        static public readonly string Title;
//        static readonly public string AppName = Application.ProductName;

//        abstract static internal void Help();
//    }
//}


