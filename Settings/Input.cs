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
using Cliver;

namespace Cliver.Bot
{
    public partial class Settings
    {
        public static readonly InputSettings Input;

        //[Cliver.SettingsAttributes.Config(Optional = true)]
        public class InputSettings : Cliver.UserSettings
        {
            public string File = "input.tsv";
            //public string File = "input.txt";
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

            override protected void Loaded()
            {
                if (!File.Contains(":"))
                    File = Cliver.Log.AppCompanyUserDataDir + "\\" + File;
                if (!System.IO.File.Exists(File))
                {
                    string file0 = Cliver.Log.AppDir + "\\" + PathRoutines.GetFileName(File); 
                    if (!System.IO.File.Exists(file0))
                        throw new Exception("Cannot find the original Input file: " + file0);
                    string d = PathRoutines.GetFileDir(File);
                    if (!Directory.Exists(d))
                        throw new Exception("Input file directory '" + d + "' does not exist. Modify its path in the settings file.");
                    System.IO.File.Copy(file0, File);
                    Save();
                }

                if (FileFormat == FileFormatEnum.NULL)
                {
                    switch (PathRoutines.GetFileExtension(File).ToLower())
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
                            throw new Exception("Unknown option: " + PathRoutines.GetFileExtension(File).ToLower());
                    }
                }
            }
        }
    }
}