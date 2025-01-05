using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace Task1
{
    internal class XmlData : Data
    {
        private string filePath;

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
            buyers = xmlDoc.Descendants("buyer").Select(b => new Buyers(
                (int)b.Attribute("id"),
                (string)b.Attribute("name"),
                (string)b.Attribute("address"),
                (string)b.Attribute("tel"),
                (string)b.Attribute("person")
            )).ToList();

            // Чтение товаров
            items = xmlDoc.Descendants("item").Select(i => new Items(
                (int)i.Attribute("id"),
                (string)i.Attribute("name"),
                (int)i.Attribute("value"),
                (string)i.Attribute("description"),
                (bool)i.Attribute("have")
            )).ToList();

            // Чтение заказов
            orders = xmlDoc.Descendants("order").Select(o => new Orders(
                (int)o.Attribute("id"),
                (Buyers)o.Attribute("buyer_id"),
                (Items)o.Attribute("item_id"),
                DateTime.Parse((string)o.Attribute("date"))
            )).ToList();

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
