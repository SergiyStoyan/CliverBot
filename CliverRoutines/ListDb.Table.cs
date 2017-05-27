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
        public class testDocument : ListDb.Document
        {
            public string A = DateTime.Now.ToString() + "\r\n" + DateTime.Now.Ticks.ToString();
            public string B = "test";
            public long C = DateTime.Now.Ticks;
            public DateTime D = DateTime.Now;

            static public void Test()
            {
                ListDb.Table<testDocument> t = ListDb.Table<testDocument>.Get();
                //t.Drop();
                t.Save(new testDocument());
                t.Save(new testDocument());
                t.Save(new testDocument());
                testDocument d = t.Last();
                d.A = @"changed";
                t.Save(d);
                t.Remove(t.First());
                t.Flush();
            }
        }

        public class Document
        {

        }

        public class Table<D> : List<D>, IDisposable where D : Document, new()
        {
            public static Table<D> Get(string directory = null)
            {
                directory = get_normalized_directory(directory);

                WeakReference wr;
                string key = directory + "\\" + typeof(D).Name;
                if (!table_keys2table.TryGetValue(key, out wr)
                    || !wr.IsAlive
                    )
                    {
                        Table<D> t = new Table<D>(directory);
                        wr = new WeakReference(t);
                        table_keys2table[key] = wr;
                    }
                return (Table<D>)wr.Target;
            }
            static Dictionary<string, WeakReference> table_keys2table = new Dictionary<string, WeakReference>();

            static string get_normalized_directory(string directory = null)
            {
                if (directory == null)
                    directory = Cliver.Log.GetAppCommonDataDir();
                return PathRoutines.GetNormalizedPath(directory);
            }

            public readonly string Log = null;
            TextWriter log_writer;
            public readonly string File = null;
            TextWriter file_writer;
            readonly string new_file;
            //public Type DocumentType;
            public Modes Mode = Modes.FLUSH_TABLE_ON_CLOSE;
            public readonly string Name;

            public enum Modes
            {
                NULL = 0,
                KEEP_OPEN_TABLE_FOREVER = 1,//requires explicite call Close()
                FLUSH_TABLE_ON_CLOSE = 2,
            }

            Table(string directory = null)
            {
                directory = get_normalized_directory(directory);

                Name = typeof(D).Name + "s";

                File = directory + "\\" + Name + ".listdb";
                new_file = File + ".new";
                Log = directory + "\\" + Name + ".listdb.log";

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
                        if (System.IO.File.Exists(Log))
                        {
                            foreach (string l in System.IO.File.ReadAllLines(Log))
                            {
                                Match m = Regex.Match(l, @"flushed:\s+(\d+)");
                                if (m.Success)
                                {
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    for (int i = 0; i < p1; i++)
                                    {
                                        D d = JsonConvert.DeserializeObject<D>(fr.ReadLine());
                                        base.Add(d);
                                    }
                                    continue;
                                }
                                m = Regex.Match(l, @"deleted:\s+(\d+)");
                                if (m.Success)
                                {
                                    base.RemoveAt(int.Parse(m.Groups[1].Value));
                                    continue;
                                }
                                m = Regex.Match(l, @"replaced:\s+(\d+)");
                                if (m.Success)
                                {
                                    D d = JsonConvert.DeserializeObject<D>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 >= base.Count)
                                        throw new Exception("Log file broken.");
                                    base.RemoveAt(p1);
                                    base.Insert(p1, d);
                                    continue;
                                }
                                m = Regex.Match(l, @"added:\s+(\d+)");
                                if (m.Success)
                                {
                                    D d = JsonConvert.DeserializeObject<D>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 != base.Count)
                                        throw new Exception("Log file broken.");
                                    base.Add(d);
                                    continue;
                                }
                                m = Regex.Match(l, @"inserted:\s+(\d+)");
                                if (m.Success)
                                {
                                    D d = JsonConvert.DeserializeObject<D>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 >= base.Count)
                                        throw new Exception("Log file broken.");
                                    base.Insert(p1, d);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            for (string s = fr.ReadLine(); s != null; s = fr.ReadLine())
                                base.Add(JsonConvert.DeserializeObject<D>(s));
                        }
                        if (fr.ReadLine() != null)
                            throw new Exception("Log file broken.");
                    }
                }

                file_writer = new StreamWriter(File, true);
                ((StreamWriter)file_writer).AutoFlush = true;
                log_writer = new StreamWriter(Log, true);
                ((StreamWriter)log_writer).AutoFlush = true;
            }

            ~Table()
            {
                Dispose();
            }

            public void Dispose()
            {
                try
                {
                    if ((Mode & Modes.FLUSH_TABLE_ON_CLOSE) == Modes.FLUSH_TABLE_ON_CLOSE)
                        Flush();
                    if (file_writer != null)
                        file_writer.Dispose();
                    if (log_writer != null)
                        log_writer.Dispose();
                }
                catch
                {
                    //when Dispose is called from finalizer, files may be closed and so an exception be thrown
                }
            }

            public void Flush()
            {
                log_writer.WriteLine("flushing");

                using (TextWriter new_file_writer = new StreamWriter(new_file, false))
                {
                    foreach (D d in this)
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
                log_writer.WriteLine("flushed: " + base.Count);
            }

            public void Drop()
            {
                base.Clear();

                if (file_writer != null)
                    file_writer.Dispose();
                if (System.IO.File.Exists(File))
                    System.IO.File.Delete(File);

                if (log_writer != null)
                    log_writer.Dispose();
                if (System.IO.File.Exists(Log))
                    System.IO.File.Delete(Log);
            }

            public void Clear()
            {
                base.Clear();

                if (file_writer != null)
                    file_writer.Dispose();
                file_writer = new StreamWriter(File, false);

                if (log_writer != null)
                    log_writer.Dispose();
                log_writer = new StreamWriter(Log, false);
            }

            /// <summary>
            /// Use it rather than Add/Insert to make Table work as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            public SaveResults Save(D document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("replaced: " + i);
                    return SaveResults.UPDATED;
                }
                else
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.Add(document);
                    log_writer.WriteLine("added: " + (base.Count - 1));
                    return SaveResults.ADDED;
                }
            }
            public enum SaveResults
            {
                ADDED,
                UPDATED,
            }

            public void Add(D document)
            {
                file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                base.Add(document);
                log_writer.WriteLine("added: " + (base.Count - 1));
            }

            public void AddRange(IEnumerable<D> documents)
            {
                throw new Exception("TBD");
                base.AddRange(documents);
            }

            public void Insert(int index, D document)
            {
                file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                base.Insert(index, document);
                log_writer.WriteLine("inserted: " + index);
            }

            public void InsertRange(int index, IEnumerable<D> documents)
            {
                throw new Exception("TBD");
                base.InsertRange(index, documents);
            }

            public bool Remove(D document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    base.RemoveAt(i);
                    log_writer.WriteLine("deleted: " + i);
                    return true;
                }
                return false;
            }

            public int RemoveAll(Predicate<D> match)
            {
                throw new Exception("TBD");
                return base.RemoveAll(match);
            }

            public void RemoveAt(int index)
            {
                base.RemoveAt(index);
                log_writer.WriteLine("deleted: " + index);
            }

            public void RemoveRange(int index, int count)
            {
                for (int i = index; i < count; i++)
                {
                    base.RemoveAt(i);
                    log_writer.WriteLine("deleted: " + i);
                }
            }

            public D GetPrevious(D document)
            {
                if (document == null)
                    return null;
                int i = IndexOf(document);
                if (i < 1)
                    return null;
                return this[i - 1];
            }

            public D GetNext(D document)
            {
                if (document == null)
                    return null;
                int i = this.IndexOf(document);
                if (i + 1 >= this.Count)
                    return null;
                return this[i + 1];
            }
        }

        public static string GetNormalized(string s)
        {
            if (s == null)
                return null;
            return Regex.Replace(s.ToLower(), @" +", " ").Trim();
        }
    }
}