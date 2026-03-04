using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia_Demo_Kanban.Models;
using Avalonia_Demo_Kanban.ViewModels;

namespace Avalonia_Demo_Kanban.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, Task_DragOver);
        AddHandler(DragDrop.DropEvent, Task_Drop);

        // ensure ghost overlay starts hidden
        GhostItem.IsVisible = false;
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
                var addingColumn = vm.Columns.FirstOrDefault(c => c.IsCreatingTask);
                if (addingColumn is null) return;

                // Cancel if PointerPressed source was another add task button (other column) or was outside of current task input (textbox or confirm button)
                if (IsAddTaskButton(source) || !IsAddTaskInputOrChild(source))
                {
                    addingColumn.CancelTaskInputCommand.Execute(null);
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
                vm.CreateTaskCommand.Execute(null);
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

    // --- drag & drop logic ------------------------------------------------

    private async void Task_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border taskBorder) return;
        if (taskBorder.DataContext is not TaskItem task) return;
        if (DataContext is not MainWindowViewModel vm) return;
        
        var sourceColumn = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
        if (sourceColumn is null) return;

        vm.DraggingTaskItem = task;
        vm.DragginTaskBorder = taskBorder;

        var mousePosition = e.GetPosition(this);
        var mouseOffset = e.GetPosition(taskBorder);

        GhostItem.RenderTransform = new TranslateTransform(mousePosition.X - mouseOffset.X, mousePosition.Y - mouseOffset.Y);
        GhostItem.IsVisible = true;

        GhostItem.Height = taskBorder.Bounds.Height;
        GhostItem.Width = taskBorder.Bounds.Width;

        taskBorder.IsVisible = false;

        // Prepare the DataTransfer
        var dragData = new DataTransfer();
        var dragDataItem0 = DataTransferItem.Create(DataFormat.Text, task.Id);
        var dragDataItem1 = DataTransferItem.Create(DataFormat.Text, mouseOffset.X.ToString());
        var dragDataItem2 = DataTransferItem.Create(DataFormat.Text, mouseOffset.Y.ToString());
        dragData.Add(dragDataItem0);
        dragData.Add(dragDataItem1);
        dragData.Add(dragDataItem2);

        // Start DragDrop operation
        var effects = await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Move);

        // If drag was cancelled or dropped outside, restore the original task visibility
        if (effects == DragDropEffects.None)
        {
            GhostItem.IsVisible = false;
            taskBorder.IsVisible = true;
        }
    }

    // While task is being dragged
    private void Task_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;

        var mousePosition = e.GetPosition(this);
        var mouseOffsetX = Convert.ToDouble(e.DataTransfer.Items[1].TryGetText());
        var mouseOffsetY = Convert.ToDouble(e.DataTransfer.Items[2].TryGetText());
        GhostItem.RenderTransform = new TranslateTransform(mousePosition.X - mouseOffsetX, mousePosition.Y - mouseOffsetY);
    }

    // When task is dropped
    private void Task_Drop(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;
        if (DataContext is not MainWindowViewModel vm) return;

        var taskId = e.DataTransfer.Items[0].TryGetText();
        var task = vm.GetTaskFromId(taskId);

        var source = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
        var target = (sender as Border)?.DataContext as KanbanColumnViewModel;

        if (task != null && source != null && target != null && source != target)
        {
            source.RemoveTask(task);
            target.AddTask(task);
        }

        GhostItem.IsVisible = false;
        vm.DragginTaskBorder.IsVisible = true;
    }

}