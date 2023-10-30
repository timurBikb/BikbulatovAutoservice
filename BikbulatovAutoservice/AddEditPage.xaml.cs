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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BikbulatovAutoservice
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        // добавим новое поле, которое будет хранить в себе  экземпляр
        // добавленного сервиса (услуги)
        private Service _currentService = new Service();

        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                _currentService = SelectedService;

            // При инициализации установим DataContext страницы - этот созданный объект
            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Cost == 0)
                errors.AppendLine("Укажите стоимость улуги");

            if (_currentService.Discount < 0 || _currentService.Discount > 100)
                errors.AppendLine("Укажите скидку от 0 до 100");

            if (_currentService.Duration > 240 || _currentService.Duration < 0)
                errors.AppendLine("Укажите длительность услуги от 0 до 240");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var allService = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();
            allService = allService.Where(p => p.Title == _currentService.Title).ToList();

            if (allService.Count == 0)
            {
                if (_currentService.ID == 0)
                    Bikbulatov_autoserviceEntities.GetContext().Service.Add(_currentService);

                // сохранить изменения, если никаких ошибок не получилось при этом
                try
                {
                    Bikbulatov_autoserviceEntities.GetContext().SaveChanges();
                    MessageBox.Show("информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Уже существует такая услуга");
            }
        }
    }
}
