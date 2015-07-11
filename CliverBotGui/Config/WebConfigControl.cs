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
    public partial class WebConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "Web";

        public WebConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.HttpRequestTimeoutInSeconds, "Http request timeout in seconds.");
            toolTip1.SetToolTip(this.CrawlTimeIntervalInMss, "Time interval in miliseconds between http requests within the same work thread.");
            toolTip1.SetToolTip(this.HttpUserAgent, "Http User-Agent header value.");
            toolTip1.SetToolTip(this.MaxDownloadedFileLength, "If file size in bytes exeeds this value, the file will be chopped during downloading.");
            //toolTip1.SetToolTip(this.HttpRequestAcceptHeader, "Http Accept header value.");
            //toolTip1.SetToolTip(this.TextModeDownloadableContentTypePattern, "Http response having no Content-Type value matched to this regex pattern will be broken.");
            toolTip1.SetToolTip(this.MaxHttpRedirectionCount, "Maximal count of http redirections allowed.");            
        }
    }
}

