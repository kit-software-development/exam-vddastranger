﻿using MySql.Data.MySqlClient;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Server
{
    public class DataBaseManagerEventArgs : EventArgs
    {
        public string Query { get; set; }
        public string Exception { get; set; }
    }

    public class DataBaseManager : IDataBase
    {
        // event
        public event EventHandler<DataBaseManagerEventArgs> ConnectToDB;
        public event EventHandler<DataBaseManagerEventArgs> ExecuteReader;
        public event EventHandler<DataBaseManagerEventArgs> ExecuteNonQuery;

        // singleton
        static DataBaseManager instance = null;
        static readonly object padlock = new object();

        private MySqlConnection mySqlConnect;

        private MySqlCommand mySqlCommand;

        // коннект к бд
        private static string dbHost = Settings.DB_HOST;
        private static string database = Settings.DB;
        private static string uid = Settings.DB_ROOT;
        private static string password = Settings.DB_PASS;
        string connectionDetails = "SERVER=" + dbHost + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        private bool isConnected = false;

        // DataTable для сохранения бд
        private DataTable table;

        private List<string> parameters;

        Int64 lastInsertedID;

        // singleton
        public static DataBaseManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new DataBaseManager();

                    return instance;
                }
            }
        }

        private DataBaseManager()
        {
            connectToDb();
            table = table = new DataTable();
            parameters = new List<string>();
        }

        public Int64 getLastInsertedID()
        {
            return lastInsertedID;
        }

        private void connectToDb()
        {
            try
            {
                mySqlConnect = new MySqlConnection(connectionDetails);
                mySqlConnect.Open();
                isConnected = true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                OnConnectToDB(except(ex));
            }
        }

        public void closeConnection()
        {
            isConnected = false;
            mySqlConnect.Close();
            mySqlConnect.Dispose();
        }

        private void addValueToParameters()
        {
            if (parameters.Count > 0)
                parameters.ForEach(delegate (string parameter)
                {
                    string[] sparameters = parameter.ToString().Split('\x7F');
                    mySqlCommand.Parameters.AddWithValue(sparameters[0], sparameters[1]);
                });

            parameters.Clear();
        }

        private void dBCommand(string query)
        {
            if (isConnected == false)
                connectToDb();
            else
            {
                using (mySqlCommand = new MySqlCommand(query, mySqlConnect))
                {
                    addValueToParameters();
                }
            }
        }

        public void bind(string field, string value)
        {
            parameters.Add("@" + field + "\x7F" + value);
        }

        public void bind(string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    bind(fields[i], fields[i + 1]);
                    i += 1;
                }
            }
        }

        private void selectDb(string query)
        {
            table = new DataTable();
            try
            {
                dBCommand(query);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                table.Load(reader);
            }
            catch (MySqlException ex)
            {
                string exception = "Exception : SQL Query : \n\r" + query + "\n\r";
                OnExecuteReader(query, except(ex));
            }
        }
        // Update Delete Insert
        private int nonQuery(string query)
        {
            int affected = 0;

            try
            {
                dBCommand(query);
                affected = mySqlCommand.ExecuteNonQuery();
                lastInsertedID = mySqlCommand.LastInsertedId;
            }
            catch (MySqlException ex)
            {
                string exception = "Exception : SQL Query : \n\r" + query + "\n\r";
                OnExecuteNonQuery(query, except(ex));
            }

            return affected;
        }

        public DataTable allFromTable(string tableName)
        {
            selectDb("SELECT * FROM " + tableName);
            return table;
        }

        public int executeNonQuery(string query)
        {
            int affectedRows = nonQuery(query);

            return affectedRows;
        }

        public DataTable manySelect(string query)
        {
            selectDb(query);
            return table;
        }

        public string singleSelect(string query)
        {
            selectDb(query);

            if (table.Rows.Count > 0)
                return table.Rows[0][0].ToString();

            return string.Empty;
        }

        public string[] tableToRow()
        {
            string[] row = new string[table.Columns.Count];

            if (table.Rows.Count > 0)
            {
                for (int i = 0; i++ < table.Columns.Count; row[i - 1] = table.Rows[0][i - 1].ToString()) ;
                return row;
            }
            else return null;
        }

        public List<string> tableToColumn()
        {
            List<string> column = new List<string>();

            if (table.Columns.Count > 0)
                for (int i = 0; i++ < table.Rows.Count; column.Add(table.Rows[i - 1][0].ToString())) ;

            return column;
        }

        public static string except(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        // events
        protected virtual void OnConnectToDB(string exception)
        {
            ConnectToDB?.Invoke(this, new DataBaseManagerEventArgs() { Exception = exception });
        }
        protected virtual void OnExecuteReader(string query, string exception)
        {
            ExecuteReader?.Invoke(this, new DataBaseManagerEventArgs() { Query = query, Exception = exception });
        }
        protected virtual void OnExecuteNonQuery(string query, string exception)
        {
            ExecuteNonQuery?.Invoke(this, new DataBaseManagerEventArgs() { Query = query, Exception = exception });
        }
    }
}
