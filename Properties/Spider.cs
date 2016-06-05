namespace Cliver.Bot.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Spider {
        
        public Spider() {
            this.SettingChanging += this.SettingChangingEventHandler;
            this.SettingsSaving += this.SettingsSavingEventHandler;
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) 
        {
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            if (UnchangableDomainPartNumber <= 1)
                if (!LogMessage.AskYesNo("UnchangableDomainPartNumber is < 2. That will make the bot crawl through external links of sites. This may consume all RAM on your comp and hang the app. Are you sure to proceed?", false))
                    Cliver.Log.Main.Exit("Exit since UnchangableDomainPartNumber < 2");

            if (MaxPageCountPerSite < 0)
                if (!LogMessage.AskYesNo("MaxPageCountPerSite is < 0. That will make the bot crawl through all links of the site independently on their quantity. This may consume all RAM on your comp and hang the app. Are you sure to proceed?", false))
                    Cliver.Log.Main.Exit("MaxPageCountPerSite is < 0");
        }
    }
}
