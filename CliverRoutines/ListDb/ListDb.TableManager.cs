/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace Cliver
{
    public partial class ListDb
    {
        static ListDb()
        {
        }

        public enum Modes
        {
            NULL,
            KEEP_ALL_OPEN_TABLES_FOREVER,//requires explicite call Close()
            CLOSE_TABLE_ON_DISPOSE,
        }

        public enum SaveResults
        {
            ADDED,
            UPDATED,
        }

        public class TableManager<T, D> where T : Table<D> where D : Document, new()
        {
            public int Count = 0;
            public readonly List<D> List = new List<D>();
            public readonly string Log = null;
            TextWriter log_writer;
            public readonly string File = null;
            TextWriter file_writer;
            readonly string new_file;
            public readonly Type Type;
            //public Type DocumentType;
            public Modes Mode
            {
                set
                {
                    mode = value;
                    switch (mode)
                    {
                        case Modes.CLOSE_TABLE_ON_DISPOSE:
                            break;
                        case Modes.KEEP_ALL_OPEN_TABLES_FOREVER:
                            break;
                        default:
                            throw new Exception("No option: " + mode);
                    }
                }
                get
                {
                    return mode;
                }
            }
            Modes mode = Modes.CLOSE_TABLE_ON_DISPOSE;

            public TableManager(string directory)
            {
                Type = typeof(T);
                File = directory + "\\" + Type.Name + ".listdb";
                new_file = File + ".new";
                Log = directory + "\\" + Type.Name + ".listdb.log";

                if (System.IO.File.Exists(new_file))
                {
                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(File);
                    System.IO.File.Move(new_file, File);
                    if (System.IO.File.Exists(Log))
                        System.IO.File.Delete(Log);
                }

                if (System.IO.File.Exists(File))
                {
                    List<int> deleted = new List<int>();
                    string[] log_ls;
                    if (System.IO.File.Exists(Log))
                        log_ls = System.IO.File.ReadAllLines(Log);
                    else
                        log_ls = new string[0];
                    foreach (string l in log_ls)
                    {
                        Match m = Regex.Match(l, @"deleted:\s+(\d+)");
                        if (m.Success)
                        {
                            deleted.Add(int.Parse(m.Groups[1].Value));
                            continue;
                        }
                        //m = Regex.Match(l, @"added:\s+(\d+)");
                    }
                    base_index = deleted.Count;
                    using (TextReader fr = new StreamReader(File))
                    {
                        int i = 0;
                        for (string l = fr.ReadLine(); l != null; l = fr.ReadLine())
                        {
                            if (deleted.Contains(i++))
                                continue;
                            List.Add(JsonConvert.DeserializeObject<D>(l));
                        }
                    }
                    if (log_ls.Length > 0)
                    {
                        Match m = Regex.Match(log_ls[log_ls.Length - 1], @"replacing:\s+(\d+)\s+with\s+(\d+)");
                        if (m.Success)
                        {//replacing was broken so delete the new document if it was added
                            int i2 = int.Parse(m.Groups[2].Value);
                            if (i2 < List.Count)
                            {
                                log_writer = new StreamWriter(Log, true);
                                ((StreamWriter)log_writer).AutoFlush = true;
                                Delete(List[i2]);
                                log_writer.Dispose();
                                base_index--;
                            }
                        }
                    }
                }

                file_writer = new StreamWriter(File, true);
                ((StreamWriter)file_writer).AutoFlush = true;
                log_writer = new StreamWriter(Log, true);
                ((StreamWriter)log_writer).AutoFlush = true;
            }
            int base_index = 0;

            public SaveResults Save(D document)
            {
                SaveResults sr;
                int i = List.IndexOf(document);
                if (i >= 0)
                {
                    log_writer.WriteLine("replacing: " + (base_index + i) + " with " + (base_index + List.Count));
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("deleted: " + (base_index + i) + " added: " + (base_index + List.Count));
                    sr = SaveResults.UPDATED;
                }
                else
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("added: " + (base_index + List.Count));
                    sr = SaveResults.ADDED;
                }
                List.Add(document);
                return sr;
            }

            public void Delete(D document)
            {
                int i = List.IndexOf(document);
                if (i >= 0)
                {
                    List.RemoveAt(i);
                    log_writer.WriteLine("deleted: " + (base_index + i));
                }
            }

            public void Flush()
            {
                log_writer.WriteLine("flushing");

                using (TextWriter new_file_writer = new StreamWriter(new_file, false))
                {
                    foreach (D d in List)
                        new_file_writer.WriteLine(JsonConvert.SerializeObject(d, Formatting.None));
                    new_file_writer.Flush();
                }

                if (file_writer != null)
                    file_writer.Dispose();
                if (System.IO.File.Exists(File))
                    System.IO.File.Delete(File);
                System.IO.File.Move(new_file, File);
                file_writer = new StreamWriter(File, true);
                ((StreamWriter)file_writer).AutoFlush = true;

                if (log_writer != null)
                    log_writer.Dispose();
                log_writer = new StreamWriter(Log, false);
                ((StreamWriter)log_writer).AutoFlush = true;
                log_writer.WriteLine("flushed");

                base_index = 0;
            }

            public void Drop()
            {
                if (file_writer != null)
                {
                    file_writer.Dispose();
                    file_writer = null;
                }
                if (System.IO.File.Exists(File))
                    System.IO.File.Delete(File);

                if (log_writer != null)
                {
                    log_writer.Dispose();
                    log_writer = null;
                }
                if (System.IO.File.Exists(Log))
                    System.IO.File.Delete(Log);
            }
        }

        public static string GetNormalized(string s)
        {
            if (s == null)
                return null;
            return System.Text.RegularExpressions.Regex.Replace(s.ToLower(), @" +", " ").Trim();
        }
    }
}