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
//        static readonly public ProgramMode Mode;

//        public class ProgramMode : StringEnum
//        {
//            public static readonly ProgramMode DIALOG = new ProgramMode("DIALOG");
//            public static readonly ProgramMode AUTOMATIC = new ProgramMode("AUTOMATIC");

//            protected ProgramMode(string value) : base(value) { }
//        }

//        public class CommandLineParameters : StringEnum
//        {
//            public static readonly CommandLineParameters SILENTLY = new CommandLineParameters("-silently");

//            protected CommandLineParameters(string value) : base(value) { }
//        }

//        static public bool IsParameterSet<T>(T parameter) where T : CommandLineParameters
//        {
//            return Regex.IsMatch(Environment.CommandLine, @"\s" + parameter.Value, RegexOptions.IgnoreCase);
//        }

//        public static void Initialize<T>(string title = null) where T : CommandLineParameters
//        {
//            ProgramMode mode = IsParameterSet(CommandLineParameters.SILENTLY) ? ProgramMode.AUTOMATIC : ProgramMode.DIALOG;
//            typeof(BaseProgram).GetField("Mode").SetValue(null, mode);
//            if (title == null)
//            {
//                AssemblyName ean = Assembly.GetEntryAssembly().GetName();
//                title = ean.Name;
//                if (ean.Version.Major > 0 || ean.Version.Minor > 0)
//                    title += ean.Version.Major + "." + ean.Version.Minor;
//            }
//            typeof(BaseProgram).GetField("Title").SetValue(null, mode);
//        }

//        //public string[] GetParametersSet()
//        //{
//        //    List<string> parameters = new List<string>();
//        //    foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
//        //        if (Regex.IsMatch(Environment.CommandLine, (string)fi.GetValue(null), RegexOptions.IgnoreCase))
//        //            parameters.Add(fi.Name);
//        //    return (string[])parameters.ToArray();
//        //}

//        static BaseProgram()
//        {
//        }

//        public static DateTime GetCompiledTime(Assembly assembly = null)
//        {
//            if(assembly == null)
//                assembly = Assembly.GetEntryAssembly();
//            string filePath = assembly.Location;
//            const int c_PeHeaderOffset = 60;
//            const int c_LinkerTimestampOffset = 8;
//            byte[] b = new byte[2048];
//            System.IO.Stream s = null;

//            try
//            {
//                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
//                s.Read(b, 0, 2048);
//            }
//            finally
//            {
//                if (s != null)
//                    s.Close();
//            }

//            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
//            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
//            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//            dt = dt.AddSeconds(secondsSince1970);
//            dt = dt.ToLocalTime();
//            return dt;
//        }

//        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//            LogMessage.Exit("Unhandled exception: " + e.ExceptionObject.ToString());
//        }

//        static public readonly string Title;
//        static readonly public string AppName = Application.ProductName;
//    }

//    public class StringEnum
//    {
//        public static readonly StringEnum NOT_SET = new StringEnum(null);

//        public override string ToString()
//        {
//            return Value;
//        }

//        protected StringEnum(string value)
//        {
//            this.Value = value;
//        }

//        public string Value { get; private set; }
//    }
//}


