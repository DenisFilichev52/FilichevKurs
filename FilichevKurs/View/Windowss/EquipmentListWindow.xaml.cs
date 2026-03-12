using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для EquipmentListWindow.xaml
    /// </summary>

        public partial class EquipmentListWindow : Window
        {
        private List<MilitaryEquipment> allEquipment = new List<MilitaryEquipment>(); // ← Инициализация здесь

        public ICommand OpenDetailsCommand { get; set; }

        public EquipmentListWindow()
        {
            InitializeComponent();
            InitializeCommand();
            LoadData();        // ← Сначала загружаем данные
            LoadFilters();     // ← Потом фильтры
            CheckUserRole();
        }

        private void InitializeCommand()
        {
            OpenDetailsCommand = new RelayCommand(param =>
            {
                if (param is MilitaryEquipment equipment)
                    OpenEquipmentDetails(equipment);
            });

            dgEquipment.DataContext = this;
        }

        private void LoadData()
        {
            try
            {
                allEquipment = App.context.MilitaryEquipment
                    .Include("EquipmentTypes")
                    .Include("EquipmentStatus")
                    .ToList() ?? new List<MilitaryEquipment>(); // ← Защита от null

                dgEquipment.ItemsSource = allEquipment;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                allEquipment = new List<MilitaryEquipment>();
            }
        }

        private void ApplyFiltersAndSort()
        {
            // ← Проверка на null
            if (allEquipment == null || allEquipment.Count == 0)
                return;

            var query = allEquipment.AsQueryable();

            // Фильтр по типу
            if (cmbType.SelectedValue != null && (int)cmbType.SelectedValue != 0)
            {
                int typeId = (int)cmbType.SelectedValue;
                query = query.Where(eq => eq.TypeID == typeId);
            }

            // Фильтр по статусу
            if (cmbStatus.SelectedValue != null && (int)cmbStatus.SelectedValue != 0)
            {
                int statusId = (int)cmbStatus.SelectedValue;
                query = query.Where(eq => eq.StatusID == statusId);
            }

            // Сортировка
            switch (cmbSort?.SelectedIndex ?? 0)
            {
                case 1: query = query.OrderBy(eq => eq.Name); break;
                case 2: query = query.OrderByDescending(eq => eq.Name); break;
                case 5: query = query.OrderBy(eq => eq.ProductionYear); break;
                case 6: query = query.OrderByDescending(eq => eq.ProductionYear); break;
                default: query = query.OrderBy(eq => eq.EquipmentID); break;
            }

            dgEquipment.ItemsSource = query.ToList();
        }

        private void FilterOrSortChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        // Остальные методы без изменений...
        private void LoadFilters()
        {
            try
            {
                var types = App.context.EquipmentTypes?.ToList() ?? new List<EquipmentTypes>();
                types.Insert(0, new EquipmentTypes { TypeID = 0, TypeName = "Все типы" });
                cmbType.ItemsSource = types;
                cmbType.DisplayMemberPath = "TypeName";
                cmbType.SelectedValuePath = "TypeID";
                cmbType.SelectedIndex = 0;

                var statuses = App.context.EquipmentStatus?.ToList() ?? new List<EquipmentStatus>();
                statuses.Insert(0, new EquipmentStatus { StatusID = 0, StatusName = "Все статусы" });
                cmbStatus.ItemsSource = statuses;
                cmbStatus.DisplayMemberPath = "StatusName";
                cmbStatus.SelectedValuePath = "StatusID";
                cmbStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильтров: {ex.Message}", "Ошибка");
            }
        }

        private void BtnParticipants_Click(object sender, RoutedEventArgs e)
        {
            // ИСПРАВЛЕНО: Получаем Events из Tag, а не из DataContext
            if (sender is Button btn && btn.Tag is Events evt)
            {
                EventParticipantsWindow participantsWindow = new EventParticipantsWindow(evt);
                participantsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Ошибка: не удалось получить данные мероприятия", "Ошибка");
            }
        }
        private void CheckUserRole()
        {
            if (UserSession.CurrentUserRole == "Клиент")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                
                
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            cmbType.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            if (cmbSort != null) cmbSort.SelectedIndex = 0;

            dgEquipment.ItemsSource = allEquipment;
        }

        private void OpenEquipmentDetails(MilitaryEquipment equipment)
        {
            if (equipment == null) return;

            var detailsWindow = new EquipmentDetailsWindow(equipment);
            detailsWindow.ShowDialog();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEquipmentWindow();

            if (addWindow.ShowDialog() == true)
            {
                // Обновляем список после добавления
                LoadData();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipment.SelectedItem is MilitaryEquipment selected)
            {
                MessageBox.Show($"Редактирование: {selected.Name}");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipment.SelectedItem is MilitaryEquipment selected)
            {
                if (MessageBox.Show($"Удалить {selected.Name}?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        App.context.MilitaryEquipment.Remove(selected);
                        App.context.SaveChanges();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Класс RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        public RelayCommand(Action<object> execute) => this.execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => execute(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
