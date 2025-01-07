using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class OrderInfo : Page
    {
        private MainWindow _mainWindow;
        Orders Order { get; set; }
        private Dictionary<int, int> itemQuantities = new Dictionary<int, int>();
        public ObservableCollection<Items> ItemsCollection { get; set; }
        public OrderInfo(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            SaveButton.IsEnabled = false;
            DelOrCancelButton.IsEnabled = false;
            buyerComboBox.ItemsSource = mainWindow.All_buyers;
            ItemsCollection = new ObservableCollection<Items>(mainWindow.All_items);
            items.ItemsSource = ItemsCollection;
            DelOrCancelButton.IsEnabled = true;
            SaveButton.IsEnabled = true;
        }



        public OrderInfo(Orders order, MainWindow mainWindow) : this(mainWindow)
        {
            buyerComboBox.SelectedValue = order.buyer;
            total_value.Text = $"{order.total_value}";
            date.Text = $"{order.date:dd.MM.yyyy HH:mm:ss}";

            // Устанавливаем ItemsSource и выделяем выбранные элементы
            ItemsCollection = new ObservableCollection<Items>(mainWindow.All_items);
            items.ItemsSource = ItemsCollection;
            foreach (var selectedItem in order.items)
            {
                var matchingItem = mainWindow.All_items.FirstOrDefault(i => i.id == selectedItem.Item.id);
                if (matchingItem != null)
                {
                    items.SelectedItems.Add(matchingItem);
                }
                itemQuantities[selectedItem.Item.id] = selectedItem.Quantity;
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
            var selectedItemsWithQuantities = GetSelectedItemsWithQuantities();

            if (selectedItemsWithQuantities.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите хотя бы один товар.");
                return;
            }

            // Обновляем заказ
            List<(Items Item, int Quantity)> updatedItems = selectedItemsWithQuantities
                .Select(i => (Item: _mainWindow.All_items.First(item => item.id == i.Id), Quantity: i.Quantity))
                .ToList();

            // Обновляем данные заказа в списке
            int index = _mainWindow.All_orders.FindIndex(o => o.id == Order.id);
            if (index >= 0)
            {
                _mainWindow.All_orders[index] = new Orders(Order.id, selectedBuyer, updatedItems, newDate);
                Xceed.Wpf.Toolkit.MessageBox.Show("Заказ успешно сохранен.");
            }

            // Обновляем страницу
            _mainWindow.UpdatePage();
            _mainWindow.OrdersInfoFrame.Content = new OrderInfo(_mainWindow);

            // Очищаем текущий заказ
            this.Order = null;
            SaveButton.IsEnabled = false;
        }


        private List<string> ReadQuantities()
        {
            List<string> str=[];
            var selectedItems = items.SelectedItems;

            foreach (var selectedItem in selectedItems)
            {
                var id = (int)selectedItem.GetType().GetProperty("id").GetValue(selectedItem);
                var name = (string)selectedItem.GetType().GetProperty("name").GetValue(selectedItem);

                if (itemQuantities.TryGetValue(id, out int quantity))
                {
                    str.Add($"{id},{name},{quantity}");
                }
                else
                {
                    str.Add($"{id},{name}");
                }
            }
            return str;
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is int id)
            {
                if (int.TryParse(textBox.Text, out int quantity) && quantity >= 0)
                {
                    itemQuantities[id] = quantity;
                }
                else
                {
                    itemQuantities.Remove(id);
                    textBox.Text = "0"; // Установить 0, если значение невалидное
                }
            }
        }


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
            var selectedItemsWithQuantities = GetSelectedItemsWithQuantities();

            if (selectedItemsWithQuantities.Count == 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Выберите хотя бы один товар.");
                return;
            }

            // Создаем новый заказ
            int newOrderId = _mainWindow.All_orders.Max(o => o.id) + 1;
            List<(Items Item, int Quantity)> newItems = selectedItemsWithQuantities
                .Select(i => (Item: _mainWindow.All_items.First(item => item.id == i.Id), Quantity: i.Quantity))
                .ToList();

            Orders newOrder = new Orders(newOrderId, selectedBuyer, newItems, newDate);
            _mainWindow.All_orders.Add(newOrder);

            Xceed.Wpf.Toolkit.MessageBox.Show("Новый заказ успешно добавлен.");

            // Обновляем страницу
            _mainWindow.OrdersInfoFrame.Content = new OrderInfo(_mainWindow);
            _mainWindow.UpdatePage();

            // Очищаем текущий заказ
            SaveButton.IsEnabled = false;
            this.Order = null;
        }


    }
}
//}
