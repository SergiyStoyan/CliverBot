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

    - use LiteSQL or DBLite as a Storage/data engine 
    VERDICT: append-file-storage is the fastest solution because of: 1)writting to the end of file; 2)keeping all the items ready in RAM. 
Drawbacks of append-file-storage: 1)growing log file (not quickly as only states are appended); 2)large amount of items will inundate RAM.
So, for large size data a db storage is required.

    - ? Bot static session methods move to a session subclass singleton within CustomBot 
    (: methods like GetAbout, FataLError should be static anyway)

    - uppercase methods to be overriden in custmization classes

    - BotGui - manage buttons array

    - 
*/

namespace Cliver.Bot
{
    public abstract partial class Session
    {
        static Session()
        {
            Log.Writer.Exitig += (string message) =>
            {
                //CustomizationApi.FatalError();
                Close();
            };

            input_item_type_names2input_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(InputItem) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.InputItem.Initialize(input_item_type_names2input_item_type.Values.ToList());
            work_item_type_names2work_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where (t.BaseType == typeof(WorkItem) && t.Name != typeof(SingleValueWorkItem<>).Name) || (t.BaseType != null && t.BaseType.Name == typeof(SingleValueWorkItem<>).Name) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.WorkItem.Initialize(work_item_type_names2work_item_type.Values.ToList());
            //tag_item_type_names2tag_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where (t.BaseType == typeof(TagItem) && t.Name != typeof(SingleValueTagItem<>).Name) || (t.BaseType != null && t.BaseType.Name == typeof(SingleValueTagItem<>).Name) select t).ToDictionary(t => t.Name, t => t);
            tag_item_type_names2tag_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(TagItem) select t).ToDictionary(t => t.Name, t => t);
            Cliver.Bot.TagItem.Initialize(tag_item_type_names2tag_item_type.Values.ToList());
            if (input_item_type_names2input_item_type.Count < 1)
                throw new Exception("No InputItem derive was found");
            foreach (Type iit in input_item_type_names2input_item_type.Values)
                foreach (Type t in Assembly.GetEntryAssembly().GetTypes().Where(t => t.BaseType == typeof(Session)))
                {
                    MethodInfo mi = t.GetMethod("PROCESSOR", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { iit }, null);
                    if (mi != null)
                        input_item_types2processor_mi[iit] = mi;
                }
        }

        public static Session This
        {
            get
            {
                return This_;
            }
        }
        static Session This_;

        protected Session()
        {
            This_ = this;
            State = SessionState.STARTING;

            Log.Main.Inform("Loading configuration from " + Config.DefaultStorageDir);
            Config.Reload(Config.DefaultStorageDir);

            ConfigurationDir = Dir + "\\" + Config.CONFIG_FOLDER_NAME;

            Restored = false;
            Storage = new SessionStorage();
            DateTime old_start_time;
            string old_time_mark;
            SessionState old_state = Storage.ReadLastState(out old_start_time, out old_time_mark);
            switch (old_state)
            {
                case SessionState.NULL:
                    break;
                case SessionState.STARTING:
                case SessionState.COMPLETED:
                case SessionState.FATAL_ERROR:
                    break;
                case SessionState.RESTORING:
                case SessionState.RUNNING:
                case SessionState.CLOSING:
                case SessionState.UNCOMPLETED:
                case SessionState.BROKEN:
                    if (Settings.Engine.RestoreBrokenSession && !ProgramRoutines.IsParameterSet(CommandLineParameters.NOT_RESTORE_SESSION))
                    {
                        if (LogMessage.AskYesNo("Previous session " + old_time_mark + " is not completed. Restore it?", true))
                        {
                            StartTime = old_start_time;
                            TimeMark = old_time_mark;
                            Storage.WriteState(SessionState.RESTORING, new { restoring_time = RestoreTime, restoring_session_time_mark = get_time_mark(RestoreTime) });
                            Log.Main.Inform("Loading configuration from " + ConfigurationDir);
                            Config.Reload(ConfigurationDir);
                            Storage.RestoreSession();
                            Restored = true;
                        }
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + old_state);
            }

            if (!Restored)
            {
                if (old_state != SessionState.NULL)
                {
                    string old_dir_new_path = Log.WorkDir + "\\Data" + "_" + old_time_mark + "_" + old_state;
                    Log.Main.Write("The old session folder moved to " + old_dir_new_path);
                    Storage.Close();
                    if(Directory.Exists(Dir))
                        Directory.Move(Dir, old_dir_new_path);
                    PathRoutines.CreateDirectory(Dir);
                    Storage = new SessionStorage();
                }

                StartTime = Log.MainSession.CreatedTime;// DateTime.Now;
                TimeMark = get_time_mark(StartTime);
                Storage.WriteState(SessionState.STARTING, new { session_start_time = StartTime, session_time_mark = TimeMark });
                read_input_file();
                Config.CopyFiles(Dir);
            }

            Creating?.Invoke();

            CREATING();
        }
        static Dictionary<string, Type> input_item_type_names2input_item_type;
        static Dictionary<string, Type> work_item_type_names2work_item_type;
        static Dictionary<string, Type> tag_item_type_names2tag_item_type;

