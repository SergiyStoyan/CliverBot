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
using Cliver.Win;

namespace Cliver.BotGui
{
    public class CommandLineParameters 
    {
        public const string WINDOWLESS ="-windowless";
        public const string CONFIGURE = "-configure";//used only to edit settings while its performance is disabled
        public const string AUTOMATIC = "-automatic";
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

            if (CommandLine.IsParameterSet(CommandLineParameters.CONFIGURE))
            {
                Mode = ProgramMode.CONFIGURE;
                return;
            }
            if (CommandLine.IsParameterSet(CommandLineParameters.WINDOWLESS))
            {
                Mode = ProgramMode.WINDOWLESS;
                return;
            }
            if (CommandLine.IsParameterSet(CommandLineParameters.AUTOMATIC))
            {
                Mode = ProgramMode.AUTOMATIC;
                return;
            }
            Mode = ProgramMode.DIALOG;
        }

        /// <summary>
        /// By deafult each item type has its own queue. But independed named queues can be created during session.
        /// </summary>
        /// <param name="queue_name"></param>
        public static void BindProgressBar2InputItemQueue(string queue_name)
        {
            MainForm.This.ProgressBarInputItemQueueName = queue_name;
        }

        /// <summary>
        /// By deafult each item type has its own queue. But independed named queues can be created during session.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        public static void BindProgressBar2InputItemQueue<ItemT>() where ItemT : InputItem
        {
            MainForm.This.ProgressBarInputItemQueueName = typeof(ItemT).Name;
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
                        Application.Run(MainForm.This);
                        return;
                    case ProgramMode.DIALOG:
                        LogMessage.Output2Console = false;
                        LogMessage.DisableStumblingDialogs = false;
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