using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Infrastructure.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Interfaces;
using System.Data.SQLite;

namespace Calculator.Models.Memory
{
    public class MemoryDB : IMemory
    {
        public ObservableCollection<double> MemoryColl { get; }
        private string _dbname;

        public MemoryDB(string dbname = "calculator.db")
        {
            _dbname = dbname;
            MemoryColl = MemoryColl ?? new ObservableCollection<double>();
            if (File.Exists(dbname) == false)
            {
                SQLiteConnection.CreateFile(dbname);
                using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + dbname))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText =
                            @"CREATE TABLE saved_values (
                                value VARCHAR NOT NULL)";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        command.CommandText =
                            @"CREATE TABLE expressions (
                                expression VARCHAR NOT NULL,
                                value VARCHAR NOT NULL)";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + dbname))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM saved_values", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MemoryColl.Add(Convert.ToDouble(dt.Rows[i][0]));
                    }
                }
            }
        }
        public int Count
        {
            get => MemoryColl.Count;
        }
        public void Add(double value)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"INSERT INTO saved_values
                                                    VALUES (@tosave)";
                command.Parameters.AddWithValue("@tosave", value);
                command.ExecuteNonQuery();
            }
            MemoryColl.Insert(0, value);
        }

        public void Delete(int index)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"DELETE FROM saved_values
                                                WHERE rowid=@rowid";
                
                command.Parameters.AddWithValue("@rowid", MemoryColl.Count() - index);
                command.ExecuteNonQuery();
                command.UpdatedRowSource = UpdateRowSource.Both;
            }
            MemoryColl.RemoveAt(index);
        }

        public void Increase(int index, double value)
        {
            double newValue = Convert.ToDouble(MemoryColl[index]) + Convert.ToDouble(value);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"UPDATE saved_values 
                                            SET value = @value 
                                            WHERE rowid = @rowid";
                command.Parameters.AddWithValue("@rowid", index + 1);
                command.Parameters.AddWithValue("@value", newValue);
                command.ExecuteNonQuery();
            }
            MemoryColl[index] = newValue;
        }

        public void Decrease(int index, double value)
        {
            double newValue = Convert.ToDouble(MemoryColl[index]) - Convert.ToDouble(value);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"UPDATE saved_values 
                                            SET value = @value 
                                            WHERE rowid = @rowid";
                command.Parameters.AddWithValue("@rowid", index + 1);
                command.Parameters.AddWithValue("@value", newValue);
                command.ExecuteNonQuery();
            }
            MemoryColl[index] = newValue;
        }

        public void Clear()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + _dbname))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"DELETE FROM saved_values";
                command.ExecuteNonQuery();
            }
            MemoryColl.Clear();
        }
        
        public bool Any()
        {
            return MemoryColl.Any();
        }
    }
}
