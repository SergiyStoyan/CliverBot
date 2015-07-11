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
    public partial class SpiderConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "Spider";

        public SpiderConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.UnchangableDomainPartNumber, "Number of domain parts that should be the same for any url crawled by the bot. Usually it is 2 so that i.e. www.google.com is acceptable for google.com but google.co.uk is not.");
            //toolTip1.SetToolTip(this.NonSpideredExtensions, "Downloaded files mathed to this regex pattern will not be scraped for links.");
            toolTip1.SetToolTip(this.MaxDownloadLinkDepth, "Maximal depth of site crawling. Entry url has depth 0. Pages linked to the entry page have depth 1 etc.");
            toolTip1.SetToolTip(this.ComplyRobotProtocol, "Define whether the bot will comply the robot protocol, including 'nofollow' tags.");
            //toolTip1.SetToolTip(this.DownloadedContentTypePattern, "Http response having no Contant-Type matched to this regex pattern will be broken.");
            toolTip1.SetToolTip(this.MaxPageCountPerSite, "Stop spidering the site as the max number of pages were spidered. Infinity = -1");   
        }
    }
}

