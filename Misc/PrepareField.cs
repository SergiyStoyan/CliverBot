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

namespace Cliver
{
    public class PrepareField
    {
        public class Html
        {
            public static string GetCsvCell(string value, string default_value = "", string separator = ",", string separator_substitute = ";")
            {
                if (value == null)
                    return default_value;

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = Regex.Replace(value, @"[^\u0000-\u007F]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, separator, separator_substitute, RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return value;
            }

            public static string GetDbCell(string value, string default_value = "")
            {
                if (value == null)
                    return default_value;

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = Regex.Replace(value, @"[^\u0000-\u007F]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return value;
            }

            public static string GetCsvLine(dynamic o, string default_value = "", string separator = ",", string separator_substitute = ";")
            {
                List<string> ss = new List<string>();
                foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
                {
                    object p = pi.GetValue(o, null);
                    if (pi.PropertyType == typeof(string))
                        ss.Add(GetCsvCell((string)p, default_value, separator, separator_substitute));
                    else
                        ss.Add(p.ToString());
                }
                return string.Join(separator, ss);
            }

            public static Dictionary<string, object> GetDbObject(dynamic o, string default_value = "")
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
                {
                    object p = pi.GetValue(o, null);
                    if (pi.PropertyType == typeof(string))
                        p = GetDbCell((string)p, default_value);
                    d[pi.Name] = p;
                }
                return d;
            }

            public static string GetDbCellKeepingFormat(string value, string default_value = "")
            {
                if (value == null)
                    return "";

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"<(p|br|\/tr)(\s[^>]*>|>)", "\r\n", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = Regex.Replace(value, @"[^\u0000-\u007F]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"[ ]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return (HttpUtility.HtmlDecode(value)).Trim();
            }
        }

        public static string GetCsvCell(string value, string default_value = "", string separator = ",", string separator_substitute = ";")
        {
            if (value == null)
                return default_value;

            value = Regex.Replace(value, @"[^\u0000-\u007F]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = Regex.Replace(value, separator, separator_substitute, RegexOptions.Compiled | RegexOptions.Singleline);
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = value.Trim();
            if (value == "")
                return default_value;
            return value;
        }

        public static string GetDbCell(string value, string default_value = "")
        {
            if (value == null)
                return default_value;

            value = Regex.Replace(value, @"[^\u0000-\u007F]", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
            value = value.Trim();
            if (value == "")
                return default_value;
            return value;
        }

        public static string GetCsvLine(dynamic o, string default_value = "", string separator = ",", string separator_substitute = ";")
        {
            List<string> ss = new List<string>();
            foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
            {
                object p = pi.GetValue(o, null);
                if (pi.PropertyType == typeof(string))
                    ss.Add(GetCsvCell((string)p, default_value, separator, separator_substitute));
                else
                    ss.Add(p.ToString());
            }
            return string.Join(separator, ss);
        }

        public static Dictionary<string, object> GetDbObject(dynamic o, string default_value = "")
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
            {
                object p = pi.GetValue(o, null);
                if (pi.PropertyType == typeof(string))
                    p = GetDbCell((string)p, default_value);
                d[pi.Name] = p;
            }
            return d;
        }
    }
}
