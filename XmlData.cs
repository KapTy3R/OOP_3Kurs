using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace Task4_2
{
    internal class XmlData : Data
    {
        List<string> str;
        XDocument xmlDoc = new XDocument();
        string filePath;

        public XmlData(string selectedFiles) : base(selectedFiles)
        {
            this.filePath = base.filePath;
        }

        private void ValidateAndLoadFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) throw new ArgumentException("Такой файл не существует");
            this.filePath = file.FullName;
            this.xmlDoc = XDocument.Load(filePath);
        }

        public override void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders)
        {
            ValidateAndLoadFile(filePath);
            XDocument xmlDoc = XDocument.Load(filePath);

            // Чтение покупателей
            List<Buyers> buy = new List<Buyers>();

            str = xmlDoc.Descendants("buyers").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("name")}, {(string)b.Attribute("address")}, {(string)b.Attribute("tel")}, {(string)b.Attribute("person")}")).ToList();
            foreach (var s in str)
            {
                buy.Add(Lists.Enter(s, buy));
            }
            buyers.Clear();
            buyers = new List<Buyers>(buy);
            buy.Clear();
            str.Clear();


            // Чтение товаров
            List<Items> it = new List<Items>();
            str = xmlDoc.Descendants("items").Select(b => new string($"{(int)b.Attribute("id")}, {(string)b.Attribute("name")}, {(int)b.Attribute("value")}, {(string)b.Attribute("description")}, {(bool)b.Attribute("have")}")).ToList();
            foreach (var s in str)
            {
                string[] temp = s.Split(',');
                it.Add(Lists.Enter(s.Remove(s.Length - (temp[temp.Length - 1].Length + 1)), Convert.ToBoolean(temp[temp.Length - 1]), it));
            }
            items.Clear();
            items = new List<Items>(it);
            it.Clear();
            str.Clear();

            // Чтение заказов
            List<Orders> or = new List<Orders>();

            str = xmlDoc.Descendants("orders").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("buyer_id")},{(string)b.Attribute("item_id")}, {(string)b.Attribute("quantity")}, {(string)b.Attribute("date")}")).ToList();
            foreach (var s in str)
            {
                string[] temp = s.Split(',');
                or.Add(Lists.Enter(s.Remove(s.Length - (temp[temp.Length - 1].Length + 1)), DateTime.Parse(temp[temp.Length - 1]), buyers, items, or));
            }
            orders.Clear();
            orders = new List<Orders>(or);
            or.Clear();
            str.Clear();

            Console.WriteLine("Данные успешно прочитаны из XML-файла.");
        }

        public override void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders)
        {
            if (buyers == null || items == null || orders == null)
                throw new ArgumentNullException("Один или несколько списков пусты.");

            XDocument xmlDoc = new XDocument(
                new XElement("root",
                    buyers.Select(b => new XElement("buyers",
                            new XAttribute("id", b.id),
                            new XAttribute("name", b.name),
                            new XAttribute("address", b.address),
                            new XAttribute("tel", b.tel),
                            new XAttribute("person", b.person)
                        )
                    ),
                        items.Select(i => new XElement("items",
                            new XAttribute("id", i.id),
                            new XAttribute("name", i.name),
                            new XAttribute("value", i.value),
                            new XAttribute("description", i.description),
                            new XAttribute("have", i.check)
                        )
                    ),
                        orders.Select(o => new XElement("orders",
                            new XAttribute("id", o.id),
                            new XAttribute("buyer_id", o.buyer.id),
                            new XAttribute("item_id", o.item.id),
                            new XAttribute("quantity", o.item_quantity),
                            new XAttribute("date", o.date.ToString("yyyy.MM.dd HH:mm:ss"))
                        )
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
