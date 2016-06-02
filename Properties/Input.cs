using System.IO;
using System;

namespace Cliver.Bot.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Input {
        
        public Input() 
        {
            this.SettingsLoaded += Input_SettingsLoaded;
            this.SettingsSaving += Input_SettingsSaving;
        }

        void Input_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EscapedInputFieldSeparator = "'" + InputFieldSeparator + "'";
        }

        void Input_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            InputFieldSeparator = EscapedInputFieldSeparator != null ? EscapedInputFieldSeparator.Trim('\'') : "";

            if (!InputFile.Contains(":"))
            {
                if (!Directory.Exists(Cliver.Bot.Log.AppCommonDataDir))
                    Directory.CreateDirectory(Cliver.Bot.Log.AppCommonDataDir);
                string input_file2 = Cliver.Bot.Log.AppCommonDataDir + "\\" + Input.Default.InputFile;
                if (!File.Exists(input_file2))
                    File.Copy(InputFile, input_file2);
                InputFile = input_file2;
                Save();
            }
        }
    }
}
