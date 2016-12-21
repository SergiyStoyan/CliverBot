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
    public class Activator
    {
        public static T Create<T>(bool successor_only/*, params object[] args*/)
        {
            Type type;
            if (!base_types2custom_type.TryGetValue(typeof(T), out type))
            {
                type = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(T))).FirstOrDefault();
                base_types2custom_type[typeof(T)] = type;
            }
            if (type == null)
            {
                if (successor_only)
                    throw new Exception("No successor of " + typeof(T) + " was detected.");
                Log.Main.Warning("No successor of " + typeof(T) + " was detected.");
                //return (T)System.Activator.CreateInstance(typeof(T), args);
                return (T)System.Activator.CreateInstance(typeof(T), true);
            }
            //return (T)System.Activator.CreateInstance(type, args);
            return (T)System.Activator.CreateInstance(type, true);
        }
        static readonly Dictionary<Type, Type> base_types2custom_type = new Dictionary<Type, Type>();

        //        public static string GetAbout()
        //        {
        //            MethodInfo mi = BotType.GetMethod("GetAbout", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
        //            if (mi != null)
        //                return (string)mi.Invoke(null, null);
        //            return @"Compiled: " + Program.GetCustomizationCompiledTime().ToString() + @"
        //Developed by: www.cliversoft.com";
        //        }

        /// <summary>
        /// Called on any error considered fatal
        /// </summary>
        /// <param name="message"></param>
        //public static void FatalError(string message)
        //{
        //    MethodInfo mi = BotType.GetMethod("FatalError", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
        //    mi?.Invoke(null, null);
        //}
    }
}