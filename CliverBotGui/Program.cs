using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Cliver.Bot;

namespace Cliver.BotGui
{
    public class CommandLineParameters : ProgramRoutines.CommandLineParameters
    {
        public static readonly CommandLineParameters WINDOWLESS = new CommandLineParameters("-windowless");
        public static readonly CommandLineParameters CONFIGURE = new CommandLineParameters("-configure");//used only to edit settings while it cannot perform

        public CommandLineParameters(string value) : base(value) { }
    }

    public static class Program
    {
        static Program()
        {
            Log.Initialize(Cliver.Bot.Properties.Log.Default.PreWorkDir, Cliver.Bot.Properties.Log.Default.WriteLog, Cliver.Bot.Properties.Log.Default.DeleteLogsOlderDays);
            Config.Initialize();
        }

        /// <summary>
        /// By deafult each item type has its own queue. But independed named queues can be created during session.
        /// </summary>
        /// <param name="input_item_type"></param>
        public static void BindProgressBar2InputItemQueue(Type input_item_type)
        {
            MainForm.This.ProgressBarInputItemQueueName = input_item_type.Name;
        }

        public static void Run()
        {
            try
            {
                if (ProgramRoutines.IsParameterSet(CommandLineParameters.CONFIGURE))
                {
                    LogMessage.Output2Console = false;
                    Cliver.Bot.Program.Initialize();
                    Log.Main.Inform("Configure mode. Run is disabled.");
                    Application.Run(MainForm.This);
                    return;
                }

                if (Bot.Properties.App.Default.SingleProcessOnly)
                    ProcessRoutines.RunSingleProcessOnly();

                if (ProgramRoutines.IsParameterSet(CommandLineParameters.WINDOWLESS))
                {
                    LogMessage.Output2Console = true;
                    Log.Main.Inform("Windowless mode.");
                    Cliver.Bot.Program.Run();
                    return;
                }

                LogMessage.Output2Console = false;
                Cliver.Bot.Program.Initialize();
                Application.Run(MainForm.This);
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }
    }
}