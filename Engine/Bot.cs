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
    public partial class Bot
    {
        public static string GetAbout()
        {
            MethodInfo mi = __Type.GetMethod("GetAbout", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            if (mi != null)
                return (string)mi.Invoke(null, null);
            return @"Created: " + Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        //internal static ICustomCache CreateCustomCache()
        //{
        //    return (ICustomCache)create_instance_of("CustomCache");
        //}

        public static void FatalError(string message)
        {
            __FatalError?.Invoke(message);
        }

        public static void SessionCreating()
        {
            __SessionCreating?.Invoke();
        }
        
        public static void SessionClosing()
        {
            __SessionClosing?.Invoke();
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
        /// Invoked by BotCycle thread as it has been started.
        /// </summary>
        virtual public void CycleStarting()
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread when it is exiting.
        /// </summary>
        virtual public void CycleExiting()
        {
        }

        /// <summary>
        /// Invoked by default if no particular processor definition was found.
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
    }
}