//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//        26 November 2014
//Copyright: (C) 2014, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Cliver.BotGui
{
    public partial class ProxyConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "Proxy";

        public ProxyConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.ProxiesFileUri, "Absolute path or only name or url of file that contains a list of proxies to be used. Can be empty if no proxy used.");
            toolTip1.SetToolTip(this.ProxyLogin, "Name of proxy user if needed.");
            toolTip1.SetToolTip(this.ProxyPassword, "Password of proxy user if needed.");
            toolTip1.SetToolTip(this.ReloadProxyFileInSeconds, "Proxy file will be reloaded after this time specified in seconds.");
            toolTip1.SetToolTip(this.MaxAttemptCountWithNewProxy, "Maximal count of download attempts using new proxy each time.");
            toolTip1.SetToolTip(this._1_ProxyTypeHttp, "Check if proxies to be used are HTTP.");
            toolTip1.SetToolTip(this._1_ProxyTypeSocks5, "Check if proxies to be used are SOCKS5.");            
        }
    }
}

