using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BikbulatovAutoservice
{
    /// <summary>
    /// Логика взаимодействия для SingUpPage.xaml
    /// </summary>
    public partial class SingUpPage : Page
    {
        private Service _currentService = new Service();
        public SingUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            // При инициализации установим DataContext страницы - этот созданный объект
            // чтобы на форму подгрузить выбранные наименование услуги и длительность
            DataContext = _currentService;

            // вытащим из бд таблицу Клиент
            var _currentClient = Bikbulatov_autoserviceEntities.GetContext().Client.ToList();
            // свяжем ее с комбобоксом
            ComboClient.ItemsSource = _currentClient;
        }


        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            MessageBox.Show("Вы записались на услугу");
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (!IsValidTime(s))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());

                int sum = startHour + startMin + _currentService.Duration;

                int EndHour = sum / 60 % 24;
                int EndMin = sum % 60;
                s = EndHour.ToString() + ":" + EndMin.ToString("D2");
                TBEnd.Text = s;
             }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"[0-9:]|[APap][Mm]"))
            {
                e.Handled = true; // Отклонить ввод, если символы не соответствуют формату времени.
            }
        }

        private bool IsValidTime(string input)
        {
            // Проверка, что ввод является действительным временем в формате "часы:минуты".
            // Например, "12:34" будет считаться допустимым временем.

            // Разбиваем строку на часы и минуты.
            string[] timeParts = input.Split(':');

            if (timeParts.Length != 2)
            {
                return false; // Не содержит ровно одно двоеточие.
            }

            if (!int.TryParse(timeParts[0], out int hours) || !int.TryParse(timeParts[1], out int minutes))
            {
                return false; // Не удалось преобразовать часы и минуты в числа.
            }

            // Проверяем, что часы и минуты находятся в допустимых диапазонах.
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                return false; // Недопустимые часы или минуты.
            }

            return true; // Все проверки пройдены, это действительное время.
        }
    }
}
