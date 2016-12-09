//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.IO;

namespace Cliver.Bot
{
    public partial class Bot
    {
        static Bot()
        {
            BotType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(Cliver.Bot.Bot))).FirstOrDefault();
            if (BotType == null)
                throw new Exception("No Bot type subclass was detected.");

            SessionType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(Cliver.Bot.Session))).FirstOrDefault();
            if (SessionType == null)
                throw new Exception("No Session type subclass was detected.");

            BotCycleType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(Cliver.Bot.BotCycle))).FirstOrDefault();
            if (BotCycleType == null)
                throw new Exception("No BotCycle type subclass was detected.");
        }

        internal static readonly Type BotType = null;
        internal static readonly Type SessionType = null;
        internal static readonly Type BotCycleType = null;

        public static string GetAbout()
        {
            MethodInfo mi = BotType.GetMethod("GetAbout", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            if (mi != null)
                return (string)mi.Invoke(null, null);
            return @"Compiled: " + Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        /// <summary>
        /// Called on any error considered fatal
        /// </summary>
        /// <param name="message"></param>
        public static void FatalError(string message)
        {
            MethodInfo mi = BotType.GetMethod("FatalError", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            mi?.Invoke(null, null);
        }
    }

    /// <summary>
    /// An empty sample of CliverBot customization.
    /// </summary>
    public class CustomBot : Bot
    {
        new public static string GetAbout()
        {
            return @"Compiled: " + Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        /// <summary>
        /// Called on any error considered fatal
        /// </summary>
        /// <param name="message"></param>
        new public static void FatalError(string message)
        {
        }

        public class CustomSession : Session
        {
            public override void CREATING()
            {
            }

            public override void CLOSING()
            {
            }
        }

        public class CustomBotCycle : BotCycle
        {
            override public void STARTING()
            {
            }

            override public void EXITING()
            {
            }
        }
    }
}