////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        sergey_stoyan@yahoo.com
////        http://www.cliversoft.com
////        26 September 2006
////Copyright: (C) 2006, Sergey Stoyan
////********************************************************************************************
//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using System.Linq;
//using System.Diagnostics;
//using System.Reflection;

//namespace Cliver.BotGui
//{
//    public partial class BotGui
//    {
//        static BotGui()
//        {
//        }

//        internal static T Create<T>()
//        {
//            Type type;
//            if (!base_types2custom_type.TryGetValue(typeof(T), out type))
//            {
//                type = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(T))).FirstOrDefault();
//                base_types2custom_type[typeof(T)] = type;
//            }
//            if(type == null)
//                return (T)Activator.CreateInstance(typeof(T));
//            return (T)Activator.CreateInstance(type);
//        }
//        static readonly Dictionary< Type, Type> base_types2custom_type = new Dictionary<Type, Type>();
//    }
//}
