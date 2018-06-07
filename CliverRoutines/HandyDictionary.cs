/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cliver
{
    public class HandyDictionary<KT,VT> : IDisposable
    {
        public HandyDictionary(Func<KT, VT> get_object)
        {
            getObject = get_object;
        }
        Func<KT, VT> getObject;

        ~HandyDictionary()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (keys2values != null)
                {
                    if (typeof(VT) is IDisposable)
                        foreach (VT v in keys2values.Values)
                            ((IDisposable)v).Dispose();
                    keys2values = null;
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                if (typeof(VT) is IDisposable)
                    foreach (VT v in keys2values.Values)
                        ((IDisposable)v).Dispose();
                keys2values.Clear();
            }
        }

        public VT this[KT k]
        {
            get
            {
                lock (this)
                {
                    VT v;
                    if (!keys2values.TryGetValue(k, out v))
                    {
                        v = getObject(k);
                        keys2values[k] = v;
                    }
                    return v;
                }
            }
        }
        Dictionary<KT,VT> keys2values = new Dictionary<KT, VT>();
    }
}