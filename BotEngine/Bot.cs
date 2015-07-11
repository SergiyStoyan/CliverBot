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
    /// <summary>
    /// Most important interface that defines certain routines of CliverBot customization.
    /// </summary>
    public class Bot
    {
        static public string GetAbout()
        {
            return @"WEB CRAWLER
Created: " + Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        /// <summary>
        /// Allows to access to CliverBot api
        /// </summary>
        readonly protected BotCycle BotCycle;

        static public BotT __GetInstanceForThisThread<BotT>() where BotT : Bot
        {
            return BotCycle.GetBotForThisThread<BotT>();
        }

        /// <summary>
        /// Invoked when the session is in creating stage. Can be not defined. If throw an Exception, the session is stopped and closed.
        /// </summary>
        static public void SessionCreating()
        {
        }

        static public void FillStartInputItemQueue(InputItemQueue start_input_item_queue, Type start_input_item_type)
        {
            Log.Main.Write("Filling queue of " + start_input_item_queue.Name + " with input file.");

            if (!File.Exists(Properties.Input.Default.InputFile))
                throw (new Exception("Input file " + Properties.Input.Default.InputFile + " does not exist."));

            if (Path.GetExtension(Properties.Input.Default.InputFile).StartsWith(".xls", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Reading excel was not implemented");

            FileReader fr = new FileReader(Properties.Input.Default.InputFile, Properties.Input.Default.InputFieldSeparator);
            for (FileReader.Row r = fr.ReadLine(); r != null; r = fr.ReadLine())
                InputItem.Add2QueueBeforeStart(start_input_item_queue, start_input_item_type, r.Headers.ToDictionary(x => x, x => r[x]));
        }

        /// <summary>
        /// Invoked while closing the session.
        /// </summary>
        static public void SessionClosing()
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread as it has been started.
        /// </summary>
        virtual public void CycleBeginning()
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread when it is exiting.
        /// </summary>
        virtual public void CycleFinishing()
        {
        }

        /// <summary>
        /// Invoked by default if no particulare processor definition was found.
        /// </summary>
        virtual public void PROCESSOR(InputItem item)
        {
            //__input_item_type2processor_actions[item.GetType()](item);
            try 
            {
                __input_item_type2processor_mis[item.GetType()].Invoke(this, new object[] { item });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
        
        internal void __InitializePROCESSORs()
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
