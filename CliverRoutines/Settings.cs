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
    public class Settings
    {
        static public T Load<T>(string file, bool ignore_file_content = false) where T : Settings, new()
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

        string __file;

        public void Save()
        {
            Cliver.SerializationRoutines.Json.Save(__file, this);
        }
    }
}
