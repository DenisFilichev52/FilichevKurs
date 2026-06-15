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
    /// Логика взаимодействия для ClientRegistrationWindow.xaml
    /// </summary>
    public partial class ClientRegistrationWindow : Window
    {
        private List<ClientViewModel> clients;
        private List<EventComboViewModel> events;

        public ClientRegistrationWindow()
        {
            InitializeComponent();
            LoadData();
            WindowState = WindowState.Maximized;
        }

        private void LoadData()
        {
            try
            {
                // Загружаем клиентов
                clients = App.context.Clients
                    .Select(c => new ClientViewModel
                    {
                        ClientID = c.ClientID,
                        FullName = c.LastName + " " + c.FirstName + " " + (c.MiddleName ?? ""),
                        LastName = c.LastName,
                        FirstName = c.FirstName,
                        MiddleName = c.MiddleName
                    })
                    .OrderBy(c => c.FullName)
                    .ToList();

                cmbClient.ItemsSource = clients;

                // Загружаем активные мероприятия (ИСПРАВЛЕНО)
                var eventsFromDb = App.context.Events
                    .Include("EventTypes")
                    .Where(e => e.IsActive == true)
                    .OrderBy(e => e.EventDate)
                    .ToList();

                events = eventsFromDb.Select(e => new EventComboViewModel
                {
                    EventID = e.EventID,
                    EventInfo = e.EventName + " (" + e.EventDate.ToString("dd.MM.yyyy") + ")",
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Price = e.Price
                }).ToList();

                cmbEvent.ItemsSource = events;

                if (events.Count == 0)
                {
                    ShowMessage("Нет доступных мероприятий!", false);
                    btnRegister.IsEnabled = false;
                }

                cmbEvent.SelectionChanged += cmbEvent_SelectionChanged;
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка загрузки данных: " + ex.Message, false);
            }
        }

        private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string search = txtFullName.Text.ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                cmbClient.SelectedItem = null;
                return;
            }

            var found = clients.FirstOrDefault(c =>
                c.FullName.ToLower().Contains(search) ||
                c.LastName.ToLower().Contains(search));

            if (found != null)
            {
                cmbClient.SelectedItem = found;
            }
        }

        private void cmbEvent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbEvent.SelectedItem is EventComboViewModel evt)
            {
                borderInfo.Visibility = Visibility.Visible;
                txtSelectedEvent.Text = evt.EventName;
                txtEventDate.Text = "Дата: " + evt.EventDate.ToString("dd.MM.yyyy");
                txtEventPrice.Text = "Цена: " + (evt.Price == 0 ? "Бесплатно" : evt.Price.Value.ToString("C"));
            }
            else
            {
                borderInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            int? clientId = null;
            string clientName = "";

            if (cmbClient.SelectedItem is ClientViewModel selectedClient)
            {
                clientId = selectedClient.ClientID;
                clientName = selectedClient.FullName;
            }
            else if (!string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                string searchName = txtFullName.Text.Trim().ToLower();
                var found = clients.FirstOrDefault(c =>
                    c.FullName.ToLower() == searchName ||
                    (c.LastName + " " + c.FirstName).ToLower() == searchName);

                if (found != null)
                {
                    clientId = found.ClientID;
                    clientName = found.FullName;
                }
                else
                {
                    ShowMessage("Клиент с таким ФИО не найден в базе данных!", false);
                    return;
                }
            }
            else
            {
                ShowMessage("Выберите клиента или введите ФИО!", false);
                return;
            }

            if (!(cmbEvent.SelectedItem is EventComboViewModel evt))
            {
                ShowMessage("Выберите мероприятие!", false);
                return;
            }

            var existing = App.context.EventParticipations
                .FirstOrDefault(ep => ep.EventID == evt.EventID && ep.ClientID == clientId);

            if (existing != null)
            {
                ShowMessage("Этот клиент уже зарегистрирован на выбранное мероприятие!", false);
                return;
            }

            try
            {
                var participation = new EventParticipations
                {
                    EventID = evt.EventID,
                    ClientID = clientId.Value,
                    RegistrationDate = DateTime.Now,
                    IsPaid = evt.Price == 0,
                    PaymentAmount = evt.Price ?? 0,
                    Attended = false
                };

                App.context.EventParticipations.Add(participation);
                App.context.SaveChanges();

                ShowMessage("Клиент '" + clientName + "' успешно зарегистрирован!", true);

                cmbClient.SelectedItem = null;
                txtFullName.Clear();
                cmbEvent.SelectedItem = null;
                borderInfo.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка при регистрации: " + ex.Message, false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            txtMessage.Text = message;
            txtMessage.Foreground = isSuccess ?
                new SolidColorBrush(Color.FromRgb(22, 163, 74)) :
                new SolidColorBrush(Color.FromRgb(233, 69, 96));
            txtMessage.Visibility = Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ClientViewModel
    {
        public int ClientID { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
    }

    public class EventComboViewModel
    {
        public int EventID { get; set; }
        public string EventInfo { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? Price { get; set; }
    }
}
