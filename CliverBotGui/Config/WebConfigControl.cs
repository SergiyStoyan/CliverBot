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

        override protected void SetToolTip()
        {
            toolTip1.SetToolTip(this.UseFilesFromCache, "Define whether the bot will use files from the cache rather then download them from the net.");
            toolTip1.SetToolTip(this.HttpRequestTimeoutInSeconds, "Http request timeout in seconds.");
            toolTip1.SetToolTip(this.CrawlTimeIntervalInMss, "Time interval in miliseconds between http requests within the same work thread.");
            toolTip1.SetToolTip(this.HttpUserAgent, "Http User-Agent header value.");
            toolTip1.SetToolTip(this.MaxDownloadedFileLength, "If file size in bytes exeeds this value, the file will be chopped during downloading.");
            //toolTip1.SetToolTip(this.HttpRequestAcceptHeader, "Http Accept header value.");
            //toolTip1.SetToolTip(this.TextModeDownloadableContentTypePattern, "Http response having no Content-Type value matched to this regex pattern will be broken.");
            toolTip1.SetToolTip(this.MaxHttpRedirectionCount, "Maximal count of http redirections allowed.");
            toolTip1.SetToolTip(this.LogDownloadedFiles, "When checked, the bot will store all downloaded files to disk.");
            toolTip1.SetToolTip(this.LogPostRequestParameters, "Whether http post parameters are to be saved in cache.");
        }
    }
}

