using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Interaction;
using Xceed.Wpf.Toolkit;

namespace UserInterface;

public partial class WorkingHoursWindow : Window
{
    public WorkingHoursWindow()
    {
        InitializeComponent();
        Closing += (sender, args) => { args.Cancel = true; Hide(); hoursHolder = null; };
    }

    public void SetWorkingHours(
        Dictionary<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>> workingHours,
        OnModified onModified,
        OnRemoved onRemoved)
    {
        hoursHolder = workingHours;
        this.onModified = onModified;
        this.onRemoved = onRemoved;
        UpdateHoursContent();
    }

    private void UpdateHoursContent()
    {
        if (hoursHolder == null)
            return;

        WorkingHoursGrid.Children.Clear();
        foreach (Enterprise.DayOfWeek day in Enum.GetValues(typeof(Enterprise.DayOfWeek)))
        {
            StackPanel dayPanel = new StackPanel();
            dayPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(dayPanel, (int)day);
            Grid.SetColumn(dayPanel, 0);

            TextBlock dayTextBlock = new TextBlock() { Text = day.ToString() };
            dayTextBlock.Width = 65;

            TimePicker startPicker = new TimePicker();
            startPicker.Format = DateTimeFormat.Custom;
            startPicker.FormatString = "HH:mm";
            if (hoursHolder.ContainsKey(day))
            {
                startPicker.Value = new DateTime(2023, 1, 1, hoursHolder[day].Item1.Hour, hoursHolder[day].Item1.Minute, 0);
            }
            else
            {
                startPicker.IsEnabled = false;
                startPicker.Value = new DateTime(2023, 1, 1, 0, 0, 0);
            }
            startPicker.ValueChanged += (sender, args) =>
            {
                onModified.Invoke(day, new TimeOnly(startPicker.Value.Value.Hour, startPicker.Value.Value.Minute), hoursHolder[day].Item2);
            };

            TextBlock dashTextBlock = new TextBlock() { Text = "-" };
            dashTextBlock.Margin = new Thickness(3, 0, 3, 0);

            TimePicker endPicker = new TimePicker();
            endPicker.Format = DateTimeFormat.Custom;
            endPicker.FormatString = "HH:mm";
            if (hoursHolder.ContainsKey(day))
            {
                endPicker.Value = new DateTime(2023, 1, 1, hoursHolder[day].Item2.Hour, hoursHolder[day].Item2.Minute, 0);
            }
            else
            {
                endPicker.IsEnabled = false;
                endPicker.Value = new DateTime(2023, 1, 1, 0, 0, 0);
            }
            endPicker.ValueChanged += (sender, args) =>
            {
                onModified.Invoke(day, hoursHolder[day].Item1, new TimeOnly(endPicker.Value.Value.Hour, endPicker.Value.Value.Minute));
            };

            CheckBox dayCheckBox = new CheckBox();
            dayCheckBox.Margin = new Thickness(0, 0, 3, 0);
            dayCheckBox.IsChecked = hoursHolder.ContainsKey(day);
            dayCheckBox.Checked += (sender, args) =>
            {
                onModified.Invoke(day, new TimeOnly(startPicker.Value.Value.Hour, startPicker.Value.Value.Minute), new TimeOnly(endPicker.Value.Value.Hour, endPicker.Value.Value.Minute));
                startPicker.IsEnabled = true;
                endPicker.IsEnabled = true;
            };
            dayCheckBox.Unchecked += (sender, args) =>
            {
                onRemoved.Invoke(day);
                startPicker.IsEnabled = false;
                endPicker.IsEnabled = false;
            };

            dayPanel.Children.Add(dayCheckBox);
            dayPanel.Children.Add(dayTextBlock);
            dayPanel.Children.Add(startPicker);
            dayPanel.Children.Add(dashTextBlock);
            dayPanel.Children.Add(endPicker);

            WorkingHoursGrid.Children.Add(dayPanel);
        }
    }

    public delegate void OnModified(Enterprise.DayOfWeek day, TimeOnly start, TimeOnly end);
    public delegate void OnRemoved(Enterprise.DayOfWeek day);

    private OnModified onModified;
    private OnRemoved onRemoved;
    private Dictionary<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>>? hoursHolder;
}