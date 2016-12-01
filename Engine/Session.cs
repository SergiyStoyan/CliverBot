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
- log clean question - is it possible to make synchrone?
- ? Bot static session methods move to a session subclass singleton within CustomBot
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
            State = StateEnum.STARTING;

            Log.Main.Inform("Loading configuration from " + Config.DefaultStorageDir);
            Config.Reload(Config.DefaultStorageDir);

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

            if (Settings.Engine.WriteSessionRestoringLog)
            {
                items_xtw = new XmlTextWriter(Log.SessionDir + "\\" + ITEMS_FILE_NAME, Encoding.UTF8);
                items_xtw.Formatting = Formatting.Indented;
                items_xtw.WriteStartDocument();
                items_xtw.WriteStartElement("Items");
            }

            Restored = false;
            if (Settings.Engine.RestoreBrokenSession && !ProgramRoutines.IsParameterSet(CommandLineParameters.NOT_RESTORE_SESSION))
            {
                string restored_session_dir = null;
                Restored = this.restore(ref StartTime, ref restored_session_dir);
                if (closing_thread != null)
                    return;
                if (Restored)
                {//rename session folder
                 //!!! xml logs must be closed and re-open !!!
                    //string start_time_mark = Regex.Match(restored_session_dir, @"\d{12}").Value;
                    //Log.MainSession.Close(start_time_mark);
                }
            }

            if (!Restored)
            {
                StartTime = Log.MainSession.CreatedTime;// DateTime.Now;
                Log.Main.Write("No session was restored so reading input Items from the input file");
                read_input_file();
                Config.CopyFiles(Log.SessionDir);
            }

            Creating?.Invoke();

            Bot.SessionCreating();
        }
        Dictionary<string, Type> input_item_type_name2input_item_types;
        Dictionary<string, Type> work_item_type_name2work_item_types;
        Dictionary<string, Type> tag_item_type_name2tag_item_types;

        void ThreadLog_Exitig(string message)
        {
            //CustomizationApi.FatalError();
            Close();
        }

        internal readonly Counter ProcessorErrors = new Counter("processor_errors", Settings.Engine.MaxProcessorErrorNumber, max_error_count);
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
                //Bot.__Initialize();
                Log.Initialize(Log.Mode.SESSIONS, Cliver.Bot.Settings.Log.PreWorkDir, Cliver.Bot.Settings.Log.WriteLog, Cliver.Bot.Settings.Log.DeleteLogsOlderDays);
                Log.Main.Inform("Version compiled: " + Cliver.Bot.Program.GetCustomizationCompiledTime().ToString());
                Log.Main.Inform("Command line parameters: " + string.Join("|", Environment.GetCommandLineArgs()));

                if (This != null)
                    throw new Exception("Previous session was not closed.");
                new Session();
                if (This == null)
                    return;
                BotCycle.Start();
                Session.State = StateEnum.RUNNING;
                This.set_session_state(StateEnum.RUNNING, "session_start_time", This.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
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
                State = StateEnum.CLOSING;
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

                    if (This.IsUnprocessedInputItem)
                        State = StateEnum.BROKEN;
                    else if (This.IsItem2Restore)
                        State = StateEnum.UNCOMPLETED;
                    else
                        State = StateEnum.COMPLETED;

                    if (This.input_item_queue_name2input_item_queues.Count > 0)
                            This.set_session_state(State);

                    This.workflow_xtw.WriteEndElement();
                    This.workflow_xtw.WriteEndDocument();
                    This.workflow_xtw.Close();

                    try
                    {
                        Bot.SessionClosing();
                    }
                    catch (Exception e)
                    {
                        Session.State = StateEnum.FATAL_ERROR;
                        LogMessage.Error(e);
                        Bot.FatalError(e.Message);
                    }

                    try
                    {
                        Closing?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Session.State = StateEnum.FATAL_ERROR;
                        LogMessage.Error(e);
                        Bot.FatalError(e.Message);
                    }

                    InputItemQueue.Close();
                    FileWriter.ClearSession();
                    Log.Main.Write("Closing the bot session: " + Session.State.ToString());
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Session.State = StateEnum.FATAL_ERROR;
                    LogMessage.Error(e);
                    Bot.FatalError(e.Message);
                }
                finally
                {
                    StateEnum state = State;
                    This_ = null;
                    string sd = Log.SessionDir;
                    Cliver.Log.ClearSession();
                    switch(state)
                    {
                        case StateEnum.COMPLETED:
                        //case StateEnum.UNCOMPLETED://cannot change paths as they can be used in a restored session
                        //case StateEnum.BROKEN:
                        //case StateEnum.FATAL_ERROR:
                            try
                            {
                                Directory.Move(sd, sd + "_" + state);
                            }
                            catch (Exception e)
                            {
                                LogMessage.Error(e);
                                Bot.FatalError(e.Message);
                            }
                            break;
                    }
                }
            }

            try
            {
                Closed?.Invoke();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
                Bot.FatalError(e.Message);
            }
        }
        enum SessionFolderMark
        {
            DECLINED,
            COMPLETED
        }

        void read_input_file()
        {
            Type start_input_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where t.IsSubclassOf(typeof(InputItem)) && !t.IsGenericType select t).First();
            InputItemQueue start_input_item_queue = GetInputItemQueue(start_input_item_type.Name);
            FillStartInputItemQueue(start_input_item_queue, start_input_item_type);
        }

        public enum StateEnum
        {
            NULL,
            STARTING,
            RESTORING,//restoring phase
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
            private set
            {
                if (This == null)
                    throw new Exception("Trying to set session state while no session exists.");
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