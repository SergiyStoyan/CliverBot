//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections.Generic;
using Cliver;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

namespace Cliver.BotGui
{
    /// <summary>
    /// Used to customize GUI. 
    /// </summary>
    public static class Api
    {
        public static IEnumerable<string> ConfigControlSections
        {
            get { return _ConfigControlSections; }
            set
            {
                if (Application.OpenForms.OfType<MainForm>().Any())
                    throw new Exception("_ConfigControlSections cannot be set.");
                _ConfigControlSections = value;
            }
        }
        static IEnumerable<string> _ConfigControlSections = null;

        public static BaseForm ToolsForm
        {
            get { return _ToolsForm; }
            set
            {
                if (Application.OpenForms.OfType<MainForm>().Any())
                    throw new Exception("_ToolsForm cannot be set.");
                _ToolsForm = value;
            }
        }
        static BaseForm _ToolsForm = null;

        public static Type BotThreadControlType
        {
            get { return _BotThreadControlType; }
            set
            {
                if (Bot.Session.State != Bot.Session.StateEnum.NULL)
                    throw new Exception("_BotThreadControlType cannot be set.");
                _BotThreadControlType = value;
            }
        }
        static Type _BotThreadControlType = null;

        internal static BotThreadControl CreateBotThreadControl(int id)
        {
            return (BotThreadControl)Activator.CreateInstance(BotThreadControlType, id);
        }
    }
}
