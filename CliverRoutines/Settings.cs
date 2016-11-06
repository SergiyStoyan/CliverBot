//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Cliver
{
    public class SettingsBase
    {
        static public T Load<T>(string file) where T : SettingsBase, new()
        {
            return get<T>(file, false);
        }

        static public T Create<T>(string file) where T : SettingsBase, new()
        {
            return get<T>(file, true);
        }

        static T get<T>(string file, bool ignore_file_content) where T : SettingsBase, new()
        {
            if (!file.Contains(":"))
                file = Log.GetAppCommonDataDir() + "\\" + file;
            T s;
            if (!ignore_file_content && File.Exists(file))
                s = Cliver.SerializationRoutines.Json.Load<T>(file);
            else
                s = new T();
            s.__file = file;
            return s;
        }

        //Settings(string file)
        //{
        //    __File = file;
        //}

        public string __File
        {
            get { return __file; }
        }
        string __file;

        public void Save()
        {
            Cliver.SerializationRoutines.Json.Save(__file, this);
        }
    }
}
