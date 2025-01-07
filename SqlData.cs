using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using SQLitePCL;
using System.IO;

namespace Task4_2
{
    internal class SqlData : Data
    {
        private string connectionString;

        public SqlData(string selectedFiles) :base(selectedFiles)
        {
            connectionString = $"Data Source={base.filePath};";
            //Batteries.Init();
        }

        public override void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders)
        {
            List<Buyers> buy = new List<Buyers>();
            List<Items> it = new List<Items>();
            List<Orders> or = new List<Orders>();

            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // Чтение таблицы Buyers
                    string queryBuyers = "SELECT id, name, address, tel, person FROM Buyers";
                    using (SqliteCommand command = new SqliteCommand(queryBuyers, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buy.Add(new Buyers(
                                id: reader.GetInt32(0),
                                name: reader.GetString(1),
                                address: reader.GetString(2),
                                tel: reader.GetString(3),
                                person: reader.GetString(4)
                            ));
                        }
                    }

                    // Чтение таблицы Items
                    string queryItems = "SELECT id, name, value, description, have FROM Items";
                    using (SqliteCommand command = new SqliteCommand(queryItems, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            it.Add(new Items(
                                id: reader.GetInt32(0),
                                name: reader.GetString(1),
                                value: reader.GetInt32(2),
                                description: reader.GetString(3),
                                check: reader.GetBoolean(4)
                            ));
                        }
                    }

                    // Чтение таблицы Orders
                    string queryOrders = "SELECT id, buyer_id, item_id, quantity, date FROM Orders";
                    using (SqliteCommand command = new SqliteCommand(queryOrders, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            or.Add(new Orders(
                                id: reader.GetInt32(0),
                                buyer: (FindHeap.FindElementUsingHeap(buy, reader.GetInt32(1))),
                                item: (FindHeap.FindElementUsingHeap(it, reader.GetInt32(2))),
                                item_quantity: reader.GetInt32(3),
                                date: reader.GetDateTime(4)
                            ));
                        }
                    }
                }
                orders.Clear(); orders = new List<Orders>(or); or.Clear();
                buyers.Clear(); buyers = new List<Buyers>(buy); buy.Clear();
                items.Clear(); items = new List<Items>(it); it.Clear();
                Console.WriteLine("Данные успешно прочитаны из базы данных.");
            }
            catch { throw new ArgumentException("Ошибка подключения к базе данных"); }
        }

        public override void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders)
        {
            FileInfo file = new FileInfo(base.filePath);
            try { 
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string createTables = @"
            CREATE TABLE IF NOT EXISTS Buyers (
                id INTEGER PRIMARY KEY,
                name TEXT,
                address TEXT,
                tel TEXT,
                person TEXT
            );
            CREATE TABLE IF NOT EXISTS Items (
                id INTEGER PRIMARY KEY,
                name TEXT,
                value INTEGER,
                description TEXT,
                have BOOLEAN
            );
            CREATE TABLE IF NOT EXISTS Orders (
                id INTEGER PRIMARY KEY,
                buyer_id INTEGER,
                item_id INTEGER,
                quantity INTEGER,
                date TEXT,
                FOREIGN KEY (buyer_id) REFERENCES Buyers(id),
                FOREIGN KEY (item_id) REFERENCES Items(id)
            );";
                using (SqliteCommand command = new SqliteCommand(createTables, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Очистка таблиц перед записью
                string clearTables = @"
                    DELETE FROM Buyers;
                    DELETE FROM Items;
                    DELETE FROM Orders;";
                using (SqliteCommand command = new SqliteCommand(clearTables, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Запись данных в таблицу Buyers
                foreach (var buyer in buyers)
                {
                    string insertBuyer = "INSERT INTO Buyers (id, name, address, tel, person) VALUES (@id, @name, @address, @tel, @person)";
                    using (SqliteCommand command = new SqliteCommand(insertBuyer, connection))
                    {
                        command.Parameters.AddWithValue("@id", buyer.id);
                        command.Parameters.AddWithValue("@name", buyer.name);
                        command.Parameters.AddWithValue("@address", buyer.address);
                        command.Parameters.AddWithValue("@tel", buyer.tel);
                        command.Parameters.AddWithValue("@person", buyer.person);
                        command.ExecuteNonQuery();
                    }
                }

                // Запись данных в таблицу Items
                foreach (var item in items)
                {
                    string insertItem = "INSERT INTO Items (id, name, value, description, have) VALUES (@id, @name, @value, @description, @have)";
                    using (SqliteCommand command = new SqliteCommand(insertItem, connection))
                    {
                        command.Parameters.AddWithValue("@id", item.id);
                        command.Parameters.AddWithValue("@name", item.name);
                        command.Parameters.AddWithValue("@value", item.value);
                        command.Parameters.AddWithValue("@description", item.description);
                        command.Parameters.AddWithValue("@have", item.check);
                        command.ExecuteNonQuery();
                    }
                }

                // Запись данных в таблицу Orders
                foreach (var order in orders)
                {
                    string insertOrder = "INSERT INTO Orders (id, buyer_id, item_id, quantity, date) VALUES (@id, @buyerId, @itemId, @quantity, @date)";
                    using (SqliteCommand command = new SqliteCommand(insertOrder, connection))
                    {
                        command.Parameters.AddWithValue("@id", order.id);
                        command.Parameters.AddWithValue("@buyerId", order.buyer.id);
                        command.Parameters.AddWithValue("@itemId", order.item.id);
                        command.Parameters.AddWithValue("@quantity", order.item_quantity);
                        command.Parameters.AddWithValue("@date", order.date);
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Данные успешно записаны в базу данных.");
            }
            }
            catch { throw new ArgumentException("Что-то пошло не так"); }
        }
    }
}
