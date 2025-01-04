using System.Net;
using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Globalization;
using Microsoft.Win32;


namespace Task1
{
    class Program {

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            List<Buyers> All_buyers = new List<Buyers>();
            List<Items> All_items = new List<Items>();
            List<Orders> All_orders = new List<Orders>();
            string temp;


            do
            {
                Console.WriteLine("\nЧто хотите сделать?\n1. Ввести покупателя\n2. Ввести продукт\n3. Ввести заказ\n4. Показать всех покупателей\n5. Показать все продукты\n6. Показать все заказы\n7. Удалить покупателя\n8. Удалить продукт\n9. Удалить заказ\n0. Выйти");
                ConsoleKeyInfo keyInfo=Console.ReadKey();
                temp = $"{keyInfo.Key}".Remove(0,1);
                if (!int.TryParse(temp, out int result))
                {
                    Console.WriteLine("Неправильная кнопка\n");
                }

                switch (Convert.ToInt32(temp))
            {
                case 1: Console.WriteLine("\nВведите информацию по покупателю через запятую, в следующем формате: id, название, адрес, телефон, имя контактного лица:"); temp = Console.ReadLine(); try { var add = Lists.Enter(temp, All_buyers); All_buyers.Add(add); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break;
                case 2: Console.WriteLine("\nВведите информацию по продукту через запятую, в следующем формате: id, название, стоимость, описание:"); temp = Console.ReadLine(); bool have = false;
                     R: Console.WriteLine("\nЭтот товар есть в наличии?(Y/N)");
                        keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Y) have = true;
                        else if (keyInfo.Key == ConsoleKey.N) have = false;
                        else goto R;
                        try {var add=Lists.Enter(temp, have, All_items);  All_items.Add(add); } catch(ArgumentException ex) {Console.WriteLine($"{ex.Message}"); }; break;
                case 3: Console.WriteLine("\nВведите информацию по заказу через запятую, в следующем формате: id сделки, id клиента, id товара, количество товара:"); temp = Console.ReadLine(); string dateFormat = "yyyy.MM.dd HH:mm:ss"; // Формат даты
                    DateTime parsedDate;
                    while (true)
                    {
                        Console.WriteLine($"\nВведите дату и время в формате \"{dateFormat}\":");
                        string input = Console.ReadLine();

                        // Проверка ввода
                        if (!DateTime.TryParseExact(input, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            Console.WriteLine("\nНеверный формат. Попробуйте снова.\n");
                        else { try { var add = Lists.Enter(temp, parsedDate, All_buyers, All_items, All_orders); All_orders.Add(add); } catch (ArgumentException ex) { Console.WriteLine($"{ex.Message}"); }; break; }
   
                    } break;
                    case 4: Console.WriteLine(Lists.Outs(All_buyers)); break;
                    case 5: Console.WriteLine(Lists.Outs(All_items)); break;
                    case 6: Console.WriteLine(Lists.Outs(All_orders)); break;
                    case 7: Console.WriteLine("\nВведите id покупателя, который нужно удалить:"); temp=Console.ReadLine(); try { int tempint = Convert.ToInt32(temp); Lists.Del(ref All_buyers, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    case 8: Console.WriteLine("\nВведите id продукта, который нужно удалить:"); temp = Console.ReadLine(); try { int tempint = Convert.ToInt32(temp); Lists.Del(ref All_items, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    case 9: Console.WriteLine("\nВведите id заказа, который нужно удалить:"); temp = Console.ReadLine(); temp = Console.ReadLine(); try { int tempint = Convert.ToInt32(temp); Lists.Del(ref All_orders, tempint); } catch (ArgumentException) { Console.WriteLine("\nЧто-то пошло не так\n"); }; break;
                    default: Environment.Exit(0); break;
            }; 
        } while (true);

    }
}
}
