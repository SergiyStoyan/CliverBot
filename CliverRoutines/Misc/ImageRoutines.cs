//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UsI9Pdf
{
    class ImageRoutines
    {
        public static Image GetScaledImage(Image image, Size max_size)
        {
            var ratio = Math.Min((double)max_size.Width / image.Width, (double)max_size.Height / image.Height);
            var w = (int)(image.Width * ratio);
            var h = (int)(image.Height * ratio);
            var i = new Bitmap(w, h);
            using (var graphics = Graphics.FromImage(i))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(image, 0, 0, w, h);
            }
            return i;
        }

        public static Image GetCroppedByColor(Image image, Color color)
        {
            Bitmap b = new Bitmap(image);
            int y = b.Height, height = 0, x = b.Width, width = 0;
            for (int h = 0; h < b.Height; h++)
            {
                bool another_color_found = false;
                int wl = 0;
                for (; wl < b.Width; wl++)
                {
                    Color c = b.GetPixel(wl, h);
                    if (c != color)
                    {
                        another_color_found = true;
                        if (x > wl)
                            x = wl;
                        break;
                    }
                }
                for (int wr = b.Width - 1; wr > wl; wr--)
                {
                    Color c = b.GetPixel(wr, h);
                    if (c != color)
                    {
                        another_color_found = true;
                        if (width <= wr)
                            width = wr + 1;
                        break;
                    }
                }
                if (another_color_found)
                {
                    if (height <= h)
                        height = h + 1;
                    if (y > h)
                        y = h;
                }
            }
            if (y >= b.Height)
                return new Bitmap(0, 0);
            b = new Bitmap(width - x, height - y);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, x, y, width, height);
            }
            //b.Save("2.png");
            return b;
        }
    }
}