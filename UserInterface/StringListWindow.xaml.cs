using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserInterface;

public partial class StringListWindow : Window
{
    public StringListWindow()
    {
        InitializeComponent();
        Closing += (sender, args) => {
            args.Cancel = true;
            Hide();
            listHolder = null;
            onModified = null;
            ItemBox.Text = "";
        };
    }

    public void SetList(List<string> list, OnModified onModified)
    {
        listHolder = list;
        this.onModified = onModified;
        UpdateObjectsList();
    }

    private void UpdateObjectsList()
    {
        if (listHolder == null)
            return;

        StringsList.Children.Clear();
        for (int i = 0; i < listHolder.Count; i++)
        {
            int copy = i;
            Grid itemPanel = new Grid();
            itemPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            itemPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            itemPanel.Margin = new Thickness(2, 0, 2, 3);

            TextBox itemTextBlock = new TextBox() { Text = listHolder[i] };
            itemTextBlock.Margin = new Thickness(0, 0, 3, 0);
            itemTextBlock.TextChanged += (sender, args) =>
            {
                onModified?.Invoke(copy, itemTextBlock.Text);
            };
            Grid.SetColumn(itemTextBlock, 0);

            Button deleteButton = new Button() { Content = "Delete" };
            deleteButton.Click += (sender, args) =>
            {
                onModified?.Invoke(copy, null);
                UpdateObjectsList();
            };
            Grid.SetColumn(deleteButton, 1);

            itemPanel.Children.Add(itemTextBlock);
            itemPanel.Children.Add(deleteButton);
            StringsList.Children.Add(itemPanel);
        }
    }

    public delegate void OnModified(int index, string? newValue);
    private OnModified? onModified;
    private List<string>? listHolder;

    private void OnAddNewItem(object sender, RoutedEventArgs e)
    {
        onModified?.Invoke(-1, ItemBox.Text);
        UpdateObjectsList();
    }
}