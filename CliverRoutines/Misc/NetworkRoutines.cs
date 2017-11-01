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
using System.Net.Sockets;
using System.Net;


namespace Cliver
{
    public static class NetworkRoutines
    {
        static public IPEndPoint GetLocalIp()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                return socket.LocalEndPoint as IPEndPoint;
            }
        }

        static public string GetLocalIpAsString()
        {
            return GetLocalIp().Address.ToString();
        }
    }
}

