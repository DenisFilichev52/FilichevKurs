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
    /// Логика взаимодействия для AddEquipmentWindow.xaml
    /// </summary>
    public partial class AddEquipmentWindow : Window
    {
        public AddEquipmentWindow()
        {
            InitializeComponent();
            LoadData();
            WindowState = WindowState.Maximized;
        }

        private void LoadData()
        {
            cmbType.ItemsSource = App.DbContext.EquipmentTypes.ToList();
            cmbType.DisplayMemberPath = "TypeName";
            cmbType.SelectedValuePath = "EquipmentTypeID"; // важно!

            cmbStatus.ItemsSource = App.DbContext.EquipmentStatus.ToList();
            cmbStatus.DisplayMemberPath = "StatusName";
            cmbStatus.SelectedValuePath = "StatusID"; // важно!
            cmbStatus.SelectedIndex = 0; // выбираем первый по умолчанию
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInventoryNumber.Text))
            {
                MessageBox.Show("Введите инвентарный номер");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название");
                return;
            }

            // Проверяем что выбран тип
            if (cmbType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип техники");
                return;
            }

            // Берем ID напрямую из объекта, а не через SelectedValue
            var selectedType = cmbType.SelectedItem as EquipmentTypes;
            int typeId = selectedType.TypeID;

            var selectedStatus = cmbStatus.SelectedItem as EquipmentStatus;
            int statusId = selectedStatus?.StatusID ?? 1;

            var equipment = new MilitaryEquipment
            {
                InventoryNumber = txtInventoryNumber.Text,
                Name = txtName.Text,
                TypeID = typeId,
                Manufacturer = GetStringOrNull(txtManufacturer),
                ProductionYear = GetIntOrNull(txtProductionYear),
                Weight = GetDecimalOrNull(txtWeight),
                Length = GetDecimalOrNull(txtLength),
                Width = GetDecimalOrNull(txtWidth),
                Height = GetDecimalOrNull(txtHeight),
                Crew = GetIntOrNull(txtCrew),
                MaxSpeed = GetIntOrNull(txtMaxSpeed),
                EnginePower = GetIntOrNull(txtEnginePower),
                ArmorType = GetStringOrNull(txtArmorType),
                Armament = GetStringOrNull(txtArmament),
                Description = GetStringOrNull(txtDescription),
                StatusID = statusId,
                ArrivalDate = dpArrivalDate.SelectedDate
            };

            App.DbContext.MilitaryEquipment.Add(equipment);
            App.DbContext.SaveChanges();

            MessageBox.Show("Техника добавлена");
            this.DialogResult = true;
            this.Close();
        }

        private string GetStringOrNull(TextBox textBox)
        {
            return string.IsNullOrWhiteSpace(textBox.Text) ? null : textBox.Text;
        }

        private int? GetIntOrNull(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text)) return null;
            return int.TryParse(textBox.Text, out int result) ? result : (int?)null;
        }

        private decimal? GetDecimalOrNull(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text)) return null;
            var text = textBox.Text.Replace('.', ',');
            return decimal.TryParse(text, out decimal result) ? result : (decimal?)null;
        }
        private int? ParseInt(string text)
        {
            return int.TryParse(text, out int result) ? result : (int?)null;
        }

        private decimal? ParseDecimal(string text)
        {
            return decimal.TryParse(text, out decimal result) ? result : (decimal?)null;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
