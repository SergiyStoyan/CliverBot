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
using System.Collections;
using Cliver;
using System.Diagnostics;
using System.Windows.Forms;
using Cliver.Bot;

namespace Cliver.BotGui
{
    partial class BotThreadControl
    {
        public static BotThreadControl GetBotThreadControlById(int bot_cycle_id)
        {
            return MainForm.This.BotThreadManagerForm.GetBotThreadControlById(bot_cycle_id);
        }

        public static BotThreadControl GetInstanceForThisThread()
        {
            return GetBotThreadControlById(Log.Thread.Id);
        }
    }
}
