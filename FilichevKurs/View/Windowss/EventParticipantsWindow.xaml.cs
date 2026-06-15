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
using System.IO;
using FilichevKurs.Model;

namespace FilichevKurs.View.Windowss
{
    /// <summary>
    /// Логика взаимодействия для EventParticipantsWindow.xaml
    /// </summary>
    public partial class EventParticipantsWindow : Window
    {
        private Events currentEvent;
        private Random random = new Random();

        public EventParticipantsWindow(Events evt)
        {
            InitializeComponent();
            currentEvent = evt;
            LoadEventInfo();
            LoadParticipants();
            WindowState = WindowState.Maximized;
        }

        private void LoadEventInfo()
        {
            txtEventName.Text = currentEvent.EventName;
            string typeName = currentEvent.EventTypes != null ? currentEvent.EventTypes.TypeName : "Без типа";
            txtEventInfo.Text = currentEvent.EventDate.ToString("dd.MM.yyyy") + " | " +
                               (currentEvent.Location ?? "Место не указано") + " | " + typeName;
        }

        private void LoadParticipants()
        {
            try
            {
                var rawParticipants = App.context.EventParticipations
                    .Where(ep => ep.EventID == currentEvent.EventID)
                    .ToList();

                var participants = new List<ParticipantViewModel>();
                int rowNum = 1;

                foreach (var ep in rawParticipants)
                {
                    var client = App.context.Clients.FirstOrDefault(c => c.ClientID == ep.ClientID);

                    participants.Add(new ParticipantViewModel
                    {
                        ParticipationID = ep.ParticipationID,
                        ClientID = ep.ClientID,
                        RowNumber = rowNum++,
                        FullName = client != null ?
                            client.LastName + " " + client.FirstName + " " + (client.MiddleName ?? "") : "Неизвестно",
                        Phone = client?.Phone ?? "Не указан",
                        IsAttended = ep.Attended == true,
                        IsPaid = ep.IsPaid == true,
                        RegistrationDate = ep.RegistrationDate,
                        RegistrationDateString = ep.RegistrationDate.HasValue ?
                            ep.RegistrationDate.Value.ToString("dd.MM.yyyy") : "--.--.----",
                        AttendanceText = ep.Attended == true ? "Присутствовал" : "Не пришел",
                        PaymentText = ep.IsPaid == true ? "Оплачено" : "Долг"
                    });
                }

                participants = participants.OrderBy(p => p.FullName).ToList();

                // Перенумерация после сортировки
                for (int i = 0; i < participants.Count; i++)
                {
                    participants[i].RowNumber = i + 1;
                }

                dgParticipants.ItemsSource = participants;

                // Статистика
                int total = participants.Count;
                int attended = participants.Count(p => p.IsAttended);
                int paid = participants.Count(p => p.IsPaid);
                int debt = total - paid;

                txtTotalCount.Text = total.ToString();
                txtAttendedCount.Text = attended.ToString();
                txtPaidCount.Text = paid.ToString();
                txtDebtCount.Text = debt.ToString();

                if (debt > 0)
                {
                    txtDebtCount.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                }
                else
                {
                    txtDebtCount.Foreground = new SolidColorBrush(Color.FromRgb(139, 92, 246));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки участников: " + ex.Message, "Ошибка");
            }
        }

        private void btnAddRandom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var existingClientIds = App.context.EventParticipations
                    .Where(ep => ep.EventID == currentEvent.EventID)
                    .Select(ep => ep.ClientID)
                    .ToList();

                var availableClients = App.context.Clients
                    .Where(c => !existingClientIds.Contains(c.ClientID))
                    .ToList();

                if (availableClients.Count == 0)
                {
                    MessageBox.Show("Нет доступных клиентов для добавления!",
                                  "Информация",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    return;
                }

                int countToAdd = Math.Min(random.Next(1, 6), availableClients.Count);
                int addedCount = 0;

                for (int i = 0; i < countToAdd; i++)
                {
                    int randomIndex = random.Next(availableClients.Count);
                    var randomClient = availableClients[randomIndex];
                    availableClients.RemoveAt(randomIndex);

                    var participation = new EventParticipations
                    {
                        EventID = currentEvent.EventID,
                        ClientID = randomClient.ClientID,
                        RegistrationDate = DateTime.Now.AddDays(-random.Next(0, 10)),
                        IsPaid = random.Next(2) == 1,
                        PaymentAmount = currentEvent.Price ?? 0,
                        Attended = random.Next(3) == 0
                    };

                    App.context.EventParticipations.Add(participation);
                    addedCount++;
                }

                App.context.SaveChanges();

                MessageBox.Show("Добавлено случайных участников: " + addedCount, "Успех");
                LoadParticipants();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message, "Ошибка");
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var participants = dgParticipants.ItemsSource as List<ParticipantViewModel>;
                if (participants == null || participants.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта!", "Информация");
                    return;
                }

                string csv = "№;ФИО;Телефон;Дата регистрации;Присутствие;Оплата\n";
                foreach (var p in participants)
                {
                    csv = csv + p.RowNumber + ";" +
                          p.FullName + ";" +
                          p.Phone + ";" +
                          p.RegistrationDateString + ";" +
                          p.AttendanceText + ";" +
                          p.PaymentText + "\n";
                }

                string fileName = "Участники_" + currentEvent.EventName.Replace(" ", "_") + "_" +
                                 DateTime.Now.ToString("yyyyMMdd") + ".csv";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fullPath = System.IO.Path.Combine(desktopPath, fileName);

                File.WriteAllText(fullPath, csv, System.Text.Encoding.UTF8);

                MessageBox.Show("Файл сохранен: " + fullPath, "Экспорт завершен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта: " + ex.Message, "Ошибка");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ParticipantViewModel
    {
        public int ParticipationID { get; set; }
        public int? ClientID { get; set; }
        public int RowNumber { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool IsAttended { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationDateString { get; set; }
        public string AttendanceText { get; set; }
        public string PaymentText { get; set; }
    }
}