//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using Cliver;
using System.Windows.Forms;

namespace CliverBotCustomization
{
    class CliverBotCustomization
    {
        [STAThread]
        static void Main()
        {
            Config.General.RestoreBrokenSession = true;
            Config.General.WriteSessionRestoringLog = true;
            Config.Save();            
            //Cliver.Program.Run();//It is the entry when the app does not have GUI.
            Cliver.ProgramGui.Run();//It is the entry when the app uses the default GUI.
        }
    }

    /// <summary>
    /// Defines look of GUI. May be not implemented.
    /// </summary>
    public class CustomBotGui : ACustomBotGui
    {
        override public string[] GetDefaultConfigSectionList()
        {
            return null;
        }

        override public string[] GetCustomConfigControls()
        {
            return null;
        }

        override public BaseForm GetToolsForm()
        {
            return null;
        }
    }

    /// <summary>
    /// Most important interface that defines certain routines of CliverBot customisation.
    /// </summary>
    public class CustomBot : ACustomBot
    {
        /// <summary>
        /// Invoked when the session is in creating stage. Can be not defined. If throw an Exception, the session is stopped and closed.
        /// </summary>
        new static public void SessionCreating()
        {
            //Set the order which queues are to be processed by. When it is not set, it is built automatically along LIFO rule.
            Session.SetInputItemQueuesOrder(typeof(Product), typeof(Category), typeof(Site));

            //Set the queue which the progress bar will reflect. 
            ProgramGui.SetProgressInputItemType(typeof(Product));
            
            //It is possible to add InputItems to queues before BotCycle started            
            Session.Add<Site>(new { Url = "www.google.com" });
            
            counters["product"] = 0;
        }

        /// <summary>
        /// Invoked when the session is closing.
        /// </summary>
        new static public void SessionClosing()
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread as it has been started.
        /// </summary>
        public override void CycleBeginning()
        {
        }

        /// <summary>
        /// Invoked by BotCycle thread when it is exiting.
        /// </summary>
        public override void CycleFinishing()
        {
        }

        /// <summary>
        /// Custom InputItem types are defined as classes based on InputItem
        /// </summary>
        public class Site : InputItem
        {
            /// <summary>
            /// Attribute KeyField can be set to indicate what fields are keyed. When it is not used, all public value or string types in the class are keyed.
            ///Otherwise, attribute NotKeyField can be set to exclude the marked field from the key. 
            ///KeyField and NotKeyField cannot be used at the same time within the same InputItem class.
            /// </summary>
            [KeyField]
            //Fields must be public value or string type.
            readonly public string Url;

            /// <summary>
            /// InputItem can define a method to process it. 
            /// When it is not defined within InputItem class, it must be defined in CustomBot as PROCESSOR_[InputItem class name]
            /// </summary>
            /// <param name="bc">BotCycle that keeps the current thread</param>
            override public void PROCESSOR(BotCycle bc)
            {
                bc.Add(new Category(url: Url + "?q=1", t: new Category.Tag(name: "fff", description: "ttttt")));

                //it is possible to get the current CustomBot when access to common members is needed
                ((CustomBot)bc.CB).counter++;
            }
        }
        int counter = 0;

        public class Category : InputItem
        {
            readonly public string Url;

            /// <summary>
            /// Custom TagItem is designed to be linked by many InputItem objects, to save memory.
            /// It is not key field.
            /// It is serialized automatically just as InputItem's.
            /// </summary>
            public class Tag : TagItem
            {
                readonly public string Name;
                readonly public string Description;

                /// <summary>
                /// Constructor is not a must but makes creating/adding a custom InputItem handier.
                /// Be sure it does no action that can influence on session restoring.
                /// </summary>
                public Tag(string name, string description)
                {
                    Name = name;
                    Description = description;
                }
            }
            readonly public Tag T;

            /// <summary>
            /// Constructor is not a must but makes creating/adding a custom InputItem handier.
            /// Be sure it does no action that can influence on session restoring.
            /// </summary>
            public Category(string url, Tag t)
            {
                Url = url;
                T = t;
            }

            override public void PROCESSOR(BotCycle bc)
            {
                for (int i = 0; i < 3; i++)
                    bc.Add(new Product(url: i + Url));

                //list next page
                bc.Add(new Category(url: "qqqqq2", t: T));
            }
        }

        public class Product : InputItem
        {
            //It is possible to declare derivatives of InputItem that are parents for this item.
            //When defined, they are set by the system automatically.
            //Parent InputItem is the item that is current when new items are added to the system.
            //Also, can be defined not direct parent but grand-parents also.
            //As not always praent item types are the same, these memebers can be null and so should be checked for null and can be used as flags.
            readonly public Category Category;

            [KeyField]
            readonly public string Url;

            /// <summary>
            /// Constructor is not a must but makes creating/adding a custom InputItem handier.
            /// Be sure it does no action that can influence on session restoring.
            /// </summary>
            public Product(string url)
            {
                Url = url;
            }
        }
                
        /// <summary>
        /// When PROCESSOR is not defined within InputItem class, it must be defined in CustomBot as PROCESSOR_[InputItem class name]
        /// It is handier when access to CustomBot members is needed.
        /// </summary>
        /// <param name="item">custom InputItem</param>
        public void PROCESSOR_Product(Product item)
        {
            counters["product"] = counters["product"] + 1;
            if (counters["product"] == 3)
                //ProcessorException has a flag that specifies how to restore the current item.
                throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get product: " + item.Url);
        }

        /// <summary>
        /// A demo of WorkItem and SingleValueWorkItem
        /// </summary>
        static SingleValueWorkItemDictionary<Counter, int> counters = Session.GetSingleValueWorkItemDictionary<Counter, int>();

        /// <summary>
        /// A demo of WorkItem and SingleValueWorkItem
        /// </summary>
        public class Counter : SingleValueWorkItem<int>
        {
        }
    }
}
