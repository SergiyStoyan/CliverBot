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
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Cliver.Bot
{
    static public class CustomizationApi
    {
        public static readonly string CUSTOM_NAMESPACE;
        public const string CUSTOM_BOT_CLASS_NAME = "CustomBot";

        static CustomizationApi()
        {
            CustomAssembly = Assembly.GetEntryAssembly();
            CUSTOM_NAMESPACE = (from t in CustomAssembly.GetExportedTypes() where t.Name == CUSTOM_BOT_CLASS_NAME select t.Namespace).FirstOrDefault();
            if (CUSTOM_NAMESPACE == null)
                LogMessage.Exit("Could not find class " + CUSTOM_BOT_CLASS_NAME + " in the entry assembly.");
            bot_type = CustomAssembly.GetType(CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME);
            if (bot_type == null)
                LogMessage.Exit("Could not find class " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME + " in the entry assembly.");

            try
            {
                session_creating = (Action)Delegate.CreateDelegate(typeof(Action), bot_type.GetMethod("SessionCreating", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME + ".SessionCreating was not found.");
            }
            try
            {
                session_closing = (Action)Delegate.CreateDelegate(typeof(Action), bot_type.GetMethod("SessionClosing", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME + ".SessionClosing was not found.");
            }
            try
            {
                fatal_error = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), bot_type.GetMethod("FatalError", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME + ".FatalError was not found.");
            }
            try
            {
                fill_start_input_item_queue = (Action<InputItemQueue, Type>)Delegate.CreateDelegate(typeof(Action<InputItemQueue, Type>), bot_type.GetMethod("FillStartInputItemQueue", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_CLASS_NAME + ".FillStartInputItemQueue was not found.");
            }            
        }
        internal static readonly Assembly CustomAssembly;
        static readonly Type bot_type;

        static readonly Action session_creating;
        static readonly Action session_closing;
        static readonly Func<string> fatal_error;
        static readonly Action<InputItemQueue, Type> fill_start_input_item_queue;

        /// <summary>
        /// Create instance of specified class in CliverBotCustomization assembly
        /// </summary>
        /// <param name="class_name">class to be instantiated</param>
        /// <returns>class instance</returns>
        static object create_instance_of(string class_name)
        {
            try
            {
                return CustomAssembly.CreateInstance(CUSTOM_NAMESPACE + "." + class_name);
            }
            catch (Exception e)
            {
                Log.Main.Warning(e);
            }
            return null;
        }

        internal static ICustomCache CreateCustomCache()
        {
            return (ICustomCache)create_instance_of("CustomCache");
        }

        internal static Bot CreateBot()
        {
            Bot cb = (Bot)Activator.CreateInstance(bot_type);
            cb.__InitializePROCESSORs();
            return cb;
        }

        internal static void SessionCreating()
        {
            session_creating?.Invoke();
        }

        internal static void SessionClosing()
        {
            session_closing?.Invoke();
        }

        internal static void FatalError(string message)
        {
            fatal_error?.Invoke();
        }

        internal static void FillStartInputItemQueue(InputItemQueue start_input_item_queue, Type start_input_item_type)
        {
            fill_start_input_item_queue.Invoke(start_input_item_queue, start_input_item_type);
            if (start_input_item_queue.CountOfNew < 1)
                LogMessage.Error("Input queue is empty so nothing is to do. Check your input data.");
        }
        
        public static string GetAbout()
        {
            return (string)bot_type.GetMethod("GetAbout", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        }
    }
}
