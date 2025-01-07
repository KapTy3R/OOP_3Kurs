using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Buyers> All_buyers = new List<Buyers>();
        public List<Items> All_items = new List<Items>();
        public List<Orders> All_orders = new List<Orders>();
        string temp;
        public MainWindow()
        {
            InitializeComponent();
            OrdersFrame.Content = new OrdersPage();
            BuyersFrame.Content = new BuyersPage();
            ItemsFrame.Content = new ItemsPage();
            OrdersInfoFrame.Navigate(new OrderInfo(this)); 
        }

        private int OpenDialog(ref string selectedFiles) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML-файлы (*.xml)|*.xml|SQLite-файлы (*.sqlite, *.db, *.sqlite3, *.db3, .sl3, .sq3)|*.sqlite; *.db; *.sqlite3; *.db3; .sl3; .sq3)|JSON-файлы (*.json)|*.json";
            openFileDialog.FilterIndex = 1;
            
            bool? result = openFileDialog.ShowDialog();
            if (result == true) {
                selectedFiles = openFileDialog.FileName;
                return openFileDialog.FilterIndex;
            } else return -1;
        }

        private int SaveDialog(ref string selectedFiles)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-файлы (*.xml)|*.xml|SQLite-файлы (*.sqlite, *.db, *.sqlite3, *.db3, .sl3, .sq3)|*.sqlite; *.db; *.sqlite3; *.db3; .sl3; .sq3)|JSON-файлы (*.json)|*.json";
            saveFileDialog.FilterIndex = 1;

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                selectedFiles = saveFileDialog.FileName;
                return saveFileDialog.FilterIndex;
            }
            else return -1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePage();
        }

        public void ChangeSecondPage(OrderInfo orderInfo) {
            OrdersInfoFrame.Content = orderInfo;
            OrdersInfoFrame.Navigate(orderInfo);
        }

        public void UpdateInfo()
        {
            switch (TabControl_Main.SelectedIndex)
            {
                case 0: OrdersInfoFrame.Navigate(new OrderInfo(this)); break;
                case 1: BuyersFrame.Navigate(new BuyersPage(All_buyers)); break;
                case 2: ItemsFrame.Navigate(new ItemsPage(All_items)); break;
            }
        }


        public void UpdatePage() {
            switch (TabControl_Main.SelectedIndex) {
                case 0: OrdersFrame.Navigate(new OrdersPage(All_orders, this));break;
                case 1: BuyersFrame.Navigate(new BuyersPage(All_buyers)); break;
                case 2: ItemsFrame.Navigate(new ItemsPage(All_items)); break;
            }
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            
            MessageBoxResult result= MessageBox.Show("Вы уверены, что хотите выйти?","Подтвердите выход",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) { this.Close(); }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            string selectedFiles = "";
            int key = SaveDialog(ref selectedFiles);
            if (key != -1)
            {
                switch (key)
                {
                    case 1: try { XmlData xmlData = new XmlData(selectedFiles); xmlData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                    case 2: try { SqlData sqlData = new SqlData(selectedFiles); sqlData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                    case 3: try { JsonData JsonData = new JsonData(selectedFiles); JsonData.Write(All_buyers, All_items, All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                }
            }

        }

        private void Upload(object sender, RoutedEventArgs e)
        {
            string selectedFiles="";
            int key=OpenDialog(ref selectedFiles);
            if (key != -1) {
                switch (key) {
                    case 1: try { XmlData xmlData = new XmlData(selectedFiles); xmlData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                    case 2: try { SqlData sqlData = new SqlData(selectedFiles); sqlData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                    case 3: try { JsonData JsonData = new JsonData(selectedFiles); JsonData.Read(ref All_buyers, ref All_items, ref All_orders); } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); }; break;
                }
                UpdatePage();
                UpdateInfo();

            }
        }
    }
}
