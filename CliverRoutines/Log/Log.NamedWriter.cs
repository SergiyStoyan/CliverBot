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
            public class NamedWriter : Writer
            {
                NamedWriter(Session session, string name, string file_name)
                    : base(name, file_name, session)
                {
                    this.session = session;
                }

                readonly Session session;

                public static NamedWriter Get(Session session, string name)
                {
                    return get_log_thread(session, name);
                }
                                
                static NamedWriter get_log_thread(Session session, string name)
                {
                    lock (session)
                    {
                        NamedWriter tl = null;
                        if (!session.names2nw.TryGetValue(name, out tl))
                        {
                            try
                            {
                                string log_name = session.TimeMark + "_" + name + ".log";
                                tl = new NamedWriter(session, name, log_name);
                                session.names2nw.Add(name, tl);
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