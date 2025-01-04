
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;

namespace Task1
{
    internal class ReadFromXML
    {
        List<string> str;
        string filepath;
        XDocument xmlDoc = new XDocument();
        static List<Buyers> buy=new List<Buyers>();
        static List<Orders> or=new List<Orders>();
        static List<Items> it = new List<Items>();

        public ReadFromXML()
        {
            Console.WriteLine("\nВведите полный путь к файлу:");
            string filePath = Console.ReadLine();
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) throw new ArgumentException("Такой файл не существует");
            else { this.filepath = file.FullName; 
                this.xmlDoc = XDocument.Load(filePath);
            }
        }

        public ReadFromXML(ref List<Buyers> buyers, ref List<Items> items) : this() {
            buyers = new List<Buyers>();

            str = xmlDoc.Descendants("buyers").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("name")}, {(string)b.Attribute("address")}, {(string)b.Attribute("tel")}, {(string)b.Attribute("person")}")).ToList();
            foreach (var s in str)
            {
                buyers.Add(Lists.Enter(s, buyers));
            }
            buy=new List<Buyers>(buyers);
            buyers.Clear();

            items = new List<Items>();
            items = xmlDoc.Descendants("items").Select(b => new Items((int)b.Attribute("id"), (string)b.Attribute("name"), (int)b.Attribute("value"), (string)b.Attribute("description"), (bool)b.Attribute("have"))).ToList();
            it = new List<Items>(items);
            items.Clear();
            str.Clear();
        }

        public ReadFromXML(ref List<Orders> orders, ref List<Buyers> buyers, ref List<Items> items) : this(ref buyers, ref items)
        {
            orders = new List<Orders>();

            str = xmlDoc.Descendants("orders").Select(b => new string($"{(int)b.Attribute("id")},{(string)b.Attribute("buyer_id")},{(string)b.Attribute("item_id")}, {(string)b.Attribute("quantity")}, {(string)b.Attribute("date")}")).ToList();
            foreach (var s in str)
            {
                string[] temp = s.Split(',');
                orders.Add(Lists.Enter(s.Remove(s.Length - (temp[temp.Length-1].Length+1)), DateTime.Parse(temp[temp.Length-1]), buy, it, orders));
            }
            or = new List<Orders>(orders);
            orders.Clear();
            str.Clear();
        }

        public static void Read(ref List<Orders> orders, ref List<Buyers> buyers, ref List<Items> items) {
            orders = new List<Orders>(or); buyers = new List<Buyers>(buy); items = new List<Items>(it);
            Console.WriteLine("Успешно");
        }

    }
}