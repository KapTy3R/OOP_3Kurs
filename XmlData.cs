﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace Task1
{
    internal class XmlData : Data
    {
        private string filePath;
        List<string> str;
        XDocument xmlDoc = new XDocument();
        static List<Buyers> buy = new List<Buyers>();
        static List<Orders> or = new List<Orders>();
        static List<Items> it = new List<Items>();

        public XmlData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Укажите путь к файлу.");
            this.filePath = filePath;
        }

        public override void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.");

            XDocument xmlDoc = XDocument.Load(filePath);

            // Чтение покупателей
            buyers = new List<Buyers>();

            str = xmlDoc.Descendants("buyers").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("name")}, {(string)b.Attribute("address")}, {(string)b.Attribute("tel")}, {(string)b.Attribute("person")}")).ToList();
            foreach (var s in str)
            {
                buyers.Add(Lists.Enter(s, buyers));
            }
            buy = new List<Buyers>(buyers);
            buyers.Clear();
            str.Clear();


            // Чтение товаров
            items = new List<Items>();
            str = xmlDoc.Descendants("items").Select(b => new string($"{(int)b.Attribute("id")}, {(string)b.Attribute("name")}, {(int)b.Attribute("value")}, {(string)b.Attribute("description")}, {(bool)b.Attribute("have")}")).ToList();
            foreach (var s in str)
            {
                string[] temp = s.Split(',');
                items.Add(Lists.Enter(s.Remove(s.Length - (temp[temp.Length - 1].Length + 1)), Convert.ToBoolean(temp[temp.Length - 1]), items));
            }
            it = new List<Items>(items);
            items.Clear();
            str.Clear();

            // Чтение заказов
            orders = new List<Orders>();

            str = xmlDoc.Descendants("orders").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("buyer_id")},{(string)b.Attribute("item_id")}, {(string)b.Attribute("quantity")}, {(string)b.Attribute("date")}")).ToList();
            foreach (var s in str)
            {
                string[] temp = s.Split(',');
                orders.Add(Lists.Enter(s.Remove(s.Length - (temp[temp.Length - 1].Length + 1)), DateTime.Parse(temp[temp.Length - 1]), buy, it, orders));
            }
            or = new List<Orders>(orders);
            orders.Clear();
            str.Clear();

            Console.WriteLine("Данные успешно прочитаны из XML-файла.");
        }

        public override void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders)
        {
            if (buyers == null || items == null || orders == null)
                throw new ArgumentNullException("Один или несколько списков пусты.");

            XDocument xmlDoc = new XDocument(
                new XElement("root",
                    new XElement("buyers",
                        buyers.Select(b => new XElement("buyer",
                            new XAttribute("id", b.id),
                            new XAttribute("name", b.name),
                            new XAttribute("address", b.address),
                            new XAttribute("tel", b.tel),
                            new XAttribute("person", b.person)
                        ))
                    ),
                    new XElement("items",
                        items.Select(i => new XElement("item",
                            new XAttribute("id", i.id),
                            new XAttribute("name", i.name),
                            new XAttribute("value", i.value),
                            new XAttribute("description", i.description),
                            new XAttribute("have", i.check)
                        ))
                    ),
                    new XElement("orders",
                        orders.Select(o => new XElement("order",
                            new XAttribute("id", o.id),
                            new XAttribute("buyer_id", o.buyer.id),
                            new XAttribute("item_id", o.item.id),
                            new XAttribute("date", o.date.ToString("yyyy.MM.dd HH:mm:ss"))
                        ))
                    )
                )
            );

            try
            {
                xmlDoc.Save(filePath);
                Console.WriteLine("Данные успешно записаны в XML-файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи файла: {ex.Message}");
            }
        }
    }
}
