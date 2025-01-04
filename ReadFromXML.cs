
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;

namespace Task1
{
    public class ReadFromXML
    {
        public string SelectXmlFile()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Выберите XML-файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
        public void LoadXmlFile(string filePath) {
            XDocument xmlDoc = new XDocument();
            try
            {
                xmlDoc = XDocument.Load(filePath);
            }
            catch { throw new ArgumentException("\nЧто-то пошло не так при загрузке файла.\n"); };

            buyers = xmlDoc.Descendants("buyers").Select(b => new Buyers((int)b.Attribute("id"), (string)b.Attribute("name"), (string)b.Attribute("address"), (string)b.Attribute("tel"), (string)b.Attribute("person"))).ToList();
            items = xmlDoc.Descendants("items").Select(b => new Items((int)b.Attribute("id"), (string)b.Attribute("name"), (int)b.Attribute("value"), (string)b.Attribute("description"), (bool)b.Attribute("have"))).ToList();
            orders = xmlDoc.Descendants("orders").Select(b => new Orders((int)b.Attribute("id"), FindHeap.FindElementUsingHeap(buyers, (int)b.Attribute("buyer_id")), FindHeap.FindElementUsingHeap(items, (int)b.Attribute("item_id")), (int)b.Attribute("quantity"), (DateTime)b.Attribute("date"))).ToList();
        }
    
    }
}