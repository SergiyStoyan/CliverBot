//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;


namespace Cliver
{
    public static class SleepRoutines
    {
        public static object WaitForCondition(Func<object> check_condition, int mss)
        {
            object o = null;
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                o = check_condition();
                if (o != null)
                    break;
                Application.DoEvents();
            }
            return o;
        }

        public static void Wait(int mss)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
                Application.DoEvents();
        }        
    }
}

