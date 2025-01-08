using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Windows;
using Task1;

namespace Task1
{
    internal class XmlData : Data
    {
        List<string> str;
        XDocument xmlDoc = new XDocument();
        string filePath;

        public XmlData() : base()
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

            var buyersElements = xmlDoc.Element("root")?.Elements("buyers").Select(b => new
            {
                Id = b.Attribute("id") != null ? (int)b.Attribute("id") : 0,
                Name = b.Attribute("name") != null ? (string)b.Attribute("name") : string.Empty,
                Address = b.Attribute("address") != null ? (string)b.Attribute("address") : string.Empty,
                Tel = b.Attribute("tel") != null ? (string)b.Attribute("tel") : string.Empty,
                Person = b.Attribute("person") != null ? (string)b.Attribute("person") : string.Empty
            }).ToList();

            foreach (var buyer in buyersElements)
            {
                buy.Add(Lists.Enter($"{buyer.Id},{buyer.Name},{buyer.Address},{buyer.Tel},{buyer.Person}", buy));
            }
            buyers.Clear();
            buyers = new List<Buyers>(buy);
            buy.Clear();

            // Чтение товаров
            List<Items> it = new List<Items>();
            var itemsElements = xmlDoc.Element("root")?.Elements("items").Select(b => new
            {
                Id = b.Attribute("id") != null ? (int)b.Attribute("id") : 0,
                Name = b.Attribute("name") != null ? (string)b.Attribute("name") : string.Empty,
                Value = b.Attribute("value") != null ? (int)b.Attribute("value") : 0,
                Description = b.Attribute("description") != null ? (string)b.Attribute("description") : string.Empty,
                Have = b.Attribute("have") != null ? (bool)b.Attribute("have") : false
            }).ToList();

            foreach (var item in itemsElements)
            {
                it.Add(Lists.Enter($"{item.Id},{item.Name},{item.Value},{item.Description}", item.Have, it));
            }
            items.Clear();
            items = new List<Items>(it);
            it.Clear();

            // Чтение заказов
            List<Orders> or = new List<Orders>();
            //var itemsElementsOr = xmlDoc.Element("root")?.Element("orders")?.Elements("items");
            var ordersElements = xmlDoc.Element("root")?.Elements("orders").Select(b => new
            {
                Id = b.Attribute("id") != null ? (int)b.Attribute("id") : throw new ArgumentException("В одном из заказов отсутствует ID"),
                Buyer_id = b.Attribute("buyer_id") != null ? (int)b.Attribute("buyer_id") : throw new ArgumentException("В одном из заказов отсутствует ID клиента"),
                Date = b.Attribute("date") != null ? (DateTime)b.Attribute("date") : throw new ArgumentException("В одном из заказов отсутствует время"),
                items = b?.Elements("items")?.Elements("item").Select(j => new {
                    Item_id = j.Attribute("id") != null ? (string)j.Attribute("id") : throw new ArgumentException("В одном из продуктов отсутствует ID"),
                    Quantity = j.Attribute("quantity") != null ? (string)j.Attribute("quantity") : throw new ArgumentException("В одном из продуктов отсутствует количество"),
                }).ToList()
            }).ToList();


            //var itemsElements = xmlDoc.Element("root")?.Element("orders")?.Elements("items");

            foreach (var orderElement in ordersElements)
            {

                // Сборка строки с информацией о заказе
                string orderInfo = $"{orderElement.Id},{orderElement.Buyer_id}"; // Строка для "id заказа, id покупателя"

                // Сборка строки с информацией о продуктах
                /*
                .Select(item => $"{(item.Attribute("item_id") != null ? (int)item.Attribute("item_id") : 0)} - {(item.Attribute("quantity") != null ? (int)item.Attribute("quantity") : 0)}")
                .ToList();*/
                string productInfo = "";
                foreach (var t in orderElement.items)
                {
                    productInfo += $"{t.Item_id} - {t.Quantity}, "; // Строка для "id продукта1 - количество1, id продукта2 - количество2"   
                }
                productInfo = productInfo.Substring(0, productInfo.Length - 2);

                // Добавление заказа в список
                or.Add(Lists.Enter(orderInfo, productInfo, orderElement.Date, buyers, items, or));
            }

            orders.Clear();
            orders = new List<Orders>(or);
            or.Clear();

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
                            new XAttribute("date", o.date.ToString("yyyy.MM.dd HH:mm:ss")),
                            new XElement("items",
                            o.items.Select(oi => new XElement("item",
                                    new XAttribute("id", oi.Item.id),
                                    new XAttribute("quantity", oi.Quantity))
                            )
                        )
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
