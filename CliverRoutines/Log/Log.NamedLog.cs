//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliver
{
    public partial class Log
    {
        partial class Session
        {
            public class NamedLog : Thread
            {
                NamedLog(Session session, string name, string file_name)
                    : base(name, file_name)
                {
                    this.session = session;
                }

                readonly Session session;

                public static NamedLog Get(Session session, string name)
                {
                    return get_log_thread(session, name);
                }

                override protected string get_directory()
                {
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ONLY_LOG:
                            return Cliver.Log.WorkDir + @"\";
                        //case Cliver.Log.Mode.SINGLE_SESSION:
                        case Cliver.Log.Mode.SESSIONS:
                            return session.Path + @"\";
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                }
                
                static NamedLog get_log_thread(Session session, string name)
                {
                    lock (session.names2tl)
                    {
                        NamedLog tl = null;
                        if (!session.names2tl.TryGetValue(name, out tl))
                        {
                            try
                            {
                                string log_name = session.TimeMark + "_" + name + ".log";
                                tl = new NamedLog(session, name, log_name);
                                session.names2tl.Add(name, tl);
                            }
                            catch (Exception e)
                            {
                                Cliver.Log.Main.Error(e);
                            }
                        }
                        return tl;
                    }
                }
            }
        }
    }
}