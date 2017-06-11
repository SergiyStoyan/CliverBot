//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cliver.BotWeb;

namespace Cliver.BotCustomization
{
    public partial class IeRoutineBotThreadControl : BotGui.BotThreadControl
    {
        public IeRoutineBotThreadControl(int id)
            : base(id)
        {
            InitializeComponent();
        }

        ~IeRoutineBotThreadControl()
        {
            if (Browser != null)
            {
                Browser.Stop();
                Browser.Dispose();
                Browser = null;
            }
        }

        public WebRoutine WR
        {
            set
            {
                this.wr = value;
                if (wr == null)
                    return;
                wr.OnLoading += WebRoutine_Loading;
                wr.OnProgress += WebRoutine_Progress;
            }
        }
        WebRoutine wr;

        void WebRoutine_Progress(int max, int point)
        {
            this.Invoke(() =>
            {
                if (point < 0)
                {
                    progressBar.Maximum = 1;
                    progressBar.Value = 0;
                    return;
                }
                if (max <= 0)
                    progressBar.Maximum = point * 2;
                else if (max < point)
                    progressBar.Maximum = point;
                else
                    progressBar.Maximum = max;
                progressBar.Value = point;
            });
        }

        void WebRoutine_Loading(string m)
        {
            labelState.SetText(m);
        }
    }
}