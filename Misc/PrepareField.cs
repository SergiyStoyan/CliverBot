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
            public static string GetCsvField(string value, string default_value = "", string separator = ",", string separator_substitute = ";")
            {
                if (value == null)
                    return default_value;

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = remove_notprintables_regex.Replace(value, " ");
                value = Regex.Replace(value, separator, separator_substitute, RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return value;
            }

            public static string GetDbField(string value, string default_value = "")
            {
                if (value == null)
                    return default_value;

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = remove_notprintables_regex.Replace(value, " ");
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
                        ss.Add(GetCsvField((string)p, default_value, separator, separator_substitute));
                    else
                        ss.Add(p.ToString());
                }
                return string.Join(separator, ss);
            }

            public static string GetCsvLine(object[] values, string default_value = "", string separator = ",", string separator_substitute = ";")
            {
                List<string> ss = new List<string>();
                foreach (object v in values)
                {
                    if (v is string)
                        ss.Add(GetCsvField((string)v, default_value, separator, separator_substitute));
                    else
                        ss.Add(v.ToString());
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
                        p = GetDbField((string)p, default_value);
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
                value = remove_notprintables_regex.Replace(value, " ");
                value = Regex.Replace(value, @"[ ]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return (HttpUtility.HtmlDecode(value)).Trim();
            }
        }

        public static string GetCsvField(string value, string default_value = "", string separator = ",", string separator_substitute = ";")
        {
            if (value == null)
                return default_value;

            value = remove_notprintables_regex.Replace(value, " ");
            value = Regex.Replace(value, separator, separator_substitute, RegexOptions.Compiled | RegexOptions.Singleline);
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = value.Trim();
            if (value == "")
                return default_value;
            return value;
        }

        public static string GetDbField(string value, string default_value = "")
        {
            if (value == null)
                return default_value;

            value = remove_notprintables_regex.Replace(value, " ");
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
                    ss.Add(GetCsvField((string)p, default_value, separator, separator_substitute));
                else
                    ss.Add(p.ToString());
            }
            return string.Join(separator, ss);
        }

        public static string GetCsvLine(object[] values, string default_value = "", string separator = ",", string separator_substitute = ";")
        {
            List<string> ss = new List<string>();
            foreach (object v in values)
            {
                if (v is string)
                    ss.Add(GetCsvField((string)v, default_value, separator, separator_substitute));
                else
                    ss.Add(v.ToString());
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
                    p = GetDbField((string)p, default_value);
                d[pi.Name] = p;
            }
            return d;
        }

        //static Regex remove_notprintables_regex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled | RegexOptions.Singleline);
        static Regex remove_notprintables_regex = new Regex(@"[^\u0000-\u00b0]", RegexOptions.Compiled | RegexOptions.Singleline);
        //static Regex remove_notprintables_regex = new Regex(@"[^\x20-\x7E]", RegexOptions.Compiled | RegexOptions.Singleline);

        public static string Trim(string s, int length, string ending = "...")
        {
            if (s.Length <= length)
                return s;
            return s.Substring(0, length) + ending;
        }
    }
}
