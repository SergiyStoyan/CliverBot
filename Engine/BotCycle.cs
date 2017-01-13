//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using System.Reflection;

namespace Cliver.Bot
{
    public partial class BotCycle
    {
        static BotCycle()
        {
            foreach (Type iit in Session.InputItemTypes)
                foreach (Type t in Assembly.GetEntryAssembly().GetTypes().Where(t => t.BaseType == typeof(BotCycle)))
                {
                    MethodInfo mi = t.GetMethod("__Processor", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { iit }, null);
                    if (mi != null)
                        input_item_types2processor_mi[iit] = mi;
                }

            TimerCallback tc = new TimerCallback(BotCycle.watch_for_threads);
            watcher = new System.Threading.Timer(tc, null, 0, 10000);
        }
        static readonly System.Threading.Timer watcher = null;

        static void watch_for_threads(object state)
        {
            lock (threads2bot_cycle)
            {
                List<Thread> dead_threads = new List<Thread>();
                foreach (Thread t in threads2bot_cycle.Keys)
                {
                    if (t.IsAlive)
                        continue;
                    Log.Error("The thread with ID: " + threads2bot_cycle[t].Id + " is not alive. Removing it.");
                    dead_threads.Add(t);
                }
                foreach (Thread t in dead_threads)
                    close_thread(t);
            }
        }

        internal static void Start()
        {
            lock (threads2bot_cycle)//locked until threads2bot_cycle[t] = bc; to have a correct thread number
            {
                if (threads2bot_cycle.Count >= Settings.Engine.MaxBotThreadNumber)
                    return;
                BotCycle bc = null;
                Thread t = ThreadRoutines.Start(() =>
                {
                    bc = Activator.Create<BotCycle>(false);
                    bc.bot_cycle();
                }
                );
                if (!SleepRoutines.WaitForCondition(() => { return bc != null && bc.Id >= 0; }, 100000))
                    throw new Exception("Could not start BotCycle thread");
                threads2bot_cycle[t] = bc;
            }
        }

        internal static void Abort()
        {
            lock (threads2bot_cycle)
            {
                foreach (Thread t in threads2bot_cycle.Keys)
                {
                    threads2bot_cycle[t].run = false;
                    t.Abort();
                }
                threads2bot_cycle.Clear();
            }
        }

        static void close_thread(Thread t)
        {
            try
            {
                lock (threads2bot_cycle)
                {
                    BotCycle bc;
                    if (threads2bot_cycle.TryGetValue(t, out bc))
                    {
                        if (t != Thread.CurrentThread)
                        {
                            bc.run = false;
                            t.Abort();
                        }
                        threads2bot_cycle.Remove(t);
                    }
                    if (threads2bot_cycle.Count == 0)
                        Session.Close();
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        protected BotCycle()
        {
            //Id = 1;
            //lock (threads2bot_cycle)
            //{
            //    foreach (BotCycle bc in threads2bot_cycle.Values)
            //        if (Id <= bc.Id)
            //            Id = bc.Id + 1;
            //}
            Id = Log.Id;
        }
        internal readonly int Id = -1;
        bool run = true;

        void bot_cycle()
        {
            try
            {
                try
                {
                    Created?.Invoke(Id);

                    __Starting();
                    while (run)
                    {
                        current_item = Session.This.GetNext();
                        if (current_item == null)
                            return;
                        InputItemState state = InputItemState.COMPLETED;
                        try
                        {
                            current_item.__Processor(this);
                        }
                        catch (ThreadAbortException)
                        {
                            Thread.ResetAbort();
                            return;
                        }
                        catch (Session.FatalException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            if (e is TargetInvocationException)
                            {
                                e = e.InnerException;
                                //throw;
                            }
                            if (e is ProcessorException)
                            {
                                switch (((ProcessorException)e).Type)
                                {
                                    case ProcessorExceptionType.ERROR:
                                        state = InputItemState.ERROR;
                                        break;
                                    //case ProcessorExceptionType.FatalError:                                    
                                    case ProcessorExceptionType.RESTORE_AS_NEW:
                                        state = InputItemState.ERROR_RESTORE_AS_NEW;
                                        Session.This.IsItem2Restore = true;
                                        break;
                                    case ProcessorExceptionType.COMPLETED:
                                        break;
                                    default: throw new Exception("No case for " + ((ProcessorException)e).Type.ToString());
                                }
                            }
                            else
                            {
                                if (TreatExceptionAsFatal)
                                    throw new Session.FatalException(e);
                                state = InputItemState.ERROR;
                            }
                            Log.Error(e);
                        }

                        current_item.__State = state;

                        if (state == InputItemState.ERROR || state == InputItemState.ERROR_RESTORE_AS_NEW)
                            Session.This.ProcessorErrors.Increment();
                        else
                            Session.This.ProcessorErrors.Reset();

                        Start();
                    }
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                //catch (Exception e)
                //{
                //    throw new Session.FatalException(e);
                //}
                finally
                {
                    __Exiting();
                    if (Finishing != null)
                        Finishing.Invoke(Id);
                }
            }
            catch (Exception e)
            {
                Session.__ErrorClose(e, true);
            }
            finally
            {
                close_thread(Thread.CurrentThread);
            }
        }
        InputItem current_item;
        static readonly Dictionary<Thread, BotCycle> threads2bot_cycle = new Dictionary<Thread, BotCycle>();

        internal static InputItem GetCurrentInputItemForThisThread()
        {
            lock (threads2bot_cycle)
            {
                BotCycle bc;
                if (threads2bot_cycle.TryGetValue(Thread.CurrentThread, out bc))
                    return bc.current_item;
                return null;
            }
        }
    }

    public class ProcessorException : Exception
    {
        public ProcessorException(ProcessorExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public ProcessorException(ProcessorExceptionType type, string message, Exception exception)
            : base(message, exception)
        {
            Type = type;
        }

        readonly public ProcessorExceptionType Type;
    }

    public enum ProcessorExceptionType
    {
        COMPLETED,
        RESTORE_AS_NEW,
        ERROR,
        //FatalError
    }
}