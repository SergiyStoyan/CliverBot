/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cliver
{
    public class HandyDictionary<KT, VT> : IDisposable, IEnumerable<KeyValuePair<KT, VT>> //where VT: class
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

        virtual public void Dispose()
        {
            lock (this)
            {
                if (keys2values != null)
                {
                    if (IsDisposable(typeof(VT)))
                        foreach (VT v in keys2values.Values)
                            ((IDisposable)v).Dispose();
                    keys2values = null;
                }
            }
        }

        static bool IsDisposable(Type t)
        {
            return typeof(IDisposable).IsAssignableFrom(t);
        }

        virtual public void Clear()
        {
            lock (this)
            {
                if (IsDisposable(typeof(VT)))
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
            protected set
            {
                lock (this)
                {
                    keys2values[k] = value;
                }
            }
        }
        Dictionary<KT, VT> keys2values = new Dictionary<KT, VT>();

        public IEnumerator<KeyValuePair<KT, VT>> GetEnumerator()
        {
            return keys2values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Dictionary<KT, VT>.KeyCollection Keys
        {
            get
            {
                return keys2values.Keys;
            }
        }

        public Dictionary<KT, VT>.ValueCollection Values
        {
            get
            {
                return keys2values.Values;
            }
        }
    }
}

///********************************************************************************************
//        Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Cliver
//{
//    public class HandyDictionary<KT, VT> : Dictionary<KT, VT>, IDisposable  //where VT: class
//    {
//        public HandyDictionary(Func<KT, VT> get_object)
//        {
//            getObject = get_object;
//        }
//        Func<KT, VT> getObject;

//        ~HandyDictionary()
//        {
//            Dispose();
//        }

//        public Enumerator GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        virtual public void Dispose()
//        {
//            Clear();
//        }

//        static bool IsDisposable(Type t)
//        {
//            return typeof(IDisposable).IsAssignableFrom(t);
//        }

//        virtual public void Clear()
//        {
//            lock (this)
//            {
//                if (IsDisposable(typeof(VT)))
//                    foreach (VT v in base.Values)
//                        ((IDisposable)v).Dispose();
//                base.Clear();
//            }
//        }

//        public VT this[KT k]
//        {
//            get
//            {
//                lock (this)
//                {
//                    VT v;
//                    if (!base.TryGetValue(k, out v))
//                    {
//                        v = getObject(k);
//                        base[k] = v;
//                    }
//                    return v;
//                }
//            }
//        }
//    }
//}