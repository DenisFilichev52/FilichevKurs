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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль!";
                return;
            }

            try
            {
                // Проверка авторизации (в реальном проекте пароль должен быть хеширован)
                var employee = App.context.Employees
                    .FirstOrDefault(emp => emp.Email == login && emp.IsActive == true);

                if (employee != null)
                {
                    // Успешная авторизация
                    UserSession.CurrentUserId = employee.EmployeeID;
                    UserSession.CurrentUserName = $"{employee.LastName} {employee.FirstName}";
                    UserSession.CurrentUserRole = employee.Positions.PositionName;
                    UserSession.IsLoggedIn = true;

                    // Открываем главное окно
                    MainWin mainWindow = new MainWin();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // Проверяем среди клиентов
                    var client = App.context.Clients
                        .FirstOrDefault(c => c.Email == login);

                    if (client != null)
                    {
                        UserSession.CurrentUserId = client.ClientID;
                        UserSession.CurrentUserName = $"{client.LastName} {client.FirstName}";
                        UserSession.CurrentUserRole = "Клиент";
                        UserSession.IsLoggedIn = true;

                        MainWin mainWindow = new MainWin();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        txtError.Text = "Неверный логин или пароль!";
                    }
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Ошибка подключения к базе данных: " + ex.Message;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.ShowDialog();
        }
    }
}
