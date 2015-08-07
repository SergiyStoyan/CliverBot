//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web;
using System.Collections.Generic;

namespace Cliver.Bot
{
    /// <summary>
    /// static part
    /// </summary>
    public partial class FileWriter
    {
        public class Html
        {
            public static string PrepareField(string str)
            {
                return Cliver.PrepareField.Html.GetCsvField(str, Properties.Output.Default.OutputEmptyFieldSubstitute, Properties.Output.Default.OutputFieldSeparator, Properties.Output.Default.OutputFieldSeparatorSubstitute);
            }

            public static string PrepareFields(params string[] strs)
            {
                return Cliver.PrepareField.Html.GetCsvLine(strs, Properties.Output.Default.OutputEmptyFieldSubstitute, Properties.Output.Default.OutputFieldSeparator, Properties.Output.Default.OutputFieldSeparatorSubstitute);
            }
        }        

        public static string PrepareField(string str)
        {
            return Cliver.PrepareField.GetCsvField(str, Properties.Output.Default.OutputEmptyFieldSubstitute, Properties.Output.Default.OutputFieldSeparator, Properties.Output.Default.OutputFieldSeparatorSubstitute);
        }
        
        public static string PrepareFields(params string[] strs)
        {
            return Cliver.PrepareField.GetCsvLine(strs, Properties.Output.Default.OutputEmptyFieldSubstitute, Properties.Output.Default.OutputFieldSeparator, Properties.Output.Default.OutputFieldSeparatorSubstitute);
        }

        public static string Write(string file, byte[] binary)
        {
            if (binary == null)
                return null;
            string file_abs_path = get_file_abs_path(file, false, false);
            File.WriteAllBytes(file_abs_path, binary);
            return file_abs_path;
        }

        public static string Write(string file, string str)
        {
            if (str == null)
                return null;
            string file_abs_path = get_file_abs_path(file, false, false);
            File.WriteAllText(file_abs_path, str);
            return file_abs_path;
        }
    }
}
