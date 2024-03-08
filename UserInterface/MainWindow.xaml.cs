using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Interaction;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OwnershipTypes.ItemsSource = Enum.GetValues(typeof(Enterprise.OwnershipType)).Cast<Enterprise.OwnershipType>();
            OwnershipTypes.SelectedIndex = 0;
            List<string> enumOptions = new() { "" };
            foreach (Enterprise.OwnershipType day in Enum.GetValues(typeof(Enterprise.OwnershipType)))
            {
                enumOptions.Add(day.ToString());
            }
            FilterOwnership.ItemsSource = enumOptions;
            FilterOwnership.SelectedIndex = 0;
            UpdateEnterprisesList();
        }

        ~MainWindow()
        {
            interactionHolder.Dispose();
        }

        private void UpdateEnterprisesList()
        {
            EnterprisesGrid.Children.Clear();
            foreach (Enterprise enterprise in interactionHolder.FindByFilter(filter))
            {
                Grid enterprisesGrid = new Grid();
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                enterprisesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                TextBox nameBox = new TextBox() { Text = enterprise.Name };
                nameBox.TextChanged += (sender, args) =>
                {
                    interactionHolder.ChangeName(enterprise, nameBox.Text);
                };
                Grid.SetColumn(nameBox, 0);

                TextBox classBox = new TextBox() { Text = enterprise.Class.ToString() };
                classBox.Margin = new Thickness(2, 0, 2, 0);
                classBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                classBox.PreviewTextInput += NumbersOnly;
                classBox.TextChanged += (sender, args) =>
                {
                    if (byte.TryParse(classBox.Text, out var result))
                    {
                        interactionHolder.ChangeClass(enterprise, result);
                    }
                };
                Grid.SetColumn(classBox, 1);

                TextBox addressBox = new TextBox() { Text = enterprise.Address };
                addressBox.TextChanged += (sender, args) =>
                {
                    interactionHolder.ChangeAddress(enterprise, addressBox.Text);
                };
                Grid.SetColumn(addressBox, 2);

                TextBox specialityBox = new TextBox() { Text = enterprise.Speciality };
                specialityBox.Margin = new Thickness(2, 0, 2, 0);
                specialityBox.TextChanged += (sender, args) =>
                {
                    interactionHolder.ChangeSpeciality(enterprise, specialityBox.Text);
                };
                Grid.SetColumn(specialityBox, 3);

                ComboBox ownershipTypes = new ComboBox();
                ownershipTypes.ItemsSource = Enum.GetValues(typeof(Enterprise.OwnershipType)).Cast<Enterprise.OwnershipType>();
                ownershipTypes.SelectedIndex = (int)enterprise.Ownership;
                ownershipTypes.SelectionChanged += (sender, args) =>
                {
                    interactionHolder.ChangeOwnership(enterprise, (Enterprise.OwnershipType)ownershipTypes.SelectedItem);
                };
                Grid.SetColumn(ownershipTypes, 4);

                Button numbersButton = new Button() { Content = "Номери" };
                numbersButton.Margin = new Thickness(2, 0, 2, 0);
                numbersButton.Click += (sender, args) =>
                {
                    stringsListWindow.SetList(
                        enterprise.PhoneNumbers,
                        (index, newValue) =>
                        {
                            interactionHolder.ChangePhoneNumbers(enterprise, index, newValue);
                        });
                    stringsListWindow.Title = "Зміна Номерів • " + enterprise.Name;
                    stringsListWindow.Show();
                };
                Grid.SetColumn(numbersButton, 5);

                Button servicesButton = new Button() { Content = "Послуги" };
                servicesButton.Click += (sender, args) =>
                {
                    stringsListWindow.SetList(
                        enterprise.Services,
                        (index, newValue) =>
                        {
                            interactionHolder.ChangeServices(enterprise, index, newValue);
                        });
                    stringsListWindow.Title = "Зміна Послуг • " + enterprise.Name;
                    stringsListWindow.Show();
                };
                Grid.SetColumn(servicesButton, 6);

                Button workingHoursButton = new Button() { Content = "Час Роботи" };
                workingHoursButton.Margin = new Thickness(2, 0, 2, 0);
                workingHoursButton.Click += (sender, args) =>
                {
                    workingHoursWindow.SetWorkingHours(
                        enterprise.WorkingHours,
                        (day, start, end) =>
                        {
                            interactionHolder.ChangeWorkingHours(enterprise, day, start, end);
                        },
                        day =>
                        {
                            interactionHolder.RemoveWorkingHours(enterprise, day);
                        });
                    workingHoursWindow.Show();
                };
                Grid.SetColumn(workingHoursButton, 7);

                Button deleteButton = new Button() { Content = "Видалити" };
                deleteButton.Click += (sender, args) =>
                {
                    interactionHolder.DeleteEnterprise(enterprise);
                    stringsListWindow.Hide();
                    workingHoursWindow.Hide();
                    UpdateEnterprisesList();
                };

                Grid.SetColumn(deleteButton, 8);

                enterprisesGrid.Children.Add(nameBox);
                enterprisesGrid.Children.Add(classBox);
                enterprisesGrid.Children.Add(addressBox);
                enterprisesGrid.Children.Add(specialityBox);
                enterprisesGrid.Children.Add(ownershipTypes);
                enterprisesGrid.Children.Add(numbersButton);
                enterprisesGrid.Children.Add(servicesButton);
                enterprisesGrid.Children.Add(workingHoursButton);
                enterprisesGrid.Children.Add(deleteButton);

                EnterprisesGrid.Children.Add(enterprisesGrid);
            }
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            e.Handled = !byte.TryParse(textBox.Text + e.Text, out var result);
        }

        private void OnShowWorkingHours(object sender, RoutedEventArgs e)
        {
            workingHoursWindow.SetWorkingHours(
                hoursHolder,
                (day, start, end) =>
                {
                    hoursHolder[day] = new Tuple<TimeOnly, TimeOnly>(start, end);
                },
                day =>
                {
                    hoursHolder.Remove(day);
                });
            workingHoursWindow.Show();
        }

        private void OnShowNumbers(object sender, RoutedEventArgs e)
        {
            stringsListWindow.Title = "Зміна Номерів";
            DisplayListEditor(numbersHolder);
        }

        private void OnShowServices(object sender, RoutedEventArgs e)
        {
            stringsListWindow.Title = "Зміна Послуг";
            DisplayListEditor(servicesHolder);
        }

        private void DisplayListEditor(List<string> listToEdit)
        {
            stringsListWindow.SetList(
                listToEdit,
                (index, newValue) =>
                {
                    if (newValue == null)
                    {
                        listToEdit.RemoveAt(index);
                    }
                    else if (index == -1)
                    {
                        listToEdit.Add(newValue);
                    }
                    else
                    {
                        listToEdit[index] = newValue;
                    }
                });
            stringsListWindow.Show();
        }

        private void OnAddNewEnterprise(object sender, RoutedEventArgs e)
        {
            interactionHolder.CreateEnterprise(
                NameBox.Text,
                byte.Parse(ClassBox.Text),
                AddressBox.Text,
                SpecialityBox.Text,
                OwnershipTypes.SelectedItem is Enterprise.OwnershipType ownership
                    ? ownership
                    : Enterprise.OwnershipType.Private,
                numbersHolder.ToList(),
                servicesHolder.ToList(),
                hoursHolder.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value)
            );
            UpdateEnterprisesList();
        }

        private void FilterNameChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterName.Text == "")
            {
                filter.Name = null;
                UpdateEnterprisesList();
                return;
            }
            filter.Name = FilterName.Text;
            UpdateEnterprisesList();
        }

        private void FilterClassChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterClass.Text == "")
            {
                filter.Class = null;
                UpdateEnterprisesList();
                return;
            }
            filter.Class = byte.TryParse(FilterClass.Text, out var result) ? result : null;
            UpdateEnterprisesList();
        }

        private void FilterAddressChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterAddress.Text == "")
            {
                filter.Address = null;
                UpdateEnterprisesList();
                return;
            }
            filter.Address = FilterAddress.Text;
            UpdateEnterprisesList();
        }

        private void FilterSpecialityChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterSpeciality.Text == "")
            {
                filter.Speciality = null;
                UpdateEnterprisesList();
                return;
            }
            filter.Speciality = FilterSpeciality.Text;
            UpdateEnterprisesList();
        }

        private void FilterOwnershipChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterOwnership.SelectedIndex == 0)
            {
                filter.Ownership = null;
                UpdateEnterprisesList();
                return;
            }
            filter.Ownership = (Enterprise.OwnershipType)FilterOwnership.SelectedIndex - 1;
            UpdateEnterprisesList();
        }

        private void FilterPhoneNumber(object sender, RoutedEventArgs e)
        {
            stringsListWindow.Title = "Фільтр Номерів";
            stringsListWindow.SetList(
                filter.PhoneNumbers,
                (index, newValue) =>
                {
                    if (newValue == null)
                    {
                        filter.PhoneNumbers.RemoveAt(index);
                    }
                    else if (index == -1)
                    {
                        filter.PhoneNumbers.Add(newValue);
                    }
                    else
                    {
                        filter.PhoneNumbers[index] = newValue;
                    }
                    UpdateEnterprisesList();
                });
            stringsListWindow.Show();
        }

        private void FilterServices(object sender, RoutedEventArgs e)
        {
            stringsListWindow.Title = "Фільтр Послуг";
            stringsListWindow.SetList(
                filter.Services,
                (index, newValue) =>
                {
                    if (newValue == null)
                    {
                        filter.Services.RemoveAt(index);
                    }
                    else if (index == -1)
                    {
                        filter.Services.Add(newValue);
                    }
                    else
                    {
                        filter.Services[index] = newValue;
                    }
                    UpdateEnterprisesList();
                });
            stringsListWindow.Show();
        }

        private void FilterWorkingHours(object sender, RoutedEventArgs e)
        {
            workingHoursWindow.SetWorkingHours(
                filter.WorkingHours,
                (day, start, end) =>
                {
                    filter.WorkingHours[day] = new Tuple<TimeOnly, TimeOnly>(start, end);
                    UpdateEnterprisesList();
                },
                day =>
                {
                    filter.WorkingHours.Remove(day);
                    UpdateEnterprisesList();
                });
            workingHoursWindow.Show();
        }

        private readonly StringListWindow stringsListWindow = new();
        private readonly WorkingHoursWindow workingHoursWindow = new();
        private readonly InteractionHolder interactionHolder = new("database.json");
        private readonly Enterprise.Filter filter = new();
        private readonly List<string> numbersHolder = new();
        private readonly List<string> servicesHolder = new();
        private readonly Dictionary<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>> hoursHolder = new();
    }
}