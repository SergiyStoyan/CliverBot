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

        void Log_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            if (string.IsNullOrEmpty(PreWorkDir))
            {
                PreWorkDir = Cliver.Bot.Log.AppDir.Substring(0, Cliver.Bot.Log.AppDir.IndexOf(":")) + @":\CliverBotSessions";//used if GetRegistry will write error to log
                PreWorkDir = RegistryRoutines.GetValue("GeneralWorkDir");
                if (PreWorkDir == null)
                {
                    PreWorkDir = Cliver.Bot.Log.AppDir.Substring(0, Cliver.Bot.Log.AppDir.IndexOf(":")) + @":\CliverBotSessions";
                    if (!LogMessage.AskYesNo("A folder where the application will store log data is not specified. By default it will be created in the following path:" + PreWorkDir + ".\nClick Yes if you agree, click No if you want to specify another location.", true, false))
                    {
                        System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
                        f.Description = "Specify a folder where the application will store log data.";
                        while (f.ShowDialog(/*MainForm.This*/) != System.Windows.Forms.DialogResult.OK) ;
                        PreWorkDir = f.SelectedPath;
                    }
                    Save();
                    RegistryRoutines.SetValue("GeneralWorkDir", PreWorkDir);
                }
            }            
        }
    }
}
