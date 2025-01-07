using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Task4_2
{
    internal abstract class Data
    {
        List<string> str;
        public string filePath { get; set; }
        
        // Конструктор, запрашивающий путь через консоль
        public Data()
        {/*
            Console.WriteLine("\nВведите полный путь к файлу:");
            this.filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(this.filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.");*/
        }
        public Data(string path)
        {
            this.filePath = path;
            if (string.IsNullOrWhiteSpace(this.filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.");
            else { { MessageBox.Show("Успешно!", "Подтверждение", MessageBoxButton.OK, MessageBoxImage.Information); }; }
        }

        public abstract void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders);
        public abstract void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders);
    }
}
