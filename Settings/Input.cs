using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace Cliver.Bot
{
    public partial class Settings
    {
        //[Cliver.Settings.Obligatory]
        public static readonly InputClass Input;

        public class InputClass : Cliver.Settings
        {
            //public string File = "input.csv";
            public string File = "input.txt";
            public FileFormatEnum FileFormat = FileFormatEnum.NULL;

            //[ScriptIgnore]
            //[Newtonsoft.Json.JsonIgnore]
            //public string CompleteFile
            //{
            //    get
            //    {
            //        return File.Contains(":") ? File : Cliver.Log.AppDir + "\\" + File;
            //    }
            //}

            override public void Loaded()
            {
                if (!File.Contains(":"))
                    File = Cliver.Log.AppCommonDataDir + "\\" + File;
                if (!System.IO.File.Exists(File))
                {
                    string file0 = Cliver.Log.AppDir + "\\" + PathRoutines.GetFileNameFromPath(File);
                    if (!System.IO.File.Exists(file0))
                        throw new Exception("Cannot find the original Input file: " + file0);
                    System.IO.File.Copy(file0, File);
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