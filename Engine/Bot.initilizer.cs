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
    public partial class Bot
    {
        internal static void __Initialize()
        {
        }

        static Bot()
        {
            __Type = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(Cliver.Bot.Bot))).FirstOrDefault();
            if (__Type == null)
                throw new Exception("No Bot type subclass was detected.");

            Session.OnFatalError FatalError = null;
            try
            {
                FatalError = (Session.OnFatalError)Delegate.CreateDelegate(typeof(Session.OnFatalError), __Type.GetMethod("FatalError", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch 
            {
                Log.Main.Warning("Method " + __Type.FullName + "::FatalError is not defined.");
            }
            Session.FatalError += FatalError != null ? FatalError : Bot.FatalError;

            Session.OnCreating SessionCreating = null;
            try
            {
                SessionCreating = (Session.OnCreating)Delegate.CreateDelegate(typeof(Session.OnCreating), __Type.GetMethod("SessionCreating", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + __Type.FullName + "::SessionCreating is not defined.");
            }
            Session.Creating += SessionCreating != null ? SessionCreating : Bot.SessionCreating;

            Session.OnClosing SessionClosing = null;
            try
            {
                SessionClosing = (Session.OnClosing)Delegate.CreateDelegate(typeof(Session.OnClosing), __Type.GetMethod("SessionClosing", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static));
            }
            catch
            {
                Log.Main.Warning("Method " + __Type.FullName + "::SessionClosing is not defined.");
            }
            Session.Closing += SessionClosing != null ? SessionClosing : Bot.SessionClosing;
        }

        internal static readonly Type __Type = null;

        internal static Bot __Create()
        {
            Bot cb = (Bot)Activator.CreateInstance(__Type);
            cb.__initializePROCESSORs();
            return cb;
        }

        void __initializePROCESSORs()
        {
            List<Type> input_item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(InputItem) select t).ToList();
            foreach (Type item_type in input_item_types)
            {
                string processor_name = "PROCESSOR";
                MethodInfo mi = item_type.GetMethod(processor_name, BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly | BindingFlags.Public, null, new Type[] { typeof(BotCycle) }, null);
                if (mi != null)
                    continue;
                mi = this.GetType().GetMethod(processor_name, BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly | BindingFlags.Public, null, new Type[] { item_type }, null);
                if (mi == null)
                {
                    mi = this.GetType().GetMethod(processor_name, BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly | BindingFlags.Public, null, new Type[] { typeof(InputItem) }, null);
                    if (mi == null)
                        throw new Exception("No '" + processor_name + "' for '" + item_type.Name + "' is found in " + this.GetType().Name);
                }
                __input_item_type2processor_mis[item_type] = mi;
            }
        }
        //internal protected readonly Dictionary<Type, Action<InputItem>> __input_item_type2processor_actions = new Dictionary<Type, Action<InputItem>>();
        internal protected readonly Dictionary<Type, MethodInfo> __input_item_type2processor_mis = new Dictionary<Type, MethodInfo>();
    }
}