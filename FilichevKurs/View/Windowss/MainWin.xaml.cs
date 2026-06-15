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
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        public MainWin()
        {
            InitializeComponent();
            LoadUserInfo();
            WindowState = WindowState.Maximized;

        }


        private void LoadUserInfo()
        {
            txtUserName.Text = UserSession.CurrentUserName;
            txtUserRole.Text = UserSession.CurrentUserRole;
            txtWelcome.Text = $"Рады видеть вас, {UserSession.CurrentUserName}! " +
                            $"Исследуйте нашу коллекцию современной военной техники " +
                            $"и присоединяйтесь к уникальным мероприятиям.";
        }

        // Новый обработчик для кнопки Главная
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            // Уже на главной, можно добавить прокрутку вверх
            // или обновление контента
        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            EquipmentListWindow eqWindow = new EquipmentListWindow();
            eqWindow.Show();
        }

        private void btnEvents_Click(object sender, RoutedEventArgs e)
        {
            EventsListWindow evWindow = new EventsListWindow();
            evWindow.Show();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow();
            clientsWindow.Show();
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            // Берем ID текущего пользователя из сессии
            int currentUserId = UserSession.CurrentUserId; // или как там называется свойство

            ProfileWindow profWindow = new ProfileWindow(currentUserId);
            profWindow.Show();
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел статистики в разработке", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Clear();
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}