using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Cliver.Bot
{
    public partial class Settings
    {
        public static readonly InputClass Input;

        public class InputClass : Cliver.Settings
        {
            public string File = "input.csv";
            public FileFormats FileFormat = FileFormats.NULL;

            override public void Loaded()
            {
                if (!File.Contains(":"))
                {
                    string file2 = Cliver.Log.GetAppCommonDataDir() + "\\" + File;
                    if (!System.IO.File.Exists(file2))
                        if (System.IO.File.Exists(File))
                            System.IO.File.Copy(File, file2);
                    File = file2;
                    Save();
                }
                if (FileFormat == FileFormats.NULL)
                {
                    switch (PathRoutines.GetFileExtensionFromPath(File).ToLower())
                    {
                        case "csv":
                            FileFormat = FileFormats.CSV;
                            break;
                        case "txt":
                        case "tsv":
                        case "tab":
                            FileFormat = FileFormats.TSV;
                            break;
                    }
                }
            }
        }
    }
}