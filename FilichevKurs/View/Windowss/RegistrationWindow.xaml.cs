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
using FilichevKurs.Model;

namespace FilichevKurs.View.Windowss
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            dpBirthDate.SelectedDate = DateTime.Now.AddYears(-30);
            WindowState = WindowState.Maximized;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowMessage("Заполните все обязательные поля (отмечены *)!", false);
                return;
            }

            try
            {
                var existingClient = App.context.Clients
                    .FirstOrDefault(c => c.Email == txtEmail.Text.Trim());

                if (existingClient != null)
                {
                    ShowMessage("Пользователь с таким email уже существует!", false);
                    return;
                }

                var newClient = new Clients
                {
                    LastName = txtLastName.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    MiddleName = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim(),
                    BirthDate = dpBirthDate.SelectedDate,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PassportNumber = string.IsNullOrWhiteSpace(txtPassport.Text) ? null : txtPassport.Text.Trim(),
                    RegistrationDate = DateTime.Now,
                    IsRegular = false,
                    DiscountPercent = 0
                };

                App.context.Clients.Add(newClient);
                App.context.SaveChanges();

                ShowMessage("Регистрация успешно завершена! Можете закрыть окно и войти.", true);
                ClearFields();
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка при регистрации: " + ex.Message, false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            txtMessage.Text = message;
            txtMessage.Foreground = isSuccess
                ? new SolidColorBrush(Color.FromRgb(22, 163, 74))  // Зеленый
                : new SolidColorBrush(Color.FromRgb(233, 69, 96));  // Красный
            txtMessage.Visibility = Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearFields()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtPassport.Clear();
        }
    }
}
