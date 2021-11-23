using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Web.Script.Serialization;

namespace Cliver.BotGui
{
    public partial class Settings
    {
        public static readonly GuiSettings Gui;

        public class GuiSettings : Cliver.UserSettings
        {
            public System.Drawing.Size ConfigFormSize = System.Drawing.Size.Empty;
            public int LastOpenConfigTabIndex = -1;
        }
    }
}