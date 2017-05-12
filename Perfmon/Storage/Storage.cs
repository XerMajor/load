using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Shamsullin.Common.Extensions;

namespace Perfmon.Storage
{
    public class Storage : IStorage
    {
        private static readonly List<string> Tables; 

        private readonly object _tablesLock = new object(); 

        private static readonly string ConnectionString = $"Data Source=Results\\{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.db;Version=3;";

        private const string NewTableSql = "CREATE TABLE {table} (Timestamp DATETIME, Value REAL)";

        static Storage()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "SELECT name FROM sqlite_master WHERE type='table'";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    var reader = command.ExecuteReader();
                    var response = reader.ToDataTable();
                    Tables = response.Rows.Cast<DataRow>().SelectList(x => x[0].ToStr());
                }
            }
        }

        public void Set(DateTimeOffset timestamp, string name, double? value)
        {
            CreateTableIfNotExists(name);
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = $"INSERT INTO {name}(Timestamp, Value) VALUES (@timestamp, @value)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@timestamp", timestamp);
                    command.Parameters.AddWithValue("@value", value);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTableIfNotExists(string name)
        {
            if (!Tables.Contains(name))
            {
                lock (_tablesLock)
                {
                    if (!Tables.Contains(name))
                    {
                        using (var connection = new SQLiteConnection(ConnectionString))
                        {
                            connection.Open();
                            using (var command = new SQLiteCommand(NewTableSql.Replace("{table}", name), connection))
                            {
                                command.ExecuteNonQuery();
                                Tables.Add(name);
                            }
                        }
                    }
                }
            }
        }
    }
}
