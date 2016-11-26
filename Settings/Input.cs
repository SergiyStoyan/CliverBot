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
        [Cliver.Settings.Obligatory]
        public static readonly InputClass Input;

        public class InputClass : Cliver.Settings
        {
            public string File = "input.csv";
            public FileFormatEnum FileFormat = FileFormatEnum.NULL;

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
                if (FileFormat == FileFormatEnum.NULL)
                {
                    switch (PathRoutines.GetFileExtensionFromPath(File).ToLower())
                    {
                        case "csv":
                            FileFormat = FileFormatEnum.CSV;
                            break;
                        case "txt":
                        case "tsv":
                        case "tab":
                            FileFormat = FileFormatEnum.TSV;
                            break;
                        default:
                            throw new Exception("Unknown option: " + PathRoutines.GetFileExtensionFromPath(File).ToLower());
                    }
                }
            }
        }
    }
}