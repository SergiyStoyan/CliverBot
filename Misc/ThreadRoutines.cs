//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Web;

namespace Cliver.Bot
{
    /// <summary>
    /// Sleep not freezing the app
    /// </summary>
    public static class ThreadRoutines
    {
        public static void Wait(long milliseconds, int poll_interval_in_mss = 20)
        {
            if (milliseconds / 2 < poll_interval_in_mss) poll_interval_in_mss /= 2;
            DateTime t = DateTime.Now.AddMilliseconds(milliseconds);
            while (t > DateTime.Now)
            {
                Application.DoEvents();
                Thread.Yield();
                if(poll_interval_in_mss > 0)
                    Thread.Sleep(poll_interval_in_mss);
            }
        }

        /// <summary>
        /// Waiting not freezing the app
        /// </summary>
        public static object WaitForCondition(Func<object> check_condition, int timeout_in_mss, int poll_interval_in_mss = 20)
        {
            object o = null;
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
            while (dt > DateTime.Now)
            {
                o = check_condition();
                if (o != null)
                    break;
                Application.DoEvents();
                Thread.Sleep(poll_interval_in_mss);
            }
            return o;
        }   
    }
}

