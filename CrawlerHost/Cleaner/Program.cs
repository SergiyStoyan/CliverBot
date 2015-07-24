using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.CrawlerHostCleaner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                LogMessage.DisableStumblingDialogs = ProgramRoutines.IsParameterSet(CommandLineParameters.AUTOMATIC);
                Log.LOGGING_MODE = Log.LoggingMode.ONLY_LOG;
                LogMessage.Output2Console = true;
                ProcessRoutines.RunSingleProcessOnly();
                CrawlerHost.ServiceApi.Initialize();
                Cleaner.Do();
                CrawlerHost.ServiceApi.Complete(true);
            }
            catch (Exception e)
            {
                CrawlerHost.DbApi.Message(e);
                CrawlerHost.ServiceApi.Complete(false);
            }
        }
    }
}