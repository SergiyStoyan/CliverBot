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

namespace Cliver.Bot
{
    /// <summary>
    /// static part
    /// </summary>
    public partial class FileWriter
    {
        static object static_lock_variable = new object();

        /// <summary>
        /// Remove html tags, convert html entities, trim, remove delimiter spaces etc.
        /// </summary>
        /// <param name="str">field to be prepared</param>
        /// <param name="output_field_delimiter">field will be stripped from field delimiter passed here. Should be null if not needed.</param>
        //public static string PrepareField(string str, string output_field_delimiter)
        //{
        //    lock (static_lock_variable)
        //    {
        //        if (str == null)
        //            return "";

        //        str = Regex.Replace(str, "<!--.*?-->|[\n\r]", "", RegexOptions.Compiled | RegexOptions.Singleline);
        //        str = Regex.Replace(str, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
        //        str = HttpUtility.HtmlDecode(str);
        //        str = Regex.Replace(str, @"\t", " ", RegexOptions.Compiled | RegexOptions.Singleline);
        //        if (output_field_delimiter != null)
        //            str = Regex.Replace(str, output_field_delimiter, Properties.Output.Default.OutputFieldSeparatorSubstitute, RegexOptions.Compiled | RegexOptions.Singleline);
        //        str = Regex.Replace(str, @"\s\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                
        //        return str.Trim();
        //    }
        //}
        
        /// <summary>
        /// Remove html tags, convert html entities, trim, remove unnecessary spaces etc.
        /// </summary>
        /// <param name="str">field to be prepared</param>
        /// <returns>prepared value</returns>
        public static string PrepareField(string str, FieldFormat field_format)
        {
            if (str == null)
                return Properties.Output.Default.OutputEmptyFieldSubstitute;

            str = Regex.Replace(str, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);

            switch (field_format)
            {
                case FieldFormat.SV_FILE:
                    str = Regex.Replace(str, "<.*?>|[\n\r]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                    str = HttpUtility.HtmlDecode(str);
                    str = Regex.Replace(str, Properties.Output.Default.OutputFieldSeparator, Properties.Output.Default.OutputFieldSeparatorSubstitute, RegexOptions.Compiled | RegexOptions.Singleline);
                    break;
                case FieldFormat.DB_TABLE:
                    str = Regex.Replace(str, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                    str = HttpUtility.HtmlDecode(str);
                    break;
            }

            str = Regex.Replace(str, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	

            str = str.Trim();
            if (str == "")
                return Properties.Output.Default.OutputEmptyFieldSubstitute;

            return str;
        }

        public enum FieldFormat
        {
            /// <summary>
            /// prepare as db table field
            /// </summary>
            DB_TABLE,
            /// <summary>
            /// prepare as separated file field
            /// </summary>
            SV_FILE
        }

        /// <summary>
        /// Remove html tags, convert html entities, trim, remove unnecessary spaces etc. for saving within db table.
        /// </summary>
        /// <param name="str">field to be prepared</param>
        /// <returns>prepared value</returns>
        public static string PrepareField(string str)
        {
            return PrepareField(str, FieldFormat.DB_TABLE);
        }
        
        /// <summary>
        /// Remove html tags, convert html entities, trim, remove delimiter spaces, replaces default field separator etc.
        /// </summary>
        /// <param name="strs">array of fields to be prepared</param>
        /// <returns>field delimitered string</returns>
        public static string PrepareFields(params string[] strs)
        {
            string line = null;
            foreach (string str in strs)
            {
                string s = PrepareField(str, FieldFormat.SV_FILE);

                if (line == null)
                    line = s;
                else
                    line += Properties.Output.Default.OutputFieldSeparator + s;
            }
            return line;
        }

        /// <summary>
        /// Remove html tags saving text format, convert html entities.
        /// </summary>
        /// <param name="str">field to be prepared</param>
        public static string PrepareFieldSavingFormat(string str)
        {
            if (str == null)
                return "";

            str = Regex.Replace(str, "<!--.*?-->|[\r\n]", "", RegexOptions.Compiled | RegexOptions.Singleline);
            str = Regex.Replace(str, @"<(p|br|\/tr)(\s[^>]*>|>)", "\r\n", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "<.*?>", "", RegexOptions.Compiled | RegexOptions.Singleline);
            str = Regex.Replace(str, @"\t", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            str = Regex.Replace(str, @"[ ]{2,}", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
            str = Regex.Replace(str, @"[ ]\r", "\r", RegexOptions.Compiled | RegexOptions.Singleline);

            return (HttpUtility.HtmlDecode(str)).Trim();
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
