using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Task1
{
    internal abstract class Data
    {
        List<string> str;
        public string filepath { get; set; }
        XDocument xmlDoc = new XDocument();
        public Data()
        {
            Console.WriteLine("\nВведите полный путь к файлу:");
            string filePath = Console.ReadLine();
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) throw new ArgumentException("Такой файл не существует");
            else
            {
                this.filepath = file.FullName;
                this.xmlDoc = XDocument.Load(filePath);
            }
        }

        public abstract void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders);
        public abstract void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders);
    }
}
