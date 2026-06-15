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
using System.Windows.Shapes;

namespace FilichevKurs.View.Windowss
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        public ClientsWindow()
        {
            InitializeComponent();
            LoadClients();
            WindowState = WindowState.Maximized;
        }

        private void LoadClients()
        {
            dgClients.ItemsSource = App.DbContext.Clients.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddClientWindow();

            if (addWindow.ShowDialog() == true)
            {
                LoadClients(); // обновляем список
            }
        }
    }
}