        internal static Type[] InputItemTypes { get { return input_item_type_names2input_item_type.Values.ToArray(); } }

        internal readonly Counter ProcessorErrors = new Counter("processor_errors", Settings.Engine.MaxProcessorErrorNumber, max_error_count);
        static void max_error_count(int count)
        {
            Session.FatalErrorClose("Fatal error: errors in succession: " + count);
        }

        static string get_time_mark(DateTime dt)
        {
            return dt.ToString("yyMMddHHmmss");
        }

        public readonly string Dir = PathRoutines.CreateDirectory(Log.WorkDir + "\\Data");

        /// <summary>
        /// Time when the session was restored if it was.
        /// </summary>
        public readonly DateTime RestoreTime = DateTime.Now;

        /// <summary>
        /// Time when this session or rather restored session was started,.
        /// </summary>
        public readonly DateTime StartTime;//must be equal to RestoreTime if no restart
        public readonly string TimeMark;

        public readonly bool Restored;

        public readonly string ConfigurationDir;

        public static void Start()
        {
            try
            {
                Log.Initialize(Log.Mode.SESSIONS, Settings.Log.PreWorkDir, Settings.Log.WriteLog, Settings.Log.DeleteLogsOlderDays, Program.LogSessionPrefix);
                Log.Main.Inform("Version compiled: " + Program.GetCustomizationCompiledTime().ToString());
                Log.Main.Inform("Command line parameters: " + string.Join("|", Environment.GetCommandLineArgs()));

                if (This != null)
                    throw new Exception("Previous session was not closed.");
                Activator.Create<Session>(true);
                if (This == null)
                    return;
                BotCycle.Start();
                Session.State = SessionState.RUNNING;
                This.Storage.WriteState(SessionState.RUNNING, new { });
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
                State = SessionState.CLOSING;
                This.Storage.WriteState(State, new { });
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
                    Log.Main.Write("Closing the bot session: " + Session.State.ToString());
                    BotCycle.Abort();

                    if (This.IsUnprocessedInputItem)
                        State = SessionState.BROKEN;
                    else if (This.IsItem2Restore)
                        State = SessionState.UNCOMPLETED;
                    else
                        State = SessionState.COMPLETED;

                    This.Storage.WriteState(State, new { });

                    try
                    {
                        CLOSING();
                    }
                    catch (Exception e)
                    {
                        Session.State = SessionState.FATAL_ERROR;
                        This.Storage.WriteState(State, new { });
                        LogMessage.Error(e);
                        FATAL_ERROR(e.Message);
                    }

                    try
                    {
                        Closing?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Session.State = SessionState.FATAL_ERROR;
                        This.Storage.WriteState(State, new { });
                        LogMessage.Error(e);
                        FATAL_ERROR(e.Message);
                    }

                    InputItemQueue.Close();
                    FileWriter.ClearSession();
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    Session.State = SessionState.FATAL_ERROR;
                    This.Storage.WriteState(State, new { });
                    LogMessage.Error(e);
                    FATAL_ERROR(e.Message);
                }
                finally
                {
                    Storage.Close();
                    switch (State)
                    {
                        case SessionState.NULL:
                        case SessionState.STARTING:
                        case SessionState.COMPLETED:
                        case SessionState.FATAL_ERROR:
                            Directory.Move(Dir, Dir + "_" + TimeMark + "_" + State);
                            break;
                        case SessionState.RESTORING:
                        case SessionState.RUNNING:
                        case SessionState.CLOSING:
                        case SessionState.UNCOMPLETED:
                        case SessionState.BROKEN:
                            break;
                        default:
                            throw new Exception("Unknown option: " + State);
                    }
                    This_ = null;
                    Cliver.Log.ClearSession();
                }
            }

            try
            {
                Closed?.Invoke();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
                FATAL_ERROR(e.Message);
            }
        }

        void read_input_file()
        {
            Log.Main.Write("Loading InputItems from the input file.");
            Type start_input_item_type = (from t in Assembly.GetEntryAssembly().GetTypes() where t.IsSubclassOf(typeof(InputItem)) && !t.IsGenericType select t).First();
            InputItemQueue start_input_item_queue = GetInputItemQueue(start_input_item_type.Name);
            FillStartInputItemQueue(start_input_item_queue, start_input_item_type);
        }

        public static SessionState State
        {
            get
            {
                if (This == null)
                    return SessionState.NULL;
                return This._state;
            }
            private set
            {
                if (This == null)
                    throw new Exception("Trying to set session state while no session exists.");
                if (This._state >= SessionState.COMPLETED)
                    return;
                if (This._state > value)
                    throw new Exception("Session state cannot change from " + This._state + " to " + value);
                This._state = value;
            }
        }
        SessionState _state = SessionState.NULL;
    }

    public enum SessionState
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
}