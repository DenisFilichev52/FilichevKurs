using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using FilichevKurs.Model;

namespace FilichevKurs.View.Windowss
{
    /// <summary>
    /// Логика взаимодействия для EquipmentDetailsWindow.xaml
    /// </summary>
    public partial class EquipmentDetailsWindow : Window
    {
        private MilitaryEquipment equipment;

        public EquipmentDetailsWindow(MilitaryEquipment selectedEquipment)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            if (selectedEquipment == null)
            {
                MessageBox.Show("Ошибка: техника не выбрана", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            equipment = selectedEquipment;
            LoadData();
            LoadPhotoFromDatabase(); // ← Новый метод загрузки фото
        }

        /// <summary>
        /// Загрузка фото напрямую из SQL Server
        /// </summary>
        private void LoadPhotoFromDatabase()
        {
            try
            {
                // Получаем строку подключения из контекста
                string connectionString = App.context.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Photo FROM MilitaryEquipment WHERE EquipmentID = @EquipmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EquipmentID", equipment.EquipmentID);

                        var result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            byte[] photoBytes = (byte[])result;

                            if (photoBytes.Length > 0)
                            {
                                DisplayPhoto(photoBytes);
                                return;
                            }
                        }
                    }
                }

                // Если фото не найдено
                ShowPlaceholder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фото: {ex.Message}", "Ошибка");
                ShowPlaceholder();
            }
        }

        /// <summary>
        /// Отображение фото из массива байт
        /// </summary>
        private void DisplayPhoto(byte[] photoBytes)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();

                using (MemoryStream stream = new MemoryStream(photoBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                imgEquipment.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения фото: {ex.Message}", "Ошибка");
                ShowPlaceholder();
            }
        }

        /// <summary>
        /// Показать заглушку
        /// </summary>
        private void ShowPlaceholder()
        {
            // Создаем простую заглушку с текстом
            var placeholderText = new TextBlock
            {
                Text = "ФОТО\nОТСУТСТВУЕТ",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            // Очищаем Image и показываем заглушку в родительском контейнере
            imgEquipment.Source = null;

        }

        // Остальные методы LoadData, SetStatusColor, btnClose_Click без изменений...
        private void LoadData()
        {
            txtPhotoTitle.Text = equipment.Name;
            txtType.Text = equipment.EquipmentTypes?.TypeName?.ToUpper();
            txtName.Text = equipment.Name;
            txtManufacturer.Text = equipment.Manufacturer ?? "Не указан";
            txtYear.Text = equipment.ProductionYear?.ToString() ?? "Не указан";
            txtWeight.Text = equipment.Weight.HasValue ? $"{equipment.Weight} т" : "Не указан";
            txtCrew.Text = equipment.Crew.HasValue ? $"{equipment.Crew} чел." : "Не указан";

            if (equipment.Length.HasValue && equipment.Width.HasValue && equipment.Height.HasValue)
            {
                txtDimensions.Text = $"{equipment.Length} × {equipment.Width} × {equipment.Height} м";
            }
            else
            {
                txtDimensions.Text = "Не указаны";
            }

            txtSpeed.Text = equipment.MaxSpeed.HasValue ? $"{equipment.MaxSpeed} км/ч" : "Не указана";
            txtEngine.Text = equipment.EnginePower.HasValue ? $"{equipment.EnginePower} л.с." : "Не указан";
            txtArmor.Text = equipment.ArmorType ?? "Информация отсутствует";
            txtArmament.Text = equipment.Armament ?? "Информация отсутствует";
            txtDescription.Text = equipment.Description ?? "Описание отсутствует";
            txtStatus.Text = equipment.EquipmentStatus?.StatusName ?? "Неизвестно";
            txtInventory.Text = equipment.InventoryNumber;
            txtArrivalDate.Text = equipment.ArrivalDate.HasValue
                ? equipment.ArrivalDate.Value.ToString("dd.MM.yyyy")
                : "Неизвестно";

            SetStatusColor(equipment.EquipmentStatus?.StatusName);
        }

        private void SetStatusColor(string statusName)
        {
            Brush backgroundBrush = Brushes.White;
            Brush borderBrush = Brushes.Gray;
            Brush textBrush = Brushes.Gray;

            switch (statusName)
            {
                case "Представлен":
                    backgroundBrush = new SolidColorBrush(Color.FromRgb(240, 253, 244));
                    borderBrush = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                    textBrush = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                    break;
                case "В ремонте":
                    backgroundBrush = new SolidColorBrush(Color.FromRgb(254, 242, 242));
                    borderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                    textBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                    break;
                case "На хранении":
                    backgroundBrush = new SolidColorBrush(Color.FromRgb(255, 251, 240));
                    borderBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    textBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    break;
                case "На реставрации":
                    backgroundBrush = new SolidColorBrush(Color.FromRgb(245, 243, 255));
                    borderBrush = new SolidColorBrush(Color.FromRgb(139, 92, 246));
                    textBrush = new SolidColorBrush(Color.FromRgb(139, 92, 246));
                    break;
            }

            borderStatus.Background = backgroundBrush;
            borderStatus.BorderBrush = borderBrush;
            txtStatus.Foreground = textBrush;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}