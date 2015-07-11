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

namespace Cliver.BotGui
{
    /// <summary>
    /// Interface used to customize GUI. May be not implemented.
    /// </summary>
    public class BotGui
    {
        /// <summary>
        /// Invoked when ConfigForm is filled with ConfigControls.
        /// </summary>
        /// <returns>array of ConfigControls's names, if null then any found is used</returns>
        virtual public string[] GetConfigControlNames()
        {
            return null;
        }

        /// <summary>
        /// Invoked when the app is started. 
        /// </summary>
        /// <returns>ToolsForm</returns>
        virtual public BaseForm GetToolsForm()
        {
            return null;
        }
              
        /// <summary>
        /// Invoked when bot cycle object is created.
        /// </summary>
        /// <returns></returns>
        virtual public Type GetBotThreadControlType()
        {
            return typeof(BotThreadControl);
        }
    }
}
