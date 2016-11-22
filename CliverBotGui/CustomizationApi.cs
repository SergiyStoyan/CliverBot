////********************************************************************************************
////Author: Sergey Stoyan, CliverSoft.com
////        http://cliversoft.com
////        stoyan@cliversoft.com
////        sergey.stoyan@gmail.com
////        27 February 2007
////Copyright: (C) 2007, Sergey Stoyan
////********************************************************************************************
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;
//using System.IO;
//using System.Diagnostics;
//using System.Linq;
//using System.Windows.Forms;
//using Cliver.Bot;

//namespace Cliver.BotGui
//{
//    static public class CustomizationGuiApi
//    {
//        public static readonly string CUSTOM_NAMESPACE;
//        public const string CUSTOM_BOT_CLASS_NAME = "CustomBot";
//        public const string CUSTOM_BOT_GUI_CLASS_NAME = "CustomBotGui";
//        public const string CUSTOM_BOT_THREAD_CONTROL_CLASS_NAME = "CustomBotThreadControl";

//        static CustomizationGuiApi()
//        {
//            assembly = Assembly.GetEntryAssembly();
//            CUSTOM_NAMESPACE = (from t in assembly.GetExportedTypes() where t.Name == CUSTOM_BOT_CLASS_NAME select t.Namespace).FirstOrDefault();
//            if (CUSTOM_NAMESPACE == null)
//                LogMessage.Exit("Could not find class " + CUSTOM_BOT_CLASS_NAME + " in the entry assembly.");

//            bot_gui_type = assembly.GetType(CUSTOM_NAMESPACE + "." + CUSTOM_BOT_GUI_CLASS_NAME);
//            if (bot_gui_type == null)
//            {
//                Log.Inform("Could not find class " + CUSTOM_NAMESPACE + "." + CUSTOM_BOT_GUI_CLASS_NAME + " in the entry assembly.");
//                BotGui = new BotGui();
//            }
//            else
//                BotGui = (BotGui)Activator.CreateInstance(bot_gui_type);

//            bot_thread_control_type = BotGui.GetBotThreadControlType();
//        }
//        static readonly Assembly assembly;
//        static readonly Type bot_gui_type;
//        static readonly public BotGui BotGui;
//        static readonly public Type bot_thread_control_type;

//        internal static BotThreadControl CreateBotThreadControl(int id)
//        {
//            return (BotThreadControl)Activator.CreateInstance(bot_thread_control_type, id);
//        }
//    }
//}