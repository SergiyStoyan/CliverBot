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
        public static readonly OutputClass Output;

        public class OutputClass : Cliver.Settings
        {
            public string FileName = null;
            public FileFormatEnum FileFormat = FileFormatEnum.NULL;
            public int FileChunkSizeInBytes = 1000000;
            public bool Append = false;
            public bool Write2CommonFolder = false;
            [ScriptIgnore]
            public Cliver.FieldPreparation.FieldSeparator FieldSeparator;

            override public void Loaded()
            {
                if (FileName == null)
                {
                    if (FileFormat == FileFormatEnum.NULL)
                    {
                        FileName = Cliver.Log.EntryAssemblyName;
                        FileFormat = FileFormatEnum.TSV;
                    }
                    string fiel_name = Cliver.Log.EntryAssemblyName;
                    switch (FileFormat)
                    {
                        case FileFormatEnum.CSV:
                            FileName = fiel_name + ".csv";
                            break;
                        case FileFormatEnum.TSV:
                            FileName = fiel_name + ".tsv";
                            break;
                        case FileFormatEnum.XLS:
                            throw new Exception("XLS format not implemented.");
                        default:
                            throw new Exception("Unknown option: " + FileFormat);
                    }
                }
                else
                {
                    if (FileFormat == FileFormatEnum.NULL)
                    {
                        switch (PathRoutines.GetFileExtensionFromPath(FileName).ToLower())
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
                                throw new Exception("Unknown option: " + PathRoutines.GetFileExtensionFromPath(FileName).ToLower());
                        }
                    }
                }
                switch (FileFormat)
                {
                    case FileFormatEnum.CSV:
                        FieldSeparator = Cliver.FieldPreparation.FieldSeparator.COMMA;
                        break;
                    case FileFormatEnum.TSV:
                        FieldSeparator = Cliver.FieldPreparation.FieldSeparator.TAB;
                        break;
                    case FileFormatEnum.XLS:
                        throw new Exception("XLS format not implemented.");
                    case FileFormatEnum.NULL:
                        throw new Exception("File format not defined.");
                    default:
                        throw new Exception("Unknown option: " + FileFormat);
                }
                
            }
        }
    }
}