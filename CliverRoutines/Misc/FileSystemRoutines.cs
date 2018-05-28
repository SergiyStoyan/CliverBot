using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cliver
{
   public class FileSystemRoutines
    {
       static public List<string> GetFiles(string directory, bool include_subfolders = true)
        {
            List<string> fs = Directory.EnumerateFiles(directory).ToList();
            if (include_subfolders)
                foreach (string d in Directory.EnumerateDirectories(directory))
                    fs.AddRange(GetFiles(d));
            return fs;
        }

        public static string CreateDirectory(string path, bool unique = false)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (unique)
            {
                int i = 1;
                string p = path + "_" + i;
                for (; Directory.Exists(p); p = p + "_" + (++i)) ;
                path = p;
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
