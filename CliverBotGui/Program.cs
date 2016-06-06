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
        public static readonly CommandLineParameters AUTOMATIC = new CommandLineParameters("-automatic");

        public CommandLineParameters(string value) : base(value) { }
    }

    public static class Program
    {
        public enum ProgramMode
        {
            AUTOMATIC,
            DIALOG, 
            CONFIGURE,
            WINDOWLESS
        }

        static readonly public ProgramMode Mode;

        static Program()
        {
            Cliver.Bot.Program.Initialize();

            if (ProgramRoutines.IsParameterSet(CommandLineParameters.CONFIGURE))
            {
                Mode = ProgramMode.CONFIGURE;
                return;
            }
            if (ProgramRoutines.IsParameterSet(CommandLineParameters.WINDOWLESS))
            {
                Mode = ProgramMode.WINDOWLESS;
                return;
            }
            if (ProgramRoutines.IsParameterSet(CommandLineParameters.AUTOMATIC))
            {
                Mode = ProgramMode.AUTOMATIC;
                return;
            }
            Mode = ProgramMode.DIALOG;
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
                switch (Mode)
                {
                    case ProgramMode.AUTOMATIC:
                        LogMessage.Output2Console = false;
                        LogMessage.DisableStumblingDialogs = true;
                        Config.Initialize();
                        Application.Run(MainForm.This);
                        return;
                    case ProgramMode.WINDOWLESS:
                        LogMessage.Output2Console = true;
                        LogMessage.DisableStumblingDialogs = true;
                        Cliver.Bot.Program.Run();
                        return;
                    case ProgramMode.CONFIGURE:
                        LogMessage.Output2Console = false;
                        LogMessage.DisableStumblingDialogs = false;
                        Config.Initialize();
                        Application.Run(MainForm.This);
                        return;
                    case ProgramMode.DIALOG:
                        LogMessage.Output2Console = false;
                        LogMessage.DisableStumblingDialogs = false;
                        Config.Initialize();
                        Application.Run(MainForm.This);
                        break;
                    default:
                        throw new Exception("Unknown mode: " + Mode);
                }
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }
    }
}