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

        public CommandLineParameters(string value) : base(value) { }
    }

    public static class Program
    {
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
                if (ProgramRoutines.IsParameterSet(CommandLineParameters.WINDOWLESS))
                {
                    Cliver.Bot.Program.Run();
                    return;
                }

                LogMessage.Output2Console = false;
                Cliver.Bot.Program.Initialize();

                if (Bot.Properties.App.Default.SingleProcessOnly)
                    ProcessRoutines.RunSingleProcessOnly();

                Application.Run(MainForm.This);
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }
    }
}