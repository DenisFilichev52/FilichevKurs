using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для EventsListWindow.xaml
    /// </summary>
    public partial class EventsListWindow : Window
    {
        private List<Events> allEvents = new List<Events>();

        public EventsListWindow()
        {
            InitializeComponent();
           
            LoadData(); // Автозагрузка
        }

       

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                wpEvents.Children.Clear();

                // Загружаем данные Include как раньше
                allEvents = App.context.Events
                    .Include("EventTypes")
                    .Include("Employees")
                    .ToList();

                if (allEvents.Count == 0)
                {
                    TextBlock noData = new TextBlock
                    {
                        Text = "Нет данных о мероприятиях",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(20)
                    };
                    wpEvents.Children.Add(noData);
                    return;
                }

                // Фильтры
                var filteredEvents = allEvents.AsEnumerable();

                

                if (dpDateFrom.SelectedDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(ev =>
                        ev.EventDate >= dpDateFrom.SelectedDate.Value);
                }
                if (dpDateTo.SelectedDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(ev =>
                        ev.EventDate <= dpDateTo.SelectedDate.Value);
                }

                

                var eventsList = filteredEvents.OrderBy(ev => ev.EventDate).ToList();

                if (eventsList.Count == 0)
                {
                    TextBlock noData = new TextBlock
                    {
                        Text = "Нет мероприятий по выбранным фильтрам",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(20)
                    };
                    wpEvents.Children.Add(noData);
                    return;
                }

                // Создаем карточки
                foreach (var evt in eventsList)
                {
                    Border card = CreateEventCard(evt);
                    wpEvents.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private Border CreateEventCard(Events evt)
        {
            Border card = new Border
            {
                Width = 320,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                Margin = new Thickness(10)
            };

            card.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                ShadowDepth = 2,
                BlurRadius = 8,
                Opacity = 0.1
            };

            StackPanel content = new StackPanel();

            // Заголовок с типом и статусом
            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Border iconBorder = new Border
            {
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(8),
                Background = new SolidColorBrush(Color.FromRgb(240, 247, 255)),
                Margin = new Thickness(0, 0, 12, 0)
            };
            TextBlock icon = new TextBlock
            {
                Text = GetIcon(evt.EventTypes?.TypeName),
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            iconBorder.Child = icon;
            Grid.SetColumn(iconBorder, 0);

            TextBlock typeText = new TextBlock
            {
                Text = evt.EventTypes?.TypeName ?? "Без типа",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(typeText, 1);

            Border statusBorder = new Border
            {
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(8, 4, 8, 4),
                Background = evt.IsActive == true
                    ? new SolidColorBrush(Color.FromRgb(240, 253, 244))
                    : new SolidColorBrush(Color.FromRgb(254, 242, 242))
            };
            TextBlock statusText = new TextBlock
            {
                Text = evt.IsActive == true ? "Активно" : "Отменено",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = evt.IsActive == true
                    ? new SolidColorBrush(Color.FromRgb(22, 163, 74))
                    : new SolidColorBrush(Color.FromRgb(220, 38, 38))
            };
            statusBorder.Child = statusText;
            Grid.SetColumn(statusBorder, 2);

            header.Children.Add(iconBorder);
            header.Children.Add(typeText);
            header.Children.Add(statusBorder);
            content.Children.Add(header);

            // Название
            TextBlock name = new TextBlock
            {
                Text = evt.EventName,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(26, 26, 46)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 12, 0, 12)
            };
            content.Children.Add(name);

            // Дата
            StackPanel datePanel = new StackPanel { Orientation = Orientation.Horizontal };
            datePanel.Children.Add(new TextBlock
            {
                Text = "📅 ",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175))
            });
            datePanel.Children.Add(new TextBlock
            {
                Text = evt.EventDate.ToString("dd.MM.yyyy"),
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(75, 85, 99))
            });
            if (evt.StartTime.HasValue)
            {
                datePanel.Children.Add(new TextBlock
                {
                    Text = " в ",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175))
                });
                datePanel.Children.Add(new TextBlock
                {
                    Text = evt.StartTime.Value.ToString(@"hh\:mm"),
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(75, 85, 99))
                });
            }
            content.Children.Add(datePanel);

            // Место
            StackPanel locationPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 12)
            };
            locationPanel.Children.Add(new TextBlock
            {
                Text = "📍 ",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175))
            });
            locationPanel.Children.Add(new TextBlock
            {
                Text = evt.Location ?? "Не указано",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(107, 114, 128))
            });
            content.Children.Add(locationPanel);

            // Кнопки действий
            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 0)
            };

            Button btnParticipants = new Button
            {
                Content = "👥 Участники",
                Background = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                Foreground = Brushes.White,
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 0, 8, 0),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand
            };
            // ВАЖНО: Сохраняем событие в Tag
            btnParticipants.Tag = evt;
            btnParticipants.Click += (sender, e) =>
            {
                Button btn = sender as Button;
                Events eventObj = btn.Tag as Events;
                if (eventObj != null)
                {
                    EventParticipantsWindow participantsWindow = new EventParticipantsWindow(eventObj);
                    participantsWindow.ShowDialog();
                }
            };
            buttonsPanel.Children.Add(btnParticipants);

            Button btnReg = new Button
            {
                Content = "✓ Записаться",
                Background = new SolidColorBrush(Color.FromRgb(233, 69, 96)),
                Foreground = Brushes.White,
                Padding = new Thickness(12, 8, 12, 8),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand,
                Tag = evt
            };
            btnReg.Click += BtnReg_Click;
            buttonsPanel.Children.Add(btnReg);

            content.Children.Add(buttonsPanel);

            // Цена
            StackPanel pricePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 12, 0, 0)
            };
            pricePanel.Children.Add(new TextBlock
            {
                Text = "Стоимость: ",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175))
            });
            TextBlock price = new TextBlock
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            if (evt.Price == 0)
            {
                price.Text = "Бесплатно";
                price.Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74));
            }
            else
            {
                price.Text = evt.Price.HasValue ? evt.Price.Value.ToString("C") : "0 ₽";
                price.Foreground = new SolidColorBrush(Color.FromRgb(26, 26, 46));
            }
            pricePanel.Children.Add(price);
            content.Children.Add(pricePanel);

            card.Child = content;
            return card;
        }

        private void BtnParticipants_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Events evt)
            {
                //EventParticipantsWindow participantsWindow = new EventParticipantsWindow(evt);
                //participantsWindow.ShowDialog();
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Events evt)
            {
                RegisterForEvent(evt);
            }
        }

        private void RegisterForEvent(Events evt)
        {
            if (UserSession.CurrentUserRole != "Клиент")
            {
                MessageBox.Show("Регистрация доступна только для клиентов.",
                              "Требуется авторизация",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                return;
            }

            string priceText = evt.Price == 0 ? "Бесплатно" :
                (evt.Price.HasValue ? evt.Price.Value.ToString("C") : "0");

            var result = MessageBox.Show(
                "Зарегистрироваться на мероприятие?\n\n" +
                "Название: " + evt.EventName + "\n" +
                "Дата: " + evt.EventDate.ToString("dd.MM.yyyy") + "\n" +
                "Цена: " + priceText,
                "Подтверждение регистрации",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var existing = App.context.EventParticipations
                        .FirstOrDefault(ep => ep.EventID == evt.EventID &&
                                             ep.ClientID == UserSession.CurrentUserId);

                    if (existing != null)
                    {
                        MessageBox.Show("Вы уже зарегистрированы на это мероприятие!",
                                      "Информация",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                        return;
                    }

                    var participation = new EventParticipations
                    {
                        EventID = evt.EventID,
                        ClientID = UserSession.CurrentUserId,
                        RegistrationDate = DateTime.Now,
                        IsPaid = evt.Price == 0,
                        PaymentAmount = evt.Price ?? 0,
                        Attended = false
                    };

                    App.context.EventParticipations.Add(participation);
                    App.context.SaveChanges();

                    MessageBox.Show("Вы успешно зарегистрированы!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка регистрации: " + ex.Message, "Ошибка");
                }
            }
        }

        private string GetIcon(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return "📅";
            string lower = typeName.ToLower();
            if (lower.Contains("аэро")) return "✈️";
            if (lower.Contains("танк")) return "🎯";
            if (lower.Contains("выстав")) return "🚁";
            if (lower.Contains("рекон")) return "⚔️";
            if (lower.Contains("мастер")) return "🎓";
            if (lower.Contains("ноч")) return "🌙";
            if (lower.Contains("открыт")) return "🎉";
            return "📅";
        }

        private void btnRegisterClient_Click(object sender, RoutedEventArgs e)
        {
            ClientRegistrationWindow regWindow = new ClientRegistrationWindow();
            regWindow.ShowDialog();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
           
            dpDateFrom.SelectedDate = null;
            dpDateTo.SelectedDate = null;
            
            LoadData();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
    }
}
