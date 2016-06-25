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
                NamedLog(Session session, string name, string log_file)
                    : base(name, log_file)
                {
                    this.session = session;
                }

                readonly Session session;

                public static NamedLog Get(Session session, string name)
                {
                    return get_log_thread(session, name);
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
                                string log_file;
                                switch (Cliver.Log.mode)
                                {
                                    case Cliver.Log.Mode.ONLY_LOG:
                                        log_file = Cliver.Log.WorkDir + @"\" + Cliver.Log.EntryAssemblyName + "_";
                                        break;
                                    //case Cliver.Log.Mode.SINGLE_SESSION:
                                    case Cliver.Log.Mode.SESSIONS:
                                        log_file = session.Path + @"\";
                                        break;
                                    default:
                                        throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                                }
                                log_file += session.TimeMark + "_" + name + ".log";

                                tl = new NamedLog(session, name, log_file);
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