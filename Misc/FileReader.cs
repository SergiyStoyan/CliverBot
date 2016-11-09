//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Cliver.Bot
{
    /// <summary>
    /// Read delimitered file routines. Thread-safe.
    /// </summary>
    public partial class FileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_path"></param>
        /// <param name="delimiter"></param>
        /// <param name="headers">if null, then the first line is considered to be a header</param>
        /// <param name="comment_marks">lines that begins with a comment mark is omitted</param>
        public FileReader(string file_path, string delimiter, string[] headers = null, bool ignore_space_lines = true, string[] comment_marks = null, bool no_headers = false)
        {
            if (comment_marks == null)
                comment_marks = new string[] { "#" };
            comment_or_empty_string_regex = new Regex(@"^" + (ignore_space_lines ? @"\s*" : "") + "(" + string.Join("|", (from x in comment_marks select Regex.Escape(x)).ToArray()) + ")" + (ignore_space_lines ? @"?" : "") + "$");
            split_regex = new Regex(Regex.Escape(delimiter));
            Stream s = File.Open(file_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            TR = new StreamReader(s);

            if (!no_headers)
            {
                if (headers == null)
                {
                    string row = null;
                    do
                    {
                        row = TR.ReadLine();
                        if (row == null)
                            throw new Exception("No header found in " + file_path);
                    } while (comment_or_empty_string_regex.IsMatch(row));
                    headers = split_regex.Split(row.Trim());
                }

                if ((from x in headers where string.IsNullOrWhiteSpace(x) select x).FirstOrDefault() != null)
                    throw new Exception("One of header names is empty in the file: " + file_path);
                Headers = headers;
            }
            else
                Headers = null;
        }

        TextReader TR = null;
        public readonly string[] Headers;
        Regex comment_or_empty_string_regex;
        Regex split_regex;

        public void Close()
        {
            lock (this)
            {
                if (TR != null)
                {
                    TR.Close();
                }
                TR = null;
            }
        }

        public Row ReadLine()
        {
            lock (this)
            {
                string row = null;
                do
                {
                    row = TR.ReadLine();
                    if (row == null) 
                        return null;
                } while (comment_or_empty_string_regex.IsMatch(row));
                string[] vs = split_regex.Split(row);
                return new Row(Headers, vs);
            }
        }

        public class Row
        {
            OrderedDictionary row = null;

            //public readonly string[] Headers;
            public IEnumerable<string> Headers
            {
                get
                {
                    return row.Keys.Cast<string>();
                }
            }

            public string this[int index]
            {
                get
                {
                    return (string)row[index];
                }
            }

            public string this[string key]
            {
                get
                {
                    return (string)row[key];
                }
            }

            public int Count
            {
                get
                {
                    return row.Count;
                }
            }

            internal Row(string[] headers, string[] values)
            {
                row = new OrderedDictionary();
                if (headers != null)
                {
                    if (values.Length != headers.Length)
                        throw new Exception("Number of values is not equal to number of headers:\n" + string.Join(" | ", headers) + "\n" + string.Join(" | ", values));
                    for (int i = 0; i < headers.Length; i++)
                        row[headers[i]] = values[i];
                }
                else
                {
                    for (int i = 0; i < values.Length; i++)
                        row.Add(i, values[i]);
                }
            }
        }

        public IEnumerable<Row> ReadAll()
        {
            lock (this)
            {
                for (FileReader.Row r = ReadLine(); r != null; r = ReadLine())
                    yield return r;
            }
        }
    }
}
