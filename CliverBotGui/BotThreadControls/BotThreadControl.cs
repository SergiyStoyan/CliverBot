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
using Cliver.Bot;

namespace Cliver.BotGui
{
    public partial class BotThreadControl : UserControl
    {
        new internal bool Visible
        {
            set
            {
                base.Visible = value;
                OnVisible(value);
                if (value)
                    BringToFront();
            }
            get
            {
                return base.Visible;
            }
        }
        
        virtual public void OnVisible(bool visible)
        {
        }
        
        public readonly int Id;

        public BotThreadControl(int id)
            : base()
        {
            IntPtr ip = Handle;//it is gotten here in order that this.InvokeRequired to work correctly            
            Id = id;
            Text = "Bot Thread #" + id.ToString();
        }

        //required for designer viewer
        public BotThreadControl()
            : base()
        {
        }
    }
}