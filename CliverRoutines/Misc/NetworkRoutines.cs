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
        static public IPAddress GetLocalIp(IPAddress destination_ip)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect(destination_ip, 0);
                }
                catch(Exception e)
                {
                    return null;
                }
                IPEndPoint iep = (IPEndPoint)socket.LocalEndPoint;
                if (iep == null)
                    return null;
                return iep.Address;
            }
        }

        static public string GetLocalIpAsString(IPAddress destination_ip)
        {
            return GetLocalIp(destination_ip)?.ToString();
        }
    }
}

