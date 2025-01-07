using System;
using System.Collections.Generic;
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

namespace Task4_2
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private MainWindow _mainWindow;
        public OrdersPage()
        {
            InitializeComponent();
        }

        public OrdersPage(List<Orders> orders, MainWindow mainWindow) : this()
        {
            listView.ItemsSource = orders;
            int sum = 0;
            foreach (Orders o in orders) {
                foreach (var zz in o.items) {
                    sum+=zz.Quantity;
                }
            }
            _mainWindow = mainWindow;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem is Orders selectedItem)
            {
                OrderInfo orderInfo = new OrderInfo(selectedItem);
                _mainWindow.ChangeSecondPage(orderInfo);
            }
        }
    }
}
