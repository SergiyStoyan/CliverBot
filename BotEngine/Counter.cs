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
    public class Counter
    {
        public Counter(string name, int max_count, OnMaxCount on_max_count = null)
        {
            this.name = name;
            this.max_count = max_count;
            this.on_max_count = on_max_count;
        }
        private string name;
        private int max_count;
        private int count = 0;

        public void Increment()
        {
            lock (this)
            {
                if (max_count < 0) return;
                count++;
                if (count > max_count)
                {
                    if (on_max_count != null)
                        on_max_count(count);
                    else
                        throw new Exception("Counter " + name + " exceeded " + max_count);
                    //Log.Exit("Counter " + name + " exceeded " + max_count);
                }
            }
        }

        public delegate void OnMaxCount(int count);
        OnMaxCount on_max_count = null;

        public void Reset()
        {
            lock (this)
            {
                count = 0;
            }
        }

        public int Count
        {
            get
            {
                lock (this)
                {
                    return count;
                }
            }
        }
    }
}