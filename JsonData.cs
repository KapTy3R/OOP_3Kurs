using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace Task1
{
    internal class JsonData : Data
    {
        private string filePath;

        public JsonData() : base()
        {
            this.filePath = base.filePath;
        }

        // Чтение данных из JSON
        public override void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден");

            string json = File.ReadAllText(filePath);
            dynamic jsonData = JsonConvert.DeserializeObject(json);

            // Чтение покупателей
            List<Buyers> buy = new List<Buyers>();
            foreach (var buyerData in jsonData.buyers)
            {
                buy.Add(Lists.Enter($"{buyerData.id},{buyerData.name},{buyerData.address},{buyerData.tel}, {buyerData.person}", buy));
            }
            buyers.Clear();
            buyers = new List<Buyers>(buy);
            buy.Clear();

            // Чтение товаров
            List<Items> it = new List<Items>();
            foreach (var itemData in jsonData.items)
            {
                it.Add(Lists.Enter($"{itemData.id},{itemData.name},{itemData.value},{itemData.description}", itemData.have, it));
            }
            items.Clear();
            items = new List<Items>(it);
            it.Clear();

            // Чтение заказов
            List<Orders> or = new List<Orders>();
            foreach (var orderData in jsonData.orders)
            {
                or.Add(Lists.Enter($"{orderData.id},{orderData.buyer_id},{orderData.item_id},{orderData.quantity}", orderData.date, buyers, items, or));
            }
            orders.Clear();
            orders = new List<Orders>(or);
            or.Clear();

            Console.WriteLine("Данные успешно прочитаны из JSON-файла.");
        }

        // Запись данных в JSON
        public override void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders)
        {
            if (buyers == null || items == null || orders == null)
                throw new ArgumentNullException("Один или несколько списков пусты.");

            var jsonData = new
            {
                buyers = buyers,
                items = items,
                orders = orders
            };

            try
            {
                string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Данные успешно записаны в JSON-файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи файла: {ex.Message}");
            }
        }
    }
}
