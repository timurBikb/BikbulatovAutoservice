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
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            // добавляем строки
                // загрузить в список из бд
            var currentServices = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();
                // связать с нашим литсвью
            ServiceListView.ItemsSource = currentServices;
            // добавили строки

            ComboType.SelectedIndex = 0;

            // вызаваем UpdateServices()
            UpdateServices();
        }

        private void UpdateServices()
        {
            // берем из бд данный таблицы Сервис
            var currentServices = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();

            // прописываем фильтрацию по условию задания
            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 0 && p.Discount <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 0 && p.Discount < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 5 && p.Discount < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 15 && p.Discount < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 30 && p.Discount < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 70 && p.Discount < 100)).ToList();
            }

            // реализуем поиск данных в листвью при вводе текста в окно поиска
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            // реализуем сортировку по убыванию
            if (RButtonDown.IsChecked.Value)
            {
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            // реализуем сортировку по возрастанию
            if (RButtonUp.IsChecked.Value)
            {
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }
            // отображаем итоги поиска/фильтрации/сортировки
            ServiceListView.ItemsSource = currentServices;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // открыть окно редактирования/добавления услуг
            Manager.MainFrame.Navigate(new AddEditPage(null));

            UpdateServices();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // открыть окно редактирования/добавления услуг
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));

            UpdateServices();
        }

        //private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (Visibility == Visibility.Visible)
        //    {
        //        Bikbulatov_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
        //        ServiceListView.ItemsSource = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();
        //    }

        //    UpdateServices();
        //}

        private void ServiceListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Bikbulatov_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();
            }

            UpdateServices();
        }

        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }
        */
    }
}
