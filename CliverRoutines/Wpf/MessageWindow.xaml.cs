using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace Cliver.Wpf
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(string caption, Icon icon, string message, string[] buttons, int default_button, Window owner/*, bool button_auto_size = false*/)
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIconImageSource();

            this.ResizeMode = ResizeMode.CanResize;

            Owner = owner;

            if (icon == null)
                image.Width = 0;
            else
                image.Source = icon.ToImageSource();

            this.Title = caption;
            TextRange tr = new TextRange(this.message.Document.ContentEnd, this.message.Document.ContentEnd);
            tr.Text = message;

            if (buttons != null)
            {
                for (int i = buttons.Length - 1; i >= 0; i--)
                {
                    Button b = new Button();
                    b.Tag = i;
                    b.Content = buttons[i];
                    //b.AutoSize = true;
                    b.Click += b_Click;
                    this.buttons.Children.Add(b);
                    if (i == default_button)
                        b.Focus();
                }

                //if (!button_auto_size)
                //{
                //    Size max_size = new Size(0, 0);
                //    foreach (Button b in flowLayoutPanel1.Controls)
                //    {
                //        if (b.Width > max_size.Width)
                //            max_size.Width = b.Width;
                //        if (b.Height > max_size.Height)
                //            max_size.Height = b.Height;
                //    }
                //    foreach (Button b in flowLayoutPanel1.Controls)
                //    {
                //        b.AutoSize = false;
                //        b.Size = max_size;
                //    }
                //}
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
            base.ShowDialog();
            return clicked_button;
        }

        //private void Message_ContentsResized(object sender, ContentsResizedEventArgs e)
        //{
        //    var rtb = (RichTextBox)sender;
        //    Size s = this.Size;
        //    {
        //        int h = e.NewRectangle.Height - rtb.Height;
        //        if (h > 0)
        //        {
        //            int h2 = Screen.PrimaryScreen.WorkingArea.Height * 3 / 4 - this.Height;
        //            s.Height += h2 < h ? h2 : h;
        //        }
        //    }
        //    {
        //        int w = e.NewRectangle.Width - rtb.Width;
        //        if (w > 0)
        //        {
        //            int w2 = Screen.PrimaryScreen.WorkingArea.Width * 3 / 4 - this.Width;
        //            s.Width += w2 < w ? w2 : w;
        //        }
        //    }
        //    {
        //        if (s.Height > s.Width)
        //        {
        //            s.Height -= 100;
        //            s.Width += 100;
        //        }
        //    }
        //    this.Size = s;
        //}

        //protected override void WndProc(ref System.Windows.Forms.Message m)
        //{
        //    if (m.Msg == 0x0112) // WM_SYSCOMMAND
        //    {
        //        switch ((Int32)m.WParam)
        //        {
        //            case 0xF030: // Maximize event - SC_MAXIMIZE from Winuser.h
        //                restored_size = this.Size;
        //                break;
        //            case 0xF120: // Restore event - SC_RESTORE from Winuser.h
        //                this.Size = restored_size;
        //                break;
        //        }
        //    }
        //    base.WndProc(ref m);
        //}
        //Size restored_size;

        public new void Close()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        base.Close();
                    }
                    catch { }//if closed already
                });
            }
            catch { }//if closed already
        }
    }
}
