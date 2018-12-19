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
    /// Writne file routines. Thread-safe.
    /// </summary>
    public partial class FileWriter
    {
        static object static_lock_variable = new object();

        static FileWriter _this = null;
        public static FileWriter This
        {
            get
            {
                lock (static_lock_variable)
                {
                    if (_this == null)
                    {
                        _this = new FileWriter(
                            (Settings.Output.Write2CommonFolder ? Log.WorkDir + @"\":"") + Settings.Output.FileName,
                            Settings.Output.Append,
                            !Settings.Output.Write2CommonFolder,
                            Settings.Output.FileChunkSizeInBytes
                            );
                    }

                    return _this;
                }
            }
        }

        /// <summary>
        /// Absolute path of the file.
        /// </summary>
        public string FilePath
        {
            get
            {
                lock (this)
                {
                    return file_abs_path;
                }
            }
        }
        
        /// <summary>
        /// Creates and opens file for writting
        /// </summary>
        /// <param name="file">[full] file name</param>
        /// <param name="append_output">if false then counter is added to the file name will be created to avoid rewritting the old file</param>
        /// <param name="add_time_mark">if true, the time mark is added to the file name</param>
        /// <param name="max_file_size">if > 0 then new file is created when size is exceeded</param>
        public FileWriter(string file, bool append_output, bool add_time_mark, int max_file_size)
        {
            lock (this)
            {
                this.file = file;
                this.append_output = append_output;
                this.add_time_mark = add_time_mark;
                this.max_file_size = max_file_size;
                open_file();
            }
        }  

        const string OUTPUT_DIR_NAME = "output";

        static string get_file_abs_path(string file, bool add_time_mark, bool append_output)
        {
            string file_abs_path = file;
            if (!file_abs_path.Contains(":"))
                file_abs_path = FileSystemRoutines.CreateDirectory((Session.This != null ? Session.This.Dir : Log.WorkDir) + "\\" + OUTPUT_DIR_NAME) + "\\" + file_abs_path;

            if (add_time_mark)
            {
                string perfix = "_" + DateTime.Now.ToString("yyMMddHHmmss");
                int point = file_abs_path.LastIndexOf(".");
                if (point < 0)
                    file_abs_path = file_abs_path + perfix;
                else
                    file_abs_path = file_abs_path.Insert(point, perfix);
            }

            if (!append_output)
            {
                string fap2 = file_abs_path;
                int counter = 0;
                while (File.Exists(fap2))
                {
                    fap2 = Regex.Replace(fap2, "(" + Regex.Escape(Path.GetFileNameWithoutExtension(file_abs_path)) + @"_)(\d+)",
                        (Match m) =>
                        {
                            counter = int.Parse(m.Groups[2].Value) + 1;
                            return m.Groups[1].Value + counter;
                        }
                        );
                    if (counter < 1)
                        fap2 = Regex.Replace(fap2, @"\.[^\.]+$", @"_1$0");
                }
                file_abs_path = fap2;
            }
            return file_abs_path;
        }

        void open_file()
        {
            lock (this)
            {
                try
                {
                    file_abs_path = get_file_abs_path(file, add_time_mark, append_output);
                    tw = new StreamWriter(file_abs_path, append_output);//, Encoding.Unicode);//System.Text.Encoding.GetEncoding("ISO-8859-1")
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }

        string file = null;
        bool append_output = false;
        bool add_time_mark = true;
        int max_file_size = 0;
        TextWriter tw = null;
        string file_abs_path = null;

        /// <summary>
        /// Used to close file in order to start a new session
        /// </summary>
        internal static void ClearSession()
        {
            lock (static_lock_variable)
            {
                if (_this != null)
                {
                    _this.Close();
                    _this = null;
                }
            }
        }

        /// <summary>
        /// Used to close file in order to start a new session
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (tw != null)
                {
                    tw.Close();
                    tw = null;
                }
            }
        }

        public void Write(string str)
        {
            lock (this)
            {
                try
                {
                    //if (str == null)
                    //    return;
                    if (max_file_size > 0)
                    {
                        FileInfo g = new FileInfo(file_abs_path);
                        if (g.Length >= max_file_size)
                        {
                            tw.Close();
                            open_file(); 
                        }                        
                    }

                    tw.Write(str);
                    tw.Flush();
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }

        /// <summary>
        /// Strip string from new lines and write it to file
        /// </summary>
        /// <param name="str">string</param>
        public void WriteLine(string str)
        {
            lock (this)
            {
                if (str == null)
                    return;
                str = Regex.Replace(str, "[\r\n]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                Write(str + "\r\n");
            }
        }

        /// <summary>
        /// Write parameters delimited with OutputFieldSeparator to file line.
        /// OutputFieldSeparator is removed from within values
        /// </summary>
        /// <param name="values">values to be in line</param>
        public void WriteLine(params string[] values)
        {
            lock (this)
            {                
                string line = FieldPreparation.GetCsvLine(values, Settings.Output.FieldSeparator, false);
                WriteLine(line);
            }
        }        
        
        /// <summary>
        /// Prepare parameters and write them delimited with OutputFieldSeparator to the file.
        /// </summary>
        /// <param name="strs">values to be printed in the line</param>
        public void PrepareAndWriteLine(params string[] strs)
        {
            lock (this)
            {
                string line = PrepareFields(strs);
                WriteLine(line);
            }
        }

        public void PrepareAndWriteHtmlLine(params string[] strs)
        {
            lock (this)
            {
                string line = Html.PrepareFields(strs);
                WriteLine(line);
            }
        }
        
        /// <summary>
        /// Writes parameters as cells in html table.
        /// Html entities are encoded within values.
        /// </summary>
        /// <param name="encode_html_entities">define if encode html entities automatically</param>
        /// <param name="strs">values to be in html row</param>
        public void WriteHtmlTableRow(bool encode_html_entities, params string[] strs)
        {
            lock (this)
            {
                Write("<tr>");
                foreach (string str in strs)
                {
                    if(encode_html_entities)
                        Write("<td>" + HttpUtility.HtmlEncode(str) + "</td>");
                    else
                        Write("<td>" + str + "</td>");
                }
                Write("</tr>\r\n");
            }
        }

        /// <summary>
        /// Should invoked before using WriteHtmlTableRow
        /// </summary>
        /// <param name="set_border">define if border is</param>
        public void OpenHtmlTable(bool set_border)
        {
            lock (this)
            {
                if (set_border)
                    Write("<table border='1'>\r\n");
                else
                    Write("<table>\r\n");
            }
        }

        /// <summary>
        /// Should be invoked after using WriteHtmlTableRow
        /// </summary>
        public void CloseHtmlTable()
        {
            lock (this)
            {
                Write("</table>\r\n");
            }
        }

        /// <summary>
        /// Write header to the output file
        /// </summary>
        /// <param name="values"></param>
        public void WriteHeader(params string[] values)
        {
            lock (this)
            {
                header = values;
                WriteLine(header);
            }
        }
        string[] header = null;

        /// <summary>
        /// Should be invoked after WriteHeader. 
        /// </summary>
        /// <param name="name2value_pairs">each odd parameter considered to be column name, each next parameter - column value</param>
        public void PrepareAndWriteLineWithHeader(params string[] name2value_pairs)
        {
            lock (this)
            {
                Dictionary<string, string> hline = new Dictionary<string, string>();
                for (int i = 0; i < name2value_pairs.Length; i += 2)
                    hline[name2value_pairs[i]] = name2value_pairs[i + 1];
                string[] line = new string[header.Length];
                {
                    int i = 0;
                    foreach (string head in header)
                        line[i++] = hline[head];
                }
                PrepareAndWriteLine(line);
            }
        }

        /// <summary>
        /// Should be invoked after WriteHeader. 
        /// </summary>
        /// <param name="name2value_pairs">each odd parameter considered to be column name, each next parameter - column value</param>
        public void PrepareAndWriteHtmlLineWithHeader(params string[] name2value_pairs)
        {
            lock (this)
            {
                Dictionary<string, string> hline = new Dictionary<string, string>();
                for (int i = 0; i < name2value_pairs.Length; i += 2)
                    hline[name2value_pairs[i]] = name2value_pairs[i + 1];
                string[] line = new string[header.Length];
                {
                    int i = 0;
                    foreach (string head in header)
                        line[i++] = hline[head];
                }
                PrepareAndWriteHtmlLine(line);
            }
        }
    }
}
