//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//        26 November 2014
//Copyright: (C) 2014, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Cliver.BotGui
{
    public partial class GeneralConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "General";

        public GeneralConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void SetToolTip()
        {
            toolTip1.SetToolTip(this.MaxBotThreadNumber, "Maximal work thread count that the bot may run.");
            toolTip1.SetToolTip(this.RestoreErrorItemsAsNew, "Define whether items marked with error during the previous session will be reprocessed in the next session as new.");
            toolTip1.SetToolTip(this.WriteSessionRestoringLog, "Define whether the bot will write files needed to restore session.");
            toolTip1.SetToolTip(this.RestoreBrokenSession, "Define whether the bot will restore the previous session if it was broken.");
            //toolTip1.SetToolTip(this.SingleProcessOnly, "When checked, the bot will exit if its instance runs already on this machine.");
            //toolTip1.SetToolTip(this.RunSilently, "When checked, the bot will start session immediately and will not display any message boxes during its work.");
            toolTip1.SetToolTip(this.MaxProcessorErrorNumber, "Max number of consecutive errors in bot cycle before automatic exit. Set -1 not to exit.");
            toolTip1.SetToolTip(this.MaxTime2WaitForSessionStopInSecs, "Timeout of wait for closing session before exit.");          
        }
    }
}

