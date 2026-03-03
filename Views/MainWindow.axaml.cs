using Avalonia.Controls;
using Avalonia.Input;
using System.Linq;

using Avalonia_Demo_Kanban.ViewModels;

namespace Avalonia_Demo_Kanban.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (DataContext is MainWindowViewModel vm)
        {
            var source = e.Source as Control;
            if (source != null)
            {
                // Check if currently open add task command (exit if not)
                var addingColumn = vm.Columns.FirstOrDefault(c => c.IsAddingTask);
                if (addingColumn is null) return;

                // Cancel if PointerPressed source was another add task button (other column) or was outside of current task input (textbox or confirm button)
                if (IsAddTaskButton(source) || !IsAddTaskInputOrChild(source))
                {
                    addingColumn.CancelAddTaskCommand.Execute(null);
                }
            }
        }
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        // Submit task on Enter (without modifiers), allow Shift+Enter for new line
        if (e.Key == Key.Return && e.KeyModifiers == KeyModifiers.None)
        {
            e.Handled = true;
            if (sender is TextBox textBox && textBox.DataContext is KanbanColumnViewModel vm)
            {
                vm.AddTaskCommand.Execute(null);
            }
        }
    }

    private bool IsAddTaskInputOrChild(Control control)
    {
        // Check if the control is a TextBox or a Button with "✓" content, or their children
        if (control is TextBox)
            return true;

        if (control is Button button && button.Content?.ToString() == "✓")
            return true;

        // Check if this control is a child of a TextBox or submit button
        var parent = control.Parent;
        while (parent != null)
        {
            if (parent is TextBox)
                return true;
            if (parent is Button btn && btn.Content?.ToString() == "✓")
                return true;
            parent = parent.Parent;
        }

        return false;
    }

    private bool IsAddTaskButton(Control control)
    {
        // Check if this is a "+" add button or a child of one
        if (control is Button button && button.Content?.ToString() == "+")
            return true;

        var parent = control.Parent;
        while (parent != null)
        {
            if (parent is Button btn && btn.Content?.ToString() == "+")
                return true;
            parent = parent.Parent;
        }

        return false;
    }
}