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
    public static class Program
    {
        //public static void SetProgressInputItemQueue(string input_item_queue_name)
        //{
        //    MainForm.SetProgressInputItemQueue(input_item_queue_name);
        //}

        /// <summary>
        /// By deafult each item type has its own queue. But independed named queues can be created during session.
        /// </summary>
        /// <param name="input_item_type"></param>
        public static void BindProgressBar2InputItemQueue(Type input_item_type)
        {
            MainForm.ProgressBarInputItemQueueName = input_item_type.Name;
        }

        public static void Run()
        {
            try
            {
                if (Regex.IsMatch(Environment.CommandLine, "-windowless", RegexOptions.IgnoreCase) || Bot.Properties.General.Default.RunSilently)
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