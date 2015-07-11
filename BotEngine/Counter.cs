//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Cliver.Bot
{
    /// <summary>
    /// Used to amass errors and brake execution when too much
    /// </summary>
    class Counter
    {
        public Counter(string name, int max_count)
        {
            this.name = name;
            this.max_count = max_count;
        }
        private string name;
        private int max_count;
        private int count = 0;

        public void Increment()
        {
            if (max_count < 0) return;
            count++;
            if (count > max_count)
                throw new Exception("Counter " + name + " exceeded " + max_count);
                //Log.Exit("Counter " + name + " exceeded " + max_count);
        }

        public void Reset()
        {
            count = 0;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }
    }
}
