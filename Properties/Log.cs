﻿//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System.Text.RegularExpressions;

namespace Cliver.Bot.Properties 
{    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Log 
    { 
        public Log()
        {
            this.SettingsLoaded += Log_SettingsLoaded;
        }

        //new public void Reload()
        //{
        //    base.Reload();
        //    Log_SettingsLoaded(null, null);
        //}

        void Log_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            if (string.IsNullOrEmpty(PreWorkDir))
            {
                PreWorkDir = Regex.Replace(Cliver.Bot.Log.AppDir, @":.*", @":\CliverBotSessions", RegexOptions.IgnoreCase| RegexOptions.Singleline);
                if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA)
                {
                    if (!LogMessage.AskYesNo("A folder where the application will store log data is not specified. By default it will be created in the following path:" + PreWorkDir + ".\nClick Yes if you agree, click No if you want to specify another location.", true, false))
                    {
                        System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
                        f.Description = "Specify a folder where the application will store log data.";
                        while (f.ShowDialog(/*MainForm.This*/) != System.Windows.Forms.DialogResult.OK) ;
                        PreWorkDir = f.SelectedPath;
                    }
                }
                else
                    LogMessage.Inform("A folder where the application will store log data is: " + PreWorkDir + ".\nIt can be changed in the app's settings");
                Save();
            }            
        }
    }
}
