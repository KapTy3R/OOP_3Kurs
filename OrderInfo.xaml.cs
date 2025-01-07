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
    /// Логика взаимодействия для OrderInfo.xaml
    /// </summary>
    public partial class OrderInfo : Page
    {
        public OrderInfo()
        {
            InitializeComponent();
        }

        public OrderInfo(Orders orders) : this() {
            buyer.Text = $"{orders.buyer}";
            total_value.Text = $"{orders.total_value}";
            items.ItemsSource = orders.items;
            date.Text = $"{orders.date}";
        }
    }
}
