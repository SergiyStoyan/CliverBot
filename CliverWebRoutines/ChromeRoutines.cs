//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;
using System.Data.SQLite;
using System.Net;

namespace Cliver.Bot
{
    public static class ChromeRoutines
    {
        public static void LoadCookies2Container(CookieContainer cookie_container)
        {
            foreach (ChromeRoutines.Cookie c in ChromeRoutines.ReadCookies())
                try
                {
                    cookie_container.Add(new System.Net.Cookie(c.name, c.AsciiDecryptedValue, c.path, c.host_key));
                }
                catch (Exception e)
                {
                    try
                    {
                        cookie_container.Add(new System.Net.Cookie(c.name, HttpUtility.UrlEncode(c.AsciiDecryptedValue), c.path, c.host_key));
                    }
                    catch (Exception e2)
                    {
                        Log.Error(e2);
                    }
                }
        }

        public class Cookie
        {
            public System.Int64 creation_utc;
            public string host_key;
            public string name;
            public string value;
            public string path;
            public System.Int64 expires_utc;
            public System.Int64 secure;
            public System.Int64 httponly;
            public System.Int64 last_access_utc;
            public System.Int64 has_expires;
            public System.Int64 persistent;
            public System.Int64 priority;
            //public byte[] encrypted_value;
            public string AsciiDecryptedValue;
            public System.Int64 firstpartyonly;
        }
        //CREATE TABLE cookies(creation_utc INTEGER NOT NULL UNIQUE PRIMARY KEY, host_key TEXT NOT NULL, name TEXT NOT NULL, value TEXT NOT NULL, path TEXT NOT NULL, 
        //expires_utc INTEGER NOT NULL, secure INTEGER NOT NULL, httponly INTEGER NOT NULL, last_access_utc INTEGER NOT NULL, has_expires INTEGER NOT NULL DEFAULT 1, 
        //persistent INTEGER NOT NULL DEFAULT 1, priority INTEGER NOT NULL DEFAULT 1, encrypted_value BLOB DEFAULT '', firstpartyonly INTEGER NOT NULL DEFAULT 0)
        
        static public IEnumerable<Cookie> ReadCookies()
        {
            var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cookies";
            if (!System.IO.File.Exists(dbPath)) throw new System.IO.FileNotFoundException("Cant find cookie store", dbPath); // race condition, but i'll risk it

            var connectionString = "Data Source=" + dbPath;
            
            using (var conn = new System.Data.SQLite.SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                var prm = cmd.CreateParameter();
                cmd.Parameters.Add(prm);
                cmd.CommandText = "SELECT * FROM cookies;Read Only=True";
                //cmd.CommandText = "PRAGMA table_info(cookies)";
                //cmd.CommandText = "SELECT sql FROM sqlite_master WHERE tbl_name = 'cookies' AND type = 'table'";

                conn.Open();
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var decoded_value = System.Security.Cryptography.ProtectedData.Unprotect((byte[])reader["encrypted_value"], null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                            var ascii_value = Encoding.ASCII.GetString(decoded_value); 
                            yield return new Cookie() {
                                creation_utc = (System.Int64)reader["creation_utc"],
                                host_key = (string)reader["host_key"],
                                name = (string)reader["name"],
                                value = (string)reader["value"],
                                path = (string)reader["path"],
                                expires_utc = (System.Int64)reader["expires_utc"],
                                secure = (System.Int64)reader["secure"],
                                httponly = (System.Int64)reader["httponly"],
                                last_access_utc = (System.Int64)reader["last_access_utc"],
                                has_expires = (System.Int64)reader["has_expires"],
                                persistent = (System.Int64)reader["persistent"],
                                priority = (System.Int64)reader["priority"],
                                AsciiDecryptedValue = ascii_value,
                                firstpartyonly = (System.Int64)reader["firstpartyonly"]
                            };
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        static public IEnumerable<Tuple<string, string>> ReadCookies(string hostName)
        {
            if (hostName == null) throw new ArgumentNullException("hostName");

            var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cookies";
            if (!System.IO.File.Exists(dbPath)) throw new System.IO.FileNotFoundException("Cant find cookie store", dbPath); // race condition, but i'll risk it

            var connectionString = "Data Source=" + dbPath + ";pooling=false";

            using (var conn = new System.Data.SQLite.SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                var prm = cmd.CreateParameter();
                prm.ParameterName = "hostName";
                prm.Value = hostName;
                cmd.Parameters.Add(prm);

                //cmd.CommandText = "SELECT name,encrypted_value FROM cookies WHERE host_key = @hostName";
                cmd.CommandText = "SELECT name,encrypted_value,host_key FROM cookies";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var encryptedData = (byte[])reader[1];
                        var decodedData = System.Security.Cryptography.ProtectedData.Unprotect(encryptedData, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                        var plainText = Encoding.ASCII.GetString(decodedData); // Looks like ASCII

                        if (Regex.IsMatch(reader.GetString(2), hostName))
                            yield return Tuple.Create(reader.GetString(2) + ":" + reader.GetString(0), plainText);
                    }
                }
                conn.Close();
            }
        }

        private static string GetChromeCookiePath()
        {
            string s = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            s += @"\Google\Chrome\User Data\Default\cookies";

            if (!File.Exists(s))
                return string.Empty;

            return s;
        }

        private static bool GetCookie_Chrome(string strHost, string strField, ref string Value)
        {
            Value = string.Empty;
            bool fRtn = false;
            string strPath, strDb;

            // Check to see if Chrome Installed
            strPath = GetChromeCookiePath();
            if (string.Empty == strPath) // Nope, perhaps another browser
                return false;

            try
            {
                strDb = "Data Source=" + strPath + ";pooling=false";

                using (SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(strDb))
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT value FROM cookies WHERE host_key LIKE '%" + strHost + "%' AND name LIKE '%" + strField + "%';";

                        conn.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Value = reader.GetString(0);
                                if (!Value.Equals(string.Empty))
                                {
                                    fRtn = true;
                                    break;
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception)
            {
                Value = string.Empty;
                fRtn = false;
            }
            return fRtn;
        }
    }
}

