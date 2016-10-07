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
            OutputFieldSeparator = EscapedOutputFieldSeparator != null ? EscapedOutputFieldSeparator.Trim('\'') : ",";
            OutputEmptyFieldSubstitute = EscapedOutputEmptyFieldSubstitute != null ? EscapedOutputEmptyFieldSubstitute.Trim('\'') : " ";
            OutputFieldSeparatorSubstitute = EscapedOutputFieldSeparatorSubstitute != null ? EscapedOutputFieldSeparatorSubstitute.Trim('\'') : ";";

            if (string.IsNullOrWhiteSpace(OutputFileName))
            {
                if (OutputFieldSeparator == "\t")
                {
                    if (string.IsNullOrWhiteSpace(OutputFieldSeparatorSubstitute) || OutputFieldSeparatorSubstitute.Contains("\t"))
                        OutputFieldSeparatorSubstitute = " ";
                    OutputFileName = Cliver.Log.EntryAssemblyName + ".tsv";
                }
                else if (OutputFieldSeparator == ",")
                {
                    if (string.IsNullOrWhiteSpace(OutputFieldSeparatorSubstitute) || OutputFieldSeparatorSubstitute.Contains(","))
                        OutputFieldSeparatorSubstitute = ";";
                    OutputFileName = Cliver.Log.EntryAssemblyName + ".csv";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(OutputFieldSeparatorSubstitute) || OutputFieldSeparatorSubstitute.Contains(" "))
                        OutputFieldSeparatorSubstitute = " ";
                    if (string.IsNullOrEmpty(OutputFileName))
                        OutputFileName = Cliver.Log.EntryAssemblyName + ".txt";
                }
            }
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EscapedOutputFieldSeparator = "'" + OutputFieldSeparator + "'";
            EscapedOutputEmptyFieldSubstitute = "'" + OutputEmptyFieldSubstitute + "'";
            EscapedOutputFieldSeparatorSubstitute = "'" + OutputFieldSeparatorSubstitute + "'";
        }
    }
}
