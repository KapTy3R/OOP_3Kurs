using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Task4_2
{
    /// <summary>
    /// Логика взаимодействия для OrderInfo.xaml
    /// </summary>
    /// 
    public class ItemsQuant : General
    {
        public int value { get; }
        public string description { get; }
        public bool check { get; }
        private int _quantity;
        public int quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(quantity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ItemsQuant(int id, string name, int value, string description, bool check, int quantity) : base(id, name)
        {
            this.id = id; this.name = name; this.value = value; this.description = description; this.check = check; this.quantity = quantity;
        }

        public override string ToString()
        {
            return $"{id}: {name} x {quantity}";
        }
    }


    public partial class OrderInfo : Page
    {
        private MainWindow _mainWindow;
        Orders Order { get; set; }
        public OrderInfo(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            SaveButton.IsEnabled = false;
            DelOrCancelButton.IsEnabled = false;
            buyerComboBox.ItemsSource = mainWindow.All_buyers;
            items.ItemsSource = GetItemsQuants(mainWindow.All_items);
            DelOrCancelButton.IsEnabled = true;
            SaveButton.IsEnabled = true;
        }


        private List<ItemsQuant> GetItemsQuants(List<Items> items) {
            List<ItemsQuant> itemsQuants = new List<ItemsQuant>();
            foreach (var item in items)
            {
                ItemsQuant temp = new ItemsQuant(item.id, item.name, item.value, item.description, item.check, 0);
                itemsQuants.Add(temp);
            }
            return itemsQuants;
        }

        private List<ItemsQuant> GetItemsQuants(List<Items> items, Orders orders)
        {
            List<ItemsQuant> itemsQuants = new List<ItemsQuant>();
            List<(Items Item, int Quantity)> its = orders.items;
            foreach (var item in items)
            {
                ItemsQuant temp;
                int t=its.FindIndex(o => o.Item.id == item.id);
                if (t != -1)
                temp = new ItemsQuant(item.id, item.name, item.value, item.description, item.check, its[t].Quantity);
                else temp = new ItemsQuant(item.id, item.name, item.value, item.description, item.check, 0);
                itemsQuants.Add(temp);
            }
            return itemsQuants;
        }

        /*
        private List<ItemsQuant> GetItemsQuants(List<ItemsQuant> itemsQuants, Orders orders)
        {
            List<ItemsQuant> itemsQuants2 = new List<ItemsQuant>();
            List<(Items Item, int Quantity)> its = orders.items;
            foreach (var item in itemsQuants)
            {
                ItemsQuant temp;
                int t = its.FindIndex(o => o.Item.id == item.id);
                if (t != -1) { 
                temp = new ItemsQuant(item.id, item.name, item.value, item.description, item.check, its[t].Quantity);
                itemsQuants2.Add(temp);
                }
            }
            return itemsQuants2;
        }
        */

        public OrderInfo(Orders order, MainWindow mainWindow) : this(mainWindow)
        {
            buyerComboBox.SelectedValue = order.buyer;
            total_value.Text = $"{order.total_value}";
            date.Text = $"{order.date:dd.MM.yyyy HH:mm:ss}";
            var allItems= GetItemsQuants(mainWindow.All_items, order);
            items.ItemsSource = allItems;
            var selectedItems = order.items;
            //items.SelectedItems.Clear();
            foreach (var item in selectedItems)
            {
                var t = allItems.Find(o => o.id == item.Item.id);
                items.SelectedItems.Add(t);
            }
            this.Order = order;
        }



        /*
                public OrderInfo(Orders order, MainWindow mainWindow) : this(mainWindow)
                {
                    buyerComboBox.SelectedValue = order.buyer;
                    total_value.Text = $"{order.total_value}";
                    //foreach (var i in order.items) {
                    //var selectedItems = mainWindow.All_items.Where(item => item.id == i.Item.id).ToList();
                    var selectedItems = order.items;
                    items.ItemsSource = mainWindow.All_items;
                    foreach (var item in selectedItems)
                    {
                        items.SelectedItems.Add(item);
                    }
                    //}
                    date.Text = $"{order.date}";
                    this._mainWindow = mainWindow;
                    this.Order = order;
                }*/

        /*
        public OrderInfo(Orders order, MainWindow mainWindow) : this(mainWindow)
        {
            buyerComboBox.SelectedValue = order.buyer;
            total_value.Text = $"{order.total_value}";

            var selectedItems = order.items.Select(i => new
            {
                OriginalItem = mainWindow.All_items,
                Quantity = i.Quantity
            }).ToList();

            items.ItemsSource = selectedItems;

            foreach (var item in selectedItems)
            {
                items.SelectedItems.Add(item);
            }

            date.Text = $"{order.date}";
            this._mainWindow = mainWindow;
            this.Order = order;
        }*/


        private void Save(object sender, RoutedEventArgs e)
        {
            if (Order == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Нет текущего заказа для сохранения.");
                return;
            }

            // Проверяем дату
            string nd = $"{date.Value}";
            var n = nd.Split(' ');
            if (n[1].Length == 7) nd = n[0] + " 0" + n[1];
            if (!DateTime.TryParseExact(nd, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Неверный формат даты. Используйте формат dd.MM.yyyy HH:mm:ss.");
                return;
            }

            // Проверяем выбранного покупателя
            Buyers selectedBuyer = buyerComboBox.SelectedItem as Buyers;
            if (selectedBuyer == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите покупателя.");
                return;
            }

            // Получаем выбранные элементы с количеством
            var selectedItems = items.SelectedItems.Cast<ItemsQuant>().ToList();
            if (selectedItems.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите хотя бы один товар.");
                return;
            }

            // Обновляем список товаров с количеством
            var updatedItems = selectedItems
                .Where(item => item.quantity > 0)
                .Select(item => (new Items(item.id, item.name, item.value, item.description, item.check), item.quantity))
                .ToList();

            if (updatedItems.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Указано количество 0 для всех товаров. Укажите корректное количество.");
                return;
            }

            // Создаем новый объект заказа с обновленными данными
            Orders updatedOrder = new Orders(
                Order.id,
                selectedBuyer,
                updatedItems,
                newDate
            );

            // Заменяем старый заказ в списке
            int index = _mainWindow.All_orders.FindIndex(o => o.id == Order.id);
            if (index >= 0)
            {
                _mainWindow.All_orders[index] = updatedOrder;
                Xceed.Wpf.Toolkit.MessageBox.Show("Заказ успешно сохранен.");
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Ошибка: заказ не найден.");
                return;
            }

            // Обновляем страницу
            _mainWindow.UpdatePage();
            _mainWindow.OrdersInfoFrame.Content = new OrderInfo(_mainWindow);

            // Сбрасываем текущий заказ
            SaveButton.IsEnabled = false;
            this.Order = null;
        }






        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is int id)
            {
                // Проверяем, что введенное значение является числом и больше или равно 0
                if (int.TryParse(textBox.Text, out int quantity) && quantity >= 0)
                {
                    // Проходим по всем элементам, чтобы найти нужный
                    foreach (var item in items.ItemsSource)
                    {
                        if (item is ItemsQuant quant && quant.id == id)
                        {
                            quant.quantity = quantity;
                            break;
                        }
                    }
                }
                else
                {
                    // Если значение некорректное, сбрасываем его в 0
                    textBox.Text = "0";
                }
            }
        }


        /*
        private List<(int Id, int Quantity)> GetSelectedItemsWithQuantities()
        {
            var selectedItems = items.SelectedItems;
            var result = new List<(int Id, int Quantity)>();

            foreach (var selectedItem in selectedItems)
            {
                var id = (int)selectedItem.GetType().GetProperty("id").GetValue(selectedItem);
                if (itemQuantities.TryGetValue(id, out int quantity))
                {
                    result.Add((id, quantity));
                }
                else
                {
                    result.Add((id, 0)); // Если Quantity не указано, используем значение по умолчанию (например, 0).
                }
            }

            return result;
        }
        */


        private void Delete(object sender, RoutedEventArgs e)
        {
            List<Orders> new_orders = _mainWindow.All_orders;
            new_orders.Remove(Order);
            _mainWindow.OrdersInfoFrame.Content = new OrderInfo(_mainWindow);
            _mainWindow.UpdatePage();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            // Проверяем дату
            string nd = $"{date.Value}";
            var n = nd.Split(' ');
            if (n[1].Length == 7) nd = n[0] + " 0" + n[1];
            if (!DateTime.TryParseExact(nd, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Неверный формат даты. Используйте формат dd.MM.yyyy HH:mm:ss.");
                return;
            }

            // Проверяем выбранного покупателя
            Buyers selectedBuyer = buyerComboBox.SelectedItem as Buyers;
            if (selectedBuyer == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите покупателя.");
                return;
            }

            // Получаем выбранные элементы с количеством
            var selectedItems = items.SelectedItems.Cast<ItemsQuant>().ToList();
            if (selectedItems.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите хотя бы один товар.");
                return;
            }

            // Создаем список товаров с количеством
            var newItems = selectedItems
                .Where(item => item.quantity > 0)
                .Select(item => (new Items(item.id, item.name, item.value, item.description, item.check), item.quantity))
                .ToList();

            if (newItems.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Указано количество 0 для всех товаров. Укажите корректное количество.");
                return;
            }

            // Создаем новый заказ
            int newOrderId = _mainWindow.All_orders.Any() ? _mainWindow.All_orders.Max(o => o.id) + 1 : 1;
            Orders newOrder = new Orders(newOrderId, selectedBuyer, newItems, newDate);
            _mainWindow.All_orders.Add(newOrder);

            Xceed.Wpf.Toolkit.MessageBox.Show("Новый заказ успешно добавлен.");

            // Обновляем страницу
            _mainWindow.OrdersInfoFrame.Content = new OrderInfo(_mainWindow);
            _mainWindow.UpdatePage();

            // Сбрасываем текущий заказ
            SaveButton.IsEnabled = false;
            this.Order = null;
        }



    }
}
//}
