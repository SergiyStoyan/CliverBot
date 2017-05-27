/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Cliver
{
    public partial class ListDb
    {
        public class Document
        {

        }

        public abstract class Table<D> : IDisposable where D : Document, new()
        {
            static readonly Dictionary<string, TableManager<Table<D>, D>> table_directories2table_manager = new Dictionary<string, TableManager<Table<D>, D>>();

            public static Modes Mode(string directory)
            {
                lock (table_directories2table_manager)
                {
                    TableManager<Table<D>, D> tm;
                    if (!table_directories2table_manager.TryGetValue(directory, out tm))
                        return Modes.NULL;
                    return tm.Mode;
                }
            }

            public static bool Mode(string directory, Modes mode)
            {
                lock (table_directories2table_manager)
                {
                    TableManager<Table<D>, D> tm;
                    if (!table_directories2table_manager.TryGetValue(directory, out tm))
                        return false;
                    tm.Mode = mode;
                    return true;
                }
            }

            public static void Close(string directory)
            {
                lock (table_directories2table_manager)
                {
                    table_directories2table_manager.Clear();
                }
            }

            static public void Flush(string directory)
            {
                lock (table_directories2table_manager)
                {
                    TableManager<Table<D>, D> tm;
                    if (!table_directories2table_manager.TryGetValue(directory, out tm))
                        return;
                    tm.Flush();
                }
            }

            static public void Drop(string directory)
            {
                lock (table_directories2table_manager)
                {
                    TableManager<Table<D>, D> tm;
                    if (!table_directories2table_manager.TryGetValue(directory, out tm))
                        tm = new TableManager<Table<D>, D>(directory);
                    else
                        table_directories2table_manager.Remove(directory);
                    tm.Drop();
                }
            }

            public Table(string directory = null)
            {
                if (directory == null)
                    directory = Log.GetAppCommonDataDir();
                Directory = PathRoutines.GetNormalizedPath(directory);

                Name = GetType().Name;
                lock (table_directories2table_manager)
                {
                    get_table_info().Count++;
                }
            }
            public readonly string Name;
            public readonly string Directory;

            protected TableManager<Table<D>, D> get_table_info()
            {
                TableManager<Table<D>, D> tm;
                if (!table_directories2table_manager.TryGetValue(Directory, out tm))
                {
                    tm = new TableManager<Table<D>, D>(Directory);
                    table_directories2table_manager[Directory] = tm;
                }
                return tm;
            }

            ~Table()
            {
                Dispose();
            }

            virtual public void Dispose()
            {
                if (Directory == null)//disposed
                    return;
                lock (table_directories2table_manager)
                {
                    TableManager<Table<D>, D> tm;
                    if (!table_directories2table_manager.TryGetValue(Directory, out tm))
                        return;
                    tm.Count--;
                    switch (tm.Mode)
                    {
                        case Modes.CLOSE_TABLE_ON_DISPOSE:
                            if (tm.Count < 1)
                                table_directories2table_manager.Remove(Directory);
                            break;
                        case Modes.KEEP_ALL_OPEN_TABLES_FOREVER:
                            break;
                        default:
                            throw new Exception("No option: " + tm.Mode);
                    }
                }
            }

            protected List<D> list
            {
                get
                {
                    return (List<D>)get_table_info().List;
                }
            }

            public List<D> GetAll()
            {
                lock (list)
                {
                    return list.ToList();
                }
            }

            /// <summary>
            /// Not safe!!!
            /// </summary>
            /// <returns></returns>
            public List<D> Get()
            {
                lock (list)
                {
                    return list;
                }
            }

            public List<D> Get(Func<D, bool> query)
            {
                lock (list)
                {
                    return list.Where(query).ToList();
                }
            }

            public bool Exists(D document)
            {
                lock (list)
                {
                    return list.IndexOf(document) >= 0;
                }
            }

            public SaveResults Save(D document)
            {
                lock (table_directories2table_manager)
                {
                    return get_table_info().Save(document);
                }
            }

            public void Delete(D document)
            {
                lock (table_directories2table_manager)
                {
                    get_table_info().Delete(document);
                }
            }

            public void Flush()
            {
                Flush(Directory);
            }

            //public void Drop()
            //{
            //    Drop(Directory);
            //}
        }
    }
}