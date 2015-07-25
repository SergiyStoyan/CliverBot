//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace Cliver
{
    /// <summary>
    /// Dynamic dialog box with many answer cases
    /// </summary>
    public partial class MessageForm : BaseForm
    {
        public MessageForm(string caption, Icon icon, string message, string[] buttons, int default_button, Form owner)
        {
            InitializeComponent();

            Owner = owner;

            int width = icon.Width;
            if (icon != null)
            {
                image_box.Image = (Image)icon.ToBitmap();
                int w = image_box.Image.Width - width;
                if (w > 0)
                {
                    this.Width += w;
                    this.message.Left += w;
                }
            }

            this.Text = caption;
            this.message.Text = message;

            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                Button b = new Button();
                b.Tag = i;
                b.Text = buttons[i];
                b.Click += b_Click;
                flowLayoutPanel1.Controls.Add(b);
                if (i == default_button)
                    b.Select();
            }
        }

        void b_Click(object sender, EventArgs e)
        {
            clicked_button = (int)((Button)sender).Tag;
            this.Close();
        }

        int clicked_button = -1;

        public int ClickedButton
        {
            get { return clicked_button; }
        }

        new public int ShowDialog()
        {
            System.Windows.Forms.DialogResult r = base.ShowDialog();
            return clicked_button;
        }

        private void Message_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            var rtb = (RichTextBox)sender;
            {
                int h = e.NewRectangle.Height - rtb.Height;
                if (h > 0)
                {
                    if (Screen.PrimaryScreen.WorkingArea.Height * 3 / 4 < this.Height + h)
                        h = Screen.PrimaryScreen.WorkingArea.Height * 3 / 4 - this.Height;
                    this.Height += h;
                }
            }
            {
                int w = e.NewRectangle.Width - rtb.Width;
                if (w > 0)
                {
                    if (Screen.PrimaryScreen.WorkingArea.Width * 3 / 4 < this.Width + w)
                        w = Screen.PrimaryScreen.WorkingArea.Width * 3 / 4 - this.Width;
                    this.Width += w;
                }
            }
        }    
    }
}
