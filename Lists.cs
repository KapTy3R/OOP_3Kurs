using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    internal class Lists
    {
        public static void Del(List<Buyers> buyers, int id)
        {
            try
            {
                buyers.Remove(FindHeap.FindElementUsingHeap(buyers, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
        }

        public static void Del(List<Items> items, int id)
        {
            try
            {
                items.Remove(FindHeap.FindElementUsingHeap(items, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
        }

        public static void Del(List<Orders> orders, int id)
        {
            try
            {
                orders.Remove(FindHeap.FindElementUsingHeap(orders, id));
                Console.WriteLine("\nУспешно удалено\n");
            }
            catch { Console.WriteLine("\nНевозможно удалить данный элемент\n"); };
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
                    if (num != null && num.buyer != null && num.item != null)
                        tempStr += $"\n{num.id}. {num.buyer.id} - {num.item.id}x{num.item_quantity} - {num.item.check} ({num.date});";
                    else return tempStr = $"Что-то пошло не так";
                }
                catch { return tempStr = "Что-то пошло не так"; }
            return tempStr;
        }

        public static Buyers Enter(string temp)
        {
            string[] str = temp.Split(',');
            if (str.Length != 5) throw new ArgumentException("Неправильный набор данных");
            else
            {
                Buyers buyer = new Buyers(Convert.ToInt32(str[0]), str[1], str[2], str[3], str[4]);
                return buyer;
            }
        }

        public static Items Enter(string temp, bool have)
        {
            string[] str = temp.Split(',');
            if (str.Length != 4) throw new ArgumentException("Неправильный набор данных");
            else
            {
                Items item = new Items(Convert.ToInt32(str[0]), str[1], Convert.ToInt32(str[2]), str[3], have);
                return item;
            }
        }

        public static Orders Enter(string temp, DateTime date, List<Buyers> All_buyers, List<Items> All_items)
        {
            string[] str = temp.Split(',');
            if (str.Length != 4) throw new ArgumentException("Неправильный набор данных");
            else
            {
                Buyers buyer = FindHeap.FindElementUsingHeap(All_buyers, Convert.ToInt32(str[1]));
                Items item = FindHeap.FindElementUsingHeap(All_items, Convert.ToInt32(str[2]));
                if (buyer == null || item == null)
                {
                    if (buyer == null) throw new ArgumentException("Покупатель не найден\n");
                    if (item == null) throw new ArgumentException("Продукт не найден\n");
                }
                Orders order = new Orders(Convert.ToInt32(str[0]), buyer, item, Convert.ToInt32(str[3]), Convert.ToDateTime(date));
                return order;
            }
        }

    }
}