//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************


using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
//using System.Configuration;
using Cliver.Bot;

namespace Cliver.Bot
{
    public class LogMessage
    {
        static object lock_variable = new object();

        /// <summary>
        /// Defines whether message boxes will be showed (run in manual mode) 
        /// </summary>
        public static bool ShowMessages = Program.Mode == ProgramMode.DIALOG;

        public static bool Output2Console = !string.IsNullOrEmpty(Console.Title);

        public static bool EmailErrors = false;

        /// <summary>
        /// Receives owner window handle. It is needed to do message box owned.
        /// </summary>
        static IWin32Window owner = null;
        internal static IWin32Window Owner
        {
            set
            {
                lock (lock_variable)
                {
                    owner = value;
                }
            }
            get
            {
                //if (owner == null) 
                //    return MainForm.ActiveForm;// !!! cross-thread exception here !!!
                //else
                return owner;
            }
        }

        public static bool AskYesNo(string message, bool silent_yes, bool write2log = true)
        {
            lock (lock_variable)
            {
                if (write2log)
                    Log.Main.Write(message);

                if (ShowMessages)
                {
                    if (!Output2Console)
                    {
                        MessageBoxDefaultButton default_button = MessageBoxDefaultButton.Button1;
                        if (!silent_yes)
                            default_button = MessageBoxDefaultButton.Button2;

                        return MessageBox.Show(Owner, message,
                            Application.ProductName,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            default_button)
                            == DialogResult.Yes;
                    }
                    else
                    {
                        Console.WriteLine(message);
                        for (; ; )
                        {
                            Console.WriteLine("Enter Y[es] or N[o]:");
                            ConsoleKeyInfo cki = Console.ReadKey();
                            Console.WriteLine();
                            switch (cki.KeyChar.ToString().ToUpper())
                            {
                                case "Y":
                                    return true;
                                case "N":
                                    return false;
                            }
                        }
                    }
                }
                else
                {
                    return silent_yes;
                }
            }
        }

        public static void Error(string message)
        {
            Log.Main.Error(message);
            if (EmailErrors)
                email(message);
            lock (lock_variable)
            {
                if (ShowMessages)
                {
                    if (!Output2Console)
                    {
                        MessageBox.Show(Owner, message,
                            Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: " + message);
                    }
                }
            }
        }

        public static void Error(Exception e)
        {
            Error(Log.GetExceptionMessage(e));
        }

        public static void Exit(string message)
        {
            if (EmailErrors)
                email(message);
            lock (lock_variable)
            {
                if (ShowMessages)
                {
                    if (!Output2Console)
                    {
                        MessageBox.Show(Owner, message,
                            Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        Console.WriteLine("EXIT: " + message);
                        Console.ReadKey();
                    }
                }
            }
            Log.Main.Exit(message);
        }

        public static void Exit(Exception e)
        {
            Exit(Log.GetExceptionMessage(e));
        }

        static void email(string message)
        {
            try
            {
            }
            catch (Exception e)
            {
                Log.Main.Error(e);
            }
        }

        public static void Inform(string message)
        {
            Log.Main.Write(message);
            lock (lock_variable)
            {
                if (ShowMessages)
                {
                    if (!Output2Console)
                    {
                        MessageBox.Show(Owner, message,
                            Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
            }
        }

        public static void Inform(Exception e)
        {
            Inform(e.Message);
        }
    }
}
