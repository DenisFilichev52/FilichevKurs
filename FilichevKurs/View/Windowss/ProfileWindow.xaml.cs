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
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow(int employeeId)
        {
            InitializeComponent();
            LoadData(employeeId);
        }

        private void LoadData(int id)
        {
            // Проверяем, есть ли контекст
            if (App.DbContext == null)
            {
                MessageBox.Show("Ошибка: база данных не подключена");
                return;
            }

            // Ищем сотрудника
            var emp = App.DbContext.Employees
                .Include("Positions") // загружаем связанную таблицу
                .FirstOrDefault(x => x.EmployeeID == id);

            // Если не нашли — показываем ошибку
            if (emp == null)
            {
                MessageBox.Show($"Сотрудник с ID {id} не найден");
                return;
            }

            // Заполняем поля
            FioText.Text = $"{emp.LastName} {emp.FirstName} {emp.MiddleName}";
            DolzhnostText.Text = emp.Positions?.PositionName ?? "Не указана";
            RoleText.Text = emp.IsActive == true ? "Активен" : "Не активен";
            IdText.Text = emp.EmployeeID.ToString();
            PhoneText.Text = emp.Phone ?? "Не указан";
            EmailText.Text = emp.Email ?? "Не указан";
            BirthDateText.Text = emp.BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
