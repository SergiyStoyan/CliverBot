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
            TimerCallback tc = new TimerCallback(BotCycle.watch_for_threads);
            watcher = new System.Threading.Timer(tc, null, 0, 10000);
        }
        static readonly System.Threading.Timer watcher = null;

        static void watch_for_threads(object state)
        {
            lock (id2bot_cycles)
            {
                List<int> dead_threads = new List<int>();
                foreach (int id in id2bot_cycles.Keys)
                {
                    if (id2bot_cycles[id].thread.IsAlive)
                        continue;
                    Log.Error("The thread with ID: " + id + " is not alive. Removing it.");
                    dead_threads.Add(id);
                }
                foreach (int id in dead_threads)
                    close_thread(id);
            }
        }

        internal static void Start()
        {
            lock (id2bot_cycles)
            {
                if (id2bot_cycles.Count >= Settings.Engine.MaxBotThreadNumber)
                    return;
            }
            new BotCycle();
        }
        
        internal static void Abort()
        {
            BotCycle[] bcs;
            lock (id2bot_cycles)
            {
                bcs = id2bot_cycles.Values.ToArray();
            }
            foreach (BotCycle bc in bcs)
            {
                bc.run = false;
                bc.thread.Abort();
            }
            lock (id2bot_cycles)
            {
                id2bot_cycles.Clear();
            }
        }

        static void close_thread(int id)
        {
            try
            {
                if (Finishing != null)
                    Finishing.Invoke(id);
                lock (id2bot_cycles)
                {
                    BotCycle bc;
                    if (id2bot_cycles.TryGetValue(id, out bc))
                    {
                        if (bc.thread != Thread.CurrentThread)
                        {
                            bc.run = false;
                            bc.thread.Abort();
                        }
                        id2bot_cycles.Remove(id);
                    }
                    if (id2bot_cycles.Count == 0)
                        Session.Close(); 
                }
            }
            catch (ThreadAbortException) { }
        }

        BotCycle()
        {
            thread = ThreadRoutines.Start(bot_cycle);
            if (!SleepRoutines.WaitForCondition(() => { return Id >= 0; }, 100000))
                throw new Exception("Could not start BotCycle thread");
        }
        readonly Thread thread;
        internal readonly int Id = -1;
        bool run = true;

        void bot_cycle()
        {
            try
            {
                try
                {
                    typeof(BotCycle).GetField("Id", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, Log.Id);
                    lock (id2bot_cycles)
                    {
                        id2bot_cycles[Id] = this;
                    }
                    Created?.Invoke(Id);

                    bot = Bot.__Create();
                    if (bot == null)
                        throw (new Exception("Could not create Bot instance."));
                    typeof(Bot).GetField("BotCycle", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(bot, this);

                    bot.CycleStarting();
                    while (run)
                    {
                        current_item = Session.This.GetNext();
                        if (current_item == null)
                            return;
                        InputItemState state = InputItemState.COMPLETED;
                        try
                        {
                            current_item.PROCESSOR(this);
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
                                    //case ProcessorExceptionType.FATAL_ERROR:                                    
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
                    bot.CycleExiting();
                }
            }
            catch (Exception e)
            {
                Session.FatalErrorClose(e);
            }
            finally
            {
                close_thread(Id);
            }
        }
        InputItem current_item;
        static readonly Dictionary<int, BotCycle> id2bot_cycles = new Dictionary<int, BotCycle>();
        Bot bot;

        internal static InputItem GetCurrentInputItemForThisThread()
        {
            lock (id2bot_cycles)
            {
                BotCycle bc;
                if (id2bot_cycles.TryGetValue(Log.Id, out bc))
                    return bc.current_item;
                return null;
            }
        }

        internal static Bot GetBotForThisThread()
        {
            lock (id2bot_cycles)
            {
                BotCycle bc;
                if (id2bot_cycles.TryGetValue(Log.Id, out bc))
                    return bc.bot;
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
        //FATAL_ERROR
    }
}
