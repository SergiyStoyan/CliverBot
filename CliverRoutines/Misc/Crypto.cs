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
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    public class Crypto3
    {
        readonly string key = null;
        
        public Crypto3(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            this.key = key;
        }

        public string Encrypt(string str)
        {
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, 8);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(str);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            byte[] bytes = Convert.FromBase64String(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, 8);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
    }

    public class Crypto2
    {
        readonly byte[] key = null;

        public Crypto2(byte[] key)
        {
            this.key = key;
        }

        public Crypto2(string key)
        {
            if (key != null)
                this.key = Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");            
            var data = Encoding.Unicode.GetBytes(str);
            byte[] encrypted = ProtectedData.Protect(data, key, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");            
            byte[] data = Convert.FromBase64String(str);            
            byte[] decrypted = ProtectedData.Unprotect(data, key, DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(decrypted);
        }
    }

    public class Crypto
    {
        readonly string key;

        public Crypto(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            this.key = Regex.Replace(key, @"\s+", "", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (key.Length < 3)
                throw new Exception("Key is too short.");
        }

        public string Encrypt(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(convert_by_halojoy(str)));
        }

        public string Decrypt(string str)
        { 
            if (str == null)
                throw new ArgumentNullException("str");
            return convert_by_halojoy(Encoding.UTF8.GetString(Convert.FromBase64String(str)));
        }

        /// <summary>
        /// EnCrypt <-> DeCrypt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string convert_by_halojoy(string str)
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
