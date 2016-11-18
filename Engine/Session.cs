//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Reflection;

/*
TBD:
- serialize InputItems into json and thus allow arrays etc
- change Cliver.Bot virtual members to event subscriptions
*/

namespace Cliver.Bot
{
    public enum ItemSourceType
    {
        DB,
        FILE
    }

    public partial class Session
    {
        public static Session This
        {
            get
            {
                return This_;
            }
        }
        static Session This_;

        Session()
        {
            This_ = this;

            Log.Writer.Exitig += ThreadLog_Exitig;

            input_item_type_name2input_item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(InputItem) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.InputItem.Initialize(input_item_type_name2input_item_types.Values.ToList());
            work_item_type_name2work_item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where (t.BaseType == typeof(WorkItem) && t.Name != typeof(SingleValueWorkItem<>).Name) || (t.BaseType != null && t.BaseType.Name == typeof(SingleValueWorkItem<>).Name) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.WorkItem.Initialize(work_item_type_name2work_item_types.Values.ToList());
            //tag_item_type_name2tag_item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where (t.BaseType == typeof(TagItem) && t.Name != typeof(SingleValueTagItem<>).Name) || (t.BaseType != null && t.BaseType.Name == typeof(SingleValueTagItem<>).Name) select t).ToDictionary(t => t.Name, t => t);
            tag_item_type_name2tag_item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(TagItem) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.TagItem.Initialize(tag_item_type_name2tag_item_types.Values.ToList());
            if (input_item_type_name2input_item_types.Count < 1)
                throw new Exception("No InputItem derive was found");

            workflow_xtw = new XmlTextWriter(Log.SessionDir + "\\" + STATES_FILE_NAME, Encoding.UTF8);
            workflow_xtw.Formatting = Formatting.Indented;
            workflow_xtw.WriteStartDocument();
            workflow_xtw.WriteStartElement("Session");

            if (Settings.General.WriteSessionRestoringLog)
            {
                items_xtw = new XmlTextWriter(Log.SessionDir + "\\" + ITEMS_FILE_NAME, Encoding.UTF8);
                items_xtw.Formatting = Formatting.Indented;
                items_xtw.WriteStartDocument();
                items_xtw.WriteStartElement("Items");
            }

            Restored = false;
            if (Settings.General.RestoreBrokenSession && !ProgramRoutines.IsParameterSet(CommandLineParameters.NOT_RESTORE_SESSION))
            {
                Restored = this.restore(ref StartTime);
                if (This == null)
                    return;
            }
            if (!Restored)
            {
                StartTime = DateTime.Now;
                Log.Main.Write("No session was restored so reading input Items from the input file");
                read_input_file();
            }

            //try
            //{
            CustomizationApi.SessionCreating();
            //}
            //catch (Exception e)
            //{
            //    LogMessage.Error(e);
            //    CustomizationApi.FatalError(e.Message);
            //    Session.Close();
            //}

            set_session_state(SessionState.STARTED, "session_start_time", StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        Dictionary<string, Type> input_item_type_name2input_item_types;
        Dictionary<string, Type> work_item_type_name2work_item_types;
        Dictionary<string, Type> tag_item_type_name2tag_item_types;

        void ThreadLog_Exitig(string message)
        {
            //CustomizationApi.FatalError();
            Close();
        }

        internal readonly Counter ProcessorErrors = new Counter("processor_errors", Settings.General.MaxProcessorErrorNumber, max_error_count);
        static void max_error_count(int count)
        {
            Session.FatalErrorClose("Fatal error: errors in succession " + count);
        }

        /// <summary>
        /// Time when the session was restored if it was.
        /// </summary>
        public readonly DateTime RestoreTime = DateTime.Now;

        /// <summary>
        /// Time when this session or rather restored session was started,.
        /// </summary>
        public readonly DateTime StartTime;//must be equal to RestoreTime if no restart

        public readonly bool Restored;

        public static void Start()
        {
            try
            {
                Log.Initialize(Log.Mode.SESSIONS, Cliver.Bot.Settings.Log.PreWorkDir, Cliver.Bot.Settings.Log.WriteLog, Cliver.Bot.Settings.Log.DeleteLogsOlderDays);
                Log.Main.Inform("Version compiled: " + Cliver.Bot.Program.GetCustomizationCompiledTime().ToString());
                Log.Main.Inform("Command line parameters: " + string.Join("|", Environment.GetCommandLineArgs()));

                if (This != null)
                    throw new Exception("Previous session was not closed.");
                new Session();
                if (This == null)
                    return;
                BotCycle.Start();
                Session.State = StateEnum.STARTED;
            }
            catch (ThreadAbortException)
            {
                Close();
                throw;
            }
            catch (Exception e)
            {
                Session.FatalErrorClose(e);
            }
        }

        /// <summary>
        /// Closes current session: closes session logs if all input Items were processed
        /// </summary>
        public static void Close()
        {
            lock (Log.MainThread)
            {
                if (This == null)
                    return;
                if (This.closing_thread != null)
                    return;
                This.closing_thread = ThreadRoutines.Start(This.close);
            }
        }
        Thread closing_thread = null;
        void close()
        {
            lock (This_)
            {
                try
                {
                    BotCycle.Abort();

                    if (This.items_xtw != null)
                    {
                        This.items_xtw.WriteEndElement();
                        This.items_xtw.WriteEndDocument();
                        This.items_xtw.Close();
                    }

                    if (This.input_item_queue_name2input_item_queues.Count > 0)
                    {
                        if (This.IsUnprocessedInputItem)
                            This.set_session_state(SessionState.ABORTED);
                        else if (This.IsItem2Restore)
                            This.set_session_state(SessionState.UNCOMPLETED);
                        else
                            This.set_session_state(SessionState.COMPLETED);
                    }
                    This.workflow_xtw.WriteEndElement();
                    This.workflow_xtw.WriteEndDocument();
                    This.workflow_xtw.Close();

                    if (This.IsUnprocessedInputItem)
                        Session.State = StateEnum.BROKEN;
                    else if (This.IsItem2Restore)
                        Session.State = StateEnum.UNCOMPLETED;
                    else
                        Session.State = StateEnum.COMPLETED;

                    try
                    {
                        CustomizationApi.SessionClosing();
                    }
                    catch (Exception e)
                    {
                        Session.State = StateEnum.FATAL_ERROR;
                        LogMessage.Error(e);
                        FatalError?.Invoke(e.Message);
                    }

                    try
                    {
                        if (Closing != null)
                            Closing.Invoke();
                    }
                    catch (Exception e)
                    {
                        LogMessage.Error(e);
                    }

                    InputItemQueue.Close();
                    FileWriter.ClearSession();
                    Log.Main.Write("Closing the bot session: " + Session.State.ToString());
                    Cliver.Log.ClearSession();

                    This_ = null;
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Session.State = StateEnum.FATAL_ERROR;
                    LogMessage.Error(e);
                    FatalError?.Invoke(e.Message);
                }
            }
        }

        void read_input_file()
        {
            Type start_input_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where t.IsSubclassOf(typeof(InputItem)) && !t.IsGenericType select t).First();
            InputItemQueue start_input_item_queue = GetInputItemQueue(start_input_item_type.Name);
            CustomizationApi.FillStartInputItemQueue(start_input_item_queue, start_input_item_type);
        }

        public enum StateEnum
        {
            NULL,
            STARTING,
            STARTED,
            RUNNING,
            CLOSING,
            COMPLETED,
            UNCOMPLETED,
            BROKEN,
            FATAL_ERROR
        }

        public static StateEnum State
        {
            get
            {
                if (This == null)
                    return StateEnum.NULL;
                return This._state;
            }
            private  set
            {
                if (This == null)
                    return;
                if (This._state >= StateEnum.COMPLETED)
                    return;
                if (This._state > value)
                    throw new Exception("Session state cannot change from " + This._state + " to " + value);
                This._state = value;
            }
        }
        StateEnum _state = StateEnum.NULL;
    }
}