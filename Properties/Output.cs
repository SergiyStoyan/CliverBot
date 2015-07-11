using System.Text.RegularExpressions;

namespace Cliver.Bot.Properties
{


    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Output
    {

        public Output()
        {
            this.SettingsSaving += this.SettingsSavingEventHandler;
            this.SettingsLoaded += Output_SettingsLoaded;
        }

        void Output_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            OutputFieldSeparator = EscapedOutputFieldSeparator != null ? EscapedOutputFieldSeparator.Trim('\'') : "";
            OutputEmptyFieldSubstitute = EscapedOutputEmptyFieldSubstitute != null ? EscapedOutputEmptyFieldSubstitute.Trim('\'') : "";
            OutputFieldSeparatorSubstitute = EscapedOutputFieldSeparatorSubstitute != null ? EscapedOutputFieldSeparatorSubstitute.Trim('\'') : "";
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EscapedOutputFieldSeparator = "'" + OutputFieldSeparator + "'";
            EscapedOutputEmptyFieldSubstitute = "'" + OutputEmptyFieldSubstitute + "'";
            EscapedOutputFieldSeparatorSubstitute = "'" + OutputFieldSeparatorSubstitute + "'";
        }
    }
}
