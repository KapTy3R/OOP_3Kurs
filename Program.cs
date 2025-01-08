using System.Net;
using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Globalization;
using Microsoft.Win32;
using System.Windows.Markup;


namespace Task1
{
    class Program
    {

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            List<Buyers> All_buyers = new List<Buyers>();
            List<Items> All_items = new List<Items>();
            List<Orders> All_orders = new List<Orders>();
            string temp, pathFiles;


            do
            {
                Console.WriteLine("\nЧто хотите сделать?\n1. Ввести покупателя\n2. Ввести продукт\n3. Ввести заказ\n4. Показать всех покупателей\n5. Показать все продукты\n6. Показать все заказы\n7. Удалить покупателя\n8. Удалить продукт\n9. Удалить заказ\n0. Загрузить данные из файла\n-.Сохранить всё в отдельный файл\n+. Выход");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                temp = $"{keyInfo.Key}";
                switch (temp)
                {
                    case "D1": Console.WriteLine("\nВведите информацию по покупателю через запятую, в следующем формате: id, название, адрес, телефон, имя контактного лица:"); temp = Console.ReadLine(); try { var add = Lists.Enter(temp, All_buyers); All_buyers.Add(add); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                    case "D2":
                        Console.WriteLine("\nВведите информацию по продукту через запятую, в следующем формате: id, название, стоимость, описание:"); temp = Console.ReadLine(); bool have = false;
                    R: Console.WriteLine("\nУ этого товара есть доставка?(Y/N)");
                        keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Y) have = true;
                        else if (keyInfo.Key == ConsoleKey.N) have = false;
                        else goto R;
                        try { if (string.IsNullOrEmpty(temp)) throw new ArgumentException("Неправильный набор данных"); var add = Lists.Enter(temp, have, All_items); All_items.Add(add); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                    case "D3":
                        Console.WriteLine("\nВведите информацию по заказу через запятую, в следующем формате: id сделки, id клиента"); temp = Console.ReadLine(); string dateFormat = "yyyy.MM.dd HH:mm:ss"; // Формат даты
                        DateTime parsedDate;
                        while (true)
                        {
                            Console.WriteLine($"\nВведите дату и время в формате \"{dateFormat}\":");
                            string input = Console.ReadLine();

                            // Проверка ввода
                            if (!DateTime.TryParseExact(input, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                Console.WriteLine("\nНеверный формат. Попробуйте снова.\n");
                            else { break; };
                        }
                        while (true)
                        {
                            Console.WriteLine($"\nВведите продукты через запятую, в следующем формате: id продукта - количество");

                            string products = Console.ReadLine();
                            if (string.IsNullOrEmpty(products) && products.Count(c => c == '-') == 0)
                            {
                                Console.WriteLine("\nНеверный формат. Попробуйте снова.\n");
                            }
                            else try { if (string.IsNullOrEmpty(temp)) throw new ArgumentException("Неправильный набор данных"); var add = Lists.Enter(temp, products, parsedDate, All_buyers, All_items, All_orders); All_orders.Add(add); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                        }
                        break;
                    case "D4": Console.WriteLine(Lists.Outs(All_buyers)); break;
                    case "D5": Console.WriteLine(Lists.Outs(All_items)); break;
                    case "D6": Console.WriteLine(Lists.Outs(All_orders)); break;
                    case "D7": Console.WriteLine("\nВведите id покупателя, который нужно удалить:"); temp = Console.ReadLine(); try { if (string.IsNullOrEmpty(temp)) throw new ArgumentException("Неправильный набор данных"); int tempint = Convert.ToInt32(temp); Lists.Del(ref All_buyers, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    case "D8": Console.WriteLine("\nВведите id продукта, который нужно удалить:"); temp = Console.ReadLine(); try { if (string.IsNullOrEmpty(temp)) throw new ArgumentException("Неправильный набор данных"); int tempint = Convert.ToInt32(temp); Lists.Del(ref All_items, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    case "D9": Console.WriteLine("\nВведите id заказа, который нужно удалить:"); temp = Console.ReadLine(); try { if (string.IsNullOrEmpty(temp)) throw new ArgumentException("Неправильный набор данных"); int tempint = Convert.ToInt32(temp); Lists.Del(ref All_orders, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    case "D0":
                    L1: Console.WriteLine("\nВ какой тип файлов загрузить данные?\n1-SQL; 2 - XML; 3 - JSON; + - Отмена");
                        keyInfo = Console.ReadKey();
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.D1: try { SqlData sqlData = new SqlData(); sqlData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.D2: try { XmlData xmlData = new XmlData(); xmlData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.D3: try { JsonData JsonData = new JsonData(); JsonData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.OemPlus: break;
                            default: goto L1;
                        }
                        break;
                    case "OemMinus":
                    L2: Console.WriteLine("\nВ какой тип файлов загрузить данные?\n1-SQL; 2 - XML; 3 - JSON; + - Отмена");
                        keyInfo = Console.ReadKey();
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.D1: try { SqlData sqlData = new SqlData(); sqlData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.D2: try { XmlData xmlData = new XmlData(); xmlData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.D3: try { JsonData JsonData = new JsonData(); JsonData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                            case ConsoleKey.OemPlus: break;
                            default: goto L2;
                        }
                        break;
                    case "OemPlus": Environment.Exit(0); break;
                    default: Console.WriteLine($"\nНеправильный выбор\n"); break;
                };
            } while (true);

        }
    }
}
