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
                // Check if a "+" button was clicked - if so, cancel any existing adding mode first
                if (IsShowAddButton(source))
                {
                    var addingColumn = vm.Columns.FirstOrDefault(c => c.IsAddingTask);
                    if (addingColumn != null)
                    {
                        addingColumn.CancelAddTaskCommand.Execute(null);
                    }
                    return; // Allow the click to proceed to the ShowAddTaskCommand
                }

                // Otherwise, cancel if clicking outside the textbox and submit button
                var addingColumn2 = vm.Columns.FirstOrDefault(c => c.IsAddingTask);
                if (addingColumn2 != null && !IsAddControlOrChild(source))
                {
                    addingColumn2.CancelAddTaskCommand.Execute(null);
                }
            }
        }
    }

    private bool IsAddControlOrChild(Control control)
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

    private bool IsShowAddButton(Control control)
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