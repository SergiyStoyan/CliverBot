//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace Cliver.Bot
{
    public class DbConnection
    {
        //static DbConnection()
        //{
        //    This = Create();
        //}

        DbConnection()
        {
        }

        public readonly static DbConnection This;

        public static DbConnection Create(string connection_string = null)
        {
            Log.Inform("connection_string: " + connection_string + "\n\n" + Log.GetStackString());

            if (connection_string == null)
                connection_string = GetPreparedDbConnectionString();

            if (Regex.IsMatch(connection_string, @"\.mdf|\.sdf  \s*=\s*System\.Data\.SqlClient", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
                return new MsSqlConnection(connection_string);
            else
                throw new Exception("Could not detect an appropriate wrapper class for " + connection_string);
        }

        public static DbConnection CreateFromNativeConnection(object connection)
        {
            if (connection == null)
                throw new Exception("Connection is null.");

            if (connection is System.Data.SqlClient.SqlConnection)
                return new MsSqlConnection((System.Data.SqlClient.SqlConnection)connection);

            throw new Exception("Could not detect an appropriate wrapper class for " + ((System.Data.SqlClient.SqlConnection)connection).ConnectionString);
        }

        /// <summary>
        /// Substitutes macros and canonizes db path.
        /// </summary>
        /// <returns></returns>
        public static string GetPreparedDbConnectionString(string db_connection_string = null, string db_path = null)
        {
            if (db_path == null)
                db_path = ConfigurationManager.AppSettings["DbPath"];
            if (Regex.IsMatch(db_path, @"^\s*[\.\\]"))
                db_path = Log.GetAbsolutePath(db_path);
            if (db_connection_string == null)
                db_connection_string = ConfigurationManager.AppSettings["DbConnectionString"];
            return Regex.Replace(db_connection_string, @"\|DbPath\|", db_path, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        protected DbConnection(string connection_string = null)
        {
            this.ConnectionString = connection_string;
        }

        protected DbConnection(System.Data.Common.DbConnection connection)
        {
            this.native_connection = connection;
        }

        public readonly string ConnectionString;

        /// <summary>
        /// Current database
        /// </summary>
        public string Database
        {
            get
            {
                return get_database();
            }
        }
        virtual protected string get_database() { throw new Exception("Not overriden"); }

        /// <summary>
        /// Native connection that must be casted.
        /// </summary>
        internal object NativeConnection
        {
            get
            {
                lock (sqls2commands)
                {
                    return get_refreshed_native_connection();
                }
            }
        }
        protected virtual object get_refreshed_native_connection() { return null; }
        protected object native_connection;

        /// <summary>
        /// Creates and caches a command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DbCommand this[string sql]
        {
            get
            {
                lock (sqls2commands)
                {
                    Cliver.Bot.DbCommand c;
                    if (!sqls2commands.TryGetValue(sql, out c))
                    {
                        c = create_command(sql);
                        sqls2commands[sql] = c;
                    }
                    return c;
                }
            }
        }
        virtual internal DbCommand create_command(string sql) { throw new Exception("Not overriden"); }
        protected Dictionary<string, DbCommand> sqls2commands = new Dictionary<string, DbCommand>();

        /// <summary>
        /// Creates a not cached command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DbCommand Get(string sql)
        {
            return create_command(sql);
        }
    }

    internal class MsSqlConnection : DbConnection
    {
        internal MsSqlConnection(string connection_string = null)
            : base(connection_string)
        {
        }

        internal MsSqlConnection(System.Data.SqlClient.SqlConnection connection)
            : base(connection)
        {
        }        

        override protected object get_refreshed_native_connection()
        {
            SqlConnection c = (SqlConnection)native_connection;
            if (c == null)
            {
                c = new SqlConnection(ConnectionString);
                native_connection = c;
                c.Open();
            }
            if (c.State != ConnectionState.Open)
            {
                c.Dispose();
                c = new SqlConnection(ConnectionString);
                native_connection = c;
                c.Open();
                Dictionary<string, DbCommand> s2cs = new Dictionary<string, DbCommand>();
                foreach (string sql in sqls2commands.Keys)
                    s2cs[sql] = new MsSqlCommand(sql, this);
                sqls2commands = s2cs;
            }
            return c;
        }

        override internal DbCommand create_command(string sql)
        {
            return new MsSqlCommand(sql, this);
        }

        override protected string get_database()
        {
            return ((SqlConnection)native_connection).Database;
        }
    }
}

        //public OdbcConnection Odbc
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (oc == null
        //                || oc.State != ConnectionState.Open
        //                || oc.State == ConnectionState.Broken
        //                || oc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    oc = new OdbcConnection(ConnectionStr);
        //                    oc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return oc;
        //        }
        //    }
        //}
        //OdbcConnection oc = null;

        //public OleDbConnection OleDb
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (lc == null
        //                || lc.State != ConnectionState.Open
        //                || lc.State == ConnectionState.Broken
        //                || lc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    lc = new OleDbConnection(ConnectionStr);
        //                    lc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return lc;
        //        }
        //    }
        //}
        //OleDbConnection lc = null;
        
        //public MySqlConnection MySql
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (mc == null
        //                || mc.State != ConnectionState.Open
        //                || mc.State == ConnectionState.Broken
        //                || mc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    mc = new MySqlConnection(ConnectionStr);
        //                    mc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return mc;
        //        }
        //    }
        //}
        //MySqlConnection mc = null;
