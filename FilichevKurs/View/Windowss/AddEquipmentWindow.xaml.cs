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
        }

        private void LoadData()
        {
            cmbType.ItemsSource = App.DbContext.EquipmentTypes.ToList();
            cmbStatus.ItemsSource = App.DbContext.EquipmentStatus.ToList();
            cmbStatus.SelectedValue = 1; // статус по умолчанию
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
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

            if (cmbType.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип техники");
                return;
            }

            // Создаем объект
            var equipment = new MilitaryEquipment
            {
                InventoryNumber = txtInventoryNumber.Text,
                Name = txtName.Text,
                TypeID = (int)cmbType.SelectedValue,

                // Необязательные поля
                Manufacturer = string.IsNullOrWhiteSpace(txtManufacturer.Text) ? null : txtManufacturer.Text,
                ProductionYear = ParseInt(txtProductionYear.Text),
                Weight = ParseDecimal(txtWeight.Text),
                Length = ParseDecimal(txtLength.Text),
                Width = ParseDecimal(txtWidth.Text),
                Height = ParseDecimal(txtHeight.Text),
                Crew = ParseInt(txtCrew.Text),
                MaxSpeed = ParseInt(txtMaxSpeed.Text),
                EnginePower = ParseInt(txtEnginePower.Text),
                ArmorType = string.IsNullOrWhiteSpace(txtArmorType.Text) ? null : txtArmorType.Text,
                Armament = string.IsNullOrWhiteSpace(txtArmament.Text) ? null : txtArmament.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                StatusID = (int?)cmbStatus.SelectedValue ?? 1,
                ArrivalDate = dpArrivalDate.SelectedDate
            };

            App.DbContext.MilitaryEquipment.Add(equipment);
            App.DbContext.SaveChanges();

            MessageBox.Show("Техника добавлена");
            this.DialogResult = true;
            this.Close();
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
