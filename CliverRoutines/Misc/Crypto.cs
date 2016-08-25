//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Text.RegularExpressions;
using System.Text;

namespace Cliver
{
    static public class Crypto
    {
        readonly string key = "";

        public Crypto(string key)
        {
            this.key = Regex.Replace(key, @"\s+", "", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (key.Length < 8)
                throw new Exception("Key is too short.");
        }

        static public string Encrypt(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(convert_by_halojoy(str)));
        }

        static public string Decrypt(string str)
        {
            return convert_by_halojoy(Encoding.UTF8.GetString(Convert.FromBase64String(str)));
        }

        /// <summary>
        /// EnCrypt <-> DeCrypt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string convert_by_halojoy(string str)
        {
            int kl = key.Length;
            if (kl > 32)
                kl = 32;

            int[] k = new int[kl];

            for (int i = 0; i < kl; i++)
                k[i] = (int)(key[i]) & 0x1F;

            int j = 0;
            StringBuilder ss = new StringBuilder(str);
            for (int i = 0; i < str.Length; i++)
            {
                int e = (int)str[i];
                ss[i] = (e & 0xE0) != 0 ? (char)(e ^ k[j]) : (char)e;
                if (++j == kl)
                    j = 0;
            }
            return ss.ToString();
        }
    }
}
