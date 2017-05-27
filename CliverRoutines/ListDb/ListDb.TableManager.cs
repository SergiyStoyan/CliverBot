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
            NULL = 0,
            KEEP_ALL_OPEN_TABLES_FOREVER = 1,//requires explicite call Close()
            CLOSE_TABLE_ON_DISPOSE = 2,
            FLUSH_ON_CLOSE = 4,
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
            public Modes Mode = Modes.CLOSE_TABLE_ON_DISPOSE;

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
                    using (TextReader fr = new StreamReader(File))
                    {
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
                                List.RemoveAt(int.Parse(m.Groups[1].Value));
                                continue;
                            }
                            m = Regex.Match(l, @"replaced:\s+(\d+)");
                            if (m.Success)
                            {
                                read_next(fr);
                                int p1 = int.Parse(m.Groups[1].Value);
                                List.RemoveAt(p1);
                                D d = List[List.Count-1];
                                List.RemoveAt(List.Count - 1);
                                List.Insert(p1, d);
                                continue;
                            }
                            m = Regex.Match(l, @"added:\s+(\d+)");
                            if (m.Success)
                            {
                                read_next(fr);
                                int p1 = int.Parse(m.Groups[1].Value);
                                if (p1 != List.Count - 1)
                                    throw new Exception("Log file broken.");
                                continue;
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
                                }
                            }
                        }
                    }
                }

                file_writer = new StreamWriter(File, true);
                ((StreamWriter)file_writer).AutoFlush = true;
                log_writer = new StreamWriter(Log, true);
                ((StreamWriter)log_writer).AutoFlush = true;
            }
            void read_next(TextReader fr)
            {
                string r = fr.ReadLine();
                List.Add(JsonConvert.DeserializeObject<D>(r));
            }

            public SaveResults Save(D document)
            {
                int i = List.IndexOf(document);
                if (i >= 0)
                {
                    log_writer.WriteLine("replacing: " + i);
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("replaced: " +  i);
                    return SaveResults.UPDATED;
                }
                else
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("added: " + List.Count);
                    List.Add(document);
                    return SaveResults.ADDED;
                }
            }

            public void Delete(D document)
            {
                int i = List.IndexOf(document);
                if (i >= 0)
                {
                    List.RemoveAt(i);
                    log_writer.WriteLine("deleted: " +  i);
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