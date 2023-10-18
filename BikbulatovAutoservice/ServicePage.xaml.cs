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
        int CountRecords; // количество записей в таблице
        int CountPage; // общее количество страниц
        int CurrentPage = 0; // Текущая страница

        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;
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
            // заполнение таблицы для постраничного вывода
            TableList = currentServices;
            // вызов функции отображения кол-ва страниц с параметрами:
            // направление 0 - начальная загрузка
            // 0 - выбранная страница
            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage) // Функция отвечающая за разделение list'а
        {
            // direction - направление. 0 - начало, 1 - предыдущая страница, 2 - следующая страница
            // selectedPage - при нажатии на стрелочки передается null,
            // при выборе определенной страницы в этой переенной находится номер страницы

            CurrentPageList.Clear(); // начальная очистка листа
            CountRecords = TableList.Count; // определение количества записей во всем списке
            // определение количества страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;
            // Проверка на правильность - если
            // CurrentPage(номер текущей страницы) "правильный"

            int min;

            if (selectedPage.HasValue) // проверка на значение не null (тк может быть null)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else // если нажата стрелка
            {
                switch (direction)
                {
                    case 1: // нажата кнопка "Предыдущая страница"
                        if (CurrentPage > 0)
                        // то есть кнопка нажата правильно и "назад" можно идти
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                            // в случае если currentPage попытается выйти из диапазона
                            // внесение данных не произойдет
                        }
                        break;

                    case 2: // нажат кнопка "следующая страница"
                        if (CurrentPage < CountPage - 1)
                        // если вперед идти можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
                if (Ifupdate) // если currentPage не вышел из диапазона, то
                {
                    PageListBox.Items.Clear();
                    // Удаление старых значений из listBox'a номеров страниц, нужно чтобы при изменении
                    // количества записей кол-во страниц динамически изменялось
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    PageListBox.SelectedIndex = CurrentPage;

                    // вывод количества записей на странице и общего количества
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    TBCount.Text = min.ToString();
                    TBallRecords.Text = " из " + CountRecords.ToString();

                    ServiceListView.ItemsSource = CurrentPageList;
                    // обновить отображение списка услуг
                    ServiceListView.Items.Refresh();
                }
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // забираем сервис, для которого нажата кнопка удалить
            var currentService = (sender as Button).DataContext as Service;

            // проверка на возможность удаления
            var currentClientServices = Bikbulatov_autoserviceEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientServices.Count != 0) // если есть записи на этот сервис
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Bikbulatov_autoserviceEntities.GetContext().Service.Remove(currentService);
                        Bikbulatov_autoserviceEntities.GetContext().SaveChanges();
                        // выводим в листвью измененную таблицу Сервис
                        ServiceListView.ItemsSource = Bikbulatov_autoserviceEntities.GetContext().Service.ToList();
                        // чтобы применились фильтры и поиск, если они были на форме изначально
                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }
        */
    }
}
