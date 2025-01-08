using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Task1
{
    internal class Lists
    {

        public static void CheckId<T>(List<T> list, int id) where T : General
        {
            foreach (var item in list)
            {
                if (item.id == id) throw new ArgumentException("Объект с таким ID уже есть");
            }
        }

        public static void Del(ref List<Buyers> buyers, int id)
        {
            try
            {
                buyers.Remove(FindHeap.FindElementUsingHeap(buyers, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
        }

        public static void Del(ref List<Items> items, int id)
        {
            try
            {
                items.Remove(FindHeap.FindElementUsingHeap(items, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
        }

        public static void Del(ref List<Orders> orders, int id)
        {
            try
            {
                orders.Remove(FindHeap.FindElementUsingHeap(orders, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
        }

        public static string RemoveLeadingSpace(string input)
        {
            if (!string.IsNullOrEmpty(input) && input[0] == ' ')
            {
                return input.Substring(1);
            }
            return input;
        }

        public static string Outs(List<Buyers> buyers)
        {
            string tempStr = "";
            foreach (var num in buyers)
                try
                {
                    if (num != null)
                        tempStr += $"\n{num.id}. {num.name}, {num.address}, {num.tel}, {num.person};";
                    else return tempStr = $"Что-то пошло не так";
                }
                catch { return tempStr = "Что-то пошло не так"; }
            return tempStr;
        }

        public static string Outs(List<Items> items)
        {
            string tempStr = "";
            foreach (var num in items)
                try
                {
                    if (num != null)
                        tempStr += $"\n{num.id}. {num.name}, {num.value}, {num.description};";
                    else return tempStr = $"Что-то пошло не так";
                }
                catch { return tempStr = "Что-то пошло не так"; }
            return tempStr;
        }

        public static string Outs(List<Orders> orders)
        {
            string tempStr = "";
            foreach (var num in orders)
                try
                {
                    if (num != null && num.buyer != null) { 
                        string str = "";
                        foreach (var items in num.items) { str += $"{items.Item}x{items.Quantity}; "; };
                        tempStr += $"\n{num.id}. {num.buyer.id} - {str} | Общее кол-во:{num.items_quantity} Общая цена:{num.total_value} ({num.date});";
                    }
                    else return tempStr = $"Что-то пошло не так";
                }
                catch { return tempStr = "Что-то пошло не так"; }
            return tempStr;
        }

        public static Buyers Enter(string temp, List<Buyers> buyers)
        {
            string[] str = temp.Split(',');
            if (str.Length != 5) throw new ArgumentException("Неправильный набор данных");
            else
            {
                for (int i = 0; i < str.Length; i++)
                    str[i] = RemoveLeadingSpace(str[i]);
                CheckId(buyers, Convert.ToInt32(str[0]));
                Buyers buyer = new Buyers(Convert.ToInt32(str[0]), str[1], str[2], str[3], str[4]);
                return buyer;
            }
        }

        public static Items Enter(string temp, bool have, List<Items> items)
        {
            string[] str = temp.Split(',');
            if (str.Length != 4) throw new ArgumentException("Неправильный набор данных");
            else
            {
                for (int i = 0; i < str.Length; i++)
                    str[i] = RemoveLeadingSpace(str[i]);
                CheckId(items, Convert.ToInt32(str[0]));
                Items item = new Items(Convert.ToInt32(str[0]), str[1], Convert.ToInt32(str[2]), str[3], have);
                return item;
            }
        }

        public static Orders Enter(string temp, string items, DateTime date, List<Buyers> All_buyers, List<Items> All_items, List<Orders> orders)
        {
            List<(Items Item, int Quantity)> all_its = [];
            string[] str = temp.Split(',');
            string[] its = items.Split(',');
            if (str.Length != 2) throw new ArgumentException("Неправильный набор данных");
            else
            {
                for (int i = 0; i < str.Length; i++)
                    str[i] = RemoveLeadingSpace(str[i]);
                for (int i = 0; i < its.Length; i++)
                    its[i] = RemoveLeadingSpace(its[i]);
                Buyers buyer = FindHeap.FindElementUsingHeap(All_buyers, Convert.ToInt32(str[1]));


                foreach (var i in its)
                {
                    var temp_i = i.Split("- ");
                    try
                    {
                        all_its.Add((FindHeap.FindElementUsingHeap(All_items, Convert.ToInt32(RemoveLeadingSpace(temp_i[0]))), Convert.ToInt32(RemoveLeadingSpace((temp_i[1])))));
                    }
                    catch { throw new ArgumentException("Не удалось обнаружить продукт\n"); }
                }

                //Items item = FindHeap.FindElementUsingHeap(All_items, Convert.ToInt32(str[2]));
                if (buyer == null || all_its == null)
                {
                    if (buyer == null) throw new ArgumentException("Покупатель не найден\n");
                    if (all_its == null) throw new ArgumentException("Продукт не найден\n");
                }
                CheckId(orders, Convert.ToInt32(str[0]));
                Orders order = new Orders(Convert.ToInt32(str[0]), buyer, all_its, Convert.ToDateTime(date));
                return order;
            }
        }

    }
}