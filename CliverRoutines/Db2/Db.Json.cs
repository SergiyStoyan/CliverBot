using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Cliver
{
    public partial class Db2
    {
        public class Json
        {
            public abstract class Table<D>: Table where D : Document, new()
            {
                public Table()
                {
                }

                protected List<D> table
                {
                    get
                    {
                        return (List<D>)get_table_info().Core;
                    }
                }
                //protected List<D> table
                //{
                //    get
                //    {
                //        List<D> v;
                //        if (!t.TryGetTarget(out v))
                //        {
                //            v = (List<D>)get_table_info().Core;
                //            t.SetTarget(v);
                //        }
                //        return v;
                //    }
                //}
                //WeakReference<List<D>> t = new WeakReference<List<D>>(null);

                protected override object create_table_core()
                {
                    string file = db_dir + "\\" + Name + ".json";
                    //if (!File.Exists(file))
                    //{
                    //    if (!Message.YesNo("The app needs data which should be downloaded over the internet. Make sure your computer is connected to the internet and then click Yes. Otherwise, the app will exit."))
                    //        Environment.Exit(0);
                    //    BeginRefresh(true);
                    //    if (!File.Exists(file))
                    //    {
                    //        Message.Error("Unfrotunately the required data has not been downloaded. Please try later.");
                    //        return new List<D>();
                    //    }
                    //}

                    string s = System.IO.File.ReadAllText(file);
                    return SerializationRoutines.Json.Deserialize<List<D>>(s);
                }

                public List<D> GetAll()
                {
                    lock (table)
                    {
                        return table.ToList();
                    }
                }

                public List<D> Get(Func<D, bool> query)
                {
                    lock (table)
                    {
                        return table.Where(query).ToList();
                    }
                }

                //static public void RefreshFile()
                //{
                //    throw new Exception("Not implemented");
                //}

                //protected static void refresh_json_file_by_request(string url)
                //{
                //    Type t = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                //    Log.Main.Inform("Refreshing table: " + t.Name);
                //    HttpResponseMessage rm = http_client.GetAsync(url).Result;
                //    if (!rm.IsSuccessStatusCode)
                //        throw new Exception("Could not refresh table: " + rm.ReasonPhrase);
                //    if (rm.Content == null)
                //        throw new Exception("Response content is null.");
                //    string s = rm.Content.ReadAsStringAsync().Result;
                //    System.IO.File.WriteAllText(db_dir + "\\" + t.Name + ".json", s);
                //}

                //protected static void refresh_json_file_by_file(string file)
                //{
                //    Type t = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                //    Log.Main.Inform("Refreshing table: " + t.Name);
                //    string[] ls = File.ReadAllLines(file);
                //    string[] hs = ls[0].Split(',');
                //    Dictionary<string, int> hs2i = new Dictionary<string, int>();
                //    for (int i = 0; i < hs.Length; i++)
                //        hs2i[hs[i]] = i;
                //    PropertyInfo[] pis = typeof(D).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                //    List<D> ds = new List<D>();
                //    for (int i = 1; i < ls.Length; i++)
                //    {
                //        string[] vs = ls[i].Split(',');
                //        D d = new D();
                //        foreach (PropertyInfo pi in pis)
                //            pi.SetValue(d, vs[hs2i[pi.Name]]);
                //        ds.Add(d);
                //    }
                //    string s = SerializationRoutines.Json.Serialize(ds);
                //    File.WriteAllText(db_dir + "\\" + t.Name + ".json", s);
                //}
            }
        }
    }
}