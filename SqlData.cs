using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task4_2
{
    internal class SqlData : Data
    {
        private string connectionString;

        public SqlData(string selectedFiles) : base(selectedFiles)
        {
            connectionString = $"Data Source={base.filePath};";
            Batteries.Init();
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
                    string queryOrders = "SELECT id, buyer_id, date FROM Orders";
                    using (SqliteCommand command = new SqliteCommand(queryOrders, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(0);
                            int buyerId = reader.GetInt32(1);
                            DateTime date = reader.GetDateTime(2);

                            List<(Items Item, int Quantity)> all_items = new List<(Items Item, int Quantity)>();
                            string queryOrderItems = "SELECT item_id, quantity FROM OrderItems WHERE order_id = @orderId";
                            using (SqliteCommand orderItemsCommand = new SqliteCommand(queryOrderItems, connection))
                            {
                                orderItemsCommand.Parameters.AddWithValue("@orderId", orderId);

                                using (SqliteDataReader orderItemsReader = orderItemsCommand.ExecuteReader())
                                {
                                    while (orderItemsReader.Read())
                                    {
                                        int itemId = orderItemsReader.GetInt32(0);
                                        int quantity = orderItemsReader.GetInt32(1);
                                        Items item = it.FirstOrDefault(i => i.id == itemId);
                                        if (item != null)
                                        {
                                            all_items.Add((item, quantity));
                                        }
                                    }
                                }
                            }

                            Buyers buyer = buy.FirstOrDefault(b => b.id == buyerId);
                            if (buyer != null)
                            {
                                or.Add(new Orders(
                                    id: orderId,
                                    buyer: buyer,
                                    items: all_items,
                                    date: date
                                ));
                            }
                        }
                    }
                }
                orders.Clear(); orders = new List<Orders>(or); or.Clear();
                buyers.Clear(); buyers = new List<Buyers>(buy); buy.Clear();
                items.Clear(); items = new List<Items>(it); it.Clear();
                Console.WriteLine("Данные успешно прочитаны из базы данных.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        public override void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // Создание таблиц
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
                            date TEXT,
                            FOREIGN KEY (buyer_id) REFERENCES Buyers(id)
                        );
                        CREATE TABLE IF NOT EXISTS OrderItems (
                            order_id INTEGER,
                            item_id INTEGER,
                            quantity INTEGER,
                            FOREIGN KEY (order_id) REFERENCES Orders(id),
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
                        DELETE FROM Orders;
                        DELETE FROM OrderItems;";
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

                    // Запись данных в таблицу Orders и OrderItems
                    foreach (var order in orders)
                    {
                        // Сохранение заказа
                        string insertOrder = "INSERT INTO Orders (id, buyer_id, date) VALUES (@id, @buyerId, @date)";
                        using (SqliteCommand command = new SqliteCommand(insertOrder, connection))
                        {
                            command.Parameters.AddWithValue("@id", order.id);
                            command.Parameters.AddWithValue("@buyerId", order.buyer.id);
                            command.Parameters.AddWithValue("@date", order.date.ToString("yyyy.MM.dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }

                        // Сохранение товаров для заказа
                        foreach (var orderItem in order.items)
                        {
                            string insertOrderItem = "INSERT INTO OrderItems (order_id, item_id, quantity) VALUES (@orderId, @itemId, @quantity)";
                            using (SqliteCommand command = new SqliteCommand(insertOrderItem, connection))
                            {
                                command.Parameters.AddWithValue("@orderId", order.id);
                                command.Parameters.AddWithValue("@itemId", orderItem.Item.id);
                                command.Parameters.AddWithValue("@quantity", orderItem.Quantity);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    Console.WriteLine("Данные успешно записаны в базу данных.");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ошибка при записи в базу данных: {ex.Message}");
            }
        }
    }
}
